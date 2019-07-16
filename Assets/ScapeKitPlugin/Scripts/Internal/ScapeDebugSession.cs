//  <copyright file="ScapeDebugSession.cs" company="Scape Technologies Limited">
//
//  ScapeDebugSession.cs
//  ScapeKitUnity
//
//  Created by nick on 1/5/2019.
//  Copyright © 2019 Scape Technologies Limited. All rights reserved.
//  </copyright>

namespace ScapeKitUnity
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using UnityEngine;
    
    public sealed class ScapeDebugSession : MonoBehaviourSingleton<ScapeDebugSession> 
    {
        public static ScapeDebugSession Instance
        {
            get
            {
                return BehaviourInstance as ScapeDebugSession;
            }
        }

#if UNITY_ANDROID && !UNITY_EDITOR

        public void SetLogConfig(LogLevel level, LogOutput output)
        {
            AndroidJavaObject debugSession = ScapeClientAndroid.Instance.GetDebugSession();
            if(debugSession != null) 
            {
                AndroidJavaClass javaLogOutputCls = new AndroidJavaClass("com.scape.scapekit.LogOutput");
                AndroidJavaClass javaEnumSetCls = new AndroidJavaClass("java.util.EnumSet");
                AndroidJavaObject javaEnumSetOutputs = javaEnumSetCls.CallStatic<AndroidJavaObject>("noneOf", javaLogOutputCls);

                List<AndroidJavaObject> javaEnumOutputs = new List<AndroidJavaObject>();
                if((output & LogOutput.FILE) == LogOutput.FILE)
                {
                    javaEnumSetOutputs.Call<bool>("add", javaLogOutputCls.GetStatic<AndroidJavaObject>("FILE"));
                }
                if((output & LogOutput.CONSOLE) == LogOutput.CONSOLE) 
                {
                    javaEnumSetOutputs.Call<bool>("add", javaLogOutputCls.GetStatic<AndroidJavaObject>("CONSOLE"));
                }

                AndroidJavaClass javaLogLevelEnumCls = new AndroidJavaClass("com.scape.scapekit.LogLevel");
                AndroidJavaObject javaLogLevel = null;
                switch(level) 
                {
                    case LogLevel.LOG_OFF:
                        javaLogLevel = javaLogLevelEnumCls.GetStatic<AndroidJavaObject>("LOG_OFF");
                        break;
                    case LogLevel.LOG_VERBOSE:
                        javaLogLevel = javaLogLevelEnumCls.GetStatic<AndroidJavaObject>("LOG_VERBOSE");
                        break;
                    case LogLevel.LOG_DEBUG:
                        javaLogLevel = javaLogLevelEnumCls.GetStatic<AndroidJavaObject>("LOG_DEBUG");
                        break;
                    case LogLevel.LOG_INFO:
                        javaLogLevel = javaLogLevelEnumCls.GetStatic<AndroidJavaObject>("LOG_INFO");
                        break;
                    case LogLevel.LOG_WARN:
                        javaLogLevel = javaLogLevelEnumCls.GetStatic<AndroidJavaObject>("LOG_WARN");
                        break;
                    case LogLevel.LOG_ERROR:
                        javaLogLevel = javaLogLevelEnumCls.GetStatic<AndroidJavaObject>("LOG_ERROR");
                        break;
                }
                if(javaLogLevel != null) 
                {
                    debugSession.Call("setLogConfig", javaLogLevel, javaEnumSetOutputs);
                }
            }
            else 
            {
                Debug.Log("SetLogConfig Error: No DebugSession");
            }
        }

        public void MockGPSCoordinates(double latitude, double longitude) 
        {
            AndroidJavaObject debugSession = ScapeClientAndroid.Instance.GetDebugSession();
            if(debugSession != null) 
            {
                debugSession.Call("mockGPSCoordinates", latitude, longitude);
            }
            else 
            {
                Debug.Log("MockGPSCoordinates Error: No DebugSession");
            }
        }

        public void SaveImages(bool save) 
        {
            AndroidJavaObject debugSession = ScapeClientAndroid.Instance.GetDebugSession();
            if(debugSession != null) 
            {
                debugSession.Call("saveImages", save);
            }
            else 
            {
                Debug.Log("MockGPSCoordinates Error: No DebugSession");
            }
        }
#elif UNITY_IPHONE && !UNITY_EDITOR
        [DllImport("__Internal")]
        public static extern void _setLogConfig(int level, int output);

        public void SetLogConfig(LogLevel level, LogOutput output)
        {
            _setLogConfig((int)level, (int)output);
        }

        [DllImport("__Internal")]
        public static extern void _mockGPSCoordinates(double latitude, double longitude);

        public void MockGPSCoordinates(double latitude, double longitude)
        {
            _mockGPSCoordinates(latitude, longitude);
        }

        [DllImport("__Internal")]
        public static extern void _saveImages(bool save);

        public void SaveImages(bool save)
        {
            _saveImages(save);
        }
#else 
        public void SetLogConfig(LogLevel level, LogOutput output) 
        {
        	Debug.Log(message: "SetLogConfig not implemented on desktop");
        }
        public void MockGPSCoordinates(double latitude, double longitude) 
        {
            Debug.Log(message: "MockGPSCoordinates not implemented on desktop");
        }
        public void SaveImages(bool save)
        {
            Debug.Log(message: "SaveImages not implemented on desktop");
        }
#endif

    }
}