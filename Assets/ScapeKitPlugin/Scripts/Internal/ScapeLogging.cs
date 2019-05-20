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
#if UNITY_IPHONE && !UNITY_EDITOR
            ScapeLoggingBridge._log((int)LogLevel.LOG_DEBUG, tag, message);
#elif UNITY_ANDROID && !UNITY_EDITOR
            ScapeLoggingBridge._log(LogLevel.LOG_DEBUG, tag, message);
#endif
        }
        public static void LogError(string tag = "SCKUnity_", string message = "")
        {
#if UNITY_IPHONE && !UNITY_EDITOR
            ScapeLoggingBridge._log((int)LogLevel.LOG_ERROR, tag, message);
#elif UNITY_ANDROID && !UNITY_EDITOR
            ScapeLoggingBridge._log(LogLevel.LOG_ERROR, tag, message);
#endif
        }

        private static class ScapeLoggingBridge
        {
#if UNITY_IPHONE && !UNITY_EDITOR
            [DllImport("__Internal")]
            public static extern void _log(int level, [MarshalAs(UnmanagedType.LPStr)]string tag, [MarshalAs(UnmanagedType.LPStr)]string message);
#elif UNITY_ANDROID && !UNITY_EDITOR
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
#endif
        }
    }
}