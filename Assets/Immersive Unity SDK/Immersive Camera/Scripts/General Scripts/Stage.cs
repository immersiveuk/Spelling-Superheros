/* Copyright (C) Immersive Interactive, Ltd - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Luke Bissell <luke@immersive.co.uk>, July 2019
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Com.Immersive.Hotspots;

namespace Com.Immersive.Cameras
{
    /// <summary>
    /// A game object which contains all the game objects and is scaled to fit the scaling mode.
    /// Has shortcut buttons which create commonly used object types.
    /// </summary>
    public class Stage : MonoBehaviour
    {
        public HotspotController hotspotControllerPrefab;
        public TwoPartBackground splitBackgroundPrefab;

        public void AddHotspotController()
        {
#if UNITY_EDITOR
            var hotspotController = PrefabUtility.InstantiatePrefab(hotspotControllerPrefab) as HotspotController;
            hotspotController.name = "Hotspot Controller";
            hotspotController.transform.SetParent(transform);
            Selection.activeGameObject = hotspotController.gameObject;
#endif
        }

        public void AddSplitBackground()
        {
#if UNITY_EDITOR
            var splitBackground = PrefabUtility.InstantiatePrefab(splitBackgroundPrefab) as TwoPartBackground;
            splitBackground.name = "Split Background";
            splitBackground.transform.SetParent(transform);
            Selection.activeGameObject = splitBackground.gameObject;
#endif
        }
    }

    //==============================================================
    // CUSTOM EDITOR
    //==============================================================

#if UNITY_EDITOR

    [CustomEditor(typeof(Stage))]
    public class StageEditor : Editor
    {

        private Stage stage;
        private int currentTab = 0;

        //Prefabs
        private SerializedProperty hotspotControllerPrefab;
        private SerializedProperty splitBackgroundPrefab;

        private void OnEnable()
        {
            stage = (Stage)target;

            //Prefabs
            hotspotControllerPrefab = serializedObject.FindProperty(nameof(stage.hotspotControllerPrefab));
            splitBackgroundPrefab = serializedObject.FindProperty(nameof(stage.splitBackgroundPrefab));
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.Space();
            currentTab = GUILayout.Toolbar(currentTab, new string[] { "Create", "Prefabs" });
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
        }


        private void OnInspectorGUICreate()
        {
            //HOTSPOT CONTROLLER
            if (GUILayout.Button("New Hotspot Controller"))
            {
                stage.AddHotspotController();
            }

            //Split Background
            if (GUILayout.Button("New Split Background"))
            {
                stage.AddSplitBackground();
            }
        }

        private void OnInspectorGUIPrefab()
        {
            EditorGUILayout.PropertyField(hotspotControllerPrefab, new GUIContent("Hotspot Controller"));
            EditorGUILayout.PropertyField(splitBackgroundPrefab, new GUIContent("Split Background"));

            serializedObject.ApplyModifiedProperties();
        }
    }

#endif

}