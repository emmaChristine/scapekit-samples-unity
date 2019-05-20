//  <copyright file="ScapeSessionBridge.cs" company="Scape Technologies Limited">
//
//  ScapeSessionBridge.cs
//  ScapeKitUnity
//
//  Created by nick on 1/5/2019.
//  Copyright © 2019 Scape Technologies Limited. All rights reserved.
//  </copyright>

namespace ScapeKitUnity
{
	using System;
	using System.Collections;
	using System.Runtime.InteropServices;
	using UnityEngine;
	
    sealed class ScapeSessionBridge
    {
#if UNITY_IPHONE && !UNITY_EDITOR
    	[DllImport("__Internal")]
        public static extern void _getMeasurements();
#elif UNITY_ANDROID && !UNITY_EDITOR
        public static void _getMeasurements() 
        {
            ScapeSessionAndroid.Instance.GetMeasurements();
        }
#endif
    }
}