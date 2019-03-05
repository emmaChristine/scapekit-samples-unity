using System;
using System.Collections.Generic;
using UnityEngine;

namespace ScapeKitUnity
{
    [Serializable]
    public class Coordinates
    {
        public double? longitude;
        public double? latitude;
    }

    [Serializable]
    public class ScapeOrientation
    {
        public double? y;
        public double? w;
        public double? z;
        public double? x;
    }

    public enum GeoSourceType 
    {
        RawSensors,
        RawSensorsAndScapeVisionEngine
    }

    public enum ScapeMeasurementStatus 
    {
        NoResults,
        UnavailableArea,
        ResultsFound,
        InternalError
    }

    public enum ScapeSessionState 
    {
        NoError,
        LocationSensorsError,
        MotionSensorsError,
        VisionEngineError,
        AuthenticationError,
        UnexpectedError
    }

    [Serializable]
    public class ScapeSessionError 
    {
        public ScapeSessionState state;
        public string message;
    }

    [Serializable]
    public class ScapeMeasurements
    {
        public double? timestamp;
        public Coordinates coordinates;
        public double? heading;
        public ScapeOrientation orientation;
        public double? rawHeightEstimate;
        public long? confidenceScore;
        public ScapeMeasurementStatus measurementsStatus;
    }
    
    [Serializable]
    public class MotionMeasurements
    {
        public List<double> acceleration;
        public double accelerationTimeStamp;
        public List<double> userAcceleration;
        public List<double> gyro;
        public double gyroTimestamp;
        public List<double> magnetometer;
        public double magnetometerTimestamp;
        public List<double> gravity;
        public List<double> attitude;
    }

    [Serializable]
    public class MotionSessionDetails
    {
        public MotionMeasurements measurements;
        public string errorMessage;
    }
    
    [Serializable]
    public class LocationMeasurements
    {
        public double timestamp;
        public Coordinates coordinates;
        public double coordinatesAccuracy;
        public double altitude;
        public double altitudeAccuracy;
        public double heading;
        public double headingAccuracy;
        public long course;
        public long speed;
    }

    [Serializable]
    public class LocationSessionDetails 
    {
        public LocationMeasurements measurements;
        public string errorMessage;
    }

}