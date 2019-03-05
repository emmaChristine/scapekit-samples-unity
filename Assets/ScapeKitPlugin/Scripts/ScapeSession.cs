using UnityEngine;
using System.Runtime.InteropServices;
using System;
using UnityEngine.Rendering;
using System.Collections;
using System.Collections.Generic;
#if UNITY_IPHONE
using UnityEngine.XR.iOS;
#endif

namespace ScapeKitUnity
{
    public sealed class ScapeSession : MonoBehaviourSingleton<ScapeSession> 
    {
        internal event Action<ScapeSessionError>    ScapeSessionErrorEvent;
        internal event Action<LocationMeasurements> DeviceLocationMeasurementsEvent;
        internal event Action<MotionMeasurements>   DeviceMotionMeasurementsEvent;
        internal event Action<ScapeMeasurements>    ScapeMeasurementsEvent;
        internal event Action<List<double>>         CameraTransformEvent;

        internal static ScapeSession Instance
        {
            get
            {
                var scapeSessionInstance = BehaviourInstance as ScapeSession;
#if UNITY_ANDROID && !UNITY_EDITOR
                var scapeSessionAndroidInstance = ScapeSessionAndroid.Instance;
                scapeSessionAndroidInstance.SetSessionEvents(scapeSessionInstance.ScapeSessionErrorEvent,
                                                            scapeSessionInstance.DeviceLocationMeasurementsEvent,
                                                            scapeSessionInstance.DeviceMotionMeasurementsEvent,
                                                            scapeSessionInstance.ScapeMeasurementsEvent,
                                                            scapeSessionInstance.CameraTransformEvent
                                                            );
#endif
                return scapeSessionInstance;
            }
        }

        public void GetMeasurements(GeoSourceType geoSourceType)
        {
#if (UNITY_IPHONE || UNITY_ANDROID) && !UNITY_EDITOR
            ScapeSessionBridge._getMeasurements((int)geoSourceType);
#endif
        }

// Used only on iOS via the C based UnitySendMessage function
#if UNITY_IPHONE && !UNITY_EDITOR

        private void OnScapeSessionError([MarshalAs(UnmanagedType.LPStr)] string SessionErrorStr)
        {
            Debug.Log("OnScapeSessionError" + SessionErrorStr);

            ScapeSessionError scapeError = JsonUtility.FromJson<ScapeSessionError>(SessionErrorStr);
            if(scapeError != null) {

               ScapeSessionErrorEvent(scapeError);
            }
        }

        private void OnDeviceLocationMeasurementsUpdated([MarshalAs(UnmanagedType.LPStr)] string LocationMeasurementsJsonStr) 
        {
            Debug.Log("OnDeviceLocationMeasurementsUpdated" + LocationMeasurementsJsonStr);

            LocationMeasurements locationMeasurements = JsonUtility.FromJson<LocationMeasurements>(LocationMeasurementsJsonStr);
            if(locationMeasurements != null) {
                DeviceLocationMeasurementsEvent(locationMeasurements);
            }
        }

        private void OnDeviceMotionMeasurementsUpdated([MarshalAs(UnmanagedType.LPStr)] string MotionMeasurementsJsonStr) 
        {
            Debug.Log("OnDeviceMotionMeasurementsUpdated" + MotionMeasurementsJsonStr);

            MotionMeasurements motionMeasurements = JsonUtility.FromJson<MotionMeasurements>(MotionMeasurementsJsonStr);
            if(motionMeasurements != null) {
                DeviceMotionMeasurementsEvent(motionMeasurements);
            }

        }

        private void OnScapeMeasurementsUpdated([MarshalAs(UnmanagedType.LPStr)] string ScapeMeasurementsJsonStr) 
        {
            Debug.Log("OnScapeMeasurementsUpdated" + ScapeMeasurementsJsonStr);

            ScapeMeasurements scapeMeasurements = JsonUtility.FromJson<ScapeMeasurements>(ScapeMeasurementsJsonStr);
            if(scapeMeasurements != null) {
                ScapeMeasurementsEvent(scapeMeasurements);
            }
        }

        private void OnCameraTransformUpdated([MarshalAs(UnmanagedType.LPStr)] string CameraTransformStr) 
        {
            Debug.Log("OnCameraTransformUpdated" + CameraTransformStr);

            List<double> cameraTransform = JsonUtility.FromJson<List<double>>(CameraTransformStr);
            if(cameraTransform.Count == 16) {
                CameraTransformEvent(cameraTransform);
            }
        }

#endif
    }
}
