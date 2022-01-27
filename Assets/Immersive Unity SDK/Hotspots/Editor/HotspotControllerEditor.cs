using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Com.Immersive.Hotspots
{
    [CustomEditor(typeof(HotspotController))]
    public class HotspotControllerEditor : Editor
    {

        private HotspotController controller;
        private int currentTab = 0;

        //Settings
        private SerializedProperty revealType;
        private SerializedProperty singlePopUpOpenAtOnce;

        //Close Button
        private SerializedProperty closeButton;

        //Hotspot glow settings
        private SerializedProperty hotspotEffects;
        private SerializedProperty hotspotGlowSettings;

        //Hotspot Prefabs
        private SerializedProperty baseHotspotPrefab;
        private SerializedProperty imageHotspotPrefab;
        private SerializedProperty invisibleHotspotPrefab;
        private SerializedProperty batchPrefab;
        private SerializedProperty multiHotspotPrefab;
        private SerializedProperty textHotspotPrefab;
        private SerializedProperty regionHotspotPrefab;

        private void OnEnable()
        {
            controller = (HotspotController)target;

            //Settings
            revealType = serializedObject.FindProperty("revealType");
            singlePopUpOpenAtOnce = serializedObject.FindProperty("singlePopUpOpenAtOnce");

            hotspotEffects = serializedObject.FindProperty("hotspotEffects");
            hotspotGlowSettings = serializedObject.FindProperty("hotspotGlowSettings");

            //Close Button
            closeButton = serializedObject.FindProperty("closeButton");

            //Hotspot Prefabs
            baseHotspotPrefab = serializedObject.FindProperty("baseHotspotPrefab");
            imageHotspotPrefab = serializedObject.FindProperty("imageHotspotPrefab");
            invisibleHotspotPrefab = serializedObject.FindProperty("invisibleHotspotPrefab");
            multiHotspotPrefab = serializedObject.FindProperty("multiHotspotPrefab");
            textHotspotPrefab = serializedObject.FindProperty("textHotspotPrefab");
            regionHotspotPrefab = serializedObject.FindProperty("regionHotspotPrefab");
            batchPrefab = serializedObject.FindProperty("batchPrefab");
        }


        public override void OnInspectorGUI()
        {
            //Settings
            EditorGUILayout.Space();

            //Navigation UI
            //OnInspectorGUINaviagtion();

            EditorGUILayout.LabelField("Settings", EditorStyles.boldLabel);
            OnInspectorGUISettings();

            //Create
            EditorGUILayout.Space();
            currentTab = GUILayout.Toolbar(currentTab, new string[] { "Create", "Hotspot Prefabs" });
            EditorGUILayout.Space();

            switch (currentTab)
            {
                case 0:
                    OnInspectorGUICreate();
                    break;
                case 1:
                    OnInspectorGUIPrefab();
                    break;
            }

            serializedObject.ApplyModifiedProperties();
        }


        private void OnInspectorGUISettings()
        {
            EditorGUILayout.PropertyField(closeButton, new GUIContent("Close Button"), true);

            EditorGUILayout.PropertyField(hotspotEffects);
            if (controller.hotspotEffects == HotspotController.HotspotEffects.Glow)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(hotspotGlowSettings, new GUIContent("Hotspot Glow Settings"), true);
                EditorGUI.indentLevel--;
            }

            EditorGUILayout.PropertyField(revealType);

            EditorGUILayout.PropertyField(singlePopUpOpenAtOnce);
        }

        private void OnInspectorGUICreate()
        {

            EditorGUILayout.LabelField("Create Hotspots");
            EditorHotspotCreator.CreateHotspotButtonsGUI(controller.transform);
            EditorGUILayout.Space();
            EditorHotspotCreator.CreateHotspotGroupButtonsGUI(controller.transform);
        }

        private void OnInspectorGUIPrefab()
        {
            EditorGUILayout.PropertyField(batchPrefab, new GUIContent("Batch"));
            EditorGUILayout.PropertyField(baseHotspotPrefab, new GUIContent("Basic Hotspot"));
            EditorGUILayout.PropertyField(imageHotspotPrefab, new GUIContent("Image Hotspot"));
            EditorGUILayout.PropertyField(invisibleHotspotPrefab, new GUIContent("Invisible Hotspot"));
            EditorGUILayout.PropertyField(multiHotspotPrefab, new GUIContent("Multi-Hotspot"));
            EditorGUILayout.PropertyField(textHotspotPrefab, new GUIContent("Text Hotspot"));
            EditorGUILayout.PropertyField(regionHotspotPrefab, new GUIContent("Region Hotspot"));
            serializedObject.ApplyModifiedProperties();
        }
    }
}
