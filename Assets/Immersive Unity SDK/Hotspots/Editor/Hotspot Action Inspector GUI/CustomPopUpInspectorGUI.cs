using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Com.Immersive.Hotspots
{
    public class CustomPopUpInspectorGUI : HotspotActionInspectorGUI
    {
        private SerializedProperty customPopUpSpawner;

        private Action applyModifiedProperties;


        private Rect createCustomPopUpButtonRect;

        private Type[] popUpTypes = new Type[0];
        private string[] options = new string[0];
        private int createTypeIndex = 0;
        private bool popUpSpawnerScriptsExist;

        public CustomPopUpInspectorGUI(SerializedProperty customPopUpSpawner, Action applyModifiedProperties)
        {
            this.customPopUpSpawner = customPopUpSpawner;
            this.applyModifiedProperties = applyModifiedProperties;
            FindTypes();
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.PropertyField(customPopUpSpawner);

            EditorGUILayout.Space();

            DrawCreatePopUpSpawnerButton();
            EditorGUILayout.Space();
            DrawCreateNewCustomPopUpScript();
        }

        private void DrawCreatePopUpSpawnerButton()
        {
            if (popUpSpawnerScriptsExist)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.PrefixLabel("");
                ScriptableObjectCreatorEditorGUI.CreateScriptableObjectButton<CustomPopUpSpawner>(ref createCustomPopUpButtonRect, (newPopUpSpawner) => 
                {
                    string path = AssetDatabase.GenerateUniqueAssetPath($"Assets/{newPopUpSpawner.GetType().Name}.asset");
                    AssetDatabase.CreateAsset(newPopUpSpawner, path);
                    AssetDatabase.SaveAssets();
                    customPopUpSpawner.objectReferenceValue = newPopUpSpawner;
                    applyModifiedProperties?.Invoke();
                    Debug.Log($"Created new CustomPopUpSpawner at {path}.");
                });

                EditorGUILayout.EndHorizontal();
            }
        }

        private void DrawCreateNewCustomPopUpScript()
        {
            EditorGUILayout.LabelField("Create New CustomPopUpSpawner Class");
            EditorGUILayout.BeginHorizontal();

            createTypeIndex = EditorGUILayout.Popup(createTypeIndex, options);
            if (GUILayout.Button("Create"))
            {
                CreateNewCustomPopUpSpawnerScript(popUpTypes[createTypeIndex]);
            }
            EditorGUILayout.EndHorizontal();
        }

        private void CreateNewCustomPopUpSpawnerScript(Type hotspotType)
        {

            Type popUpSettingsType = hotspotType.GetGenericParameter(typeof(HotspotPopUp<>), 0);

            string className = $"{hotspotType.Name}Spawner";

            string directoryPath = EditorUtility.SaveFolderPanel("Create New PopUp Spawner Script in Folder", "Assets", "");
            if (directoryPath.Length == 0)
                return;

            string path = Path.Combine(directoryPath, className + ".cs");

            string content = $"using Com.Immersive.Hotspots;\n" +
                $"public class {className} : CustomPopUpSpawner<{popUpSettingsType.FullName.Replace("+", ".")}, {hotspotType.FullName.Replace("+", ".")}> {{ }}";
            File.WriteAllText(path, content);
            AssetDatabase.Refresh();

            Debug.Log($"Created new Script {className} and saved it in {directoryPath}");
        }

        private void FindTypes()
        {
            //PopUp Spawners
            Type[] popUpSpawnerTypes = AppDomain.CurrentDomain.GetAllDerivedTypes<CustomPopUpSpawner>();
            popUpSpawnerScriptsExist = popUpSpawnerTypes.Length > 0;

            List<Type> popUpSpawnerPopUpTypes = new List<Type>();
            foreach (Type popUpSpawnerType in popUpSpawnerTypes)
            {
                var popUpType = popUpSpawnerType.GetGenericParameter(typeof(CustomPopUpSpawner<,>), 1);
                if (popUpType != null)
                    popUpSpawnerPopUpTypes.Add(popUpType);
            }

            popUpTypes = AppDomain.CurrentDomain.GetAllDerivedTypes(typeof(HotspotPopUp), true);

            List<Type> reducedPopUpTypes = new List<Type>();

            foreach (var popUpType in popUpTypes)
            {
                if (!popUpSpawnerPopUpTypes.Contains(popUpType))
                    reducedPopUpTypes.Add(popUpType);
            }

            popUpTypes = reducedPopUpTypes.ToArray();

            options = new string[popUpTypes.Length];

            for (int i = 0; i < popUpTypes.Length; i++)
                options[i] = popUpTypes[i].Name;

            if (createTypeIndex >= options.Length)
                createTypeIndex = 0;
        }
    }
}