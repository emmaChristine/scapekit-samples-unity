//  <copyright file="ScapeSessionManagerEditor.cs" company="Scape Technologies Limited">
//
//  ScapeSessionManagerEditor.cs
//  ScapeKitUnity
//
//  Created by nick on 1/5/2019.
//  Copyright © 2019 Scape Technologies Limited. All rights reserved.
//  </copyright>

namespace ScapeKitUnity
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEditor;
    using UnityEngine;

#if UNITY_EDITOR
    [CustomEditor (typeof (ScapeSessionManager))]
    class ScapeSessionManagerEditor : Editor 
    {
        SerializedProperty theCameraProp;
        SerializedProperty debugSupportProp;
        SerializedProperty logLevelProp;
        SerializedProperty logOutputProp;
        SerializedProperty confidenceThresholdProp;
        SerializedProperty checkCameraPointsUpProp;
        SerializedProperty useGPSFallbackProp;
        SerializedProperty autoUpdateProp;
        SerializedProperty timeoutUpdateProp;
        SerializedProperty distanceUpdateProp;

        public void OnEnable()
        {
            theCameraProp = serializedObject.FindProperty("theCamera");
            debugSupportProp = serializedObject.FindProperty("debugSupport");
            logLevelProp = serializedObject.FindProperty("logLevel");
            logOutputProp = serializedObject.FindProperty("logOutput");
            confidenceThresholdProp = serializedObject.FindProperty("confidenceThreshold");
            checkCameraPointsUpProp = serializedObject.FindProperty("checkCameraPointsUp");
            useGPSFallbackProp = serializedObject.FindProperty("useGPSFallback");
            autoUpdateProp = serializedObject.FindProperty("autoUpdate");
            timeoutUpdateProp = serializedObject.FindProperty("timeoutUpdate");
            distanceUpdateProp = serializedObject.FindProperty("distanceUpdate");
        } 
        public override void OnInspectorGUI() 
        {
            serializedObject.Update();

            var theCameraGUI = new GUIContent("The Camera");
            theCameraGUI.tooltip = "must be set in order to use scape measurements with AR Camera";
            EditorGUILayout.PropertyField(theCameraProp, theCameraGUI);

            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

            var debugSupportGUI = new GUIContent("Debug Support");
            debugSupportGUI.tooltip = "enables logging";
            EditorGUILayout.PropertyField(debugSupportProp, debugSupportGUI);
            var logLevelGUI = new GUIContent("Log Level");
            logLevelGUI.tooltip = "controls the amount of logging that gets output";
            EditorGUILayout.PropertyField(logLevelProp, logLevelGUI);
            var logOutputGUI = new GUIContent("Log Output");
            logOutputGUI.tooltip = "controls where the logging gets output";
            EditorGUILayout.PropertyField(logOutputProp, logOutputGUI);

            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

            var confidenceThresholdGUI = new GUIContent("Confidence Threshold");
            confidenceThresholdGUI.tooltip = "Returned scape measurements are filtered by confidence score. " +
            "The confidence score is a value between 0-5. Putting a 4 here will only accept measurements with high confidence. " +
            "Putting 0 will use all successfully returned responses.";
            EditorGUILayout.PropertyField(confidenceThresholdProp, confidenceThresholdGUI);
            var useGPSFallbackGUI = new GUIContent("Use GPS Fall back");
            useGPSFallbackGUI.tooltip = "makes use of GPS readings until ScapeMeasuremnets are returned.";
            EditorGUILayout.PropertyField(useGPSFallbackProp, useGPSFallbackGUI);

            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

            var autoUpdateGUI = new GUIContent("Auto Update");
            autoUpdateGUI.tooltip = "tells this object to control the calling of GetMeasurements()";
            EditorGUILayout.PropertyField(autoUpdateProp, autoUpdateGUI);

            if (autoUpdateProp.boolValue)
            {
                var checkCameraPointsUpGUI = new GUIContent("checkCameraPointsUpProp");
                checkCameraPointsUpGUI.tooltip = "only attempts to send image when camera is pointing above horizontal";
                EditorGUILayout.PropertyField(checkCameraPointsUpProp, checkCameraPointsUpGUI);
                var timeoutUpdateGUI = new GUIContent("timeoutUpdateProp");
                timeoutUpdateGUI.tooltip = "controls when another GetMeasurement gets called due to timeout";
                EditorGUILayout.PropertyField(timeoutUpdateProp, timeoutUpdateGUI);
                var distanceUpdateGUI = new GUIContent("distanceUpdateProp");
                distanceUpdateGUI.tooltip = "controls when another GetMeasurement gets called due to camera movement";
                EditorGUILayout.PropertyField(distanceUpdateProp, distanceUpdateGUI);
            }
            else 
            {
                checkCameraPointsUpProp.boolValue = false;
            }

            serializedObject.ApplyModifiedProperties ();
        }
    }
#endif
}