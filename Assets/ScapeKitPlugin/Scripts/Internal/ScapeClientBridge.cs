//  <copyright file="ScapeClientBridge.cs" company="Scape Technologies Limited">
//
//  ScapeClientBridge.cs
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
    
    internal sealed class ScapeClientBridge
    {
        // On iOS plugins are statically linked into
        // the executable, so we have to use __Internal as the
        // library name.
#if UNITY_IPHONE && !UNITY_EDITOR
        [DllImport("__Internal")]
        public static extern void _withApiKey([MarshalAs(UnmanagedType.LPStr)] string apiKey);

        [DllImport("__Internal")]
        public static extern void _withDebugSupport(bool isSupported);

        [DllImport("__Internal")]
        public static extern void _start();
        
        [DllImport("__Internal")]
        public static extern void _stop();

        [DllImport("__Internal")]
        public static extern bool _isStarted();

        [DllImport("__Internal")]
        public static extern void _terminate();
#elif UNITY_ANDROID && !UNITY_EDITOR
        public static void _withApiKey(string apiKey)
        {
            ScapeClientAndroid.Instance.WithApiKey(apiKey);
        }

        public static void _withDebugSupport(bool isDebug) 
        {
            ScapeClientAndroid.Instance.WithDebugSupport(isDebug);
        }

        public static void _start()
        {
            ScapeClientAndroid.Instance.StartClient();
        }

        public static void _stop()
        {
            ScapeClientAndroid.Instance.StopClient();
        }

        public static bool _isStarted()
        {
            return ScapeClientAndroid.Instance.IsStarted();
        }

        public static void _terminate()
        {
            ScapeClientAndroid.Instance.Terminate();
        }
#endif
    }
}