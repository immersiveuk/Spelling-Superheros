/* Copyright (C) Immersive Interactive, Ltd - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Luke Bissell <luke@immersive.co.uk>, Dec 2019
 */

using Com.Immersive.Scatter;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Com.Immersive.FootprintSystem
{
    /// <summary>
    /// A game object which contains all the game objects to be displayed on the floor.
    /// Has shortcut buttons which create commonly used object types.
    /// </summary>
    public class Floor : MonoBehaviour
    {
        public FloorFootprintSystem footprintSystemPrefab;
        public ScatterSystem scatterSystemPrefab;

        public void AddFootprintSystem()
        {
#if UNITY_EDITOR
            var footprintSystem = PrefabUtility.InstantiatePrefab(footprintSystemPrefab, transform) as FloorFootprintSystem;
            footprintSystem.name = "Footprint System";

            Selection.activeGameObject = footprintSystem.gameObject;
#endif
        }

        public void AddScatterSystem()
        {
#if UNITY_EDITOR
            var scatterSystem = PrefabUtility.InstantiatePrefab(scatterSystemPrefab, transform) as ScatterSystem;
            scatterSystem.name = "Scatter System";

            Selection.activeGameObject = scatterSystem.gameObject;
#endif
        }
    }

#if UNITY_EDITOR

    [CustomEditor(typeof(Floor))]
    public class FloorEditor: Editor
    {
        private Floor floor;
        private int currentTab = 0;

        //Prefabs
        private SerializedProperty footprintSystemPrefab;
        private SerializedProperty scatterSystemPrefab;

        private void OnEnable()
        {
            floor = (Floor)target;

            //Prefabs
            footprintSystemPrefab = serializedObject.FindProperty("footprintSystemPrefab");
            scatterSystemPrefab = serializedObject.FindProperty("scatterSystemPrefab");
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
            //Footprint System
            if (GUILayout.Button("New Footprint System"))
            {
                floor.AddFootprintSystem();
            }

            //Scatter System
            if (GUILayout.Button("New Scatter System"))
            {
                floor.AddScatterSystem();
            }

        }

        private void OnInspectorGUIPrefab()
        {
            EditorGUILayout.PropertyField(footprintSystemPrefab, new GUIContent("Footprint System"));
            EditorGUILayout.PropertyField(scatterSystemPrefab, new GUIContent("Scatter System"));
            serializedObject.ApplyModifiedProperties();
        }
    }

#endif

}