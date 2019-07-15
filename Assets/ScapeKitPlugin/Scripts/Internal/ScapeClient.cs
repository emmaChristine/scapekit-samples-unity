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
    using UnityEditor;

    public sealed class ScapeClient
    {
        internal event Action ClientStartedEvent, ClientStoppedEvent;
        internal event Action<string> ClientFailedEvent;

        static string apikeyFileName = "ScapeAPIKey";
        static string resPath = "Assets/Resources/";
        string apiKey = "XXX";

        private static bool setAndroidClientEvents = false;

        private static ScapeClient _instance = null;
        public static ScapeClient Instance
        {
            get
            {
                if(_instance == null)
                {
                    Debug.Log("ScapeClient create instance");
                    _instance = new ScapeClient();
                }

#if UNITY_ANDROID && !UNITY_EDITOR
                if(setAndroidClientEvents == false) {
                    Debug.Log("instance.SetClientEvents");
                    var instance = ScapeClientAndroid.Instance;
                    instance.SetClientEvents(_instance.ClientStartedEvent, _instance.ClientStoppedEvent, _instance.ClientFailedEvent);
                    setAndroidClientEvents = true;
                }
#endif
                return _instance;
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
            return WithApiKey(apiKey);
        }

        /// <summary>
        /// 
        /// </summary>
        public ScapeClient WithApiKey(string apiKey)
        {
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
            Debug.Log("Start Scape Client");
#if (UNITY_IPHONE || UNITY_ANDROID) && !UNITY_EDITOR
            ScapeClientBridge._start();
#endif
        }

        /// <summary>
        /// 
        /// </summary>
        public void StopClient()
        {
            Debug.Log(message: "Stop Scape Client");
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
                if(apiKey.Length == 0) 
                {
                    return;
                }
                if (!Directory.Exists(resPath))
                {
                    Directory.CreateDirectory(resPath);
                }
                
                using (StreamWriter writer = new StreamWriter(resPath + apikeyFileName + ".txt", false))
                {
                    writer.WriteLine(apiKey.Trim());
                }
            }
            catch(Exception e)
            {
                ScapeLogging.LogError(message: "Failed to save apikey to '" + resPath + "'");
            }
        }

        public static string RetrieveKeyFromResources()
        {
            try
            {
            #if UNITY_EDITOR
                using (StreamReader streamReader = new StreamReader(resPath + apikeyFileName + ".txt")) 
                {
                    string apiKey = streamReader.ReadLine();
                
                    return apiKey;
                }
            #else
                string apiKey = Resources.Load<TextAsset>(apikeyFileName).ToString();

                return apiKey;
            #endif
            }
            catch (Exception ex)
            {
                ScapeLogging.LogError("Exception retrieving apikey: " + ex.ToString());
                return "";
            }
        }
    }
}
