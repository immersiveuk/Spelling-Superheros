using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using Unity.EditorCoroutines.Editor;
using UnityEditor;
using UnityEngine;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;
using PackageInfo = UnityEditor.PackageManager.PackageInfo;

namespace Immersive.Presets
{ 
    public class ImmersiveSetupWindow : EditorWindow
    {
        private bool immersiveStateAvailable = false,
        importingState = false;
        
        // Add menu named "My Window" to the Window menu
        [MenuItem("Immersive Interactive/Setup", false, 0)]
        static void Init()
        {
            // Get existing open window or if none, make a new one:
            ImmersiveSetupWindow window = (ImmersiveSetupWindow)GetWindow(typeof(ImmersiveSetupWindow));
            window.name = "Immersive Interactive Setup";
            window.titleContent = new GUIContent("Immersive Interactive Setup");
            window.minSize = new Vector2(500, 550);
            window.Show();
        }

        void OnGUI()
        {

            EditorGUILayout.Space();
            GUILayout.Label("Immersive Interactive Setup", EditorStyles.boldLabel);
            EditorGUILayout.Space();

            EditorGUILayout.LabelField("When first setting up a Immersive Unity project, complete the following steps to set up the project for best results.", EditorStyles.wordWrappedLabel) ;

            EditorGUILayout.Space();

            DrawTextMeshProImport();

            EditorGUILayout.Space();

            DrawImmersiveStateImport();

            EditorGUILayout.Space();

            DrawTextureImportPreset();

            EditorGUILayout.Space();

            DrawGitSettings();

            EditorGUILayout.Space();

            DrawPlayerSettings();
            
            EditorGUILayout.Space();

            DrawNewSceneButtons();
        }

        void DrawNewSceneButtons()
        {
            GUILayout.BeginVertical("box");

            EditorGUILayout.LabelField("New Scenes", EditorStyles.boldLabel);
            EditorGUILayout.LabelField("Quickly create new Scenes which are setup with Immersive Camera Systems.\n\nYou can also create these scenes by right clicking in the Project View and selecting: Create > Immersive Interactive.", EditorStyles.wordWrappedLabel);

            GUILayout.BeginHorizontal();
            if (GUILayout.Button(new GUIContent("2D Scene", "Scene with a 2D Immersive Camera System")))
                CreateDefaultSceneTypes.New2DScene();

            if (GUILayout.Button(new GUIContent("3D Scene", "Scene with a 3D Immersive Camera System")))
                CreateDefaultSceneTypes.New3DScene();
            GUILayout.EndHorizontal();

            GUILayout.EndVertical();
        }

        void DrawTextMeshProImport()
        {
            GUILayout.BeginVertical("box");
            EditorGUILayout.LabelField("Import TextMesh Pro", EditorStyles.boldLabel);
            if (GUILayout.Button("Import"))
            {
                TMP_PackageUtilities.ImportProjectResourcesMenu();
            }
            GUILayout.EndVertical();
        }

        private void DrawImmersiveStateImport()
        {
            EditorCoroutineUtility.StartCoroutineOwnerless(CheckForImmersiveState("Immersive State"));
            if (immersiveStateAvailable) return;

            string txt = importingState ? "Importing..." : "Import";
            GUILayout.BeginVertical(GUI.skin.box);
            GUI.enabled = !importingState;
            EditorGUILayout.LabelField("Import Immersive State", EditorStyles.boldLabel);
            if (GUILayout.Button(txt))
            {
                EditorCoroutineUtility.StartCoroutineOwnerless(ImportImmersiveState());
            }

            GUI.enabled = true;

            GUILayout.EndVertical();
        }

        private IEnumerator CheckForImmersiveState(string packageName)
        {
            ListRequest packageList = Client.List();
            while (!packageList.IsCompleted)
            {
                yield return null;
            }

            if (packageList.Result.Any(pInfo => pInfo.displayName == packageName))
            {
                immersiveStateAvailable = true;
            }
        }

        private IEnumerator ImportImmersiveState()
        {
            AddRequest packageAdd =
                Client.Add(
                    "https://github.com/immersiveuk/ImmersiveState.git?path=/com.immersiveinteractive.immersivestate");
            while (!packageAdd.IsCompleted)
            {
                importingState = true;
                yield return null;
            }

            importingState = false;
            immersiveStateAvailable = false;
        }

        void DrawTextureImportPreset()
        {
            GUILayout.BeginVertical("box");
            EditorGUILayout.LabelField("Setup Texture Import Default", EditorStyles.boldLabel);

#if UNITY_2019_4_OR_NEWER
            EditorGUILayout.LabelField("By selecting one of the below options, textures will automatically be imported as Sprites with a Pixel Per Unit value set to support the chosen aspect ratio.", EditorStyles.wordWrappedLabel);

            EditorGUILayout.BeginHorizontal();

            EditorGUI.BeginDisabledGroup(!SetSpriteImportPreset.IsSprite1080());
            if (GUILayout.Button(new GUIContent("16x9", "Pixels Per Unit = 1080")))
            {
                SetSpriteImportPreset.SetSprite1080();
            }
            EditorGUI.EndDisabledGroup();

            EditorGUI.BeginDisabledGroup(!SetSpriteImportPreset.IsSprite1200());
            if (GUILayout.Button(new GUIContent("16x10", "Pixels Per Unit = 1200")))
                SetSpriteImportPreset.SetSprite1200();

            EditorGUI.EndDisabledGroup();

            EditorGUILayout.EndHorizontal();
#else
            EditorGUILayout.LabelField("Please go to Immersive Unity SDK > Editor > Editor Assets and set \"Sprite Preset 1080\" if you are targetting 16x9 surfaces, or \"Sprite Preset 1200\" if you are targetting 16x10 surfaces.", EditorStyles.wordWrappedLabel);
#endif
            GUILayout.EndVertical();
        }

        void DrawGitSettings()
        {
            GUILayout.BeginVertical("box");

            EditorGUILayout.LabelField("Git", EditorStyles.boldLabel);

            EditorGUILayout.LabelField("Quickly add a .gitignore or a .gitattribute file to your project root directory. These files are setup to support unity projects and will utilise git-lfs.", EditorStyles.wordWrappedLabel);

            GUILayout.BeginHorizontal();

            DrawGitButton("gitignore");
            DrawGitButton("gitattributes");

            GUILayout.EndHorizontal();

            GUILayout.EndVertical();
        }

        private void DrawGitButton(string fileName)
        {
            string targetName = "." + fileName;
            string templateName = fileName + "_template";
            EditorGUI.BeginDisabledGroup(File.Exists(targetName));
            if (GUILayout.Button(new GUIContent(targetName)))
            {
                var path = GetAssetPath(templateName);
                if (path != null)
                    FileUtil.CopyFileOrDirectory(path, targetName);
                else
                    Debug.LogError("Cannot locate "+templateName + " file.");
            }
            EditorGUI.EndDisabledGroup();
        }


        private void DrawPlayerSettings()
        {
            GUILayout.BeginVertical("box");

            EditorGUILayout.LabelField("Player Settings", EditorStyles.boldLabel);

            EditorGUILayout.LabelField("Set the Player Settings to a default set of values that will display your Immersive Experience correctly.", EditorStyles.wordWrappedLabel);

            GUILayout.BeginHorizontal();

            EditorGUI.BeginDisabledGroup(!ImmersivePlayerSettings.CanSetPlayerSettings());
            if (GUILayout.Button(new GUIContent("Default Player Settings")))
            {
                ImmersivePlayerSettings.SetPlayerSettings();
            }
            EditorGUI.EndDisabledGroup();

            if (ImmersivePlayerSettings.CanDisableSplashScreen)
            {
                EditorGUI.BeginDisabledGroup(!ImmersivePlayerSettings.IsSplashScreenEnabled);
                if (GUILayout.Button(new GUIContent("Disable Splash Screen")))
                    ImmersivePlayerSettings.DisableSplashScreen();
                EditorGUI.EndDisabledGroup();
            }

            GUILayout.EndHorizontal();

            GUILayout.EndVertical();
        }


        private string GetAssetPath(string fileName)
        {
            var guids = AssetDatabase.FindAssets(fileName);
            if (guids.Length != 1)
                return null;
            else
                return AssetDatabase.GUIDToAssetPath(guids[0]);
        }
    }
}
