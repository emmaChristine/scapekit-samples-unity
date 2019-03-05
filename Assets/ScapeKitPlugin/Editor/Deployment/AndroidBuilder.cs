using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Threading;
using System.Text;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.IO;
using System.Linq;

namespace ScapeKitUnity 
{
	[InitializeOnLoad]
	internal class AndroidBuilder 
	{
		private static string keyJdk = @"JdkPath";
    	private static string keyAndroidSdk = @"AndroidSdkRoot";
    	private static string keyAndroidNdk = @"AndroidNdkRoot";

		private static string appPackageName = PlayerSettings.applicationIdentifier;
		private static string adbPath = string.Empty;
		private static string gradlePath = @"Assets/ScapeKitPlugin/Editor/Tools/Android/gradlew";

		private static string currentDirectory = Directory.GetCurrentDirectory() + "/";

		internal static AndroidBuilder Instance;

		internal string AndroidProjectPath = @"Build/Android/" + PlayerSettings.productName;
		internal string AppPath = @"Build/Android/" + PlayerSettings.productName + @"/build/outputs/apk/debug/" + PlayerSettings.productName + @"-debug.apk";
	  	internal Dictionary<string, AndroidLocalDeviceInfo> Devices;

		internal bool AppAlreadyBuilt 
		{
			get 
			{
				return AppPath != null && AppPath.Contains(".apk") && File.Exists(currentDirectory + AppPath);
			}
		}

		internal bool ProjectAlreadyGenerated
		{
			get
			{
				return AndroidProjectPath != null && File.Exists(currentDirectory + AndroidProjectPath + @"/build.gradle");
			}
		}

		internal string JdkPath
		{
			get
			{
		        return EditorPrefs.HasKey(keyJdk) ? EditorPrefs.GetString(keyJdk) : string.Empty;
			}
			set
			{
				EditorPrefs.SetString(keyJdk, value);
			}
		}

		internal string AndroidSdkPath
		{
			get
			{
		        return EditorPrefs.HasKey(keyAndroidSdk) ? EditorPrefs.GetString(keyAndroidSdk) : string.Empty;
			}
			set
			{
				EditorPrefs.SetString(keyAndroidSdk, value);
			}
		}

		internal string AndroidNdkPath
		{
			get
			{
		        return EditorPrefs.HasKey(keyAndroidNdk) ? EditorPrefs.GetString(keyAndroidNdk) : string.Empty;
			}
			set
			{
				EditorPrefs.SetString(keyAndroidNdk, value);
			}
		}

		internal class AndroidLocalDeviceInfo
		{
			internal string name;
			internal string udid;
		}

		internal AndroidBuilder()
		{
			adbPath = AndroidSdkPath + "/platform-tools/adb";

			Devices = new Dictionary<string, AndroidLocalDeviceInfo>();

			Instance = this;
		}

		internal void BuildAndDeployToDevice(AndroidLocalDeviceInfo device) 
		{
			try
			{
				BuildUnityProjectToAndroidStudio();
			} catch(System.Exception e)
			{
				UnityEngine.Debug.LogError(e);
			}

			try
			{
				BuildGeneratedAndroidProject(false, () => {
					try 
					{
						DeployApp(device);
					} catch(System.Exception e)
					{
						UnityEngine.Debug.LogError(e);
					}
				});
			} catch(System.Exception e)
			{
				UnityEngine.Debug.LogError(e);
			}
		}

		internal void BuildUnityProjectToAndroidStudio()
		{
			Debug.Log("Begin BuildUnityProjectToAndroidStudio");

			try
			{
				AppExporter.PerformAndroidBuild();
			} catch(System.Exception e)
			{
				UnityEngine.Debug.LogError(e);
			}

			Debug.Log("End BuildUnityProjectToAndroidStudio");
		}

		// Build .app with gradle
		internal void BuildGeneratedAndroidProject(bool isAsync = false, Action completed = null)
		{
			Debug.Log("Begin BuildGeneratedAndroidProject");

			EditorUtility.DisplayProgressBar("Building", "Building application, please wait..", 0.5f);

			string command = gradlePath + @" -p " + "\"" + AndroidProjectPath + "\"" + " assembleDebug";
			command.ExecuteBash(isAsync,
			(result) => {
					Debug.Log("BuildGeneratedAndroidProject success:\n" + result);

					if(completed != null)
					{
						completed();
					}
			},
			(error) => {
				Debug.LogError("BuildGeneratedAndroidProject failed:\n" + error);
			});

			Debug.Log("End BuildGeneratedAndroidProject");

			EditorUtility.ClearProgressBar();
			GUIUtility.ExitGUI();
		}

		internal void DeployToAllDevices()
		{
			foreach (var d in Devices) 
			{
				DeployApp(d.Value);
			}
		}

		// deploy to any connected device using adb
		internal void DeployApp(AndroidLocalDeviceInfo device, bool isAsync = false) 
		{
			Debug.Log("Begin DeployApp");

			EditorUtility.DisplayProgressBar("Deploying", "Deploying application, please wait..", 0.5f);

			if(adbPath.IsNullOrEmpty()) 
			{
				UnityEngine.Debug.LogError ("Cannot run the app, adb path not set");
				return;
			}

	    	var fAppPath = Directory.GetCurrentDirectory() + "/" + AppPath;

	    	if (!AppAlreadyBuilt) 
	    	{
	      		UnityEngine.Debug.LogError("App not yet built! \n please build the app before trying to deploy it");
	      		return;
	    	} 

			Debug.Log ("Deploying app " + AppPath);

			//TODO add timeout i.e if app has not responded for >10 seconds just say that it failed.

			string command = gradlePath + @" -p " + "\"" + AndroidProjectPath + "\"" + " installDebug";
			command.ExecuteBash(isAsync,
			(result) => {
					Debug.Log(result);

					string command2 = adbPath + @" shell monkey -p " + appPackageName + " 1";
					command2.ExecuteBash(isAsync,
					(result2) => {
							Debug.Log(result2);

					},
					(error2) => {
						Debug.LogError(error2);

					});
			},
			(error) => {
				Debug.LogError(error);

			});

			Debug.Log("End DeployApp");

			EditorUtility.ClearProgressBar();
			GUIUtility.ExitGUI();
		}

		internal void DeployTimeoutChecker( System.Diagnostics.Process proc, AndroidLocalDeviceInfo device,  float timeout)
		{
			ThreadStart ths = new ThreadStart (delegate() 
			{ 
				int timeStep = 500;
				int msTimeout = (int)(timeout * 1000);
				int it = 0;

				while (proc == null || proc.HasExited) 
				{
					if (proc != null && it > msTimeout) 
					{
						Debug.LogWarning("Deployment process timed out!");

						proc.Kill();
					}

					Thread.Sleep (timeStep);
					it += timeStep;
				}
			});

			var monitorThread = new Thread(ths);
			monitorThread.Start();
		}

		internal void GetAllConnectedDevices(bool isAsync = true)
		{
			if(adbPath.IsNullOrEmpty()) 
			{
				UnityEngine.Debug.LogError ("Cannot find connected devices, adb path not set");
				return;
			}

			string command = adbPath + @" shell getprop ro.serialno";
			command.ExecuteBash(isAsync, 
				(deviceID) => {

				if(deviceID == " ") // ExecuteBash returns a whitespace string by default
				{
					return;
				}

				string command2 = adbPath + @" shell getprop ro.product.model";
				command2.ExecuteBash(isAsync,
					(modelName) => {
						string command3 = adbPath + @" shell getprop ro.product.manufacturer";
						command3.ExecuteBash(isAsync, 
							(manufacturerName) => {
								AddDevice(deviceID, manufacturerName + " " + modelName);
							},
							(error) => {

							});
					},
					(error) => {
						Debug.LogError(error);
					});
				}, 
				(error) => {
					Debug.LogError(error);
				});
		}

		internal void AddDevice(string udid, string name )
		{
			if (Devices.ContainsKey(udid)) 
			{
				//TODO upate info and remove all Devices that were disconnected
				//Devices.Remove(udid);
			}
			else 
			{
				var info = new AndroidLocalDeviceInfo();
				info.name = name;
				info.udid = udid;

				Devices[udid] = info;
			}
		}
	}
}
