using System.IO;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace DamGame.Editor
{
    /// <summary>
    /// Repeatable project setup for a landscape-first Android game.
    /// Run with Unity's -executeMethod argument when creating a fresh checkout.
    /// </summary>
    public static class ProjectBootstrap
    {
        private const string ScenePath = "Assets/Scenes/DamGame.unity";

        public static void Configure()
        {
            CreateProjectFolders();

            EditorSettings.serializationMode = SerializationMode.ForceText;
            PlayerSettings.companyName = "Meghraj Singh Rana";
            PlayerSettings.productName = "Save the Village";
            PlayerSettings.defaultScreenWidth = 1920;
            PlayerSettings.defaultScreenHeight = 1080;
            PlayerSettings.defaultInterfaceOrientation = UIOrientation.LandscapeLeft;
            PlayerSettings.allowedAutorotateToLandscapeLeft = true;
            PlayerSettings.allowedAutorotateToLandscapeRight = true;
            PlayerSettings.allowedAutorotateToPortrait = false;
            PlayerSettings.allowedAutorotateToPortraitUpsideDown = false;
            PlayerSettings.SetApplicationIdentifier(NamedBuildTarget.Android, "com.meghrajsinghrana.savethevillage");
            PlayerSettings.Android.targetSdkVersion = AndroidSdkVersions.AndroidApiLevelAuto;

            EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Android, BuildTarget.Android);
            CreateBaseScene();

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("Save the Village foundation configured.");
        }

        private static void CreateProjectFolders()
        {
            CreateFolder("Assets", "Art");
            CreateFolder("Assets/Art", "Materials");
            CreateFolder("Assets/Art", "Models");
            CreateFolder("Assets/Art", "VFX");
            CreateFolder("Assets", "Audio");
            CreateFolder("Assets", "Prefabs");
            CreateFolder("Assets", "Scenes");
            CreateFolder("Assets", "Scripts");
            CreateFolder("Assets/Scripts", "Runtime");
            CreateFolder("Assets/Scripts", "UI");
            CreateFolder("Assets", "Settings");
        }

        private static void CreateFolder(string parent, string name)
        {
            var path = $"{parent}/{name}";
            if (!AssetDatabase.IsValidFolder(path))
            {
                AssetDatabase.CreateFolder(parent, name);
            }
        }

        private static void CreateBaseScene()
        {
            if (File.Exists(ScenePath))
            {
                return;
            }

            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            var root = new GameObject("GameRoot");

            var cameraObject = new GameObject("Main Camera");
            cameraObject.tag = "MainCamera";
            cameraObject.transform.SetPositionAndRotation(
                new Vector3(0f, 6f, -11f),
                Quaternion.Euler(22f, 0f, 0f));
            var camera = cameraObject.AddComponent<Camera>();
            camera.orthographic = false;
            camera.fieldOfView = 52f;
            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.backgroundColor = new Color(0.56f, 0.79f, 0.91f);

            var lightObject = new GameObject("Sun");
            lightObject.transform.rotation = Quaternion.Euler(45f, -35f, 0f);
            var light = lightObject.AddComponent<Light>();
            light.type = LightType.Directional;
            light.intensity = 1.1f;
            light.color = new Color(1f, 0.95f, 0.84f);

            new GameObject("World", typeof(Transform)).transform.SetParent(root.transform);
            new GameObject("UI", typeof(Transform)).transform.SetParent(root.transform);

            EditorSceneManager.SaveScene(scene, ScenePath);
            EditorBuildSettings.scenes = new[] { new EditorBuildSettingsScene(ScenePath, true) };
        }
    }
}
