using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static Com.Immersive.Hotspots.SplitSequencePopUpSetting.SplitPopUp;

namespace Com.Immersive.Hotspots
{
    public class SplitSequencePopupSettingsInspectorGUI : PopUpSequenceSettingsInspectorGUI<SplitSequencePopUpSetting>
    {
        private SerializedProperty keepSameMedia;
        private SerializedProperty splitPopups;
        private SerializedProperty textBackground;
        private SerializedProperty controlPanelStyle;

        private SerializedProperty includePageCount;
        private SerializedProperty pageCountSettings;
        private SerializedProperty mediaPosition;
        private SerializedProperty mediaMask;
        public SerializedProperty separation;

        protected override SerializedProperty SequenceProperty => splitPopups;

        //int count, previousCount;

        public SplitSequencePopupSettingsInspectorGUI(SerializedProperty popupSettingsSerializedProp, SplitSequencePopUpSetting splitSequencePopUpSetting) : base(popupSettingsSerializedProp, splitSequencePopUpSetting)
        {
            keepSameMedia = popupSettingsSerializedProp.FindPropertyRelative(nameof(keepSameMedia));
            splitPopups = popupSettingsSerializedProp.FindPropertyRelative(nameof(splitPopups));
            textBackground = popupSettingsSerializedProp.FindPropertyRelative(nameof(textBackground));
            controlPanelStyle = popupSettingsSerializedProp.FindPropertyRelative(nameof(controlPanelStyle));

            includePageCount = popupSettingsSerializedProp.FindPropertyRelative(nameof(includePageCount));
            pageCountSettings = popupSettingsSerializedProp.FindPropertyRelative(nameof(pageCountSettings));

            mediaPosition = popupSettingsSerializedProp.FindPropertyRelative(nameof(mediaPosition));
            mediaMask = popupSettingsSerializedProp.FindPropertyRelative(nameof(mediaMask));
            separation = popupSettingsSerializedProp.FindPropertyRelative(nameof(separation));
        }

        protected override void DrawSettings()
        {
            EditorGUILayout.PropertyField(keepSameMedia, new GUIContent("Is Media Universal", "Do all elements in sequence use the same media?"));

            EditorGUILayout.PropertyField(mediaPosition, new GUIContent("Media Position"));
            EditorGUILayout.PropertyField(mediaMask, new GUIContent("Media Mask"));
            EditorGUILayout.PropertyField(separation, new GUIContent("Separation", "Gap between image and text."));

            EditorGUILayout.PropertyField(textBackground, new GUIContent("Text Background"));
            EditorGUILayout.Space();

            base.DrawSettings();
        }

        protected override void DrawMainContentSettings()
        {
            base.DrawMainContentSettings();

            if (splitPopups.arraySize > 0 && popUpSettings.keepSameMedia)
            {
                EditorGUILayout.Space();
                EditorGUILayout.BeginVertical("box");
                DrawMediaSettings(splitPopups.GetArrayElementAtIndex(0));
                EditorGUILayout.EndVertical();
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

        void DrawTextSettings(SerializedProperty textPageProp)
        {
            EditorGUILayout.LabelField("Text Settings", EditorStyles.boldLabel);

            SerializedProperty includeTitleProp = textPageProp.FindPropertyRelative("includeTitle");
            SerializedProperty titleProp = textPageProp.FindPropertyRelative("title");
            SerializedProperty spaceProp = textPageProp.FindPropertyRelative("space");
            SerializedProperty bodyProp = textPageProp.FindPropertyRelative("body");

            EditorGUILayout.PropertyField(includeTitleProp, new GUIContent("Include Title"), true);

            if (includeTitleProp.boolValue)
            {
                //Title
                EditorGUILayout.PropertyField(titleProp, new GUIContent("Title"), true);

                EditorGUILayout.PropertyField(spaceProp, new GUIContent("Gap Between Title and Body"), true);
            }

            //Body
            EditorGUILayout.PropertyField(bodyProp, new GUIContent("Body"), true);
        }

        void DrawMediaSettings(SerializedProperty textPageProp)
        {
            SerializedProperty mediaType = textPageProp.FindPropertyRelative("mediaType");
            SerializedProperty image = textPageProp.FindPropertyRelative("image");
            SerializedProperty video = textPageProp.FindPropertyRelative("video");
            SerializedProperty loopVideo = textPageProp.FindPropertyRelative("loopVideo");
            SerializedProperty videoControl = textPageProp.FindPropertyRelative("videoControl");

            EditorGUILayout.LabelField("Media Settings", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;

            //MEDIA TYPE

            //Media Position
            EditorGUILayout.PropertyField(mediaType, new GUIContent("MediaType"), true);

            //IMAGE
            if ((MediaType)mediaType.intValue == MediaType.Image)
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

        protected override void DrawSequenceElement(int index)
        {
            DrawTextSettings(splitPopups.GetArrayElementAtIndex(index));
            if (popUpSettings.keepSameMedia == false)
                DrawMediaSettings(splitPopups.GetArrayElementAtIndex(index));
        }
    }
}