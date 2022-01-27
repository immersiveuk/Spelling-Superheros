using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Com.Immersive.Hotspots
{
    public class ActivateAndDisableHotspotInspectorGUI : HotspotActionInspectorGUI
    {
        private SerializedProperty objectsToHideProp;
        private SerializedProperty objectsToRevealProp;

        public ActivateAndDisableHotspotInspectorGUI(SerializedProperty objectsToHideProp, SerializedProperty objectsToRevealProp)
        {
            this.objectsToHideProp = objectsToHideProp;
            this.objectsToRevealProp = objectsToRevealProp;
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.PropertyField(objectsToHideProp, true);
            EditorGUILayout.PropertyField(objectsToRevealProp, true);
        }
    }
}