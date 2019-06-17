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
        SerializedProperty debugConfigProp;
        SerializedProperty confidenceThresholdProp;
        SerializedProperty checkCameraPointsUpProp;
        SerializedProperty useGPSFallbackProp;
        SerializedProperty autoUpdateProp;
        SerializedProperty timeoutUpdateProp;
        SerializedProperty distanceUpdateProp;

        public void OnEnable()
        {
            theCameraProp = serializedObject.FindProperty("theCamera");
            debugConfigProp = serializedObject.FindProperty("debugConfig");
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


            var debugConfigGUI = new GUIContent("Debug Config");
            debugConfigGUI.tooltip = "A scriptable object which can be optionally added to provide runtime debug support features." + 
            "Create a new instance of this object via menu Assets/Create/ScapeKit/ScapeDebugConfig";
            EditorGUILayout.PropertyField(debugConfigProp, debugConfigGUI);

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
            autoUpdateGUI.tooltip = "If set to true the client will repeatedly update scape measurements throughout the session." +
            "Otherwise the client will effectively stop after the first successful measurement is returned.";
            EditorGUILayout.PropertyField(autoUpdateProp, autoUpdateGUI);

            if (autoUpdateProp.boolValue)
            {
                var timeoutUpdateGUI = new GUIContent("Timeout Update Prop");
                timeoutUpdateGUI.tooltip = "controls when another GetMeasurement gets called due to timeout";
                EditorGUILayout.PropertyField(timeoutUpdateProp, timeoutUpdateGUI);
                var distanceUpdateGUI = new GUIContent("Distance Update Prop");
                distanceUpdateGUI.tooltip = "controls when another GetMeasurement gets called due to camera movement";
                EditorGUILayout.PropertyField(distanceUpdateProp, distanceUpdateGUI);
            }
            
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

            var checkCameraPointsUpGUI = new GUIContent("Check Camera Points Up Prop");
            checkCameraPointsUpGUI.tooltip = "only attempts to send image when camera is pointing above horizontal";
            EditorGUILayout.PropertyField(checkCameraPointsUpProp, checkCameraPointsUpGUI);

            serializedObject.ApplyModifiedProperties ();
        }
    }
#endif
}