using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Com.Immersive.Hotspots
{
    [CustomEditor(typeof(MultiHotspot))]
    public class MultiHotspotEditor : Editor
    {

        private MultiHotspot multiHotspot;

        //General Settings
        private SerializedProperty onClickAction;


        private void OnEnable()
        {
            multiHotspot = (MultiHotspot)target;

            //GeneralSettings
            onClickAction = serializedObject.FindProperty("_clickAction");

        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.Space();
            OnInspectorGUISettings();

            EditorGUILayout.LabelField("Create Hotspots");
            EditorHotspotCreator.CreateHotspotButtonsGUI(multiHotspot.transform);

            serializedObject.ApplyModifiedProperties();

        }

        private void OnInspectorGUISettings()
        {
            EditorGUILayout.LabelField("Settings");
            //General Settings
            EditorGUILayout.PropertyField(onClickAction, new GUIContent("When Selected", "What should be done to the when it is selected."));
        }
    } 
}