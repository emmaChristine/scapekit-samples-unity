//  <copyright file="ScapeSessionManager.cs" company="Scape Technologies Limited">
//
//  ScapeSessionManager.cs
//  ScapeKitUnity
//
//  Created by nick on 1/5/2019.
//  Copyright © 2019 Scape Technologies Limited. All rights reserved.
//  </copyright>

namespace ScapeKitUnity
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
#if UNITY_ANDROID
    using UnityEngine.Android;
#endif

    /// <summary>
    /// ScapeSessionManager, manages the ScapeClient, calls GetMeasurements.
    /// When AutoUpdate is set the frequency of the update can be controlled by the TimeoutUpdate field.
    /// </summary>
    public class ScapeSessionManager : MonoBehaviour
    {
        /// <summary>
        /// scapeMeasurementTimeout, the period of time a measurement will be waited for after calling GetMeasurements
        /// </summary>
        private const float ScapeMeasurementTimeout = 10.0f;

        /// <summary>
        /// theCamera, main camera object of Unity scene.
        /// Must be set in order to use scape measurements with AR Camera
        /// </summary>
        [SerializeField]
        private Camera theCamera;

        /// <summary>
        /// DebugSupport, enables logging
        /// </summary>
        [SerializeField]
        private bool debugSupport = false;
        
        /// <summary>
        /// LogLevel, controls the amount of logging that gets output
        /// </summary>
        [SerializeField]
        private LogLevel logLevel = LogLevel.LOG_ERROR;
        
        /// <summary>
        /// logOutput, controls where the logging gets output
        /// </summary>
        [SerializeField]
        private LogOutput logOutput = LogOutput.CONSOLE;

        /// <summary>
        /// Returned scape measurements are filtered by confidence score.
        /// The confidence score is a value between 0-5. Putting a 4 here will only accept measurements with high confidence.
        /// Putting 0 will use all successfully returned responses.
        /// </summary>
        [SerializeField]
        private float confidenceThreshold = 4.0f;
        
        /// <summary>
        /// checkCameraPointsUp, only attempts to send image when camera is pointing above horizontal
        /// </summary>
        [SerializeField]
        private bool checkCameraPointsUp = true;
        
        /// <summary>
        /// useGPS, will use GPS until ScapeMeasuremnets are returned.
        /// </summary>
        [SerializeField]
        private bool useGPSFallback = false;
        
        /// <summary>
        /// autoUpdate, tells this object to control the calling of GetMeasurements()
        /// </summary>
        [SerializeField]
        private bool autoUpdate = true;
        
        /// <summary>
        /// TimeoutUpdate, controls when another GetMeasurement gets called due to timeout
        /// </summary>
        [SerializeField]
        private float timeoutUpdate = -1.0f;
        
        /// <summary>
        /// distanceUpdate, controls when another GetMeasurement gets called due to camera movement
        /// </summary>
        [SerializeField]
        private float distanceUpdate = -1.0f;

        /// <summary>
        /// timeSinceUpdate, counter for timeout
        /// </summary>
        private float timeSinceUpdate = 0.0f;
        
        /// <summary>
        /// timeSinceScapeMeasurement, counter for timeout
        /// </summary>
        private float timeSinceScapeMeasurement = 0.0f;
        
        /// <summary>
        /// positionAtLastUpdate, used to check distance update
        /// </summary>
        private Vector3 positionAtLastUpdate;
        
        /// <summary>
        /// receivedScapeMeasurement, set once
        /// </summary>
        private bool receivedScapeMeasurement = false;
        
        /// <summary>
        /// geoCameraComponent, should be found attached to camera object∂
        /// </summary>
        private GeoCameraComponent geoCameraComponent = null;

        /// <summary>
        /// resetUpdateVars, used to get camera position in main thread as well as zeroing wait variables 
        /// </summary>
        private bool resetUpdateVars = false;

        /// <summary>
        /// current state of session component
        /// </summary>
        private UpdateState state = UpdateState.ClientNotStarted;

        /// <summary>
        /// controls state of Session update
        /// </summary>
        private enum UpdateState 
        {
            /// <summary>
            /// initial state, pending initialization of client 
            /// </summary>
            ClientNotStarted = 0,

            /// <summary>
            /// state needs to take another measurement imminently
            /// </summary>
            NeedsMeasurements,

            /// <summary>
            /// GetMeasrements has been called awaiting results
            /// </summary>
            TakingMeasurements,

            /// <summary>
            /// have will received results. Will remain idle until a timeout or distance update is requires
            /// another measurement to be taken
            /// </summary>
            HaveMeasurements
        }

        /// <summary>
        /// Attempt to init client at startup
        /// </summary>
        public void Start()
        {
#if UNITY_ANDROID
            if(!Permission.HasUserAuthorizedPermission(Permission.FineLocation))
            {
                Permission.RequestUserPermission(Permission.FineLocation);
            }
#endif

            InitClient();
        }

        /// <summary>
        /// update state machine
        /// </summary>
        public void Update()
        {
            if (state == UpdateState.ClientNotStarted) 
            {
                InitClient();
                return;
            }

            if (resetUpdateVars) 
            {
                ResetUpdateVars();
            }

            switch (state) 
            {
                case UpdateState.NeedsMeasurements:

                    if (checkCameraPointsUp == false || CheckCameraPointingUp()) 
                    {
                        if (geoCameraComponent) 
                        {
                            geoCameraComponent.HoldCameraPose();
                        }

                        if (ScapeClient.Instance.ScapeSession) 
                        {
                            ScapeLogging.LogDebug(message: "ScapeSessionManager UpdateState.NeedsMeasurements");
                            ScapeClient.Instance.ScapeSession.GetMeasurements();
                            ChangeState(UpdateState.TakingMeasurements);
                            timeSinceScapeMeasurement = 0.0f;
                        }
                        else
                        {
                            ScapeLogging.LogDebug(message: "ScapeSessionManager No ScapeSession!");
                        }
                    }

                    break;

                case UpdateState.HaveMeasurements:

                    if (autoUpdate && CheckRequiresUpdate()) 
                    {
                        ScapeLogging.LogDebug(message: "ScapeSessionManager CheckRequiresUpdate");
                        ChangeState(UpdateState.NeedsMeasurements);
                    }

                    break;

                case UpdateState.TakingMeasurements:

                    timeSinceScapeMeasurement += Time.deltaTime;
                    if (timeSinceScapeMeasurement > ScapeMeasurementTimeout) 
                    {
                        ScapeLogging.LogDebug(message: "ScapeSessionManager scapeMeasurementTimeout");
                        timeSinceScapeMeasurement = 0.0f;
                        ChangeState(UpdateState.NeedsMeasurements);
                    }

                    break;
            }
        }

        /// <summary>
        /// initialize the scape client, set action callbacks
        /// retrieve ScapeCaemraComponent from camera
        /// </summary>
        private void InitClient() 
        {
            ScapeClient.Instance.WithDebugSupport(debugSupport).WithResApiKey().StartClient();

            if (ScapeClient.Instance.IsStarted())
            {
                if (debugSupport) 
                {
                    ScapeDebugSession.Instance.SetLogConfig(logLevel, logOutput);
                }

                ScapeClient.Instance.ScapeSession.ScapeMeasurementsEvent += OnScapeMeasurementsEvent;
                ScapeClient.Instance.ScapeSession.DeviceLocationMeasurementsEvent += OnScapeLocationMeasurementsEvent;
                ScapeClient.Instance.ScapeSession.DeviceMotionMeasurementsEvent += OnScapeMotionMeasurementsEvent;
                ScapeClient.Instance.ScapeSession.ScapeSessionErrorEvent += OnScapeSessionError;

                if (distanceUpdate > 0.0f) 
                {
                    if (theCamera != null) 
                    {
                        positionAtLastUpdate = theCamera.transform.localPosition;
                        geoCameraComponent = theCamera.GetComponent<GeoCameraComponent>();

                        if (geoCameraComponent == null) 
                        {
                            ScapeLogging.LogError(message: "ScapeSessionManager: The Camera assigned to the Scape Session does not have a geoCameraComponent");
                        }
                    }
                    else 
                    {
                        ScapeLogging.LogError(message: "ScapeSessionManager: A Scape Camera has not been assigned to the Scape Session");
                    }
                }

                ScapeLogging.LogDebug(message: "ScapeSessionManager client Started");

                ChangeState(UpdateState.NeedsMeasurements);   
            }
        }

        /// <summary>
        /// check camera points above horizontal
        /// </summary>
        /// <returns>
        /// boolean indicating the camera is pointing up.
        /// </returns>
        private bool CheckCameraPointingUp() 
        {
            if (theCamera) 
            {
                bool nearlyUp = Vector3.Dot(theCamera.transform.forward, Vector3.up) > 0.0f;

                return nearlyUp;
            }
            else 
            {
                return true;
            }
        }

        /// <summary>
        /// change state
        /// </summary>
        /// <param name="st">
        /// The state changing to 
        /// </param>
        private void ChangeState(UpdateState st) 
        {
            state = st;
        }

        /// <summary>
        /// CheckRequiresUpdate, based on time and camera movements
        /// </summary>
        /// <returns>
        /// boolean indicating another scape measurement is required.
        /// </returns>
        private bool CheckRequiresUpdate() 
        {
            bool requiresUpate = false;
            
            if (distanceUpdate > 0.0f && theCamera) 
            {
                float movement = (theCamera.transform.localPosition - positionAtLastUpdate).magnitude;
                if (movement > distanceUpdate) 
                {
                    requiresUpate = true;
                }
            }

            timeSinceUpdate += Time.deltaTime;

            if (timeoutUpdate > 0.0f && timeSinceUpdate > timeoutUpdate) 
            {
                requiresUpate = true;
            }

            return requiresUpate;
        }

        /// <summary>
        /// Set values to begin timing again
        /// </summary>
        private void ResetUpdateVars() 
        {
            if (theCamera) 
            {
                positionAtLastUpdate = theCamera.transform.localPosition;
            }

            timeSinceUpdate = 0;
            resetUpdateVars = false;
        }

        /// <summary>
        /// Callback for ScapeMeasurements update
        /// </summary>
        /// <param name="scapeMeasurements">
        /// the information passed from ScapeKit
        /// </param>
        private void OnScapeMeasurementsEvent(ScapeMeasurements scapeMeasurements)
        {
            ScapeLogging.LogDebug(message: "ScapeSessionManager::OnScapeMeasurementsEvent");
            if (scapeMeasurements.MeasurementsStatus == ScapeMeasurementStatus.ResultsFound &&
                scapeMeasurements.ConfidenceScore > confidenceThreshold) 
            {
                GeoAnchorManager.Instance.InstantiateOrigin(scapeMeasurements.LatLng);

                if (geoCameraComponent) 
                {
                    geoCameraComponent.SynchronizeARCamera(scapeMeasurements.LatLng, (float)scapeMeasurements.Heading);
                }

                receivedScapeMeasurement = true;
                ChangeState(UpdateState.HaveMeasurements);
            }
            else 
            {
                ScapeLogging.LogDebug(message: "ScapeMeasurements ignored, " + scapeMeasurements.MeasurementsStatus.ToString() + " confidenceScore=" + scapeMeasurements.ConfidenceScore);
                ChangeState(UpdateState.NeedsMeasurements);
            }

            resetUpdateVars = true;
        }

        /// <summary>
        /// Callback for LocationMeasurements update
        /// </summary>
        /// <param name="locationMeasurements">
        /// the information passed from ScapeKit
        /// </param>
        private void OnScapeLocationMeasurementsEvent(LocationMeasurements locationMeasurements)
        {
            ScapeLogging.LogDebug(message: "ScapeSessionManager::OnScapeLocationMeasurementsEvent");

            if (useGPSFallback && receivedScapeMeasurement == false) 
            {
                geoCameraComponent.SynchronizeARCamera(locationMeasurements.LatLng, (float)locationMeasurements.Heading);   
            }
        }

        /// <summary>
        /// Callback for MotionMeasurements update
        /// </summary>
        /// <param name="motionMeasurements">
        /// the information passed from ScapeKit
        /// </param>
        private void OnScapeMotionMeasurementsEvent(MotionMeasurements motionMeasurements)
        {
        }

        /// <summary>
        /// Callback for ScapeSessionError update
        /// </summary>
        /// <param name="scapeDetails">
        /// the information passed from ScapeKit
        /// </param>
        private void OnScapeSessionError(ScapeSessionError scapeDetails)
        {
            ScapeLogging.LogError(message: scapeDetails.State.ToString() + ": " + scapeDetails.Message);
            ChangeState(UpdateState.NeedsMeasurements);

            resetUpdateVars = true;
        }
    }
}
