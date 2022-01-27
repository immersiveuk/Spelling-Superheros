using Com.Immersive.Hotspots;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Com.Immersive.Hotspots
{
    [CustomEditor(typeof(HotspotBatch))]
    public class HotspotBatchEditor : Editor
    {
        private HotspotBatch batch;

        private void OnEnable()
        {
            batch = (HotspotBatch)target;
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Create Hotspots");
            EditorHotspotCreator.CreateHotspotButtonsGUI(batch.transform);
            EditorGUILayout.Space();

            EditorGUILayout.BeginVertical("box");
            if (GUILayout.Button("New Multi-Hotspot"))
                EditorHotspotCreator.CreateMultiHotspot(batch.transform);
            EditorGUILayout.EndVertical();
        }
    } 
}