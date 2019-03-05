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
	internal class IOSBuilder 
	{
		private static string keyTeamID = @"ScapeTeamDevelopmentID";

		private static string appName = Regex.Matches(PlayerSettings.applicationIdentifier, @"(.*)\.(.*)")[0].Groups[2].Value;
		private static string iosDeployPath = "Assets/ScapeKitPlugin/Editor/Tools/iOS/ios-deploy";

		private static string currentDirectory = Directory.GetCurrentDirectory() + "/";

		internal static IOSBuilder Instance;

		internal string XcodeProjectPath = @"Build/iOS/Unity-iPhone.xcodeproj";
		internal string AppPath = @"Build/iOS/build/Products/Debug-iphoneos/" + appName + @".app";
		internal Dictionary<string, IOSLocalDeviceInfo> Devices;

		internal bool AppAlreadyBuilt 
		{
			get 
			{
				return AppPath != null && AppPath.Contains(".app") && File.Exists(currentDirectory + AppPath + "/Info.plist");
			}
		}

		internal bool ProjectAlreadyGenerated
		{
			get
			{
				return XcodeProjectPath != null && File.Exists(currentDirectory + XcodeProjectPath + @"/project.pbxproj");
			}
		}

		internal string DevelopmentID
		{
			get
			{
		        return EditorPrefs.HasKey(keyTeamID) ? EditorPrefs.GetString(keyTeamID) : string.Empty;
			}
			set
			{
				EditorPrefs.SetString(keyTeamID, value);
			}
		}

		internal class IOSLocalDeviceInfo
		{
			internal string name;
			internal string udid;
		}

		internal IOSBuilder()
		{
			Devices = new Dictionary<string, IOSLocalDeviceInfo>();

			Instance = this;
		}

		internal void BuildAndDeployToDevice(IOSLocalDeviceInfo device) 
		{
			try
			{
				BuildUnityProjectToXcode();
			} catch(System.Exception e)
			{
				UnityEngine.Debug.LogError(e);
			}

			try
			{
				BuildGeneratedXcodeProject(false, () => {
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

		internal void BuildUnityProjectToXcode()
		{
			Debug.Log("Begin BuildUnityProjectToXcode");

			AppExporter.PerformIOSBuild(DevelopmentID);

			Debug.Log("End BuildUnityProjectToXcode");
		}

		// Build .app with xcodebuild
		internal void BuildGeneratedXcodeProject(bool isAsync = false, Action completed = null)
		{
			Debug.Log("Begin BuildGeneratedXcodeProject");

			EditorUtility.DisplayProgressBar("Building", "Building application, please wait..", 0.5f);

			string command = @"xcodebuild -project " + XcodeProjectPath + " -scheme Unity-iPhone -configuration Debug -sdk iphoneos -arch arm64";
			command.ExecuteBash(isAsync,
			(result) => {
					Debug.Log("BuildGeneratedXcodeProject success:\n" + result);

					if(completed != null)
					{
						completed();
					}
			},
			(error) => {
				Debug.LogError("BuildGeneratedXcodeProject failed:\n" + error);
			});

			Debug.Log("End BuildGeneratedXcodeProject");

			EditorUtility.ClearProgressBar();
			GUIUtility.ExitGUI();
		}

		internal void DeployToAllDevices(bool useAsync = true)
		{
			foreach (var d in Devices) 
			{
				DeployApp(d.Value, useAsync);
			}
		}

		// deploy to any connected device using ios-deploy
		internal void DeployApp(IOSLocalDeviceInfo device, bool isAsync = false) 
		{
			Debug.Log("Begin DeployApp");

			EditorUtility.DisplayProgressBar("Deploying", "Deploying application, please wait..", 0.5f);

			if(iosDeployPath.IsNullOrEmpty()) 
			{
				UnityEngine.Debug.LogError ("Cannot run the app, ios-deploy path not set");
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

			string command = string.Format(@"{0} --debug --bundle {1} --id {2} --uninstall --no-wifi --justlaunch --noninteractive ", iosDeployPath, AppPath, device.udid);
			command.ExecuteBash(isAsync,
			(result) => {
					Debug.Log(result);
			},
			(error) => {
				Debug.LogError(error);
			});

			Debug.Log("End DeployApp");

			EditorUtility.ClearProgressBar();
			GUIUtility.ExitGUI();
		}

		internal void DeployTimeoutChecker( System.Diagnostics.Process proc, IOSLocalDeviceInfo device,  float timeout)
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

		internal void GetAllConnectedDevices(bool isAsync = false)
		{
			Debug.Log("Begin GetAllConnectedDevices");

			EditorUtility.DisplayProgressBar("Searching", "Looking for connected iOS devices, please wait..", 0.5f);

			if(!File.Exists(currentDirectory + "/" + iosDeployPath)) 
			{
				UnityEngine.Debug.LogError ("Cannot find connected devices, ios-deploy is not present");
				return;
			}

			string command = iosDeployPath + @" -c | grep -w 'Found' | awk '{print $3;}'";
			command.ExecuteBash(isAsync, 
				(deviceID) => {

				if(deviceID == " ") // ExecuteBash returns a whitespace string by default
				{
					return;
				}

				string command2 = iosDeployPath + @" -c | grep -w 'Found' | awk '{print $5;}'";
				command2.ExecuteBash(isAsync,
					(modelName) => {
						string command3 = iosDeployPath + @" -c | grep -w 'Found' | awk '{print $6;}'";
						command3.ExecuteBash(isAsync, 
							(modelVersion) => {
								AddDevice(deviceID, modelName + " " + modelVersion);
							},
							(error) => {
								Debug.LogError(error);
							});
					},
					(error) => {
						Debug.LogError(error);
					});
				}, 
				(error) => {
					Debug.LogError(error);
				});

			HideProgressBar();

			Debug.Log("End GetAllConnectedDevices");
		}

		internal void AddDevice (string udid, string name )
		{
			if (Devices.ContainsKey (udid)) 
			{
				//TODO upate info and remove all Devices that were disconnected
			}
			else 
			{
				var info = new IOSLocalDeviceInfo ();
				info.name = name;
				info.udid = udid;

				Devices [udid] = info;
			}
		}

		private void HideProgressBar() 
		{
			EditorUtility.ClearProgressBar();
			GUIUtility.ExitGUI();
		}
	}
}
