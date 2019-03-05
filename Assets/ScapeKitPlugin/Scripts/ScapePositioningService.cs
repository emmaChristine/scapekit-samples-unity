using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using ScapeKitUnity;
using System;

namespace ScapeKitUnity
{
    public class ScapePositioningService : MonoBehaviour
    {
    	private bool inited = false;

    	public bool scapeDebugModeOn = false;
    	public bool fetchScapeVisionEngineEveryFrame = false;
        public GeoSourceType sourceType = GeoSourceType.RawSensorsAndScapeVisionEngine;

        void InitScape()
        {
            // Start ScapeClient
            ScapeClient.Instance.WithDebugSupport(scapeDebugModeOn).WithResApiKey().StartClient();

            // Register callbacks
            ScapeClient.Instance.ScapeSession.ScapeMeasurementsEvent += OnScapeMeasurementsEvent;
            ScapeClient.Instance.ScapeSession.DeviceLocationMeasurementsEvent += OnScapeLocationMeasurementsEvent;
            ScapeClient.Instance.ScapeSession.DeviceMotionMeasurementsEvent += OnScapeMotionMeasurementsEvent;
            ScapeClient.Instance.ScapeSession.ScapeSessionErrorEvent += OnScapeSessionError;

            inited = true;
        }

        public void GetMeasurements()
        {
            if(!inited)
            {
                InitScape();
            }

            // Request scapeposition
            ScapeClient.Instance.ScapeSession.GetMeasurements(sourceType);
        }

        void Start()
        {
        }

        void Update()
        {
            if (fetchScapeVisionEngineEveryFrame)
            {
                GetMeasurements();
            }
        }

        void OnScapeMeasurementsEvent(ScapeMeasurements scapeMeasurements)
        {
            // Use the scape scapeposition
            ScapeLogging.Log(message: "OnScapeMeasurementsEvent:\n" +
                "timestamp: " + scapeMeasurements.timestamp + "\n" + 
                "coordinates: " + scapeMeasurements.coordinates.longitude + " " + scapeMeasurements.coordinates.latitude + "\n" + 
                "heading: " + scapeMeasurements.heading + "\n" +  
                "orientation: " + scapeMeasurements.orientation.x + " " + scapeMeasurements.orientation.y + " " + scapeMeasurements.orientation.z + " " + scapeMeasurements.orientation.w + "\n" + 
                "rawHeightEstimate: " + scapeMeasurements.rawHeightEstimate + "\n" + 
                "confidenceScore: " + scapeMeasurements.confidenceScore + "\n" + 
                "measurementsStatus: " + scapeMeasurements.measurementsStatus + "\n\n"
            );
        }

        void OnScapeLocationMeasurementsEvent(LocationMeasurements locationMeasurements)
        {
            ScapeLogging.Log(message: "OnScapeLocationMeasurementsEvent:\n" + 
                "timestamp: " + locationMeasurements.timestamp + "\n" + 
                "coordinates: " + locationMeasurements.coordinates.longitude + " " + locationMeasurements.coordinates.latitude + "\n" + 
                "coordinatesAccuracy: " + locationMeasurements.coordinatesAccuracy + "\n" + 
                "altitude: " + locationMeasurements.altitude + "\n" + 
                "altitudeAccuracy: " + locationMeasurements.altitudeAccuracy + "\n" + 
                "heading: " + locationMeasurements.heading + "\n" + 
                "headingAccuracy: " + locationMeasurements.headingAccuracy + "\n" + 
                "course: " + locationMeasurements.course + "\n" + 
                "speed: " + locationMeasurements.speed + "\n\n"
            );
        }

        void OnScapeMotionMeasurementsEvent(MotionMeasurements motionMeasurements)
        {
            ScapeLogging.Log(message: "OnScapeMotionMeasurementsEvent:\n" + 
                "acceleration: " + motionMeasurements.acceleration[0] + " " + motionMeasurements.acceleration[1] + " " + motionMeasurements.acceleration[2] + "\n" +
                "accelerationTimeStamp: " + motionMeasurements.accelerationTimeStamp + "\n" +
                "userAcceleration: " + motionMeasurements.userAcceleration[0] + " " + motionMeasurements.userAcceleration[1] + " " + motionMeasurements.userAcceleration[2] + "\n" +
                "gyro: " + motionMeasurements.gyro[0] + " " + motionMeasurements.gyro[1] + " " + motionMeasurements.gyro[2] + "\n" +
                "gyroTimestamp: " + motionMeasurements.gyroTimestamp + "\n" +
                "magnetometer: " + motionMeasurements.magnetometer[0] + " " + motionMeasurements.magnetometer[1] + " " + motionMeasurements.magnetometer[2] + "\n" +
                "magnetometerTimestamp: " + motionMeasurements.magnetometerTimestamp + "\n" +
                "gravity: " + motionMeasurements.gravity[0] + " " + motionMeasurements.gravity[1] + " " + motionMeasurements.gravity[2] + "\n" +
                "attitude: " + motionMeasurements.attitude[0] + " " + motionMeasurements.attitude[1] + " " + motionMeasurements.attitude[2] + "\n\n"
            );
        }

        void OnScapeSessionError(ScapeSessionError scapeDetails)
        {
            ScapeLogging.Log(message: "OnScapeSessionError:\n" + scapeDetails.state + "\n" + scapeDetails.message + "\n");
        }
    }
}
