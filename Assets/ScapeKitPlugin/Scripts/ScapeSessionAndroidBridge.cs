using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace ScapeKitUnity
{
#if UNITY_ANDROID && !UNITY_EDITOR
    internal sealed class ScapeSessionAndroid : AndroidJavaProxy
    {
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
            ScapeLogging.Log(message: "Init ScapeSessionAndroid");
        }

        public void GetMeasurements(GeoSourceType geoType)
        {
            ScapeLogging.Log(message: "GetMeasurements " + geoType);

            if(ScapeClientAndroid.Instance.ScapeClientJava == null) 
            {
                ScapeLogging.Log(message: "ScapeClientJava not instantiated");
                return;
            }

            using (AndroidJavaObject scapeSession = ScapeClientAndroid.Instance.ScapeClientJava.Call<AndroidJavaObject>("getScapeSession"))
            {
                if(geoType == GeoSourceType.RawSensorsAndScapeVisionEngine) 
                {
                    SetYChannel(scapeSession);
                }

                using(AndroidJavaClass GeoTypeEnum = new AndroidJavaClass("com.scape.scapekit.GeoSourceType"))
                {
                	if(geoType == GeoSourceType.RawSensors) 
                	{
                		using(var rawSensorsEnum = GeoTypeEnum.GetStatic<AndroidJavaObject>("RAWSENSORS"))
                    	{
                        	scapeSession.Call("getMeasurements", rawSensorsEnum, this);
                    	}
                	} else 
                	{
                		using(var visionEngineEnum = GeoTypeEnum.GetStatic<AndroidJavaObject>("RAWSENSORSANDSCAPEVISIONENGINE"))
                        {
                            scapeSession.Call("getMeasurements", visionEngineEnum, this);
                        }
                	}
                }
            }
        }

        private void SetYChannel(AndroidJavaObject scapeSession) {

            //ScapeLogging.Log(message: "SetYChannel");

            using (GoogleARCore.CameraImageBytes image = GoogleARCore.Frame.CameraImage.AcquireCameraImageBytes())
            {
                if (!image.IsAvailable)
                {
                    ScapeLogging.Log(message: "Cannot find scape scapeposition, reason: Unity ARCore Image is not available");
                    return;
                }

                int width = image.Width;
                int height = image.Height;
                IntPtr yPixelBuffer = image.Y;

                float xFocalLength = GoogleARCore.Frame.CameraImage.ImageIntrinsics.FocalLength.x;
                float yFocalLength = GoogleARCore.Frame.CameraImage.ImageIntrinsics.FocalLength.y;
                float xPrincipalPoint = GoogleARCore.Frame.CameraImage.ImageIntrinsics.PrincipalPoint.x;
                float yPrincipalPoint = GoogleARCore.Frame.CameraImage.ImageIntrinsics.PrincipalPoint.y;

                using (AndroidJavaClass scapeSessionUtils = new AndroidJavaClass("com.scape.scapekit.ScapeSessionUtils"))
                {
                    scapeSessionUtils.CallStatic("setCameraIntrins", scapeSession, xFocalLength, yFocalLength, xPrincipalPoint, yPrincipalPoint);
                    scapeSessionUtils.CallStatic("setCurrentYChannel", scapeSession, yPixelBuffer.ToInt64(), width, height);
                }
            }
        }

        internal event Action<ScapeSessionError>    ScapeSessionErrorEvent;
        internal event Action<LocationMeasurements> LocationMeasurementsEvent;
        internal event Action<MotionMeasurements>   MotionMeasurementsEvent;
        internal event Action<ScapeMeasurements>    ScapeMeasurementsEvent;
        internal event Action<List<double>>         CameraTransformEvent;

        internal void SetSessionEvents(
                Action<ScapeSessionError>    ScapeSessionErrorEventIn,
                Action<LocationMeasurements> LocationMeasurementsEventIn,
                Action<MotionMeasurements>   MotionMeasurementsEventIn,
                Action<ScapeMeasurements>    ScapeMeasurementsEventIn,
                Action<List<double>>         CameraTransformEventIn)
        {
            this.ScapeSessionErrorEvent = ScapeSessionErrorEventIn;
            this.LocationMeasurementsEvent = LocationMeasurementsEventIn;
            this.MotionMeasurementsEvent = MotionMeasurementsEventIn;
            this.ScapeMeasurementsEvent = ScapeMeasurementsEventIn;
            this.CameraTransformEvent = CameraTransformEventIn;
        }

        internal void OnScapeSessionStarted()
        {
            ScapeLogging.Log(message: "OnScapeSessionStarted");
        }
        internal void OnScapeSessionClosed()
        {
            ScapeLogging.Log(message: "OnScapeSessionClosed");
        }

        Dictionary<string, ScapeSessionState> ScapeSessionStateJavaStringMap = new Dictionary<string, ScapeSessionState>() {
            {"NO_ERROR", ScapeSessionState.NoError},
            {"LOCATION_SENSORS_ERROR", ScapeSessionState.LocationSensorsError},
            {"MOTION_SENSORS_ERROR", ScapeSessionState.MotionSensorsError},
            {"VISION_ENGINE_ERROR", ScapeSessionState.VisionEngineError},
            {"AUTHENTICATION_ERROR", ScapeSessionState.AuthenticationError},
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

        Coordinates GetCoordsFromJava(AndroidJavaObject jobj) 
        {
            Coordinates coordinates = new Coordinates();
            using(var coordsJava = jobj.Call<AndroidJavaObject>("getCoordinates"))
            {
                if(coordsJava != null) 
                {
                   coordinates.longitude = coordsJava.Call<double>("getLongitude");
                   coordinates.latitude = coordsJava.Call<double>("getLatitude");
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
                    orientation.x = orientationJava.Call<double>("getX");
                    orientation.y = orientationJava.Call<double>("getY");
                    orientation.z = orientationJava.Call<double>("getZ");
                    orientation.w = orientationJava.Call<double>("getW");
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

        public void onScapeSessionError(AndroidJavaObject scapeSession, AndroidJavaObject state, string message)
        {   
            this.ScapeSessionErrorEvent(new ScapeSessionError() 
            {
                state = GetScapeSessionStateFromJava(state),
                message = message
            });
        }

        public void onDeviceLocationMeasurementsUpdated(AndroidJavaObject scapeSession, AndroidJavaObject result)
        {
            this.LocationMeasurementsEvent( new LocationMeasurements 
            {
                timestamp = GetDoubleJavaFunction("getTimestamp", result),
                coordinates = GetCoordsFromJava(result),
                coordinatesAccuracy = GetDoubleJavaFunction("getCoordinatesAccuracy", result),
                altitude = GetDoubleJavaFunction("getAltitude", result),
                altitudeAccuracy = GetDoubleJavaFunction("getAltitudeAccuracy", result),
                heading = GetDoubleJavaFunction("getHeading", result),
                headingAccuracy = GetDoubleJavaFunction("getHeadingAccuracy", result),
                course = GetLongValueJavaFunction("getCourse", result),
                speed = GetLongValueJavaFunction("getSpeed", result)
            });
        }

        public void onDeviceMotionMeasurementsUpdated(AndroidJavaObject scapeSession, AndroidJavaObject result)
        {
            this.MotionMeasurementsEvent( new MotionMeasurements 
            {
                acceleration = GetDoubleListJavaFunction("getAcceleration", result),
                accelerationTimeStamp = GetDoubleJavaFunction("getAccelerationTimeStamp", result),
                userAcceleration = GetDoubleListJavaFunction("getUserAcceleration", result),
                gyro = GetDoubleListJavaFunction("getGyro", result),
                gyroTimestamp = GetDoubleJavaFunction("getGyroTimestamp", result),
                magnetometer = GetDoubleListJavaFunction("getMagnetometer", result),
                magnetometerTimestamp = GetDoubleJavaFunction("getMagnetometerTimestamp", result),
                gravity = GetDoubleListJavaFunction("getGravity", result),
                attitude = GetDoubleListJavaFunction("getAttitude", result)
            });
        }

        public void onScapeMeasurementsUpdated(AndroidJavaObject scapeSession, AndroidJavaObject result)
        {
            this.ScapeMeasurementsEvent( new ScapeMeasurements 
            {
                timestamp = GetDoubleJavaFunction("getTimestamp", result),
                coordinates = GetCoordsFromJava(result),
                heading = GetDoubleJavaFunction("getHeading", result),
                orientation = GetOrientationFromJava(result),
                rawHeightEstimate = GetDoubleJavaFunction("getRawHeightEstimate", result),
                confidenceScore = GetLongValueJavaFunction("getConfidenceScore", result),
                measurementsStatus = GetScapeMeasurementStatusFromJava(result.Call<AndroidJavaObject>("getMeasurementsStatus"))
            });
        }

        public void onCameraTransformUpdated(AndroidJavaObject scapeSession, AndroidJavaObject result)
        {
            this.CameraTransformEvent(GetDoubleListFromJavaObject(result));
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
