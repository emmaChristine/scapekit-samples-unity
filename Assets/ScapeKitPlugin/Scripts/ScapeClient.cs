using UnityEngine;
using System;
using System.Runtime.InteropServices;
using System.IO;

namespace ScapeKitUnity
{
    public sealed class ScapeClient : MonoBehaviourSingleton<ScapeClient>
    {
        private bool isDebugSupported = false;

        internal event Action ClientStartedEvent, ClientStoppedEvent;
        internal event Action<string> ClientFailedEvent;

        static string apikeyFileName = "ScapeAPIKey";
        static string resPath = "Assets/Resources/";
        string apiKey = "XXX";

        public static ScapeClient Instance
        {
            get
            {
                var scapeClientInstance = BehaviourInstance as ScapeClient;
#if UNITY_ANDROID && !UNITY_EDITOR
                var instance = ScapeClientAndroid.Instance;
                instance.SetClientEvents(scapeClientInstance.ClientStartedEvent, scapeClientInstance.ClientStoppedEvent, scapeClientInstance.ClientFailedEvent);
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
            ScapeLogging.Log(message: "Terminate ScapeKit");
#if (UNITY_IPHONE || UNITY_ANDROID) && !UNITY_EDITOR
            ScapeClientBridge._terminate();
#endif
        }

        public ScapeClient WithResApiKey()
        {
            string apiKey = RetrieveKeyFromResources();
            ScapeLogging.Log("Retrieved ApiKey = " + apiKey);
            return WithApiKey(apiKey);
        }

        /// <summary>
        /// 
        /// </summary>
        public ScapeClient WithApiKey(string apiKey)
        {
            ScapeLogging.Log(message: "With api key " + apiKey);
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
            ScapeLogging.Log(message: "With debug support " + isSupported);
            this.isDebugSupported = isSupported;

#if (UNITY_IPHONE || UNITY_ANDROID) && !UNITY_EDITOR
            ScapeClientBridge._withDebugSupport(this.isDebugSupported);
#endif
            return Instance;
        }

        /// <summary>
        /// 
        /// </summary>
        public void StartClient()
        {
            ScapeLogging.Log(message: "Start Scape Client");
#if (UNITY_IPHONE || UNITY_ANDROID) && !UNITY_EDITOR
            ScapeClientBridge._start();
#endif
        }

        /// <summary>
        /// 
        /// </summary>
        public void StopClient()
        {
            ScapeLogging.Log(message: "Stop Scape Client");
#if (UNITY_IPHONE || UNITY_ANDROID) && !UNITY_EDITOR
            ScapeClientBridge._stop();
#endif
        }

        /// <summary>
        /// </summary>
        public bool IsStarted()
        {
            ScapeLogging.Log(message: "Scape Client isStarted");
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
                ScapeLogging.Log(message: "Failed to save apikey to '" + resPath + "'");
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
                ScapeLogging.Log("Exception retrieving apikey: " + ex.ToString());
                return "";
            }
        }
    }
}
