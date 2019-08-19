//  <copyright file="ScapeLogging.cs" company="Scape Technologies Limited">
//
//  ScapeLogging.cs
//  ScapeKitUnity
//
//  Created by nick on 1/5/2019.
//  Copyright © 2019 Scape Technologies Limited. All rights reserved.
//  </copyright>

namespace ScapeKitUnity
{
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine;
    
    public static class ScapeLogging
    {
        public static void LogDebug(string tag = "SCKUnity_", string message = "")
        {
            if(ScapeClient.Instance.IsStarted()) 
            {
                ScapeLoggingBridge._log(LogLevel.LOG_DEBUG, tag, message);
            }
            else {
                Debug.Log(tag + " [Debug] : " + message);
            }
        }
        public static void LogError(string tag = "SCKUnity_", string message = "")
        {
            if(ScapeClient.Instance.IsStarted()) 
            {
                ScapeLoggingBridge._log(LogLevel.LOG_ERROR, tag, message);
            }
            else {
                Debug.Log(tag + " [Error] : " + message);
            }
        }

        private static class ScapeLoggingBridge
        {
#if UNITY_ANDROID && !UNITY_EDITOR && false
            private static AndroidJavaClass loggingUtils;
            internal static void _log(LogLevel level, string tag, string message)
            {
                if(loggingUtils == null) 
                {
                    loggingUtils = new AndroidJavaClass("com.scape.scapekit.internal.utils.LoggingUtils");
                    if(loggingUtils == null) 
                    {
                        Debug.Log("Failed to find Scapekit LoggingUtils");
                    }
                }
                if(loggingUtils != null)
                {
                    loggingUtils.CallStatic("log", (int)level, tag, message);
                }
            }

#elif UNITY_IPHONE && !UNITY_EDITOR && !SCAPE_C_INTERFACE
            [DllImport("__Internal")]
            private static extern void _log(int level, [MarshalAs(UnmanagedType.LPStr)]string tag, [MarshalAs(UnmanagedType.LPStr)]string message);
            internal static void _log(LogLevel level, string tag, string message)
            {
                _log((int)level, tag, message);
            }
#else 
            internal static void _log(LogLevel level, string tag, string message)
            {
                ScapeCInterface.citf_log((int)level, tag, message);
            }

#endif
        }
    }
}