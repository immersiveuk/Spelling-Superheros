using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Com.Immersive.Hotspots
{
    public class TextPopupSettingsInspectorGUI : PopUpSettingsInspectorGUI<TextPopUpDataModel.TextPopUpSetting>
    {
        public SerializedProperty includeTitle;
        public SerializedProperty title;
        public SerializedProperty space; //Space Line between Title and Body
        public SerializedProperty body;
        public SerializedProperty background;

        public TextPopupSettingsInspectorGUI(SerializedProperty popupSettingsSerializedProp, TextPopUpDataModel.TextPopUpSetting textPopUpSetting) : base(popupSettingsSerializedProp, textPopUpSetting)
        {
            includeTitle = popupSettingsSerializedProp.FindPropertyRelative(nameof(includeTitle));
            title = popupSettingsSerializedProp.FindPropertyRelative(nameof(title));
            space = popupSettingsSerializedProp.FindPropertyRelative(nameof(space));
            body = popupSettingsSerializedProp.FindPropertyRelative(nameof(body));
            background = popupSettingsSerializedProp.FindPropertyRelative(nameof(background));
        }

        protected override void DrawMainContentSettings()
        {
            EditorGUILayout.PropertyField(includeTitle, new GUIContent("Include Title"), true);

            if (popUpSettings.includeTitle)
            {
                //Title
                DrawPropertyInBox(title, new GUIContent("Title"), true);

                EditorGUILayout.PropertyField(space, new GUIContent("Gap Between Title and Body"), true);
            }

            DrawPropertyInBox(body, new GUIContent("Body"), true);
        }

        protected override void DrawSettings()
        {
            EditorGUILayout.PropertyField(background, new GUIContent("Background"), true);
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            DrawDefaultSizeSettings();
        }
    }
}