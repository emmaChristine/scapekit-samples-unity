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
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.Events;
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
        private const float ScapeMeasurementTimeout = 0.5f;

        /// <summary>
        /// theCamera, main camera object of Unity scene.
        /// Must be set in order to use scape measurements with AR Camera
        /// </summary>
        [SerializeField]
        private Camera theCamera;

        /// <summary>
        /// A scriptable object which can be optionally added to provide 
        /// debug support options
        /// </summary>
        [SerializeField]
        private ScapeDebugConfig debugConfig;

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
        /// event that gets called when ground is detected
        /// </summary>
        [SerializeField]
        private UnityEvent onGroundDetected;

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
        /// checks whether the request has reached the native lib for execution
        /// </summary>
        private bool awaitingRequestDispatch = false;

        /// <summary>
        /// used to call mocked scape measurements after some delay
        /// </summary>
        private IEnumerator coroutine;

        /// <summary>
        /// only becomes valid once we have a scape measurement and a ground plane 
        /// </summary>
        private ScapeCameraExt scapeCameraExt;

        /// <summary>
        /// a class used to ascertain the height of the ground from ARKit/Core 
        /// </summary>
        private GroundTracker groundTracker = null;

        /// <summary>
        /// boolean to signify ground plane has been detected
        /// </summary>
        private bool isGroundDetected = false;

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
            if (!Permission.HasUserAuthorizedPermission(Permission.FineLocation))
            {
                Permission.RequestUserPermission(Permission.FineLocation);
            }

            groundTracker = new GroundTrackerARCore();
#else 
            groundTracker = new GroundTrackerARKit();
#endif
            scapeCameraExt.Reset();

            InitClient();
        }

        /// <summary>
        /// public function to trigger taking measurements
        /// </summary>
        public void TakeMeasurements()
        {
            ScapeLogging.LogDebug(message: "ScapeSessionManager::TakeMeasurements()");
            if (state == UpdateState.HaveMeasurements)
            {
                ChangeState(UpdateState.NeedsMeasurements);
            }
            else
            {
                ScapeLogging.LogDebug(message: "didnt respond " + state);
            }
        }

        /// <summary>
        /// update state machine
        /// </summary>
        public void Update()
        {
            groundTracker.Update();

            if (scapeCameraExt.IsValid() == false)
            {
                bool success = false;
                var groundHeight = groundTracker.GetGroundHeight(out success);
                if (success)
                {
                    scapeCameraExt.Altitude = -groundHeight;

                    if (!isGroundDetected)
                    {
                        if (onGroundDetected != null)
                        {
                            onGroundDetected.Invoke();
                        }
                        
                        isGroundDetected = true;
                    }
                }
            }

            if (scapeCameraExt.IsValid())
            {
                InstantiateOrigin(scapeCameraExt);

                scapeCameraExt.Reset();
            }

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

                        if (MockScapeResults()) 
                        {
                            ChangeState(UpdateState.TakingMeasurements);
                            awaitingRequestDispatch = true;
                            timeSinceScapeMeasurement = 0.0f;
                            
                            if (MockScapeResults())
                            {
                                OnScapeMeasurementsRequested(((DateTimeOffset)System.DateTime.Now).ToUnixTimeSeconds());
                                ScapeLogging.LogDebug(message: "ScapeSessionManager::StartCoroutine");
                                coroutine = MockScapeResultsDelayed(debugConfig.MockScapeMeasurementsDelay);
                                StartCoroutine(coroutine);
                            }
                        }
                        else
                        {
                            ChangeState(UpdateState.TakingMeasurements);
                            ScapeClient.Instance.ScapeSession.GetMeasurements();
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
                    if (awaitingRequestDispatch && timeSinceScapeMeasurement > ScapeMeasurementTimeout) 
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
            ScapeClient.Instance.WithDebugSupport(HaveDebugSupport()).WithResApiKey().StartClient();

            // scape client can fail to start at this point due to waiting for user to grant 
            // permissions, so we keep retrying every update
            if (ScapeClient.Instance.IsStarted())
            {
                if (debugConfig != null) 
                {
                    debugConfig.ConfigureDebugSession();
                }

                ScapeClient.Instance.ScapeSession.ScapeMeasurementsRequested += OnScapeMeasurementsRequested;
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
                            ScapeLogging.LogDebug(message: "ScapeSessionManager: The Camera assigned to the Scape Session does not have a geoCameraComponent");
                        }
                    }
                    else 
                    {
                        ScapeLogging.LogDebug(message: "ScapeSessionManager: A Scape Camera has not been assigned to the Scape Session");
                    }
                }

                if (autoUpdate) 
                {
                    ChangeState(UpdateState.NeedsMeasurements);   
                }
                else
                {
                    ChangeState(UpdateState.HaveMeasurements); 
                }
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
            ScapeLogging.LogDebug(message: "ChangeState " + st);

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
        /// instantiate the origin once we have a valid ScapeCameraExt 
        /// </summary>
        /// <param name="ext">
        /// the ScapeCameraExt
        /// </param>
        private void InstantiateOrigin(ScapeCameraExt ext)
        {
            GeoAnchorManager.Instance.InstantiateOrigin(ext.Coords);

            if (geoCameraComponent) 
            {
                geoCameraComponent.SynchronizeARCamera(ext.Coords, (float)ext.Heading, (float)ext.Altitude);
            }
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
                scapeCameraExt.Coords = scapeMeasurements.LatLng;
                scapeCameraExt.Heading = scapeMeasurements.Heading;

                receivedScapeMeasurement = true;
                ChangeState(UpdateState.HaveMeasurements);
            }
            else 
            {
                ScapeLogging.LogDebug(message: "ScapeMeasurements ignored, " + scapeMeasurements.MeasurementsStatus.ToString() + " confidenceScore=" + scapeMeasurements.ConfidenceScore);
                ChangeState(UpdateState.NeedsMeasurements);
            }

            resetUpdateVars = true;

            if (MockScapeResults()) 
            {
                ScapeLogging.LogDebug(message: "ScapeSessionManager::StopCoroutine");
                StopCoroutine(coroutine);
            }
        }

        /// <summary>
        /// Event returned from core when scape measurements are underway
        /// </summary>
        /// <param name="timestamp">
        /// the timestamp the request was sent
        /// </param>
        private void OnScapeMeasurementsRequested(double timestamp) 
        {
            ScapeLogging.LogDebug(message: "ScapeSessionManager::OnScapeMeasurementsRequested");

            awaitingRequestDispatch = false;
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
                scapeCameraExt.Coords = locationMeasurements.LatLng;
                scapeCameraExt.Heading = locationMeasurements.Heading;
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
            ScapeLogging.LogDebug(message: "ScapeSessionManager::OnScapeSessionError() " + scapeDetails.State.ToString() + ": " + scapeDetails.Message);
            
            if (state == UpdateState.TakingMeasurements)
            {
                ChangeState(UpdateState.NeedsMeasurements);
            }

            resetUpdateVars = true;
        }

        /// <summary>
        /// call the mocked measurements after some delay
        /// </summary>
        /// <param name="delayTime">
        /// Delays the mocked result 
        /// </param>
        /// <returns>
        /// IEnumerator to yield the function
        /// </returns> 
        private IEnumerator MockScapeResultsDelayed(float delayTime) 
        {
            yield return new WaitForSeconds(delayTime);

            ScapeLogging.LogDebug(message: "ScapeSessionManager::Sending Mock Measurements");

            OnScapeMeasurementsEvent(debugConfig.GetMockScapeMeasurements());
        }

        /// <summary>
        /// Check whether debug session is in use and scape measurements are to be mocked
        /// </summary>
        /// <returns>
        /// whether scape measurements should be mocked
        /// </returns>
        private bool MockScapeResults() 
        {
            return debugConfig != null && debugConfig.MockScapeResults();
        }

        /// <summary>
        /// Check whether debug session is in use
        /// </summary>
        /// <returns>
        /// boolean indicating user has made use of a debug config object
        /// </returns>
        private bool HaveDebugSupport() 
        {
            return debugConfig != null;
        }

        /// <summary>
        /// A struct to hold all info required in instantiate scape camera,
        /// and therefore the rest of the scene
        /// </summary>
        internal struct ScapeCameraExt
        {
            /// <summary>
            /// coords usually returned by scapeMeasuremnts
            /// </summary>
            public LatLng Coords;

            /// <summary>
            /// altitude calcualted by arkit/core
            /// </summary>
            public double Altitude;

            /// <summary>
            /// heading usually returned by scapeMeasuremnts
            /// </summary>
            public double Heading;

            /// <summary>
            /// check whether all fields have been assigned yet
            /// </summary>
            /// <returns>
            /// boolean indicating the struct is ready to be used
            /// </returns>
            public bool IsValid()
            {
                return this.Coords.Latitude != -1.0 && this.Coords.Longitude != -1.0 
                            && this.Altitude != -1.0 && this.Heading != -1.0;
            }

            /// <summary>
            /// reset values to null defaults
            /// </summary>
            public void Reset()
            {
                this.Coords = new LatLng { Longitude = -1.0, Latitude = -1.0 };
                this.Altitude = -1.0;
                this.Heading = -1.0;
            }

            /// <summary>
            /// shutdown client on app quit
            /// </summary>
            public void OnApplicationQuit()
            {
                ScapeClient.Instance.Terminate();
            }
        }
    }
}
