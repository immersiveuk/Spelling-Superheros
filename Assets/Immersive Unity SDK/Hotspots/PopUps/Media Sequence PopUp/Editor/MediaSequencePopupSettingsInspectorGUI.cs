using Com.Immersive.Hotspots;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static Com.Immersive.Hotspots.MediaSequencePopUpSetting.MediaPopUp;
using static Com.Immersive.Hotspots.PopUpSequenceSettings ;


public class MediaSequencePopupSettingsInspectorGUI : PopUpSequenceSettingsInspectorGUI<MediaSequencePopUpSetting>
{
    private SerializedProperty mediaPopups;
    private SerializedProperty controlPanelStyle;
    private SerializedProperty useCustomButtons;
    private SerializedProperty nextButton;
    private SerializedProperty previousButton;
    private SerializedProperty mediaPosition;
    private SerializedProperty mediaMask;
    private SerializedProperty border;

    protected override SerializedProperty SequenceProperty => mediaPopups;

    public MediaSequencePopupSettingsInspectorGUI(SerializedProperty popupSettingsSerializedProp, MediaSequencePopUpSetting mediaSequencePopUpSetting) : base(popupSettingsSerializedProp, mediaSequencePopUpSetting)
    {
        mediaPopups = popupSettingsSerializedProp.FindPropertyRelative(nameof(mediaPopups));
        controlPanelStyle = popupSettingsSerializedProp.FindPropertyRelative(nameof(controlPanelStyle));
        useCustomButtons = popupSettingsSerializedProp.FindPropertyRelative(nameof(useCustomButtons));
        nextButton = popupSettingsSerializedProp.FindPropertyRelative(nameof(nextButton));
        previousButton = popupSettingsSerializedProp.FindPropertyRelative(nameof(previousButton));
        
        mediaPosition = popupSettingsSerializedProp.FindPropertyRelative(nameof(mediaPosition));
        mediaMask = popupSettingsSerializedProp.FindPropertyRelative(nameof(mediaMask));
        border = popupSettingsSerializedProp.FindPropertyRelative(nameof(border));
    }

    protected override void DrawSettings()
    {
        EditorGUILayout.PropertyField(mediaMask, new GUIContent("Media Mask"));
        EditorGUILayout.Space();
        EditorGUILayout.PropertyField(border, new GUIContent("Border"));

        base.DrawSettings();
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

    void DrawMediaSettings(SerializedProperty mediaPageProp)
    {
        SerializedProperty mediaType = mediaPageProp.FindPropertyRelative("mediaType");
        SerializedProperty image = mediaPageProp.FindPropertyRelative("image");
        SerializedProperty video = mediaPageProp.FindPropertyRelative("video");
        SerializedProperty loopVideo = mediaPageProp.FindPropertyRelative("loopVideo");

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
        }

        EditorGUI.indentLevel--;
    }

    protected override void DrawSequenceElement(int index)
    {
        DrawMediaSettings(mediaPopups.GetArrayElementAtIndex(index));
    }
}
