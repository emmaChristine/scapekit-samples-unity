//  <copyright file="ScapeClientAndroid.cs" company="Scape Technologies Limited">
//
//  ScapeClientAndroid.cs
//  ScapeKitUnity
//
//  Created by nick on 1/5/2019.
//  Copyright © 2019 Scape Technologies Limited. All rights reserved.
//  </copyright>

#if UNITY_ANDROID && !UNITY_EDITOR
using UnityEngine.Android;
#endif

namespace ScapeKitUnity
{
    using System;
    using System.Collections;
    using System.Runtime.InteropServices;
    using UnityEngine;

#if UNITY_ANDROID && !UNITY_EDITOR
    internal sealed class ScapeClientAndroid
    {
        // Explicit static constructor to tell C# compiler
        // not to mark type as beforefieldinit
        static ScapeClientAndroid()
        {
        }

        private static readonly ScapeClientAndroid instance = new ScapeClientAndroid();
        internal static ScapeClientAndroid Instance
        {
            get
            {
                return instance;
            }
        }

        private AndroidJavaObject playerActivityContext;
        private ScapeClientObserverAndroid scapeClientObserver;
        private string apiKey = "";
        private bool isDebugEnabled = false;
        private int logOutput = 0;
        private int logConfig = 0;
        
        internal AndroidJavaObject ScapeClientJava;
        internal event Action ClientStartedEvent, ClientStoppedEvent;
        internal event Action<string> ClientFailedEvent;

        private ScapeClientAndroid()
        {
            ScapeLogging.LogDebug(message: " Create ScapeClientAndroid instance");

            using (var unityPlayerClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
            {
                playerActivityContext = unityPlayerClass.GetStatic<AndroidJavaObject>("currentActivity");
            }
        }

        internal void WithApiKey(string key)
        {
            this.apiKey = key;
        }

        internal void WithDebugSupport(bool isSupported)
        {   
            this.isDebugEnabled = isSupported;
        }

        internal void StartClient()
        {

            if (!Permission.HasUserAuthorizedPermission(Permission.FineLocation))
            {
                ScapeLogging.LogDebug(message: "Cannot start ScapeClient, reason: Missing Fine Location permisssions");
                return;
            }

            if(playerActivityContext == null)
            {
                ScapeLogging.LogDebug(message: "Cannot start ScapeClient, reason: Unity player activity context is null");
                if(ClientFailedEvent != null) 
                {
                    ClientFailedEvent("Cannot start ScapeClient, reason: Unity player activity context is null");
                    return;
                }
                return;
            }

            if(String.IsNullOrEmpty(this.apiKey))
            {
                ScapeLogging.LogDebug(message: "Cannot start ScapeClient, reason: api key is empty");
                if(ClientFailedEvent != null) 
                {
                    ClientFailedEvent("Cannot start ScapeClient, reason: api key is empty");
                    return;
                }
                return;
            }
            using (AndroidJavaClass Scape = new AndroidJavaClass("com.scape.scapekit.Scape"))
            {
                using (AndroidJavaObject scapeClientBuilder = Scape.CallStatic<AndroidJavaObject>("getScapeClientBuilder"))
                {
                    using (AndroidJavaObject scapeBuilder = scapeClientBuilder.Call<AndroidJavaObject>("withDebugSupport", isDebugEnabled).Call<AndroidJavaObject>("withApiKey", apiKey).Call<AndroidJavaObject>("withContext", playerActivityContext))
                    {
                        ScapeClientJava = scapeBuilder.Call<AndroidJavaObject>("build");
                        scapeClientObserver = new ScapeClientObserverAndroid(this);
                        ScapeClientJava.Call("start", scapeClientObserver);

                        ScapeLogging.LogDebug(message: "StartClient Android successful");
                    }
                }
            }
        }

        internal void StopClient()
        {
            if (ScapeClientJava != null)
            {
                ScapeClientJava.Call("stop", scapeClientObserver);
            }
        }

        internal bool IsStarted()
        {
            if (ScapeClientJava != null)
            {
                return ScapeClientJava.Call<bool>("isStarted");
            }
            return false;
        }

        internal void Terminate()
        {
            if (ScapeClientJava != null)
            {
                ScapeClientJava.Call("terminate", scapeClientObserver);
            }
        }

        internal void OnClientStarted()
        {
            ScapeLogging.LogDebug(message: "OnClientStarted");
        
            if (ClientStartedEvent != null)
                ClientStartedEvent();
        }

        internal void OnClientStopped()
        {
            ScapeLogging.LogDebug(message: "OnClientStopped");

            if (ClientStoppedEvent != null)
                ClientStoppedEvent();
        }

        internal void OnClientError(string error)
        {
            ScapeLogging.LogDebug(message: "OnClientError");

            if (ClientFailedEvent != null)
                ClientFailedEvent(error);
        }

        internal void SetClientEvents(Action started, Action stopped, Action<string> error)
        {
            this.ClientStartedEvent = started;
            this.ClientStoppedEvent = stopped;
            this.ClientFailedEvent = error;
        }

        internal AndroidJavaObject GetDebugSession() {
            if(isDebugEnabled) {
                return ScapeClientJava.Call<AndroidJavaObject>("getDebugSession");
            }
            else {
                Debug.Log("Error: Attempting to aquire DebugSession from ScapeClient but debug support is not enabled.");
                return null;
            }
        }
    }

    internal class ScapeClientObserverAndroid : AndroidJavaProxy
    {
        private ScapeClientAndroid scapeClientAndroid;

        public ScapeClientObserverAndroid(ScapeClientAndroid scapeClientAndroid) : base("com.scape.scapekit.ScapeClientObserver")
        {
            this.scapeClientAndroid = scapeClientAndroid;
        }

        public void onClientStarted(AndroidJavaObject scapeClient)
        {
            this.scapeClientAndroid.OnClientStarted();
        }

        public void onClientStopped(AndroidJavaObject scapeClient)
        {
            this.scapeClientAndroid.OnClientStopped();
        }

        public void onClientFailed(AndroidJavaObject scapeClient, string error)
        {
            this.scapeClientAndroid.OnClientError(error);
        }
    }
#endif
}