

namespace ScapeKitUnity
{
    using System;
    using System.Text;
    using System.Collections;
    using System.Runtime.InteropServices;
    using UnityEngine;
    using AOT;

    internal sealed class ScapeClientCInterface : ScapeClient
    {

    	IntPtr scapeClientPtr = IntPtr.Zero;
    	IntPtr debugSessionPtr = IntPtr.Zero;

        private int isDebugEnabled = 0;

        private static LocationInfo lastInfo;

        private static GameObject clientGameObject = null;

        internal static ScapeClientCInterface create()
        {
            Input.location.Start();

            clientGameObject = new GameObject("ScapeClient");

            return clientGameObject.AddComponent<ScapeClientCInterface>();
        }

        /// <summary>
        /// instance of the ScapeSession
        /// </summary>
        private ScapeSession scapeSession = null;

        /// <summary>
        /// instance of the DebugSession
        /// </summary>
        private DebugSession debugSession = null;  

        public override ScapeSession ScapeSession 
        {
        	get 
        	{
                if(this.scapeSession == null) 
                {
                    if(this.IsStarted() == false) 
                    {
        				Debug.Log("ScapeClient cannot get session before starting client!");
        				return null;
        			}

        			this.scapeSession = new ScapeSessionCInterface(this.scapeClientPtr);

                    if(this.scapeSession == null) 
                    {
                        Debug.Log("this.scapeSession did not get created");
                    }
        		}

        		return this.scapeSession;
        	}
        }

        public override DebugSession DebugSession 
        {
            get
            {
                if(debugSession == null) 
                {
                    debugSession = new DebugSessionCInterface(debugSessionPtr);
                }

                return debugSession;
            }
        }

        public override ScapeClient WithApiKey(string key)
        {
            this.ApiKey = key;

            return this;
        }

        public override ScapeClient WithDebugSupport(bool isSupported)
        {   
            this.isDebugEnabled = isSupported ? 1 : 0;

            return this;
        }

        public override void StartClient()
        {
        	if(this.scapeClientPtr == IntPtr.Zero)
        	{
	        	this.scapeClientPtr = ScapeCInterface.citf_createClient(this.ApiKey, this.isDebugEnabled);

	        	if(this.IsStarted()) 
                {
	        		ScapeCInterface.citf_setClientStateCallbacks(this.scapeClientPtr, 
                        onAquireMotionMeasurements, 
                        onAquireLocationMeasurements);

	        		if(this.isDebugEnabled > 0) 
                    {
                        this.debugSessionPtr = ScapeCInterface.citf_getDebugSession(this.scapeClientPtr);
	        		}

                    setDeviceInfo();
	        	}
        	} 
        }

        public override void StopClient() 
        {

        }

        public override bool IsStarted() 
        {
        	return this.scapeClientPtr != IntPtr.Zero;
        }

        public override void Terminate() 
        {	
        	if (this.IsStarted())
        	{
                if (this.scapeSession != null) 
                {
                    this.scapeSession.Terminate();
                }
        		ScapeCInterface.citf_destroyClient(this.scapeClientPtr);
        	}
        }

        byte[] stringToFixedByteArray(string str, int max_size)
        {
            int copy_size = str.Length < max_size ? str.Length : max_size;
            byte[] result = new byte[max_size];
            Encoding.UTF8.GetBytes(str, 0, copy_size, result, 0);

            return result;
        }

        private void setDeviceInfo()
        {
            ScapeCInterface.device_info di = new ScapeCInterface.device_info();

            di.id = stringToFixedByteArray(SystemInfo.deviceUniqueIdentifier, 256);
            di.platform = stringToFixedByteArray(Enum.GetName(typeof(RuntimePlatform), Application.platform), 256);
            di.model = stringToFixedByteArray(SystemInfo.deviceModel, 256);
            di.os = stringToFixedByteArray(SystemInfo.operatingSystem, 256);
            di.os_version = stringToFixedByteArray(Enum.GetName(typeof(OperatingSystemFamily), SystemInfo.operatingSystemFamily), 256);
            di.write_directory = stringToFixedByteArray(Application.persistentDataPath, 256);

            ScapeCInterface.citf_setDeviceInfo(this.scapeClientPtr, ref di);
        }
        
        [MonoPInvokeCallback (typeof(ScapeCInterface.onAquireMotionMeasurementsDelegate))]
		static void onAquireMotionMeasurements(ref ScapeCInterface.motion_measurements mm)
		{
            Debug.Log("onAquireMotionMeasurements");
		}

        [MonoPInvokeCallback (typeof(ScapeCInterface.onAquireLocationMeasurementsDelegate))]
  		static void onAquireLocationMeasurements(ref ScapeCInterface.location_measurements lm)
  		{
  			Debug.Log("onAquireLocationMeasurements");

            lm.longitude = lastInfo.longitude;
            lm.latitude = lastInfo.latitude;
  		}

        public void Update()
        { 
            if (this.IsStarted())
            {
                if (Input.location.status == LocationServiceStatus.Running) 
                {
                   lastInfo = Input.location.lastData;
                }
            }
        }
    }
}