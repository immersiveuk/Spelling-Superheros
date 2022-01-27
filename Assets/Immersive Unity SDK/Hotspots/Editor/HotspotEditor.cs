using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SocialPlatforms;

//==============================================================
// CUSTOM EDITOR
//==============================================================

namespace Com.Immersive.Hotspots
{
    /// <summary>
    /// This implements a custom editor for a Hotspot
    /// </summary>
    [CustomEditor(typeof(HotspotScript)), CanEditMultipleObjects]
    public class HotspotEditor : Editor
    {
        private HotspotScript hotspot;

        private int currentTab = 0;

        private HotspotActionInspectorGUI hotspotActionInspectorGUI = null;

        //Hotspot Settings
        private SerializedProperty hotspotDataModel;
        private SerializedProperty clickAction;
        private SerializedProperty playAudioOnAction;
        private SerializedProperty clickAudio;
        private SerializedProperty actionType;

        //PopUpDataModels
        private SerializedProperty imagePopUpDataModel;
        private SerializedProperty imageSequencePopUpDataModel;
        private SerializedProperty videoPopUpDataModel;
        private SerializedProperty textPopUpDataModel;
        private SerializedProperty quizPopUpDataModel;
        private SerializedProperty audioPopUpDataModel;
        private SerializedProperty splitDataModel;
        private SerializedProperty quizPopUpDataModel_V2;
        private SerializedProperty textSequencePopupDataModel;

        //SceneLink Settings
        private SerializedProperty sceneLinkDataModel;

        //Reveal or Hide Settings
        private SerializedProperty objectsToHide;
        private SerializedProperty objectsToReveal;

        //Custom PopUp
        private SerializedProperty customPopUpSpawner;

        //PopUp Prefabs
        private SerializedProperty videoPopUpPrefab;
        private SerializedProperty imagePopUpPrefab;
        private SerializedProperty imageSequencePopUpPrefab;
        private SerializedProperty textPopUpPrefab;
        private SerializedProperty qAndAPopUpPrefab;
        private SerializedProperty audioPopUpPrefab;
        private SerializedProperty splitPopupPrefab;
        private SerializedProperty quizPopUpPrefab_V2;
        private SerializedProperty textSequencePopUpPrefab;

        //Find referenced objects
        private void OnEnable()
        {
            hotspot = (HotspotScript)target;

            //HotspotSettings
            hotspotDataModel = serializedObject.FindProperty(nameof(hotspot.hotspotDataModel));
            clickAction = hotspotDataModel.FindPropertyRelative(nameof(hotspot.hotspotDataModel.clickAction));
            playAudioOnAction = hotspotDataModel.FindPropertyRelative(nameof(hotspot.hotspotDataModel.playAudioOnAction));
            clickAudio = hotspotDataModel.FindPropertyRelative(nameof(hotspot.hotspotDataModel.clickAudio));
            actionType = hotspotDataModel.FindPropertyRelative(nameof(hotspot.hotspotDataModel.actionType));

            //Find PopUpDataModels
            imagePopUpDataModel = serializedObject.FindProperty(nameof(hotspot.imagePopUpDataModel));
            imageSequencePopUpDataModel = serializedObject.FindProperty(nameof(hotspot.imageSequencePopUpDataModel));
            videoPopUpDataModel = serializedObject.FindProperty(nameof(hotspot.videoPopUpDataModel));
            textPopUpDataModel = serializedObject.FindProperty(nameof(hotspot.textPopUpDataModel));
            quizPopUpDataModel = serializedObject.FindProperty(nameof(hotspot.quizPopUpDataModel));
            audioPopUpDataModel = serializedObject.FindProperty(nameof(hotspot.audioPopUpDataModel));
            splitDataModel = serializedObject.FindProperty(nameof(hotspot.splitPopupDataModel));
            quizPopUpDataModel_V2 = serializedObject.FindProperty(nameof(hotspot.quizPopUpDataModel_V2));
            textSequencePopupDataModel = serializedObject.FindProperty(nameof(hotspot.textSequencePopUpDataModel));

            //SceneLink
            sceneLinkDataModel = serializedObject.FindProperty(nameof(hotspot.sceneLinkDataModel));
            
            //Reveal or Hide Settings
            objectsToHide = serializedObject.FindProperty("objectsToHide");
            objectsToReveal = serializedObject.FindProperty("objectsToReveal");

            //Custom PopUp
            customPopUpSpawner = serializedObject.FindProperty(nameof(customPopUpSpawner));

            //PopUp Prefabs
            videoPopUpPrefab = serializedObject.FindProperty(nameof(hotspot.videoPopUpPrefab));
            imagePopUpPrefab = serializedObject.FindProperty(nameof(hotspot.imagePopUpPrefab));
            imageSequencePopUpPrefab = serializedObject.FindProperty(nameof(hotspot.imageSequencePopUpPrefab));
            textPopUpPrefab = serializedObject.FindProperty(nameof(hotspot.textPopUpPrefab));
            qAndAPopUpPrefab = serializedObject.FindProperty(nameof(hotspot.qAndAPopUpPrefab));
            audioPopUpPrefab = serializedObject.FindProperty(nameof(hotspot.audioPopUpPrefab));
            splitPopupPrefab = serializedObject.FindProperty(nameof(hotspot.splitPopupPrefab));
            quizPopUpPrefab_V2 = serializedObject.FindProperty(nameof(hotspot.quizPopUpPrefab_V2));
            textSequencePopUpPrefab = serializedObject.FindProperty(nameof(hotspot.textSequencePopUpPrefab));

            SetupHotspotActionInspectorGUI();
        }

        public override void OnInspectorGUI()
        {
            //DrawDefaultInspector();

            serializedObject.Update();

            EditorGUILayout.Space();

            //Draw Tabs
            EditorGUILayout.Space();
            currentTab = GUILayout.Toolbar(currentTab, new string[] { "Settings", "PopUps Prefabs" });
            EditorGUILayout.Space();

            switch (currentTab)
            {
                case 0:
                    OnInspectorGUISettings();
                    break;
                case 1:
                    OnInspectorGUIPopUpPrefabs();
                    break;
            }
        }

        private void OnInspectorGUISettings()
        {
            EditorGUILayout.LabelField("Hotspot Settings", EditorStyles.boldLabel);

            //General Settings
            EditorGUILayout.PropertyField(clickAction, new GUIContent("When Selected", "What should be done to the when it is selected."));

            EditorGUILayout.Space();

            //AUDIO
            EditorGUILayout.PropertyField(playAudioOnAction);

            if (hotspot.hotspotDataModel.playAudioOnAction)
            {
                EditorGUILayout.PropertyField(clickAudio, true);
            }

            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Hotspot Action Settings", EditorStyles.boldLabel);

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(actionType, new GUIContent("Action"));

            bool actionWasChanged = EditorGUI.EndChangeCheck();

            EditorGUI.indentLevel++;

            hotspotActionInspectorGUI.OnInspectorGUI();

            serializedObject.ApplyModifiedProperties();
            if (actionWasChanged)
                SetupHotspotActionInspectorGUI();
        }

        private void SetupHotspotActionInspectorGUI()
        {
            switch (hotspot.hotspotDataModel.actionType)
            {
                case ActionType.ActivateAndHideObjects:
                    hotspotActionInspectorGUI = new ActivateAndDisableHotspotInspectorGUI(objectsToHide, objectsToReveal);
                    break;
                case ActionType.SceneLink:
                    hotspotActionInspectorGUI = new SceneLinkPopupSettingsInspectorGUI(sceneLinkDataModel.FindPropertyRelative(nameof(hotspot.sceneLinkDataModel.sceneLinkSettings)), hotspot.sceneLinkDataModel.sceneLinkSettings);
                    break;

                case ActionType.ImagePopup:
                    hotspotActionInspectorGUI = new ImagePopupSettingsInspectorGUI(imagePopUpDataModel.FindPropertyRelative(nameof(hotspot.imagePopUpDataModel.popUpSetting)), hotspot.imagePopUpDataModel.popUpSetting);
                    break;
                case ActionType.ImageSequencePopup:
                    hotspotActionInspectorGUI = new ImageSequencePopUpSettingsInspectorGUI(imageSequencePopUpDataModel.FindPropertyRelative(nameof(hotspot.imagePopUpDataModel.popUpSetting)), hotspot.imageSequencePopUpDataModel.popUpSetting);
                    break;
                case ActionType.QuizPopup:
                    hotspotActionInspectorGUI = new QuizPopupSettingsInspectorGUI(quizPopUpDataModel.FindPropertyRelative(nameof(hotspot.quizPopUpDataModel.popUpSetting)), hotspot.quizPopUpDataModel.popUpSetting);
                    break;
                case ActionType.VideoPopup:
                    hotspotActionInspectorGUI = new VideoPopupSettingsInspectorGUI(videoPopUpDataModel.FindPropertyRelative(nameof(hotspot.videoPopUpDataModel.popUpSetting)), hotspot.videoPopUpDataModel.popUpSetting);
                    break;
                case ActionType.TextPopup:
                    hotspotActionInspectorGUI = new TextPopupSettingsInspectorGUI(textPopUpDataModel.FindPropertyRelative(nameof(hotspot.textPopUpDataModel.popUpSetting)), hotspot.textPopUpDataModel.popUpSetting);
                    break;
                case ActionType.TextSequencePopup:
                    hotspotActionInspectorGUI = new TextSequencePopupSettingsInspectorGUI(textSequencePopupDataModel, hotspot.textSequencePopUpDataModel);
                    break;
                case ActionType.AudioPopup:
                    hotspotActionInspectorGUI = new AudioPopupSettingsInspectorGUI(audioPopUpDataModel.FindPropertyRelative(nameof(hotspot.audioPopUpDataModel.popUpSetting)), hotspot.audioPopUpDataModel.popUpSetting);
                    break;
                case ActionType.SplitPopup:
                    hotspotActionInspectorGUI = new SplitPopupSettingsInspectorGUI(splitDataModel.FindPropertyRelative(nameof(hotspot.splitPopupDataModel.popUpSetting)), hotspot.splitPopupDataModel.popUpSetting);
                    break;
                case ActionType.QuizPopup_V2:
                    hotspotActionInspectorGUI = new QuizV2PopupSettingsInspectorGUI(quizPopUpDataModel_V2, hotspot.quizPopUpDataModel_V2);
                    break;
                case ActionType.Event:
                    hotspotActionInspectorGUI = new HotspotEventInspectorGUI(serializedObject.FindProperty(nameof(hotspot.hotspotEvent)));
                    break;

                case ActionType.CustomPopUp:
                    hotspotActionInspectorGUI = new CustomPopUpInspectorGUI(customPopUpSpawner, () => serializedObject.ApplyModifiedProperties());
                    break;
            }

            //Debug.Log($"Hotspot Action is {hotspotActionInspectorGUI}.");
        }

        private void OnInspectorGUIPopUpPrefabs()
        {
            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(videoPopUpPrefab, new GUIContent("Video"));
            EditorGUILayout.PropertyField(imagePopUpPrefab, new GUIContent("Image"));
            EditorGUILayout.PropertyField(imageSequencePopUpPrefab, new GUIContent("Image Sequence"));
            EditorGUILayout.PropertyField(textPopUpPrefab, new GUIContent("Text"));
            EditorGUILayout.PropertyField(qAndAPopUpPrefab, new GUIContent("Q&A"));
            EditorGUILayout.PropertyField(audioPopUpPrefab, new GUIContent("Audio"));
            EditorGUILayout.PropertyField(splitPopupPrefab, new GUIContent("Split"));
            EditorGUILayout.PropertyField(quizPopUpPrefab_V2, new GUIContent("Q&A V2"));
            EditorGUILayout.PropertyField(textSequencePopUpPrefab, new GUIContent("Text Sequence"));

            serializedObject.ApplyModifiedProperties();
        }
    }
}