//  <copyright file="ScapeClient.cs" company="Scape Technologies Limited">
//
//  ScapeClient.cs
//  ScapeKitUnity
//
//  Created by nick on 1/5/2019.
//  Copyright © 2019 Scape Technologies Limited. All rights reserved.
//  </copyright>

namespace ScapeKitUnity
{
    using System;
    using System.IO;
    using System.Runtime.InteropServices;
    using UnityEngine;

    public sealed class ScapeClient : MonoBehaviourSingleton<ScapeClient>
    {
        internal event Action ClientStartedEvent, ClientStoppedEvent;
        internal event Action<string> ClientFailedEvent;

        static string apikeyFileName = "ScapeAPIKey";
        static string resPath = "Assets/Resources/";
        string apiKey = "XXX";

        private static bool setAndroidClientEvents = false;

        public static ScapeClient Instance
        {
            get
            {
                var scapeClientInstance = BehaviourInstance as ScapeClient;
#if UNITY_ANDROID && !UNITY_EDITOR
                if(setAndroidClientEvents == false) {
                    Debug.Log("instance.SetClientEvents");
                    var instance = ScapeClientAndroid.Instance;
                    instance.SetClientEvents(scapeClientInstance.ClientStartedEvent, scapeClientInstance.ClientStoppedEvent, scapeClientInstance.ClientFailedEvent);
                    setAndroidClientEvents = true;
                }
#endif
                return scapeClientInstance;
            }
        }

        public ScapeSession ScapeSession 
        {
            get
            {
                return ScapeSession.Instance;
            }
        }

        void OnApplicationQuit()
        {
            Terminate();
        }


        /// <summary>
        /// Terminate ScapeClient
        /// </summary>
        public void Terminate()
        {
            ScapeLogging.LogDebug(message: "Terminate ScapeKit");
#if (UNITY_IPHONE || UNITY_ANDROID) && !UNITY_EDITOR
            ScapeClientBridge._terminate();
#endif
        }

        public ScapeClient WithResApiKey()
        {
            string apiKey = RetrieveKeyFromResources();
            ScapeLogging.LogDebug("Retrieved ApiKey = " + apiKey);
            return WithApiKey(apiKey);
        }

        /// <summary>
        /// 
        /// </summary>
        public ScapeClient WithApiKey(string apiKey)
        {
            ScapeLogging.LogDebug(message: "With api key " + apiKey);
            this.apiKey = apiKey;

#if (UNITY_IPHONE || UNITY_ANDROID) && !UNITY_EDITOR
            ScapeClientBridge._withApiKey(this.apiKey);
#endif
            return Instance;
        }

        /// <summary>
        /// 
        /// </summary>
        public ScapeClient WithDebugSupport(bool isSupported)
        {
            ScapeLogging.LogDebug(message: "With debug support " + isSupported);

#if (UNITY_IPHONE || UNITY_ANDROID) && !UNITY_EDITOR
            ScapeClientBridge._withDebugSupport(isSupported);
#endif
            return Instance;
        }

        /// <summary>
        /// 
        /// </summary>
        public void StartClient()
        {
            ScapeLogging.LogDebug(message: "Start Scape Client");
#if (UNITY_IPHONE || UNITY_ANDROID) && !UNITY_EDITOR
            ScapeClientBridge._start();
#endif
        }

        /// <summary>
        /// 
        /// </summary>
        public void StopClient()
        {
            ScapeLogging.LogDebug(message: "Stop Scape Client");
#if (UNITY_IPHONE || UNITY_ANDROID) && !UNITY_EDITOR
            ScapeClientBridge._stop();
#endif
        }

        /// <summary>
        /// </summary>
        public bool IsStarted()
        {
            bool isStarted = false;
#if (UNITY_IPHONE || UNITY_ANDROID) && !UNITY_EDITOR
            isStarted = ScapeClientBridge._isStarted();  
#endif
            return isStarted;
        }

        // Used only on iOS via the C based UnitySendMessage function
#if UNITY_IPHONE && !UNITY_EDITOR
        private void OnClientStarted([MarshalAs(UnmanagedType.LPStr)] string ignore)
        {
            if (ClientStartedEvent != null)
                ClientStartedEvent();
        }

        private void OnClientStopped([MarshalAs(UnmanagedType.LPStr)] string ignore)
        {
            if (ClientStoppedEvent != null)
                ClientStoppedEvent();
        }

        private void OnClientFailed([MarshalAs(UnmanagedType.LPStr)] string error)
        {
            if (ClientFailedEvent != null)
                ClientFailedEvent(error);
        }
#endif
        public static void SaveApiKeyToResource(string apiKey) 
        {
            try
            {
                if (!Directory.Exists(resPath))
                {
                    Directory.CreateDirectory(resPath);
                }
                StreamWriter writer = new StreamWriter(resPath + apikeyFileName + ".txt", false);
                writer.WriteLine(apiKey.Trim());
                writer.Close();
            }
            catch(Exception e)
            {
                ScapeLogging.LogDebug(message: "Failed to save apikey to '" + resPath + "'");
            }
        }

        public static string RetrieveKeyFromResources()
        {
            try
            {
                string apiKey = Resources.Load<TextAsset>(apikeyFileName).ToString();

                return apiKey;
            }
            catch (Exception ex)
            {
                ScapeLogging.LogDebug("Exception retrieving apikey: " + ex.ToString());
                return "";
            }
        }
    }
}
