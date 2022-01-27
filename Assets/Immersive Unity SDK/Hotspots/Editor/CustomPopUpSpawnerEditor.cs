using Com.Immersive.Hotspots;
using System;
using UnityEditor;


namespace Com.Immersive.Hotspots
{
    /// <summary>
    /// A Custom Inspector for a CustomPopUpSpawner Scriptable Object.
    /// This inspector will try and find a PopUpInspectorGUI<> class that displays the appropriate hotspots settings.
    /// If none are found the default inspector for the PopUpSettings property will be used.
    /// </summary>
    [CustomEditor(typeof(CustomPopUpSpawner<,>), true)]
    public class CustomPopUpSpawnerEditor : Editor
    {
        protected SerializedProperty popUpPrefab;
        protected SerializedProperty popUpSettings;

        private HotspotActionInspectorGUI inspectorGUI = null;

        protected virtual void OnEnable()
        {
            popUpPrefab = serializedObject.FindProperty(nameof(popUpPrefab));
            popUpSettings = serializedObject.FindProperty(nameof(popUpSettings));
            inspectorGUI = FindInspectorGUI();
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(popUpPrefab);
            if (popUpPrefab.objectReferenceValue == null)
            {
                EditorGUILayout.HelpBox("A PopUp Prefab is required.", MessageType.Error);
            }
            DrawPopUpSettings();

            serializedObject.ApplyModifiedProperties();
        }

        protected virtual void DrawPopUpSettings()
        {
            if (inspectorGUI == null)
                EditorGUILayout.PropertyField(popUpSettings);
            else 
                inspectorGUI.OnInspectorGUI();
        }

        /// <summary>
        /// Uses reflection to try and locate the suitable PopUpSettingsInspectorGUI<> class to display the current PopUpSettings.
        /// If a class is found it will create and new one return it.
        /// If no class is found it will return null.
        /// </summary>
        private HotspotActionInspectorGUI FindInspectorGUI()
        {
            var targetType = target.GetType();
            var baseType = targetType.BaseType;
            if (baseType.IsGenericType)
            {
                Type TPopUpSettings = baseType.GetGenericArguments()[0];
                Type genericInspectorGUIType = (typeof(PopUpSettingsInspectorGUI<>)).MakeGenericType(new Type[] { TPopUpSettings });

                Type[] implementationTypes = AppDomain.CurrentDomain.GetAllDerivedTypes(genericInspectorGUIType);
                if (implementationTypes.Length == 0)
                    return null;

                var inspectorGUIType = implementationTypes[0];

                return (HotspotActionInspectorGUI)Activator.CreateInstance(inspectorGUIType, popUpSettings, SerializedObjectHelper.GetTargetObjectOfProperty(popUpSettings));
            }
            return null;
        }
    } 
}
