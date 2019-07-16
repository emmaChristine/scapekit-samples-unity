//  <copyright file="ScapeSessionAndroid.cs" company="Scape Technologies Limited">
//
//  ScapeSessionAndroid.cs
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

#if UNITY_ANDROID && !UNITY_EDITOR


    internal sealed class ScapeSessionAndroid : AndroidJavaProxy
    {
        public struct ARImage 
        {
            public int Width;
            public int Height;
            public IntPtr YPixelBuffer;
            public float XFocalLength;
            public float YFocalLength;
            public float XPrincipalPoint;
            public float YPrincipalPoint;
            public bool IsAvailable;
        }

        private static ScapeSessionAndroid instance = null; 
        internal static ScapeSessionAndroid Instance 
        { 
            get 
            {    
                if(instance == null) 
                {
                    instance =  new ScapeSessionAndroid();
                }
                return instance; 
            } 
        }
        
        private ScapeSessionAndroid() : base("com.scape.scapekit.ScapeSessionObserver")
        {
            ScapeLogging.LogDebug(message: "Init ScapeSessionAndroid");
        }

        public void GetMeasurements()
        {
            ARImage arImage = AcquireImageFromARCore();

            GetMeasurements(arImage);
        }

        public void GetMeasurements(ARImage image)
        {
            ScapeLogging.LogDebug(message: "GetMeasurements");

            if(ScapeClientAndroid.Instance.ScapeClientJava == null) 
            {
                ScapeLogging.LogDebug(message: "ScapeClientJava not instantiated");
                return;
            }

            using (AndroidJavaObject scapeSession = ScapeClientAndroid.Instance.ScapeClientJava.Call<AndroidJavaObject>("getScapeSession"))
            {
                SetYChannel(scapeSession, image);

                scapeSession.Call("getMeasurements", this);
            }
        }

        public ARImage AcquireImageFromARCore()
        {
            ARImage arImage = new ARImage();

            using (GoogleARCore.CameraImageBytes image = GoogleARCore.Frame.CameraImage.AcquireCameraImageBytes())
            {
                if (image.IsAvailable)
                {
                    arImage.Width = image.Width;
                    arImage.Height = image.Height;
                    arImage.YPixelBuffer = image.Y;
                    arImage.XFocalLength = GoogleARCore.Frame.CameraImage.ImageIntrinsics.FocalLength.x;
                    arImage.YFocalLength = GoogleARCore.Frame.CameraImage.ImageIntrinsics.FocalLength.y;
                    arImage.XPrincipalPoint = GoogleARCore.Frame.CameraImage.ImageIntrinsics.PrincipalPoint.x;
                    arImage.YPrincipalPoint = GoogleARCore.Frame.CameraImage.ImageIntrinsics.PrincipalPoint.y;
                    arImage.IsAvailable = true;

                    ScapeLogging.LogDebug(message:"\nCamera Intrisics:\n" +
                        "arImage.XFocalLength = " + arImage.XFocalLength + "\n" + 
                        "arImage.YFocalLength = " + arImage.YFocalLength + "\n" + 
                        "arImage.XPrincipalPoint = " + arImage.XPrincipalPoint + "\n" + 
                        "arImage.YPrincipalPoint = " + arImage.YPrincipalPoint + "\n");
                }
            }
            return arImage;
        }

        private void SetYChannel(AndroidJavaObject scapeSession, ARImage image) {


            using (AndroidJavaClass scapeSessionUtils = new AndroidJavaClass("com.scape.scapekit.ScapeSessionUtils"))
            {
                scapeSessionUtils.CallStatic("setCameraIntrins", scapeSession, 
                                                                image.XFocalLength, 
                                                                image.YFocalLength, 
                                                                image.XPrincipalPoint, 
                                                                image.YPrincipalPoint );

                scapeSessionUtils.CallStatic("setCurrentYChannel", scapeSession, 
                                                                    image.YPixelBuffer.ToInt64(), 
                                                                    image.Width, image.Height);
            }
        }
        internal event Action<double>               ScapeMeasurementsRequestedEvent;
        internal event Action<ScapeSessionError>    ScapeSessionErrorEvent;
        internal event Action<LocationMeasurements> LocationMeasurementsEvent;
        internal event Action<MotionMeasurements>   MotionMeasurementsEvent;
        internal event Action<ScapeMeasurements>    ScapeMeasurementsEvent;
        internal event Action<List<double>>         CameraTransformEvent;

        internal void SetSessionEvents(
                Action<double>               ScapeMeasurementsRequestedIn,
                Action<ScapeSessionError>    ScapeSessionErrorEventIn,
                Action<LocationMeasurements> LocationMeasurementsEventIn,
                Action<MotionMeasurements>   MotionMeasurementsEventIn,
                Action<ScapeMeasurements>    ScapeMeasurementsEventIn,
                Action<List<double>>         CameraTransformEventIn)
        {
            this.ScapeMeasurementsRequestedEvent = ScapeMeasurementsRequestedIn;
            this.ScapeSessionErrorEvent = ScapeSessionErrorEventIn;
            this.LocationMeasurementsEvent = LocationMeasurementsEventIn;
            this.MotionMeasurementsEvent = MotionMeasurementsEventIn;
            this.ScapeMeasurementsEvent = ScapeMeasurementsEventIn;
            this.CameraTransformEvent = CameraTransformEventIn;
        }

        internal void OnScapeSessionStarted()
        {
            ScapeLogging.LogDebug(message: "OnScapeSessionStarted");
        }
        internal void OnScapeSessionClosed()
        {
            ScapeLogging.LogDebug(message: "OnScapeSessionClosed");
        }

        Dictionary<string, ScapeSessionState> ScapeSessionStateJavaStringMap = new Dictionary<string, ScapeSessionState>() {
            {"NO_ERROR", ScapeSessionState.NoError},
            {"LOCATION_SENSORS_ERROR", ScapeSessionState.LocationSensorsError},
            {"MOTION_SENSORS_ERROR", ScapeSessionState.MotionSensorsError},
            {"IMAGE_SENSORS_ERROR", ScapeSessionState.ImageSensorsError},
            {"LOCKING_POSITION_ERROR", ScapeSessionState.LockingPositionError},
            {"AUTHENTICATION_ERROR", ScapeSessionState.AuthenticationError},
            {"NETWORK_ERROR", ScapeSessionState.NetworkError},
            {"UNEXPECTED_ERROR", ScapeSessionState.UnexpectedError}
        };

        ScapeSessionState GetScapeSessionStateFromJava(AndroidJavaObject jobj) 
        {
            return ScapeSessionStateJavaStringMap[jobj.Call<string>("toString")];
        }

        Dictionary<string, ScapeMeasurementStatus> ScapeMeasurementsStatusJavaStringMap = new Dictionary<string, ScapeMeasurementStatus>() 
        {
            {"NO_RESULTS", ScapeMeasurementStatus.NoResults},
            {"UNAVAILABLE_AREA", ScapeMeasurementStatus.UnavailableArea},
            {"RESULTS_FOUND", ScapeMeasurementStatus.ResultsFound},
            {"INTERNAL_ERROR", ScapeMeasurementStatus.InternalError}
        };

        ScapeMeasurementStatus GetScapeMeasurementStatusFromJava(AndroidJavaObject jobj) 
        {
            return ScapeMeasurementsStatusJavaStringMap[jobj.Call<string>("toString")];
        }

        LatLng GetCoordsFromJava(AndroidJavaObject jobj) 
        {
            LatLng coordinates = new LatLng();
            using(var coordsJava = jobj.Call<AndroidJavaObject>("getLatLng"))
            {
                if(coordsJava != null) 
                {
                   coordinates.Longitude = coordsJava.Call<double>("getLongitude");
                   coordinates.Latitude = coordsJava.Call<double>("getLatitude");
                }
                return coordinates;
            }
            return coordinates;
        }

        ScapeOrientation GetOrientationFromJava(AndroidJavaObject jobj) 
        {
            ScapeOrientation orientation = new ScapeOrientation();

            using(var orientationJava = jobj.Call<AndroidJavaObject>("getOrientation"))
            {
                if(orientationJava != null) 
                {
                    orientation.X = orientationJava.Call<double>("getX");
                    orientation.Y = orientationJava.Call<double>("getY");
                    orientation.Z = orientationJava.Call<double>("getZ");
                    orientation.W = orientationJava.Call<double>("getW");
                }
                return orientation;
            }
            return orientation;
        }

        double GetDoubleJavaFunction(string function, AndroidJavaObject jobj) 
        {
            double d = 0.0;

            using(AndroidJavaObject valueJava = jobj.Call<AndroidJavaObject>(function))
            {
                if(valueJava != null) 
                {
                    d = valueJava.Call<double>("doubleValue");
                    return d;
                }
            }
            return d;
        }

        long GetLongValueJavaFunction(string function, AndroidJavaObject jobj) 
        {
            long l = 0;
            using(AndroidJavaObject valueJava = jobj.Call<AndroidJavaObject>(function))
            {
                if(valueJava != null) 
                {
                    l = valueJava.Call<long>("longValue");
                }
                return l;
            }
            return l;
        }

        List<double> GetDoubleListFromJavaObject(AndroidJavaObject jobj) 
        {
            List<double> doubleList = new List<double>();
            using(AndroidJavaObject arrayJava = jobj.Call<AndroidJavaObject>("toArray"))
            {
                AndroidJavaObject[] doubleArray = AndroidJNIHelper.ConvertFromJNIArray<AndroidJavaObject[]>(arrayJava.GetRawObject());
                {
                    foreach(AndroidJavaObject v in doubleArray) 
                    {
                        doubleList.Add(v.Call<double>("doubleValue"));
                    }
                    return doubleList;
                }             
            }
            return doubleList;
        }

        List<double> GetDoubleListJavaFunction(string function, AndroidJavaObject jobj) 
        {
            using(AndroidJavaObject listJava = jobj.Call<AndroidJavaObject>(function))
            {
                if(listJava != null)
                {
                    return GetDoubleListFromJavaObject(listJava);
                } else
                {
                    return new List<double>();
                }
            }
        }

        // region ScapeSessionObserver JavaProxy

        public void onScapeMeasurementsRequested(AndroidJavaObject scapeSession, double timestamp)
        {
            if(this.ScapeMeasurementsRequestedEvent != null) {

                this.ScapeMeasurementsRequestedEvent(timestamp);
            }
        }

        public void onScapeSessionError(AndroidJavaObject scapeSession, AndroidJavaObject state, string message)
        {   
            ScapeLogging.LogDebug(message: "ScapeSessionAndroid::onScapeSessionError");
            if(this.ScapeSessionErrorEvent != null) 
            {      
                this.ScapeSessionErrorEvent(new ScapeSessionError() 
                {
                    State = GetScapeSessionStateFromJava(state),
                    Message = message
                });


                ScapeLogging.LogDebug(message: "ScapeSessionAndroid::onScapeSessionError - SENT");
            }
        }

        public void onDeviceLocationMeasurementsUpdated(AndroidJavaObject scapeSession, AndroidJavaObject result)
        {
            if(this.LocationMeasurementsEvent != null) 
            {
                this.LocationMeasurementsEvent( new LocationMeasurements 
                {
                    Timestamp = GetDoubleJavaFunction("getTimestamp", result),
                    LatLng = GetCoordsFromJava(result),
                    CoordinatesAccuracy = GetDoubleJavaFunction("getCoordinatesAccuracy", result),
                    Altitude = GetDoubleJavaFunction("getAltitude", result),
                    AltitudeAccuracy = GetDoubleJavaFunction("getAltitudeAccuracy", result),
                    Heading = GetDoubleJavaFunction("getHeading", result),
                    HeadingAccuracy = GetDoubleJavaFunction("getHeadingAccuracy", result),
                    Course = GetLongValueJavaFunction("getCourse", result),
                    Speed = GetLongValueJavaFunction("getSpeed", result)
                });
            }
        }

        public void onDeviceMotionMeasurementsUpdated(AndroidJavaObject scapeSession, AndroidJavaObject result)
        {
            if(this.MotionMeasurementsEvent != null) 
            {
                this.MotionMeasurementsEvent( new MotionMeasurements 
                {
                    Acceleration = GetDoubleListJavaFunction("getAcceleration", result),
                    AccelerationTimeStamp = GetDoubleJavaFunction("getAccelerationTimeStamp", result),
                    UserAcceleration = GetDoubleListJavaFunction("getUserAcceleration", result),
                    Gyro = GetDoubleListJavaFunction("getGyro", result),
                    GyroTimestamp = GetDoubleJavaFunction("getGyroTimestamp", result),
                    Magnetometer = GetDoubleListJavaFunction("getMagnetometer", result),
                    MagnetometerTimestamp = GetDoubleJavaFunction("getMagnetometerTimestamp", result),
                    Gravity = GetDoubleListJavaFunction("getGravity", result),
                    Attitude = GetDoubleListJavaFunction("getAttitude", result)
                });
            }
        }

        public void onScapeMeasurementsUpdated(AndroidJavaObject scapeSession, AndroidJavaObject result)
        {
            if(this.ScapeMeasurementsEvent != null) 
            {
                this.ScapeMeasurementsEvent( new ScapeMeasurements 
                {
                    Timestamp = GetDoubleJavaFunction("getTimestamp", result),
                    LatLng = GetCoordsFromJava(result),
                    Heading = GetDoubleJavaFunction("getHeading", result),
                    Orientation = GetOrientationFromJava(result),
                    RawHeightEstimate = GetDoubleJavaFunction("getRawHeightEstimate", result),
                    ConfidenceScore = GetDoubleJavaFunction("getConfidenceScore", result),
                    MeasurementsStatus = GetScapeMeasurementStatusFromJava(result.Call<AndroidJavaObject>("getMeasurementsStatus"))
                });
            }
        }

        public void onCameraTransformUpdated(AndroidJavaObject scapeSession, AndroidJavaObject result)
        {   
            if(this.CameraTransformEvent != null) 
            {
                this.CameraTransformEvent(GetDoubleListFromJavaObject(result));
            }
        }

        public void onScapeSessionStarted(AndroidJavaObject scapeSession, AndroidJavaObject scapeDetails)
        {
            this.OnScapeSessionStarted();
        }

        public void onScapeSessionClosed(AndroidJavaObject scapeSession, AndroidJavaObject scapeDetails)
        {
            this.OnScapeSessionClosed();
        }

        // endregion 
    }
#endif
}
