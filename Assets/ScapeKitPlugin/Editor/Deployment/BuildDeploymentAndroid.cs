using UnityEditor;
using UnityEngine;

namespace ScapeKitUnity
{
	internal static class BuildDeploymentAndroid
	{
		private static bool showAdvancedSettings = false;
        private static bool showConnectedDevices = true;
        private static bool showProjectPath = true;
        private static bool showAppPath = true;
        private static bool showAndroidSettings = true;

        private static string jdkPath = string.Empty;
        private static string sdkPath = string.Empty;
        private static string ndkPath = string.Empty;

		internal static void ShowSettings() 
        {
            if (AndroidBuilder.Instance == null) 
            {
                new AndroidBuilder();
            }


            FileSettings();

            AndroidSettings();

            DevicesSettings();

            AdvancedSettings();
        }

        private static void AndroidSettings()
        {
        	showAndroidSettings = EditorGUILayout.Foldout(showAndroidSettings, "Paths & Tools");
            if(showAndroidSettings)
            {
            	Rect PathTools = EditorGUILayout.BeginVertical("box");
            	{
            		Rect jdk = EditorGUILayout.BeginHorizontal("box");
                    {
                    	if(AndroidBuilder.Instance.JdkPath.IsNullOrEmpty())
                    	{
	                		GUILayout.Label("Enter your Java JDK path: ");
	                		jdkPath = EditorGUILayout.TextField(jdkPath);
	                		AndroidBuilder.Instance.JdkPath = jdkPath;
                    	} else 
                    	{
                    		GUILayout.Label("Java JDK path: ");
                    		jdkPath = EditorGUILayout.TextField(AndroidBuilder.Instance.JdkPath);
                    	}
                	}
                	EditorGUILayout.EndHorizontal();

                	Rect sdk = EditorGUILayout.BeginHorizontal("box");
                    {
                    	if(AndroidBuilder.Instance.AndroidSdkPath.IsNullOrEmpty())
                    	{
	                		GUILayout.Label("Enter your Android SDK path: ");
	                		sdkPath = EditorGUILayout.TextField(sdkPath);
	                		AndroidBuilder.Instance.AndroidSdkPath = sdkPath;
	                	} else 
	                	{
                    		GUILayout.Label("Android SDK path: ");
                    		sdkPath = EditorGUILayout.TextField(AndroidBuilder.Instance.AndroidSdkPath);
	                	}
                	}
                	EditorGUILayout.EndHorizontal();

                	Rect ndk = EditorGUILayout.BeginHorizontal("box");
                    {
                    	if(AndroidBuilder.Instance.AndroidNdkPath.IsNullOrEmpty())
                    	{
	                		GUILayout.Label("Enter your Android NDK path: ");
	                		ndkPath = EditorGUILayout.TextField(ndkPath);
	                		AndroidBuilder.Instance.AndroidNdkPath = ndkPath;
	                	} else 
	                	{
                    		GUILayout.Label("Android NDK path: ");
                    		ndkPath = EditorGUILayout.TextField(AndroidBuilder.Instance.AndroidNdkPath);
	                	}
                	}
                	EditorGUILayout.EndHorizontal();
            	}
            	EditorGUILayout.EndVertical();
            }
        }

		private static void AdvancedSettings()
        {
            showAdvancedSettings = EditorGUILayout.Foldout(showAdvancedSettings, "Advanced");
            if(showAdvancedSettings)
            {
                Rect ProjectBuilder = EditorGUILayout.BeginVertical("box");
                {
                    Rect studioBuilder = EditorGUILayout.BeginHorizontal("box");
                    {
                        GUILayout.Label ("Generate Android Studio project");
                        if (GUILayout.Button ("Generate")) 
                        {
                            AndroidBuilder.Instance.BuildUnityProjectToAndroidStudio();
                        }
                    }
                    EditorGUILayout.EndHorizontal();

                    if(AndroidBuilder.Instance.ProjectAlreadyGenerated)
                    {
                        Rect appBuilder = EditorGUILayout.BeginHorizontal("box");
                        {
                            GUILayout.Label("Build generated Android Studio project");
                            if (GUILayout.Button("Build")) 
                            {
                                AndroidBuilder.Instance.BuildGeneratedAndroidProject();
                            }
                        }
                        EditorGUILayout.EndHorizontal();
                    }
                }
                EditorGUILayout.EndVertical();
            }
        }

        private static void FileSettings()
        {
            if(AndroidBuilder.Instance.ProjectAlreadyGenerated)
            {
	           	showProjectPath = EditorGUILayout.Foldout(showProjectPath, "Project Path");
	            if(showProjectPath)
	            {
	                Rect appBuilder = EditorGUILayout.BeginHorizontal("box");
	                {
	                    GUILayout.Label(AndroidBuilder.Instance.AndroidProjectPath);
	                }
	                EditorGUILayout.EndHorizontal();
	            }
            }
            if(AndroidBuilder.Instance.AppAlreadyBuilt)
            {
            	showAppPath = EditorGUILayout.Foldout(showAppPath, "APK Path");
	            if(showAppPath)
	            {
	                Rect appBuilder = EditorGUILayout.BeginHorizontal("box");
	                {
	                    GUILayout.Label(AndroidBuilder.Instance.AppPath); 
	                }  
	                EditorGUILayout.EndHorizontal();
	            }
            }
        }

        private static void DevicesSettings()
        {
            showConnectedDevices = EditorGUILayout.Foldout(showConnectedDevices, string.Format("Connected Devices ({0}):", AndroidBuilder.Instance.Devices.Count));
            if(showConnectedDevices)
            {
                Rect DevicesInfo = EditorGUILayout.BeginVertical("box");
                {
                    GUIStyle style = new GUIStyle();
                    style.richText = true;

                    string buildLabel = "Build&Deploy";
                    // if the app is already built only deploy it
                    if (AndroidBuilder.Instance.AppAlreadyBuilt) 
                    {
                        buildLabel = "Deploy";
                    }

                    foreach (var d in AndroidBuilder.Instance.Devices) 
                    {
                        Rect CurrentDevice = EditorGUILayout.BeginHorizontal("box");
                        {
                            GUILayout.Label("<color=white>" + d.Value.name + "</color>" + "<color=grey>" + d.Value.udid + "</color>", style); 

                            if (GUILayout.Button(buildLabel)) 
                            {
                                if(buildLabel == "Build&Deploy") 
                                {
                                    AndroidBuilder.Instance.BuildAndDeployToDevice(d.Value);
                                } else
                                {
                                    AndroidBuilder.Instance.DeployApp(d.Value);
                                }
                            }
                        }
                        EditorGUILayout.EndHorizontal();
                    }

                    if (GUILayout.Button("Refresh")) 
                    {
                        AndroidBuilder.Instance.GetAllConnectedDevices();
                    }
                }
                EditorGUILayout.EndVertical();
            }
        }
	}
}