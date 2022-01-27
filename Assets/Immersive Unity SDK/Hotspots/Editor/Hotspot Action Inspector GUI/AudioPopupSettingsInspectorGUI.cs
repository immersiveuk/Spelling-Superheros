using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Com.Immersive.Hotspots
{
    public class AudioPopupSettingsInspectorGUI : PopUpSettingsInspectorGUI<AudioPopUpDataModel.AudioPopUpSetting>
    {
        public SerializedProperty audioClipURL;
        public SerializedProperty audioClip;
        public SerializedProperty useThumbnail;
        public SerializedProperty thumbnail;
        public SerializedProperty closeAfterPlay;
        public SerializedProperty loop;
        public SerializedProperty controlPanelStyle;

        public AudioPopupSettingsInspectorGUI(SerializedProperty popupSettingsSerializedProp, AudioPopUpDataModel.AudioPopUpSetting audioPopUpSetting) : base(popupSettingsSerializedProp, audioPopUpSetting)
        {
            audioClipURL = popupSettingsSerializedProp.FindPropertyRelative(nameof(audioClipURL));
            audioClip = popupSettingsSerializedProp.FindPropertyRelative(nameof(audioClip));
            useThumbnail = popupSettingsSerializedProp.FindPropertyRelative(nameof(useThumbnail));
            controlPanelStyle = popupSettingsSerializedProp.FindPropertyRelative(nameof(controlPanelStyle));
            thumbnail = popupSettingsSerializedProp.FindPropertyRelative(nameof(thumbnail));
            closeAfterPlay = popupSettingsSerializedProp.FindPropertyRelative(nameof(closeAfterPlay));
            loop = popupSettingsSerializedProp.FindPropertyRelative(nameof(loop));
        }

        protected override void DrawMainContentSettings()
        {
            //Audio Clip
            EditorGUILayout.PropertyField(audioClip, new GUIContent("Audio Clip"));

            //Thumbnail
            EditorGUILayout.PropertyField(useThumbnail, new GUIContent("Use Thumbnail"));

            if (popUpSettings.useThumbnail)
            {
                EditorGUILayout.PropertyField(thumbnail, new GUIContent("Thumbnail"), true);
            }
        }

        protected override void DrawSettings()
        {
            //Close when finished
            EditorGUILayout.PropertyField(closeAfterPlay, new GUIContent("Close When Finished"));

            if (!popUpSettings.closeAfterPlay)
            {
                EditorGUILayout.PropertyField(loop, new GUIContent("Loop"));
            }
        }

        protected override void DrawAdditionalControlPanelSettings()
        {
            EditorGUILayout.PropertyField(controlPanelStyle, new GUIContent("Control Panel Style"));
        }
    }
}
