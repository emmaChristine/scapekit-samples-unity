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
    using UnityEditor;
    using UnityEngine;

    /// <summary>
    /// class encapsualting all scapekit functionality
    /// </summary>
    public abstract class ScapeClient : MonoBehaviour
    {
        /// <summary>
        /// static instance of the client
        /// </summary>
        private static ScapeClient instance = null;

        /// <summary>
        /// static bool init flag
        /// Unity overrides == operator for MonoBehaviour
        /// </summary>
        private static bool instanceIsInitialized = false;

        /// <summary>
        /// the filename containing the scapeAPIkey
        /// </summary>
        private static string apikeyFileName = "ScapeAPIKey";

        /// <summary>
        /// the filepath containing the scapeAPIkey file
        /// </summary>
        private static string resPath = "Assets/Resources/";

        /// <summary>
        /// the scape api key.
        /// This should be entered through the unity gui
        /// Unity Menu SCapekit -> Account  
        /// </summary>
        private string apiKey = "XXX";     

        /// <summary>
        /// event to listen ClientStartedEvent
        /// </summary>
        public event Action ClientStartedEvent;
        
        /// <summary>
        /// event to listen ClientStoppedEvent
        /// </summary>
        public event Action ClientStoppedEvent;
        
        /// <summary>
        /// event to listen ClientFailedEvent
        /// </summary>
        public event Action<string> ClientFailedEvent;

        /// <summary>
        /// Gets static instance of the client
        /// </summary>
        public static ScapeClient Instance
        {
            get
            {
                if (instanceIsInitialized == false)
                {
                    instance = ScapeClientCInterface.create();
                    instanceIsInitialized = true;
                }

                return instance;
            }
        }

        /// <summary>
        /// Gets the instance of the ScapeSession
        /// </summary>
        public abstract ScapeSession ScapeSession
        {
            get;
        }

        /// <summary>
        /// Gets the instance of the DebugSession
        /// </summary>
        public abstract DebugSession DebugSession
        {
            get;
        }

        /// <summary>
        /// Gets or sets the apiKey
        /// </summary>
        protected virtual string ApiKey
        {
            get
            {
                return this.apiKey;
            }

            set
            {
                this.apiKey = value;
            }
        }

        /// <summary>
        /// save the api key to the specific file in resources folder
        /// </summary>
        /// <param name="apiKey">
        /// the apikey as string
        /// </param>
        public static void SaveApiKeyToResource(string apiKey) 
        {
            try
            {
                if (apiKey.Length == 0) 
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
            catch (Exception e)
            {
                ScapeLogging.LogError(message: "Failed to save apikey to '" + resPath + "'");
            }
        }

        /// <summary>
        /// used at runtime to retrieve the api key for the ScapeClient
        /// </summary>
        /// <returns>
        /// returns the apikey if found
        /// </returns>
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
                return string.Empty;
            }
        }

        /// <summary>
        /// shuts down the client on application quit
        /// </summary>
        public void OnApplicationQuit()
        {
            this.Terminate();
        }

        /// <summary>
        /// Terminate ScapeClient
        /// </summary>
        public virtual void Terminate()
        {
        }

        /// <summary>
        /// configures the client to use the scapeAPIkey saved using the Unity GUI
        /// </summary>
        /// <returns>
        /// returns this ScapeClient to allow for single line builder pattern
        /// </returns>
        public ScapeClient WithResApiKey()
        {
            this.WithApiKey(ScapeClient.RetrieveKeyFromResources());
            return this;
        }

        /// <summary>
        /// configrue api key to use
        /// </summary>
        /// <param name="apiKey">
        /// input apiKey as string
        /// </param>
        /// <returns>
        /// returns this ScapeClient to allow for single line builder pattern
        /// </returns>
        public abstract ScapeClient WithApiKey(string apiKey);

        /// <summary>
        /// create the DebugSession to use
        /// </summary>
        /// <param name="isSupported">
        /// boolean to specify whether to create the DebugSession or not.
        /// </param>
        /// <returns>
        /// returns this ScapeClient to allow for single line builder pattern
        /// </returns>
        public abstract ScapeClient WithDebugSupport(bool isSupported);

        /// <summary>
        /// start the client
        /// </summary>
        public abstract void StartClient();

        /// <summary>
        /// stop the client [deprecated]
        /// </summary>
        public abstract void StopClient();

        /// <summary>
        /// check whether the client has been started
        /// this must be true before the DebugSession or ScapeSession can be used
        /// </summary>
        /// <returns>
        /// boolean indicating whether the client is ready to be used
        /// </returns>
        public abstract bool IsStarted();

        /// <summary>
        /// OnClientStartedEvent function called by subclass to trigger event
        /// </summary>
        internal void OnClientStartedEvent()
        {
            if (this.ClientStartedEvent != null)
            {
                this.ClientStartedEvent();
            }
        }

        /// <summary>
        /// OnClientStoppedEvent function called by subclass to trigger event
        /// </summary>
        internal void OnClientStoppedEvent()
        {
            if (this.ClientStoppedEvent != null)
            {
                this.ClientStoppedEvent();
            }
        }

        /// <summary>
        /// OnClientFailedEvent function called by subclass to trigger event
        /// </summary>
        /// <param name="err">
        /// A string detailing reason for client error
        /// </param>
        internal void OnClientFailedEvent(string err)
        {
            if (this.ClientFailedEvent != null)
            {
                this.ClientFailedEvent(err);
            }
        }
    }
}
