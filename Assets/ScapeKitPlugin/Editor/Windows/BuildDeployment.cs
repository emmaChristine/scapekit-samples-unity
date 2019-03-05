﻿using UnityEditor;
using UnityEngine;

namespace ScapeKitUnity
{
	internal class BuildDeployment : EditorWindow
	{
		private static bool showPlatforms = true;
        private int selectedPlatform;
		private string apiKey;

        private static bool showApiKeySettings = true;

        [MenuItem("ScapeKit/Build and Deploy", false, 1)]
        static void Init()
        {
            EditorWindow[] allWindows = Resources.FindObjectsOfTypeAll<EditorWindow>();
            EditorWindow window = null;
            if(allWindows.Length > 1) 
            {
                window = GetWindow<BuildDeployment>("ScapeKit", true, allWindows[1].GetType());
            } else 
            {
                window = GetWindow<BuildDeployment>("ScapeKit");
            }
            window.Show();
        }

        void Awake() {
            apiKey = ScapeClient.RetrieveKeyFromResources();
        }

        void OnGUI ()
        {
            ShowAccountSettings(); 
 
            DrawUILine(new Color(1, 1, 1, 0.35f));

			showPlatforms = EditorGUILayout.Foldout(showPlatforms, "Platforms");
            if(showPlatforms)
            {
            	selectedPlatform = GUILayout.Toolbar(selectedPlatform, new string[]{ "Android", "iOS"});

	            if(selectedPlatform == 0)
	            {
	                BuildDeploymentAndroid.ShowSettings();
	            }

	            if(selectedPlatform == 1)
	            {
	                BuildDeploymentiOS.ShowSettings();
	            }
            }

            DrawUILine(new Color(1, 1, 1, 0.35f));

            ShowOpenScenesSettings();
        }

        private void ShowLogo() 
        {
            GUILayout.BeginVertical();
            {
                GUILayout.BeginVertical();
                {
                    GUILayout.BeginVertical();
                    {
                        GUILayout.Label(Utility.GetIcon("scape-logo.png"), GUILayout.Width(350), GUILayout.Height(140));
                    }
                    GUILayout.EndVertical();
                }
                GUILayout.EndVertical();
            }
            GUILayout.EndVertical();
        }

        private void ShowAccountSettings() 
        {
            showApiKeySettings = EditorGUILayout.Foldout(showApiKeySettings, "Scape Account");
            if(showApiKeySettings)
            {
                ShowLogo();

                Rect DevelopmentSettings = EditorGUILayout.BeginHorizontal("box");
                {
                    Rect DevID = EditorGUILayout.BeginHorizontal("box");
                    {
                        GUILayout.Label("Enter your Scape API Key here:");
                        apiKey = EditorGUILayout.TextField(apiKey);
                    }
                    EditorGUILayout.EndHorizontal();
                }
                EditorGUILayout.EndHorizontal();
            }
        }

        private void ShowOpenScenesSettings()
        {
        	if (OpenScenesSettings.Instance == null) 
            {
                new OpenScenesSettings();
            }

            OpenScenesSettings.Instance.ShowScenes();
        }

        internal static void DrawUILine(Color color, int thickness = 2, int padding = 10)
        {
            Rect r = EditorGUILayout.GetControlRect(GUILayout.Height(padding+thickness));
            r.height = thickness;
            r.y+=padding/2;
            r.x-=2;
            r.width +=6;
            EditorGUI.DrawRect(r, color);
        }

        void OnLostFocus()
        {
            ScapeClient.SaveApiKeyToResource(apiKey);
        }

        void OnDestroy()
        {
            ScapeClient.SaveApiKeyToResource(apiKey);
        }

        public void OnInspectorUpdate()
        {
            this.Repaint();
        }
    }
}