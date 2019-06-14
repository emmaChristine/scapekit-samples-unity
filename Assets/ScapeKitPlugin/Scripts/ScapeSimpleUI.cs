//  <copyright file="ScapeSimpleUI.cs" company="Scape Technologies Limited">
//
//  ScapeSimpleUI.cs
//  ScapeKitUnity
//
//  Created by nick on 1/5/2019.
//  Copyright © 2019 Scape Technologies Limited. All rights reserved.
//  </copyright>

namespace ScapeKitUnity
{
    using System.Collections;
    using System.Collections.Generic;
    using ScapeKitUnity;
    using UnityEngine;
    #if UNITY_ANDROID && !UNITY_EDITOR
    using UnityEngine.Android;
    #endif
    using UnityEngine.UI;

    /// <summary>
    /// ScapeSimpleUI, a class for managing a Scape session using a simple gui
    /// </summary>
    public class ScapeSimpleUI : MonoBehaviour
    {
        /// <summary>
        /// A text field to write the printout from scape too
        /// </summary>
        [SerializeField]
        private Text textField;

        /// <summary>
        /// The text to be updated
        /// </summary>
        private string newText;

        /// <summary>
        /// a boolean to signal a text update on the main thread
        /// </summary>
        private bool updateText = false;

        /// <summary>
        /// at startup check for location permissions on android
        /// then init scape session  
        /// </summary>
        public void Start() 
        {
            InitScape();
        }

        /// <summary>
        /// if the text has an update do it here on main thread
        /// </summary>
        public void Update()
        {
            if (updateText) 
            {
                textField.text = newText;
                updateText = false;
            }
        }

        /// <summary>
        /// register for scape callbacks
        /// </summary>
        private void InitScape()
        {
            // Register callbacks
            ScapeClient.Instance.ScapeSession.GetMeasurementsEvent += OnGetMeasurements;
            ScapeClient.Instance.ScapeSession.ScapeMeasurementsEvent += OnScapeMeasurementsEvent;
            ScapeClient.Instance.ScapeSession.ScapeSessionErrorEvent += OnScapeSessionError;
        }

        /// <summary>
        /// this call is connected to the GetMeasurements signal,
        /// when that get's called (potentially from anywhere), we reset the text
        /// </summary>
        private void OnGetMeasurements()
        {
            newText = textField.text + "\n\nFetching...";
            updateText = true;
        }

        /// <summary>
        /// on scape measurements result, print the result to the Text box.
        /// </summary>
        /// <param name="scapeMeasurements">
        /// scapeMeasurements from scape system
        /// </param>
        private void OnScapeMeasurementsEvent(ScapeMeasurements scapeMeasurements)
        {
            // Use the scape scape position
            newText = "OnScapeMeasurementsEvent:\n" +
                "timestamp: " + scapeMeasurements.Timestamp + "\n" + 
                "coordinates: " + scapeMeasurements.LatLng.Longitude + " " + scapeMeasurements.LatLng.Latitude + "\n" + 
                "heading: " + scapeMeasurements.Heading + "\n" +  
                "orientation: " + scapeMeasurements.Orientation.X + " " + scapeMeasurements.Orientation.Y + " " + scapeMeasurements.Orientation.Z + " " + scapeMeasurements.Orientation.W + "\n" + 
                "rawHeightEstimate: " + scapeMeasurements.RawHeightEstimate + "\n" + 
                "confidenceScore: " + scapeMeasurements.ConfidenceScore + "\n" + 
                "measurementsStatus: " + scapeMeasurements.MeasurementsStatus + "\n\n";
            updateText = true;
        }

        /// <summary>
        /// on error print the error
        /// </summary>
        /// <param name="scapeDetails">
        /// scapeDetails from scape system
        /// </param>
        private void OnScapeSessionError(ScapeSessionError scapeDetails)
        {
            // Handle an erroneous ScapeSessionError
            newText = "OnScapeSessionError:\n" + scapeDetails.State + "\n" + scapeDetails.Message + "\n";
            updateText = true;
        }
    }
}
