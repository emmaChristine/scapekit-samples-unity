using System;
using UnityEngine;
using System.Runtime.InteropServices;

namespace ScapeKitUnity
{
    internal static class ScapeLogging
    {
        internal static void Log(string tag = "SCKUnity_", string message = "")
        {
#if UNITY_IPHONE && !UNITY_EDITOR
            ScapeLoggingBridge._log(tag, message);
#elif UNITY_ANDROID && !UNITY_EDITOR
            ScapeLoggingBridge._log(tag, message);
#endif
        }

        private static class ScapeLoggingBridge
        {
#if UNITY_IPHONE && !UNITY_EDITOR
            [DllImport("__Internal")]
            public static extern void _log([MarshalAs(UnmanagedType.LPStr)]string tag, [MarshalAs(UnmanagedType.LPStr)]string message);
#elif UNITY_ANDROID && !UNITY_EDITOR
            internal static void _log(string tag, string message)
            {
                using (AndroidJavaClass loggingUtils = new AndroidJavaClass("com.scape.scapekit.internal.utils.LoggingUtils"))
                {
                    loggingUtils.CallStatic("loginfo", tag, message);
                }
            }
#endif
        }
    }
}