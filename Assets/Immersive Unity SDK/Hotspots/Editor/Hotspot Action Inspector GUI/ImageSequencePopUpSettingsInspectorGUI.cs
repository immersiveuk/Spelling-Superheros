using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Com.Immersive.Hotspots
{
    public class ImageSequencePopUpSettingsInspectorGUI : PopUpSequenceSettingsInspectorGUI<ImageSequencePopUpDataModel.ImageSequencePopUpSetting>
    {
        private SerializedProperty border;
        private SerializedProperty backgroundSprites;
        private SerializedProperty mediaMask;

        protected override SerializedProperty SequenceProperty => backgroundSprites;

        public ImageSequencePopUpSettingsInspectorGUI(SerializedProperty popupSettingsSerializedProp, ImageSequencePopUpDataModel.ImageSequencePopUpSetting popUpSettings) : base(popupSettingsSerializedProp, popUpSettings)
        {
            border = popupSettingsSerializedProp.FindPropertyRelative(nameof(border));
            backgroundSprites = popupSettingsSerializedProp.FindPropertyRelative(nameof(backgroundSprites));
            mediaMask = popupSettingsSerializedProp.FindPropertyRelative(nameof(mediaMask));
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            DrawDefaultSizeSettings();
        }

        protected override void DrawSettings()
        {
            //Border
            EditorGUILayout.PropertyField(border, new GUIContent("Border"), true);
            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(mediaMask, new GUIContent("Media Mask"), true);
            EditorGUILayout.Space();

            base.DrawSettings();
        }

        protected override void DrawSequenceElement(int index)
        {
            EditorGUILayout.PropertyField(backgroundSprites.GetArrayElementAtIndex(index), new GUIContent("Image"), true);
        }
    }
}