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
using Com.Immersive.Video360;

namespace Com.Immersive.Cameras
{
    /// <summary>
    /// A game object which contains all the game objects and is scaled to fit the scaling mode.
    /// Has shortcut button which create commonly used object types.
    /// </summary>
    public class Stage : MonoBehaviour
    {
        public HotspotController hotspotControllerPrefab;
        public VideoPlayer360 videoPlayerPrefab;
        public TwoPartBackground splitBackgroundPrefab;
        public GameObject introSequencePrefab;

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

        public void Add360VideoPlayer()
        {
#if UNITY_EDITOR
            var videoPlayer = PrefabUtility.InstantiatePrefab(videoPlayerPrefab) as VideoPlayer360;
            videoPlayer.name = "360 Video Player";
            Selection.activeGameObject = videoPlayer.gameObject;
#endif
        }

        public void AddIntroSequence()
        {
#if UNITY_EDITOR
            var introSequence = PrefabUtility.InstantiatePrefab(introSequencePrefab) as GameObject;
            introSequence.name = "Intro Sequence";
            introSequence.transform.SetParent(transform);
            Selection.activeGameObject = introSequence.gameObject;
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
        private SerializedProperty videoPlayerPrefab;
        private SerializedProperty splitBackgroundPrefab;
        private SerializedProperty introSequencePrefab;

        private void OnEnable()
        {
            stage = (Stage)target;

            //Prefabs
            hotspotControllerPrefab = serializedObject.FindProperty("hotspotControllerPrefab");
            videoPlayerPrefab = serializedObject.FindProperty("videoPlayerPrefab");
            splitBackgroundPrefab = serializedObject.FindProperty("splitBackgroundPrefab");
            introSequencePrefab = serializedObject.FindProperty("introSequencePrefab");
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

            ////360 VIDEO PLAYER
            //if (GUILayout.Button("New 360 Video Player"))
            //{
            //    stage.Add360VideoPlayer();
            //}

            //INTRO SEQUENCE
            if (GUILayout.Button("New Intro Sequence"))
            {
                stage.AddIntroSequence();
            }
        }

        private void OnInspectorGUIPrefab()
        {
            EditorGUILayout.PropertyField(hotspotControllerPrefab, new GUIContent("Hotspot Controller"));
            EditorGUILayout.PropertyField(videoPlayerPrefab, new GUIContent("360 Video Player"));
            EditorGUILayout.PropertyField(splitBackgroundPrefab, new GUIContent("Split Background"));
            EditorGUILayout.PropertyField(introSequencePrefab, new GUIContent("Intro Sequence"));

            serializedObject.ApplyModifiedProperties();
        }
    }

#endif

}