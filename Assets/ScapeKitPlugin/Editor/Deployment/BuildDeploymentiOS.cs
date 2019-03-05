using UnityEditor;
using UnityEngine;

namespace ScapeKitUnity
{
	internal static class BuildDeploymentiOS
	{
        private static bool showAdvancedSettings = false;
        private static bool showConnectedDevices = true;
        private static bool showProjectPath = true;
        private static bool showAppPath = true;
        private static bool showIOSSettings = true;

        private static string teamID = string.Empty;

        internal static void ShowSettings() 
        {
            if (IOSBuilder.Instance == null) 
            {
                new IOSBuilder();
            }

            FileSettings();

            IOSSettings();

            DevicesSettings();

            AdvancedSettings();
        }

        private static void IOSSettings()
        {
            showIOSSettings = EditorGUILayout.Foldout(showIOSSettings, "Development Settings");
            if(showIOSSettings)
            {
                Rect DevelopmentSettings = EditorGUILayout.BeginVertical("box");
                {
                    Rect DevID = EditorGUILayout.BeginHorizontal("box");
                    {
                        if(IOSBuilder.Instance.DevelopmentID.IsNullOrEmpty())
                        {
                            GUILayout.Label("Enter your Apple Developer Team ID: ");
                            teamID = EditorGUILayout.TextField(teamID);
                            IOSBuilder.Instance.DevelopmentID = teamID;
                        } else 
                        {
                            GUILayout.Label("Apple Developer Team ID: ");
                            teamID = EditorGUILayout.TextField(IOSBuilder.Instance.DevelopmentID);
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
                        GUILayout.Label ("Generate Xcode project");
                        if (GUILayout.Button ("Generate")) 
                        {
                            IOSBuilder.Instance.BuildUnityProjectToXcode();
                        }
                    }
                    EditorGUILayout.EndHorizontal();

                    if(IOSBuilder.Instance.ProjectAlreadyGenerated)
                    {
                        Rect appBuilder = EditorGUILayout.BeginHorizontal("box");
                        {
                            GUILayout.Label("Build generated Xcode project");
                            if (GUILayout.Button("Build")) 
                            {
                                IOSBuilder.Instance.BuildGeneratedXcodeProject();
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
            if(IOSBuilder.Instance.ProjectAlreadyGenerated)
            {
                showProjectPath = EditorGUILayout.Foldout(showProjectPath, "Project Path");
                if(showProjectPath)
                {
                    Rect appBuilder = EditorGUILayout.BeginHorizontal("box");
                    {
                        GUILayout.Label(IOSBuilder.Instance.XcodeProjectPath);
                    }
                    EditorGUILayout.EndHorizontal();
                }
            }
            if(IOSBuilder.Instance.AppAlreadyBuilt)
            {
                showAppPath = EditorGUILayout.Foldout(showAppPath, "APP Path");
                if(showAppPath)
                {
                    Rect appBuilder = EditorGUILayout.BeginHorizontal("box");
                    {
                        GUILayout.Label(IOSBuilder.Instance.AppPath); 
                    }  
                    EditorGUILayout.EndHorizontal();
                }
            }
        }

        private static void DevicesSettings()
        {
            showConnectedDevices = EditorGUILayout.Foldout(showConnectedDevices, string.Format("Connected Devices ({0}):", IOSBuilder.Instance.Devices.Count));
            if(showConnectedDevices)
            {
                Rect DevicesInfo = EditorGUILayout.BeginVertical("box");
                {
                    GUIStyle style = new GUIStyle();
                    style.richText = true;

                    string buildLabel = "Build&Deploy";
                    // if the app is already built only deploy it
                    if (IOSBuilder.Instance.AppAlreadyBuilt) 
                    {
                        buildLabel = "Deploy";
                    }

                    foreach (var d in IOSBuilder.Instance.Devices) 
                    {
                        Rect CurrentDevice = EditorGUILayout.BeginHorizontal("box");
                        {
                            GUILayout.Label("<color=white>" + d.Value.name + "</color>" + "<color=grey>" + d.Value.udid + "</color>", style); 

                            if (GUILayout.Button(buildLabel)) 
                            {
                                if(buildLabel == "Build&Deploy") 
                                {
                                    IOSBuilder.Instance.BuildAndDeployToDevice(d.Value);
                                } else
                                {
                                    IOSBuilder.Instance.DeployApp(d.Value);
                                }
                            }
                        }
                        EditorGUILayout.EndHorizontal();
                    }

                    if (GUILayout.Button("Refresh")) 
                    {
                        IOSBuilder.Instance.GetAllConnectedDevices();
                    }
                }
                EditorGUILayout.EndVertical();
            }
        }
	}
}