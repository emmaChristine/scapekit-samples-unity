using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ScapeKitUnity;

namespace ScapeKitUnity
{

    public class ScapeSimpleUI : MonoBehaviour
    {

        public Text UIText;

        private string  newText;
        private bool    updateText = false;

        private bool inited = false;

        void initScape()
        {
            // Start ScapeClient
            ScapeClient.Instance.WithResApiKey().WithDebugSupport(true).StartClient();

            // Register callbacks
            ScapeClient.Instance.ScapeSession.ScapeMeasurementsEvent += OnScapeMeasurementsEvent;
            ScapeClient.Instance.ScapeSession.ScapeSessionErrorEvent += OnScapeSessionError;

            inited = true;
        }

        public void GetMeasurements()
        {
            if(!inited)
            {
                initScape();
            }

            // Request scapeposition
            ScapeClient.Instance.ScapeSession.GetMeasurements((GeoSourceType)1);
        }

        void Start()
        {
        }

        void Update()
        {
            if(updateText) {
                UIText.text = newText;
                updateText = false;
            }
        }

        void OnScapeMeasurementsEvent(ScapeMeasurements scapeMeasurements)
        {
            // Use the scape scapeposition
            newText = "OnScapeMeasurementsEvent:\n" +
                "timestamp: " + scapeMeasurements.timestamp + "\n" + 
                "coordinates: " + scapeMeasurements.coordinates.longitude + " " + scapeMeasurements.coordinates.latitude + "\n" + 
                "heading: " + scapeMeasurements.heading + "\n" +  
                "orientation: " + scapeMeasurements.orientation.x + " " + scapeMeasurements.orientation.y + " " + scapeMeasurements.orientation.z + " " + scapeMeasurements.orientation.w + "\n" + 
                "rawHeightEstimate: " + scapeMeasurements.rawHeightEstimate + "\n" + 
                "confidenceScore: " + scapeMeasurements.confidenceScore + "\n" + 
                "measurementsStatus: " + scapeMeasurements.measurementsStatus + "\n\n";
            updateText = true;
        }

        void OnScapeSessionError(ScapeSessionError scapeDetails)
        {
            // Handle an erroneous ScapeSessionError
            newText = "OnScapeSessionError:\n" + scapeDetails.state + "\n" + scapeDetails.message + "\n";
            updateText = true;
        }
    }
}
