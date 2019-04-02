using UnityEditor;
using UnityEngine;
using System;

namespace ScapeKitUnity
{
	internal class BuildDeployment : EditorWindow
	{
		private static bool showPlatforms = true;
        private int selectedPlatform;
		private string apiKey;

        private static bool showApiKeySettings = true;

        [MenuItem("ScapeKit/Account", false, 1)]
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
            ShowLogo();
            DrawUILine(Color.grey);
            ShowAccountSettings(); 
            DrawUILine(Color.grey);
            ShowGeoSimOptions();
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
            
            if (GUILayout.Button("Acquire API Key!"))
            {
                this.Close();
                Application.OpenURL("https://developer.scape.io/download/");
            }
        }

        private string latitudeStr;
        private string longitudeStr;

        private void ShowGeoSimOptions() {
            {
                GUILayout.Label("Enter Potential World Coordinates for the root of the Scene");
                EditorGUILayout.BeginHorizontal();
                GUILayout.Label("Latitude"); 
                latitudeStr = GUILayout.TextField(latitudeStr);
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal();
                GUILayout.Label("Longitude"); 
                longitudeStr = GUILayout.TextField(longitudeStr);
                EditorGUILayout.EndHorizontal();
                if(GUILayout.Button("Simulate")) {
                    try {
                        float latitude = float.Parse(latitudeStr);
                        float longitude = float.Parse(longitudeStr);

                        var coords = new Coordinates {
                            longitude = longitude,
                            latitude = latitude
                        };

                        object[] obj = GameObject.FindSceneObjectsOfType(typeof (GameObject));
                        foreach (object o in obj)
                        {
                            GameObject g = (GameObject) o;
                            GeoAnchor anchor = g.GetComponent<GeoAnchor>();
                            if(anchor) {
                                anchor.OriginEvent(coords);
                                anchor.gameObject.transform.localPosition = new Vector3(anchor.ScenePos.x, 0.0f, anchor.ScenePos.y);
                            }
                        }

                    }
                    catch (Exception ex) {
                        EditorUtility.DisplayDialog("Couldn't parse Coordinates", ex.Message, "ok");
                    }
                }

            }

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