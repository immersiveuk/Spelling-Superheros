using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Com.Immersive.Hotspots
{
    public class HotspotEventInspectorGUI : HotspotActionInspectorGUI
    {
        private SerializedProperty hotspotEvent;

        public HotspotEventInspectorGUI(SerializedProperty hotspotEvent)
        {
            this.hotspotEvent = hotspotEvent;
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.PropertyField(hotspotEvent);
        }
    }
}