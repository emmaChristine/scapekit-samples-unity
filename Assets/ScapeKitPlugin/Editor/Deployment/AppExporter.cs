using UnityEngine;
using UnityEditor;
using BuildResult = UnityEditor.Build.Reporting.BuildResult;

using System;
using System.Linq;
using System.IO;
using System.Collections;
using System.Collections.Generic;

namespace ScapeKitUnity 
{
	internal class AppExporter
    {
        private static BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions 
        {
            scenes = GetEnabledScenes(),
            locationPathName = "Build",
            target = BuildTarget.iOS,
            options = BuildOptions.Development | BuildOptions.AllowDebugging | BuildOptions.AcceptExternalModificationsToPlayer,
        };

        internal static void PerformIOSBuild(string teamID)
        {
            if(teamID.IsNullOrEmpty())
            {
                throw new Exception("Set your Apple Development Team ID before building the application");
            }

            buildPlayerOptions.locationPathName = buildPlayerOptions.locationPathName + "/iOS";
            Cleanup(buildPlayerOptions.locationPathName);

            BuildTarget target = BuildTarget.iOS;
            buildPlayerOptions.target = target;

            EditorUserBuildSettings.SwitchActiveBuildTarget(target);
            EditorUserBuildSettings.allowDebugging = true;

            PlayerSettings.iOS.appleDeveloperTeamID = teamID;

            buildPlayerOptions.scenes = GetEnabledScenes();

            var report = BuildPipeline.BuildPlayer(buildPlayerOptions);

            if (report.summary.result != BuildResult.Succeeded)
                throw new Exception("iOS Build failed");

            GUIUtility.ExitGUI();    
        }

        internal static void PerformAndroidBuild()
        {
            buildPlayerOptions.locationPathName = buildPlayerOptions.locationPathName + "/Android";
            Cleanup(buildPlayerOptions.locationPathName);

            BuildTarget target = BuildTarget.Android;
            buildPlayerOptions.target = target;

            EditorUserBuildSettings.SwitchActiveBuildTarget(target);
            EditorUserBuildSettings.allowDebugging = true;
            EditorUserBuildSettings.androidBuildSystem = AndroidBuildSystem.Gradle;
            EditorUserBuildSettings.exportAsGoogleAndroidProject = true;

            PlayerSettings.SetGraphicsAPIs(target, new UnityEngine.Rendering.GraphicsDeviceType[] { UnityEngine.Rendering.GraphicsDeviceType.OpenGLES3 } );
            PlayerSettings.SetScriptingBackend(BuildTargetGroup.Android, ScriptingImplementation.IL2CPP);
            PlayerSettings.SetArchitecture(BuildTargetGroup.Android, 1);

            buildPlayerOptions.scenes = GetEnabledScenes();

            var report = BuildPipeline.BuildPlayer(buildPlayerOptions);

            if (report.summary.result != BuildResult.Succeeded)
                throw new Exception("Android Build failed"); 

            GUIUtility.ExitGUI();   
        }

        private static string[] GetEnabledScenes()
        {
            var scenes = EditorBuildSettings.scenes
                        .Where(s => s.enabled)
                        .Select(s => s.path)
                        .ToArray();

            return scenes;
        }

        private static void Cleanup(string dir)
        {
            if (Directory.Exists(dir))
            {
                Directory.Delete(dir, true);
            }
            Directory.CreateDirectory(dir);
        }
    }
}