using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static Com.Immersive.Hotspots.SplitPopUpDataModel.SplitPopUpSetting;

namespace Com.Immersive.Hotspots
{
    public class SplitPopupSettingsInspectorGUI : PopUpSettingsInspectorGUI<SplitPopUpDataModel.SplitPopUpSetting>
    {
        public SerializedProperty includeTitle;
        public SerializedProperty title;
        public SerializedProperty space; //Space Line between Title and Body
        public SerializedProperty body;
        public SerializedProperty textBackground;
        public SerializedProperty mediaType;
        public SerializedProperty image;
        public SerializedProperty video;
        public SerializedProperty mediaMask;
        public SerializedProperty loopVideo;
        public SerializedProperty videoControl;
        public SerializedProperty fixedPopupSizeImageOffset;
        public SerializedProperty separation;
        public SerializedProperty mediaPosition;

        public SplitPopupSettingsInspectorGUI(SerializedProperty popupSettingsSerializedProp, SplitPopUpDataModel.SplitPopUpSetting splitPopUpSetting) : base(popupSettingsSerializedProp, splitPopUpSetting)
        {
            includeTitle = popupSettingsSerializedProp.FindPropertyRelative(nameof(includeTitle));
            title = popupSettingsSerializedProp.FindPropertyRelative(nameof(title));
            space = popupSettingsSerializedProp.FindPropertyRelative(nameof(space));
            body = popupSettingsSerializedProp.FindPropertyRelative(nameof(body));
            textBackground = popupSettingsSerializedProp.FindPropertyRelative(nameof(textBackground));
            mediaType = popupSettingsSerializedProp.FindPropertyRelative(nameof(mediaType));
            image = popupSettingsSerializedProp.FindPropertyRelative(nameof(image));
            video = popupSettingsSerializedProp.FindPropertyRelative(nameof(video));
            mediaMask = popupSettingsSerializedProp.FindPropertyRelative(nameof(mediaMask));
            loopVideo = popupSettingsSerializedProp.FindPropertyRelative(nameof(loopVideo));
            videoControl = popupSettingsSerializedProp.FindPropertyRelative(nameof(videoControl));
            fixedPopupSizeImageOffset = popupSettingsSerializedProp.FindPropertyRelative(nameof(fixedPopupSizeImageOffset));
            separation = popupSettingsSerializedProp.FindPropertyRelative(nameof(separation));
            mediaPosition = popupSettingsSerializedProp.FindPropertyRelative(nameof(mediaPosition));
        }

        protected override void DrawMainContentSettings()
        {
            DrawTextSettings();
            EditorGUILayout.Space();
            DrawInABox(DrawMediaSettings, "Media Settings");
        }

        protected override void DrawSettings()
        {
            EditorGUILayout.PropertyField(separation, new GUIContent("Separation", "Gap between image and text."));
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            DrawDefaultSizeSettings();
        }

        void DrawTextSettings()
        {
            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.LabelField("Text Settings", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;

            EditorGUILayout.PropertyField(includeTitle, new GUIContent("Include Title"), true);

            if (popUpSettings.includeTitle)
            {
                //TITLE
                DrawPropertyInBox(title, new GUIContent("Title"), true);
                EditorGUILayout.PropertyField(space, new GUIContent("Gap Between Title and Body"), true);
            }

            //BODY
            DrawPropertyInBox(body, new GUIContent("Body"), true);

            //Background
            EditorGUILayout.PropertyField(textBackground, new GUIContent("Text Background"), true);

            //PADDING
            //EditorGUILayout.PropertyField(padding, new GUIContent("Padding", "Padding around the text"), true);

            EditorGUILayout.Space();
            EditorGUI.indentLevel--;
            EditorGUILayout.EndVertical();
        }

        void DrawMediaSettings()
        {
            EditorGUI.indentLevel++;

            //MEDIA TYPE

            //Media Position
            EditorGUILayout.PropertyField(mediaPosition, new GUIContent("Media Position"));

            EditorGUILayout.PropertyField(mediaMask, new GUIContent("Media Mask"));

            EditorGUILayout.PropertyField(mediaType, new GUIContent("MediaType"), true);

            //IMAGE
            if (popUpSettings.mediaType == MediaType.Image)
            {
                EditorGUILayout.PropertyField(image, new GUIContent("Image"), true);
            }
            //VIDEO
            else
            {
                EditorGUILayout.PropertyField(video, new GUIContent("Video"), true);
                EditorGUILayout.PropertyField(loopVideo, new GUIContent("Loop"));
                EditorGUILayout.PropertyField(videoControl, new GUIContent("Video Control"));
            }

            EditorGUI.indentLevel--;
        }
    }
}