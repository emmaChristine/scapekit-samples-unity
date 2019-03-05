using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;
using System;

namespace ScapeKitUnity
{
    sealed class ScapeSessionBridge
    {
#if UNITY_IPHONE && !UNITY_EDITOR
    	[DllImport("__Internal")]
        public static extern void _getMeasurements(int geoSourceType);
#elif UNITY_ANDROID && !UNITY_EDITOR
        public static void _getMeasurements(int geoSourceType) 
        {
            ScapeSessionAndroid.Instance.GetMeasurements((GeoSourceType)geoSourceType);
        }
#endif
    }
}