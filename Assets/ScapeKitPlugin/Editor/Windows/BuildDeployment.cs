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

        string latitudeStr;
        string longitudeStr;
        CoordinateAsset GeoRootCoords;

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

            GeoRootCoords = (CoordinateAsset)Resources.Load<CoordinateAsset>("SceneGeoRoot");
            if(!GeoRootCoords) {
                GeoRootCoords = ScriptableObject.CreateInstance<CoordinateAsset>();
                AssetDatabase.CreateAsset(GeoRootCoords, "Assets/Resources/SceneGeoRoot.asset");
                AssetDatabase.SaveAssets();
            }
            latitudeStr = GeoRootCoords.latitude.ToString();
            longitudeStr = GeoRootCoords.longitude.ToString();
        }

        void OnGUI()
        {
            ShowLogo();
            DrawUILine(Color.grey);
            ShowAccountSettings(); 
            DrawUILine(Color.grey);
            ShowGeoSimOptions();
        }

        private void ShowLogo() 
        {
            GUILayout.Label(Utility.GetIcon("scape-logo.png"), GUILayout.Width(350), GUILayout.Height(140));
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
                        GeoRootCoords.latitude = double.Parse(latitudeStr);
                        GeoRootCoords.longitude = double.Parse(longitudeStr);

                        if(GeoRootCoords.latitude == -1.0 || GeoRootCoords.longitude == -1.0) throw new Exception("Please enter valid geocoordinates");

                        var coords = new Coordinates {
                            longitude = GeoRootCoords.longitude,
                            latitude = GeoRootCoords.latitude
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
                        EditorUtility.SetDirty(GeoRootCoords); 
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