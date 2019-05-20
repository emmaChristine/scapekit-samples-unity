//  <copyright file="ScapeSession.cs" company="Scape Technologies Limited">
//
//  ScapeSession.cs
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
    using System.Runtime.InteropServices;
    using UnityEngine;
    using UnityEngine.Rendering;
    #if UNITY_IPHONE
    using UnityEngine.XR.iOS;
    using Newtonsoft.Json;
    #endif
    
    public sealed class ScapeSession : MonoBehaviourSingleton<ScapeSession> 
    {
        public event Action                       GetMeasurementsEvent;
        public event Action<double>               ScapeMeasurementsRequested; 
        public event Action<ScapeSessionError>    ScapeSessionErrorEvent;
        public event Action<LocationMeasurements> DeviceLocationMeasurementsEvent;
        public event Action<MotionMeasurements>   DeviceMotionMeasurementsEvent;
        public event Action<ScapeMeasurements>    ScapeMeasurementsEvent;
        public event Action<List<double>>         CameraTransformEvent;

        internal static ScapeSession Instance
        {
            get
            {
                var scapeSessionInstance = BehaviourInstance as ScapeSession;
#if UNITY_ANDROID && !UNITY_EDITOR
                var scapeSessionAndroidInstance = ScapeSessionAndroid.Instance;
                scapeSessionAndroidInstance.SetSessionEvents(
                                                            scapeSessionInstance.ScapeMeasurementsRequested,
                                                            scapeSessionInstance.ScapeSessionErrorEvent,
                                                            scapeSessionInstance.DeviceLocationMeasurementsEvent,
                                                            scapeSessionInstance.DeviceMotionMeasurementsEvent,
                                                            scapeSessionInstance.ScapeMeasurementsEvent,
                                                            scapeSessionInstance.CameraTransformEvent
                                                            );
#endif
                return scapeSessionInstance;
            }
        }

        public void GetMeasurements()
        {
#if (UNITY_IPHONE || UNITY_ANDROID) && !UNITY_EDITOR
            ScapeSessionBridge._getMeasurements();
            if(GetMeasurementsEvent != null) 
            {
                GetMeasurementsEvent();
            }
#endif      
        }

// Used only on iOS via the C based UnitySendMessage function
#if UNITY_IPHONE && !UNITY_EDITOR

        private void OnScapeMeasurementsRequested([MarshalAs(UnmanagedType.LPStr)] string tsStr)
        {
            ScapeLogging.LogDebug(message: "OnScapeMeasurementsRequested ts=" + tsStr);

            ScapeMeasurementsRequested(double.Parse(tsStr));
        }

        private void OnScapeSessionError([MarshalAs(UnmanagedType.LPStr)] string SessionErrorStr)
        {
            ScapeLogging.LogDebug(message: "OnScapeSessionError" + SessionErrorStr);

            ScapeSessionError scapeError = JsonConvert.DeserializeObject<ScapeSessionError>(SessionErrorStr);
            ScapeSessionErrorEvent(scapeError);
        }

        private void OnDeviceLocationMeasurementsUpdated([MarshalAs(UnmanagedType.LPStr)] string LocationMeasurementsJsonStr) 
        {
            ScapeLogging.LogDebug(message: "OnDeviceLocationMeasurementsUpdated" + LocationMeasurementsJsonStr);

            LocationMeasurements locationMeasurements = JsonConvert.DeserializeObject<LocationMeasurements>(LocationMeasurementsJsonStr);
            DeviceLocationMeasurementsEvent(locationMeasurements);
        }

        private void OnDeviceMotionMeasurementsUpdated([MarshalAs(UnmanagedType.LPStr)] string MotionMeasurementsJsonStr) 
        {
            ScapeLogging.LogDebug(message: "OnDeviceMotionMeasurementsUpdated" + MotionMeasurementsJsonStr);

            MotionMeasurements motionMeasurements = JsonConvert.DeserializeObject<MotionMeasurements>(MotionMeasurementsJsonStr);
            DeviceMotionMeasurementsEvent(motionMeasurements);

        }

        private void OnScapeMeasurementsUpdated([MarshalAs(UnmanagedType.LPStr)] string ScapeMeasurementsJsonStr) 
        {
            ScapeLogging.LogDebug(message: "OnScapeMeasurementsUpdated" + ScapeMeasurementsJsonStr);

            ScapeMeasurements scapeMeasurements = JsonConvert.DeserializeObject<ScapeMeasurements>(ScapeMeasurementsJsonStr);
            ScapeMeasurementsEvent(scapeMeasurements);
        }

        private void OnCameraTransformUpdated([MarshalAs(UnmanagedType.LPStr)] string CameraTransformStr) 
        {
            ScapeLogging.LogDebug(message: "OnCameraTransformUpdated" + CameraTransformStr);

            List<double> cameraTransform = JsonConvert.DeserializeObject<List<double>>(CameraTransformStr);
            if(cameraTransform.Count == 16) 
            {
                CameraTransformEvent(cameraTransform);
            }
        }

#endif
    }
}
