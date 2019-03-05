using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace ScapeKitUnity
{
	[InitializeOnLoad]
    internal class OpenScenesSettings
    {
    	private int selectedScenesView;
        private const int ALL_SCENES_TOOLBAR_INDEX = 1;
        private const int BUILD_SCENES_TOOLBAR_INDEX = 0;

        private static bool showScenes = true;

        private static GUIStyle ButtonTextMiddleLeft { get { return new GUIStyle(GUI.skin.button) { alignment = TextAnchor.MiddleLeft }; } }

        private static readonly string[] TOOLBAR_STRINGS = new string[2]
        {
            "All Scenes",
            "Build Scenes",
        };

        private Vector2 windowScrollPosition;

        internal static OpenScenesSettings Instance;

        internal OpenScenesSettings()
		{
			Instance = this;
		}

        private static void OpenSceneButtonsGUI(IEnumerable<string> scenePaths)
        {
            if (scenePaths.Count() == 0)
            {
                EditorGUILayout.HelpBox("No scenes to open.", MessageType.Info);
                return;
            }

            foreach (var scenePath in scenePaths)
            {
                if (GUILayout.Button(scenePath.Replace("Assets/", "").Replace(".unity", ""), ButtonTextMiddleLeft))
                {
                    if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                    {
                        EditorSceneManager.OpenScene(scenePath);
                    }
                }
            }
        }

        internal void ShowScenes()
        {
        	showScenes = EditorGUILayout.Foldout(showScenes, "Scenes");
            if(showScenes)
            {
	        	EditorGUILayout.BeginVertical();
	            EditorGUILayout.Space();

	            selectedScenesView = GUILayout.Toolbar(selectedScenesView, new string[]{ "Scenes selected for build", "All scenes"});

	            if(selectedScenesView == BUILD_SCENES_TOOLBAR_INDEX)
	            {
	                ScenesInBuildSettingsGUI();
	            }

	            if(selectedScenesView == ALL_SCENES_TOOLBAR_INDEX)
	            {
	                ScenesInProjectGUI();
	            }

	            EditorGUILayout.EndVertical();
            }
        }

        private void ScenesInProjectGUI()
        {
            windowScrollPosition = EditorGUILayout.BeginScrollView(windowScrollPosition);

            var sceneAssetGuids = AssetDatabase.FindAssets("t:scene");
            var scenePaths = sceneAssetGuids.Select(sceneAssetGuid =>
            {
                return AssetDatabase.GUIDToAssetPath(sceneAssetGuid);
            });
            OpenSceneButtonsGUI(scenePaths);

            EditorGUILayout.EndScrollView();
        }

        private void ScenesInBuildSettingsGUI()
        {
            windowScrollPosition = EditorGUILayout.BeginScrollView(windowScrollPosition);

            // Though Unity documentations states that EditorBuildSettingsScene
            // path property returns file path as listed in build settings window,
            // this is not true. In build settings scene path is listed without
            // Assets folder at path start and without .unity extension. But path
            // property returns full project path like Assets/Scenes/MyScene.unity
            var scenePaths = EditorBuildSettings.scenes.Where(s => s.enabled).Select(s => s.path);
            OpenSceneButtonsGUI(scenePaths);

            EditorGUILayout.EndScrollView();
        }
    }
}
