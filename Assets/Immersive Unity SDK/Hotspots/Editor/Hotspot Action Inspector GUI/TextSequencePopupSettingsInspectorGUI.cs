using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Com.Immersive.Hotspots
{
    public class TextSequencePopupSettingsInspectorGUI : PopUpSequenceSettingsInspectorGUI<TextSequencePopUpSetting>
    {
        private SerializedProperty textPopups;
        private SerializedProperty background;
        private SerializedProperty controlPanelStyle;
        private SerializedProperty includePageCount;
        private SerializedProperty pageCountSettings;

        protected override SerializedProperty SequenceProperty => textPopups;

        public TextSequencePopupSettingsInspectorGUI(SerializedProperty popupSettingsSerializedProp, TextSequencePopUpSetting textSequencePopUpSetting) : base(popupSettingsSerializedProp, textSequencePopUpSetting)
        {
            textPopups = popupSettingsSerializedProp.FindPropertyRelative(nameof(textPopups));
            background = popupSettingsSerializedProp.FindPropertyRelative(nameof(background));
            controlPanelStyle = popupSettingsSerializedProp.FindPropertyRelative(nameof(controlPanelStyle));
            includePageCount = popupSettingsSerializedProp.FindPropertyRelative(nameof(includePageCount));
            pageCountSettings = popupSettingsSerializedProp.FindPropertyRelative(nameof(pageCountSettings));
        }
        protected override void DrawAdditionalControlPanelSettings()
        {
            EditorGUILayout.PropertyField(controlPanelStyle, new GUIContent("Control Panel Style"));
            DrawCustomButtonControls();
        }
        protected override void DrawSequenceElement(int index)
        {
            DrawTextSettings(textPopups.GetArrayElementAtIndex(index));
        }

        protected override void DrawSettings()
        {
            //Background
            EditorGUILayout.PropertyField(background, new GUIContent("Background"));

            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(includePageCount, new GUIContent("Include Page Count"));

            if (includePageCount.boolValue)
                EditorGUILayout.PropertyField(pageCountSettings, new GUIContent("Page Count Settings"), true);

            base.DrawSettings();
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            DrawDefaultSizeSettings();
        }

        private void DrawTextSettings(SerializedProperty textPageProp)
        {
            SerializedProperty includeTitleProp = textPageProp.FindPropertyRelative("includeTitle");
            SerializedProperty titleProp = textPageProp.FindPropertyRelative("title");
            SerializedProperty spaceProp = textPageProp.FindPropertyRelative("space");
            SerializedProperty bodyProp = textPageProp.FindPropertyRelative("body");

            EditorGUILayout.PropertyField(includeTitleProp, new GUIContent("Include Title"));

            if (includeTitleProp.boolValue)
            {
                //Title
                EditorGUILayout.PropertyField(titleProp, new GUIContent("Title"), true);

                EditorGUILayout.PropertyField(spaceProp, new GUIContent("Gap Between Title and Body"), true);
            }

            //Body
            EditorGUILayout.PropertyField(bodyProp, new GUIContent("Body"), true);
        }
    }
}