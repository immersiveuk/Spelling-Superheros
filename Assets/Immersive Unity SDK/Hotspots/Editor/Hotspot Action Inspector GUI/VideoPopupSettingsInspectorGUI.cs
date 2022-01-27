using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Com.Immersive.Hotspots
{
    public class VideoPopupSettingsInspectorGUI : PopUpSettingsInspectorGUI<VideoPopUpDataModel.VideoPopUpSetting>
    {
        public SerializedProperty video;
        private SerializedProperty border;
        private SerializedProperty mediaMask;
        public SerializedProperty closeAfterPlay;
        public SerializedProperty loop ;
        public SerializedProperty controlPanelStyle;
        public SerializedProperty useCustomButtons;
        public SerializedProperty pauseButtonImage;
        public SerializedProperty playButtonImage;
        public SerializedProperty restartButtonImage;

        public VideoPopupSettingsInspectorGUI(SerializedProperty popupSettingsSerializedProp, VideoPopUpDataModel.VideoPopUpSetting videoPopUpSetting) : base(popupSettingsSerializedProp, videoPopUpSetting)
        {
            video = popupSettingsSerializedProp.FindPropertyRelative(nameof(video));
            border = popupSettingsSerializedProp.FindPropertyRelative(nameof(border));
            mediaMask = popupSettingsSerializedProp.FindPropertyRelative(nameof(mediaMask));
            closeAfterPlay = popupSettingsSerializedProp.FindPropertyRelative(nameof(closeAfterPlay));
            loop = popupSettingsSerializedProp.FindPropertyRelative(nameof(loop));
            controlPanelStyle = popupSettingsSerializedProp.FindPropertyRelative(nameof(controlPanelStyle));
            useCustomButtons = popupSettingsSerializedProp.FindPropertyRelative(nameof(useCustomButtons));
            pauseButtonImage = popupSettingsSerializedProp.FindPropertyRelative(nameof(pauseButtonImage));
            playButtonImage = popupSettingsSerializedProp.FindPropertyRelative(nameof(playButtonImage));
            restartButtonImage = popupSettingsSerializedProp.FindPropertyRelative(nameof(restartButtonImage));
        }

        protected override void DrawMainContentSettings()
        {
            EditorGUILayout.PropertyField(video, new GUIContent("Video"), true);
        }

        protected override void DrawSettings()
        {
            //Border
            EditorGUILayout.PropertyField(border, new GUIContent("Border"), true);
            EditorGUILayout.Space();

            //Mask
            EditorGUILayout.PropertyField(mediaMask, new GUIContent("Media Mask"), true);
            EditorGUILayout.Space();

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
            DrawCustomButtonControls();
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            DrawDefaultSizeSettings();
        }

        private void DrawCustomButtonControls()
        {
            //Custom buttons
            EditorGUILayout.PropertyField(useCustomButtons, new GUIContent("Use Custom Buttons"));

            if (popUpSettings.useCustomButtons)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(pauseButtonImage, new GUIContent("Pause Button"));
                EditorGUILayout.PropertyField(playButtonImage, new GUIContent("Play Button"));
                EditorGUILayout.PropertyField(restartButtonImage, new GUIContent("Restart Button"));
                EditorGUI.indentLevel--;
            }
        }
    }
}