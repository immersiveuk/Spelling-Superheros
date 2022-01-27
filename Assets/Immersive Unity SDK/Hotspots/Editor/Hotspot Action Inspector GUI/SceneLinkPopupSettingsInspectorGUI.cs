using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Com.Immersive.Hotspots
{
    public class SceneLinkPopupSettingsInspectorGUI : HotspotActionInspectorGUI
    {
        public SceneLinkDataModel.SceneLinkSetting sceneLinkSetting;

        public SerializedProperty linkName;
        public SerializedProperty fadeOut;
        public SerializedProperty fadeOutDuration;
        public SerializedProperty fadeColor;
        public SerializedProperty fadeOutAudio;

        public SceneLinkPopupSettingsInspectorGUI(SerializedProperty popupSettingsSerializedProp, SceneLinkDataModel.SceneLinkSetting sceneLinkSetting)
        {
            this.sceneLinkSetting = sceneLinkSetting;

            linkName = popupSettingsSerializedProp.FindPropertyRelative(nameof(linkName));
            fadeOut = popupSettingsSerializedProp.FindPropertyRelative(nameof(fadeOut));
            fadeOutDuration = popupSettingsSerializedProp.FindPropertyRelative(nameof(fadeOutDuration));
            fadeColor = popupSettingsSerializedProp.FindPropertyRelative(nameof(fadeColor));
            fadeOutAudio = popupSettingsSerializedProp.FindPropertyRelative(nameof(fadeOutAudio));
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.PropertyField(linkName, new GUIContent("Scene", "Which scene to link to."));
            EditorGUILayout.PropertyField(fadeOut, new GUIContent("Fade Out", "Should the scene fade out before changing scene?"));

            if (sceneLinkSetting.fadeOut)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(fadeOutDuration, new GUIContent("Fade Out Duration"));
                EditorGUILayout.PropertyField(fadeColor, new GUIContent("Fade Colour"));
                EditorGUILayout.PropertyField(fadeOutAudio, new GUIContent("Fade Audio Out"));
                EditorGUI.indentLevel--;
            }
        }
    }
}
