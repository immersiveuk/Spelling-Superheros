using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

//==============================================================
// CUSTOM EDITOR
//==============================================================

namespace Com.Immersive.Hotspots
{
#if UNITY_EDITOR

    /// <summary>
    /// This implements a custom editor for a Hotspot
    /// </summary>
    [CustomEditor(typeof(HotspotScript)), CanEditMultipleObjects]
    public class HotspotEditor : Editor
    {
        private HotspotScript hotspot;

        private int currentTab = 0;

        private Vector2 _popUpImageResolution;
        private Vector2 _popUpImageScaledResolution;

        //Hotspot Settings
        private SerializedProperty hotspotDataModel;
        private Dictionary<string, SerializedProperty> hotspotDataModelDictionary;

        //Image Settings
        private SerializedProperty imagePopUpDataModel;
        private Dictionary<string, SerializedProperty> imagePopUpDataModelDictionary;

        //Image Sequence Settings
        private SerializedProperty imageSequencePopUpDataModel;
        private Dictionary<string, SerializedProperty> imageSequencePopUpDataModelDictionary;

        //Video Settings
        private SerializedProperty videoPopUpDataModel;
        private Dictionary<string, SerializedProperty> videoPopUpDataModelDictionary;

        //Text Settings
        private SerializedProperty textPopUpDataModel;
        private Dictionary<string, SerializedProperty> textPopUpDataModelDictionary;

        //Q&A Settings
        private SerializedProperty quizPopUpDataModel;
        private Dictionary<string, SerializedProperty> quizPopUpDataModelDictionary;

        //SceneLink Settings
        private SerializedProperty sceneLinkDataModel;
        private Dictionary<string, SerializedProperty> sceneLinkDataModelDictionary;

        //Reveal or Hide Settings
        private SerializedProperty objectsToHide;
        private SerializedProperty objectsToReveal;

        //Audio popup Settings
        private SerializedProperty audioPopUpDataModel;
        private Dictionary<string, SerializedProperty> audioPopUpDataModelDictionary;

        //Split Popup Settings	
        private SerializedProperty splitDataModel;
        private Dictionary<string, SerializedProperty> splitPopUpDataModelDictionary;

        //PopUp Prefabs
        private SerializedProperty videoPopUpPrefab;
        private SerializedProperty imagePopUpPrefab;
        private SerializedProperty imageSequencePopUpPrefab;
        private SerializedProperty textPopUpPrefab;
        private SerializedProperty qAndAPopUpPrefab;
        private SerializedProperty audioPopUpPrefab;
        private SerializedProperty splitPopupPrefab;

        //Find referenced objects
        private void OnEnable()
        {
            hotspot = (HotspotScript)target;
            if (hotspot.imagePopUpDataModel.popUpSetting.background.sprite != null)
                _popUpImageResolution = hotspot.imagePopUpDataModel.popUpSetting.background.sprite.rect.size;

            //currentPopUpSettingDictionary           = new Dictionary<string, SerializedProperty>();
            hotspotDataModelDictionary = new Dictionary<string, SerializedProperty>();

            textPopUpDataModelDictionary = new Dictionary<string, SerializedProperty>();
            quizPopUpDataModelDictionary = new Dictionary<string, SerializedProperty>();
            imagePopUpDataModelDictionary = new Dictionary<string, SerializedProperty>();
            imageSequencePopUpDataModelDictionary = new Dictionary<string, SerializedProperty>();
            videoPopUpDataModelDictionary = new Dictionary<string, SerializedProperty>();
            audioPopUpDataModelDictionary = new Dictionary<string, SerializedProperty>();
            sceneLinkDataModelDictionary = new Dictionary<string, SerializedProperty>();
            splitPopUpDataModelDictionary = new Dictionary<string, SerializedProperty>();

            //HotspotSettings
            hotspotDataModel = serializedObject.FindProperty(nameof(hotspot.hotspotDataModel));
            GetChildren(hotspotDataModel, hotspotDataModelDictionary);

            //Image Settings
            imagePopUpDataModel = serializedObject.FindProperty(nameof(hotspot.imagePopUpDataModel));
            GetChildren(imagePopUpDataModel.FindPropertyRelative(nameof(hotspot.imagePopUpDataModel.popUpSetting)), imagePopUpDataModelDictionary);

            //Image Sequence Settings
            imageSequencePopUpDataModel = serializedObject.FindProperty(nameof(hotspot.imageSequencePopUpDataModel));
            GetChildren(imageSequencePopUpDataModel.FindPropertyRelative(nameof(hotspot.imageSequencePopUpDataModel.popUpSetting)), imageSequencePopUpDataModelDictionary);

            //Video Settings
            videoPopUpDataModel = serializedObject.FindProperty(nameof(hotspot.videoPopUpDataModel));
            GetChildren(videoPopUpDataModel.FindPropertyRelative(nameof(hotspot.videoPopUpDataModel.popUpSetting)), videoPopUpDataModelDictionary);

            //Text Settings
            textPopUpDataModel = serializedObject.FindProperty(nameof(hotspot.textPopUpDataModel));
            GetChildren(textPopUpDataModel.FindPropertyRelative(nameof(hotspot.textPopUpDataModel.popUpSetting)), textPopUpDataModelDictionary);

            //Q&A Settings
            quizPopUpDataModel = serializedObject.FindProperty(nameof(hotspot.quizPopUpDataModel));
            GetChildren(quizPopUpDataModel.FindPropertyRelative(nameof(hotspot.quizPopUpDataModel.popUpSetting)), quizPopUpDataModelDictionary);

            //SceneLink Settings
            sceneLinkDataModel = serializedObject.FindProperty(nameof(hotspot.sceneLinkDataModel));
            GetChildren(sceneLinkDataModel.FindPropertyRelative(nameof(hotspot.sceneLinkDataModel.popUpSetting)), sceneLinkDataModelDictionary);

            //Reveal or Hide Settings
            objectsToHide = serializedObject.FindProperty("objectsToHide");
            objectsToReveal = serializedObject.FindProperty("objectsToReveal");

            //Audio popup Settings
            audioPopUpDataModel = serializedObject.FindProperty(nameof(hotspot.audioPopUpDataModel));
            GetChildren(audioPopUpDataModel.FindPropertyRelative(nameof(hotspot.audioPopUpDataModel.popUpSetting)), audioPopUpDataModelDictionary);

            //Split popup Settings	
            splitDataModel = serializedObject.FindProperty(nameof(hotspot.splitPopupDataModel));
            GetChildren(splitDataModel.FindPropertyRelative(nameof(hotspot.splitPopupDataModel.popUpSetting)), splitPopUpDataModelDictionary);

            //PopUp Prefabs
            videoPopUpPrefab = serializedObject.FindProperty(nameof(hotspot.videoPopUpPrefab));
            imagePopUpPrefab = serializedObject.FindProperty(nameof(hotspot.imagePopUpPrefab));
            imageSequencePopUpPrefab = serializedObject.FindProperty(nameof(hotspot.imageSequencePopUpPrefab));
            textPopUpPrefab = serializedObject.FindProperty(nameof(hotspot.textPopUpPrefab));
            qAndAPopUpPrefab = serializedObject.FindProperty(nameof(hotspot.qAndAPopUpPrefab));
            audioPopUpPrefab = serializedObject.FindProperty(nameof(hotspot.audioPopUpPrefab));
            splitPopupPrefab = serializedObject.FindProperty(nameof(hotspot.splitPopupPrefab));
        }

        public override void OnInspectorGUI()
        {
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

            serializedObject.ApplyModifiedProperties();
        }

        private void OnInspectorGUISettings()
        {
            EditorGUILayout.LabelField("Hotspot Settings", EditorStyles.boldLabel);

            //General Settings
            EditorGUILayout.PropertyField(hotspotDataModelDictionary[nameof(hotspot.hotspotDataModel.clickAction)], new GUIContent("When Selected", "What should be done to the when it is selected."));

            EditorGUILayout.Space();

            //AUDIO
            EditorGUILayout.PropertyField(hotspotDataModelDictionary[nameof(hotspot.hotspotDataModel.playAudioOnAction)]);

            if (hotspot.hotspotDataModel.playAudioOnAction)
            {
                EditorGUILayout.PropertyField(hotspotDataModelDictionary[nameof(hotspot.hotspotDataModel.clickAudio)], true);
            }

            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Popup Settings", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(hotspotDataModelDictionary[nameof(hotspot.hotspotDataModel.actionType)], new GUIContent("Action"));

            EditorGUI.indentLevel++;

            switch (hotspot.hotspotDataModel.actionType)
            {
                // Image Specific Settings
                case ActionType.ImagePopup:
                    ImagePopUp();
                    break;

                // Image Sequence Specific Settings
                case ActionType.ImageSequencePopup:
                    ImageSequencePopup();
                    break;

                // Video Specific Settings
                case ActionType.VideoPopup:
                    VideoPopup();
                    break;

                //Text Specific Settings
                case ActionType.TextPopup:
                    TextPopUp();
                    break;

                //Q&A Specific Settings
                case ActionType.QuizPopup:

                    QuizPopUp();

                    break;

                //Audio Specific Settings
                case ActionType.AudioPopup:
                    AudioPopup();
                    break;

                //SplitPopup
                case ActionType.SplitPopup:

                    SplitPopup();

                    break;

                //SceneLink Specific Settings
                case ActionType.SceneLink:
                    SceneLink();

                    break;

                //Hide Or Reveal Specific Settings
                case ActionType.ActivateAndHideObjects:
                    ActivateAndHideObjects();

                    break;
            }
        }

        void ImagePopUp()
        {
            // Pop Up Position
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(imagePopUpDataModelDictionary[nameof(hotspot.imagePopUpDataModel.popUpSetting.popUpPosition)], new GUIContent("Position", "Where should the pop-up appear on screen."));

            if (hotspot.imagePopUpDataModel.popUpSetting.popUpPosition == PopUpPosition.Custom)
            {
                EditorGUILayout.PropertyField(imagePopUpDataModelDictionary[nameof(hotspot.imagePopUpDataModel.popUpSetting.popUpPositionOffset)], new GUIContent("Offset", "The offset in pixels from the hotspot position."));
            }
            EditorGUILayout.Space();

            //Close Button
            EditorGUILayout.PropertyField(imagePopUpDataModelDictionary[nameof(hotspot.imagePopUpDataModel.popUpSetting.overrideDefaultCloseButton)], new GUIContent("Override Default Close Button"), true);

            if (hotspot.imagePopUpDataModel.popUpSetting.overrideDefaultCloseButton)
            {
                EditorGUILayout.PropertyField(imagePopUpDataModelDictionary[nameof(hotspot.imagePopUpDataModel.popUpSetting.closeButton)], new GUIContent("Close Button"), true);
            }
            EditorGUILayout.Space();

            //Image
            EditorGUILayout.PropertyField(imagePopUpDataModelDictionary[nameof(hotspot.imagePopUpDataModel.popUpSetting.background)], new GUIContent("Sprite"), true);
            EditorGUILayout.Space();

            //Border
            EditorGUILayout.PropertyField(imagePopUpDataModelDictionary[nameof(hotspot.imagePopUpDataModel.popUpSetting.border)], new GUIContent("Border"), true);

            EditorGUILayout.Space();
            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Size");
            EditorGUI.indentLevel++;

            EditorGUILayout.PropertyField(imagePopUpDataModelDictionary[nameof(hotspot.imagePopUpDataModel.popUpSetting.sizeOption)], new GUIContent("Popup Mode"));


            if (hotspot.imagePopUpDataModel.popUpSetting.sizeOption == SizeOption.FixedPopupSize)
            {
                EditorGUILayout.PropertyField(imagePopUpDataModelDictionary[nameof(hotspot.imagePopUpDataModel.popUpSetting.size)], new GUIContent("Size"));

                if (!Application.isPlaying)
                {
                    if (hotspot.imagePopUpDataModel.popUpSetting.background.sprite != null && _popUpImageResolution != hotspot.imagePopUpDataModel.popUpSetting.background.sprite.rect.size)
                    {
                        _popUpImageResolution = hotspot.imagePopUpDataModel.popUpSetting.background.sprite.rect.size;
                        _popUpImageScaledResolution = hotspot.imagePopUpDataModel.popUpSetting.background.sprite.rect.size;
                        hotspot.imagePopUpDataModel.popUpSetting.size = _popUpImageResolution;
                    }

                    Vector2 popupSize;

                    if (hotspot.imagePopUpDataModel.popUpSetting.maintainAspectRatio)
                    {
                        //X value changed
                        if (_popUpImageScaledResolution.x != hotspot.imagePopUpDataModel.popUpSetting.size.x)
                        {
                            var xScale = hotspot.imagePopUpDataModel.popUpSetting.size.x / _popUpImageResolution.x;
                            popupSize = new Vector2(hotspot.imagePopUpDataModel.popUpSetting.size.x, xScale * _popUpImageResolution.y);
                        }
                        //Y value changed
                        else
                        {
                            var yScale = hotspot.imagePopUpDataModel.popUpSetting.size.y / _popUpImageResolution.y;
                            popupSize = new Vector2(yScale * _popUpImageResolution.x, hotspot.imagePopUpDataModel.popUpSetting.size.y);
                        }
                    }
                    //Freeform aspect ratio
                    else
                    {
                        popupSize = hotspot.imagePopUpDataModel.popUpSetting.size;
                    }

                    hotspot.imagePopUpDataModel.popUpSetting.size = popupSize;
                    _popUpImageScaledResolution = popupSize;
                    EditorUtility.SetDirty(hotspot);
                }

                EditorGUILayout.PropertyField(imagePopUpDataModelDictionary[nameof(hotspot.imagePopUpDataModel.popUpSetting.maintainAspectRatio)], new GUIContent("Maintain Aspect Ratio"));

                //Reset Resolution
                if (GUILayout.Button("Reset Resolution"))
                {
                    hotspot.imagePopUpDataModel.popUpSetting.size = _popUpImageResolution;
                    _popUpImageScaledResolution = _popUpImageResolution;
                    EditorUtility.SetDirty(hotspot);
                }
            }
            else if (hotspot.imagePopUpDataModel.popUpSetting.sizeOption == SizeOption.FixedContentSize)
            {
                hotspot.imagePopUpDataModel.popUpSetting.size = hotspot.imagePopUpDataModel.popUpSetting.background.sprite.rect.size;
            }
            else if (hotspot.imagePopUpDataModel.popUpSetting.sizeOption == SizeOption.FixedPercentage)
            {
                EditorGUILayout.PropertyField(imagePopUpDataModelDictionary[nameof(hotspot.imagePopUpDataModel.popUpSetting.percentage)], new GUIContent("Percentage"));
            }

            EditorGUILayout.PropertyField(imagePopUpDataModelDictionary[nameof(hotspot.imagePopUpDataModel.popUpSetting.padding)], new GUIContent("Padding"), true);
            EditorGUI.indentLevel--;
        }

        void ImageSequencePopup()
        {
            // Pop Up Position
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(imageSequencePopUpDataModelDictionary[nameof(hotspot.imageSequencePopUpDataModel.popUpSetting.popUpPosition)], new GUIContent("Position", "Where should the pop-up appear on screen."));
            if (hotspot.imageSequencePopUpDataModel.popUpSetting.popUpPosition == PopUpPosition.Custom)
            {
                EditorGUILayout.PropertyField(imageSequencePopUpDataModelDictionary[nameof(hotspot.imageSequencePopUpDataModel.popUpSetting.popUpPositionOffset)], new GUIContent("Offset", "The offset in pixels from the hotspot position."));
            }
            EditorGUILayout.Space();

            //Close Button
            EditorGUILayout.PropertyField(imageSequencePopUpDataModelDictionary[nameof(hotspot.imageSequencePopUpDataModel.popUpSetting.overrideDefaultCloseButton)], new GUIContent("Override Default Close Button"), true);
            if (hotspot.imageSequencePopUpDataModel.popUpSetting.overrideDefaultCloseButton)
            {
                EditorGUILayout.PropertyField(imageSequencePopUpDataModelDictionary[nameof(hotspot.imageSequencePopUpDataModel.popUpSetting.closeButton)], new GUIContent("Close Button"), true);
            }
            EditorGUILayout.Space();

            //Border
            EditorGUILayout.PropertyField(imageSequencePopUpDataModelDictionary[nameof(hotspot.imageSequencePopUpDataModel.popUpSetting.border)], new GUIContent("Border"), true);

            //Control buttons
            EditorGUILayout.PropertyField(imageSequencePopUpDataModelDictionary[nameof(hotspot.imageSequencePopUpDataModel.popUpSetting.customButtons)], new GUIContent("Use Custom Buttons"));

            if (hotspot.imageSequencePopUpDataModel.popUpSetting.customButtons)
            {
                EditorGUILayout.PropertyField(imageSequencePopUpDataModelDictionary[nameof(hotspot.imageSequencePopUpDataModel.popUpSetting.nextButton)], new GUIContent("Next Button Sprite"), true);

                if (hotspot.imageSequencePopUpDataModel.popUpSetting.controlPanelStyle == ImageSequencePopUpDataModel.ControlPanelStyle.Full)
                {
                    EditorGUILayout.PropertyField(imageSequencePopUpDataModelDictionary[nameof(hotspot.imageSequencePopUpDataModel.popUpSetting.previousButton)], new GUIContent("Prev Button Sprite"), true);
                }
            }

            //Control panel style
            EditorGUILayout.PropertyField(imageSequencePopUpDataModelDictionary[nameof(hotspot.imageSequencePopUpDataModel.popUpSetting.controlPanelStyle)], new GUIContent("Control Panel Style"));

            //Images
            EditorGUILayout.PropertyField(imageSequencePopUpDataModelDictionary[nameof(hotspot.imageSequencePopUpDataModel.popUpSetting.backgroundSprites)], new GUIContent("Sprites"), true);

            //Size
            EditorGUILayout.LabelField("Size");
            EditorGUI.indentLevel++;

            EditorGUILayout.PropertyField(imageSequencePopUpDataModelDictionary[nameof(hotspot.imageSequencePopUpDataModel.popUpSetting.sizeOption)], new GUIContent("Popup Mode"));

            if (hotspot.imageSequencePopUpDataModel.popUpSetting.sizeOption == SizeOption.FixedPopupSize)
            {
                EditorGUILayout.PropertyField(imageSequencePopUpDataModelDictionary[nameof(hotspot.imageSequencePopUpDataModel.popUpSetting.size)], new GUIContent("Popup Size"));
            }
            else if (hotspot.imageSequencePopUpDataModel.popUpSetting.sizeOption == SizeOption.FixedPercentage)
            {
                EditorGUILayout.PropertyField(imageSequencePopUpDataModelDictionary[nameof(hotspot.imageSequencePopUpDataModel.popUpSetting.percentage)], new GUIContent("Percentage"));
            }

            //Padding
            EditorGUILayout.PropertyField(imageSequencePopUpDataModelDictionary[nameof(hotspot.imageSequencePopUpDataModel.popUpSetting.padding)], new GUIContent("Padding"), true);
            EditorGUI.indentLevel--;
        }

        void VideoPopup()
        {
            // Pop Up Position
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(videoPopUpDataModelDictionary[nameof(hotspot.videoPopUpDataModel.popUpSetting.popUpPosition)], new GUIContent("Position", "Where should the pop-up appear on screen."));
            if (hotspot.videoPopUpDataModel.popUpSetting.popUpPosition == PopUpPosition.Custom)
            {
                EditorGUILayout.PropertyField(videoPopUpDataModelDictionary[nameof(hotspot.videoPopUpDataModel.popUpSetting.popUpPositionOffset)], new GUIContent("Offset", "The offset in pixels from the hotspot position."));
            }
            EditorGUILayout.Space();

            //Close Button
            EditorGUILayout.PropertyField(videoPopUpDataModelDictionary[nameof(hotspot.videoPopUpDataModel.popUpSetting.overrideDefaultCloseButton)], new GUIContent("Override Default Close Button"), true);
            if (hotspot.videoPopUpDataModel.popUpSetting.overrideDefaultCloseButton)
            {
                EditorGUILayout.PropertyField(videoPopUpDataModelDictionary[nameof(hotspot.videoPopUpDataModel.popUpSetting.closeButton)], new GUIContent("Close Button"), true);
            }
            EditorGUILayout.Space();

            //Spurce
            EditorGUILayout.PropertyField(videoPopUpDataModelDictionary[nameof(hotspot.videoPopUpDataModel.popUpSetting.video)], new GUIContent("Source"), true);
            EditorGUILayout.Space();

            //Close when finished
            EditorGUILayout.PropertyField(videoPopUpDataModelDictionary[nameof(hotspot.videoPopUpDataModel.popUpSetting.closeAfterPlay)], new GUIContent("Close When Finished"));

            if (!hotspot.videoPopUpDataModel.popUpSetting.closeAfterPlay)
            {
                EditorGUILayout.PropertyField(videoPopUpDataModelDictionary[nameof(hotspot.videoPopUpDataModel.popUpSetting.loop)], new GUIContent("Loop"));
            }
            EditorGUILayout.PropertyField(videoPopUpDataModelDictionary[nameof(hotspot.videoPopUpDataModel.popUpSetting.controlPanelStyle)], new GUIContent("Control Panel Style"));

            //Size
            EditorGUILayout.LabelField("Size");
            EditorGUI.indentLevel++;

            EditorGUILayout.PropertyField(videoPopUpDataModelDictionary[nameof(hotspot.videoPopUpDataModel.popUpSetting.sizeOption)], new GUIContent("Popup Mode"));

            if (hotspot.videoPopUpDataModel.popUpSetting.sizeOption == SizeOption.FixedPopupSize)
            {
                EditorGUILayout.PropertyField(videoPopUpDataModelDictionary[nameof(hotspot.videoPopUpDataModel.popUpSetting.size)], new GUIContent("Popup Size"));
            }
            else if (hotspot.videoPopUpDataModel.popUpSetting.sizeOption == SizeOption.FixedPercentage)
            {
                EditorGUILayout.PropertyField(videoPopUpDataModelDictionary[nameof(hotspot.videoPopUpDataModel.popUpSetting.percentage)], new GUIContent("Percentage"));
            }

            //EditorGUILayout.PropertyField(videoPopUpDataModelDictionary[nameof(hotspot.videoPopUpDataModel.popUpSetting.padding)], new GUIContent("Padding"), true);
            EditorGUI.indentLevel--;
        }

        void QuizPopUp()
        {
            // Pop Up Position
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(quizPopUpDataModelDictionary[nameof(hotspot.quizPopUpDataModel.popUpSetting.popUpPosition)], new GUIContent("Position", "Where should the pop-up appear on screen."));
            if (hotspot.quizPopUpDataModel.popUpSetting.popUpPosition == PopUpPosition.Custom)
            {
                EditorGUILayout.PropertyField(quizPopUpDataModelDictionary[nameof(hotspot.quizPopUpDataModel.popUpSetting.popUpPositionOffset)], new GUIContent("Offset", "The offset in pixels from the hotspot position."));
            }
            EditorGUILayout.Space();

            //Close Button
            EditorGUILayout.PropertyField(quizPopUpDataModelDictionary[nameof(hotspot.quizPopUpDataModel.popUpSetting.overrideDefaultCloseButton)], new GUIContent("Override Default Close Button"), true);
            if (hotspot.quizPopUpDataModel.popUpSetting.overrideDefaultCloseButton)
            {
                EditorGUILayout.PropertyField(quizPopUpDataModelDictionary[nameof(hotspot.quizPopUpDataModel.popUpSetting.closeButton)], new GUIContent("Close Button"), true);
            }
            EditorGUILayout.Space();

            //Question
            EditorGUILayout.PropertyField(quizPopUpDataModelDictionary[nameof(hotspot.quizPopUpDataModel.popUpSetting.question)], new GUIContent("Question"), true);

            //Background
            EditorGUILayout.PropertyField(quizPopUpDataModelDictionary[nameof(hotspot.quizPopUpDataModel.popUpSetting.background)], new GUIContent("Background"), true);

            EditorGUILayout.Space();
            EditorGUILayout.Space();

            //Answer
            EditorGUILayout.PropertyField(quizPopUpDataModelDictionary[nameof(hotspot.quizPopUpDataModel.popUpSetting.options)], new GUIContent("Answer"), true);

            //Result Property
            EditorGUILayout.PropertyField(quizPopUpDataModelDictionary[nameof(hotspot.quizPopUpDataModel.popUpSetting.result)], new GUIContent("Result Property"), true);

            //Size
            EditorGUILayout.LabelField("Size");
            EditorGUI.indentLevel++;

            EditorGUILayout.PropertyField(quizPopUpDataModelDictionary[nameof(hotspot.quizPopUpDataModel.popUpSetting.sizeOption)], new GUIContent("Popup Mode"));

            if (hotspot.quizPopUpDataModel.popUpSetting.sizeOption == SizeOption.FixedPopupSize)
            {
                EditorGUILayout.PropertyField(quizPopUpDataModelDictionary[nameof(hotspot.quizPopUpDataModel.popUpSetting.size)], new GUIContent("Popup Size"));
            }
            else if (hotspot.quizPopUpDataModel.popUpSetting.sizeOption == SizeOption.FixedPercentage)
            {
                EditorGUILayout.PropertyField(quizPopUpDataModelDictionary[nameof(hotspot.quizPopUpDataModel.popUpSetting.percentage)], new GUIContent("Percentage"));
            }

            //Padding
            EditorGUILayout.PropertyField(quizPopUpDataModelDictionary[nameof(hotspot.quizPopUpDataModel.popUpSetting.padding)], new GUIContent("Padding"), true);
            EditorGUI.indentLevel--;
        }

        void TextPopUp()
        {
            // Pop Up Position
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(textPopUpDataModelDictionary[nameof(hotspot.textPopUpDataModel.popUpSetting.popUpPosition)], new GUIContent("Position", "Where should the pop-up appear on screen."));
            if (hotspot.textPopUpDataModel.popUpSetting.popUpPosition == PopUpPosition.Custom)
            {
                EditorGUILayout.PropertyField(textPopUpDataModelDictionary[nameof(hotspot.textPopUpDataModel.popUpSetting.popUpPositionOffset)], new GUIContent("Offset", "The offset in pixels from the hotspot position."));
            }
            EditorGUILayout.Space();

            //Close Button
            EditorGUILayout.PropertyField(textPopUpDataModelDictionary[nameof(hotspot.textPopUpDataModel.popUpSetting.overrideDefaultCloseButton)], new GUIContent("Override Default Close Button"), true);
            if (hotspot.textPopUpDataModel.popUpSetting.overrideDefaultCloseButton)
            {
                EditorGUILayout.PropertyField(textPopUpDataModelDictionary[nameof(hotspot.textPopUpDataModel.popUpSetting.closeButton)], new GUIContent("Close Button"), true);
            }
            EditorGUILayout.Space();

            //Title
            EditorGUILayout.PropertyField(textPopUpDataModelDictionary[nameof(hotspot.textPopUpDataModel.popUpSetting.title)], new GUIContent("Title"), true);

            //Body
            EditorGUILayout.PropertyField(textPopUpDataModelDictionary[nameof(hotspot.textPopUpDataModel.popUpSetting.body)], new GUIContent("Body"), true);

            //Background
            EditorGUILayout.PropertyField(textPopUpDataModelDictionary[nameof(hotspot.textPopUpDataModel.popUpSetting.background)], new GUIContent("Background"), true);

            //Size
            EditorGUILayout.LabelField("Size");
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(textPopUpDataModelDictionary[nameof(hotspot.textPopUpDataModel.popUpSetting.sizeOption)], new GUIContent("Popup Mode"));

            if (hotspot.textPopUpDataModel.popUpSetting.sizeOption == SizeOption.FixedPopupSize)
            {
                EditorGUILayout.PropertyField(textPopUpDataModelDictionary[nameof(hotspot.textPopUpDataModel.popUpSetting.size)], new GUIContent("Popup Size"));
            }
            else if (hotspot.textPopUpDataModel.popUpSetting.sizeOption == SizeOption.FixedPercentage)
            {
                EditorGUILayout.PropertyField(textPopUpDataModelDictionary[nameof(hotspot.textPopUpDataModel.popUpSetting.percentage)], new GUIContent("Percentage"));
            }

            //Padding
            EditorGUILayout.PropertyField(textPopUpDataModelDictionary[nameof(hotspot.textPopUpDataModel.popUpSetting.padding)], new GUIContent("Padding"), true);
            EditorGUI.indentLevel--;
        }

        void AudioPopup()
        {
            // Pop Up Position
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(audioPopUpDataModelDictionary[nameof(hotspot.audioPopUpDataModel.popUpSetting.popUpPosition)], new GUIContent("Position", "Where should the pop-up appear on screen."));
            if (hotspot.audioPopUpDataModel.popUpSetting.popUpPosition == PopUpPosition.Custom)
            {
                EditorGUILayout.PropertyField(audioPopUpDataModelDictionary[nameof(hotspot.audioPopUpDataModel.popUpSetting.popUpPositionOffset)], new GUIContent("Offset", "The offset in pixels from the hotspot position."));
            }
            EditorGUILayout.Space();

            //Close Button
            EditorGUILayout.PropertyField(audioPopUpDataModelDictionary[nameof(hotspot.audioPopUpDataModel.popUpSetting.overrideDefaultCloseButton)], new GUIContent("Override Default Close Button"), true);
            if (hotspot.audioPopUpDataModel.popUpSetting.overrideDefaultCloseButton)
            {
                EditorGUILayout.PropertyField(audioPopUpDataModelDictionary[nameof(hotspot.audioPopUpDataModel.popUpSetting.closeButton)], new GUIContent("Close Button"), true);
            }
            EditorGUILayout.Space();

            //Audio Clip
            EditorGUILayout.PropertyField(audioPopUpDataModelDictionary[nameof(hotspot.audioPopUpDataModel.popUpSetting.audioClip)], new GUIContent("Audio Clip"));

            //Thumbnail
            EditorGUILayout.PropertyField(audioPopUpDataModelDictionary[nameof(hotspot.audioPopUpDataModel.popUpSetting.useThumbnail)], new GUIContent("Use Thumbnail"));
            if (hotspot.audioPopUpDataModel.popUpSetting.useThumbnail)
            {
                EditorGUILayout.PropertyField(audioPopUpDataModelDictionary[nameof(hotspot.audioPopUpDataModel.popUpSetting.thumbnail)], new GUIContent("Thumbnail"), true);
            }

            //Close when finished
            EditorGUILayout.PropertyField(audioPopUpDataModelDictionary[nameof(hotspot.audioPopUpDataModel.popUpSetting.closeAfterPlay)], new GUIContent("Close When Finished"));
            if (!hotspot.audioPopUpDataModel.popUpSetting.closeAfterPlay)
            {
                EditorGUILayout.PropertyField(audioPopUpDataModelDictionary[nameof(hotspot.audioPopUpDataModel.popUpSetting.loop)], new GUIContent("Loop"));
            }

            EditorGUILayout.PropertyField(audioPopUpDataModelDictionary[nameof(hotspot.audioPopUpDataModel.popUpSetting.controlPanelStyle)], new GUIContent("Control Panel Style"));
        }

        void SplitPopup()
        {
            // Pop Up Position
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(splitPopUpDataModelDictionary[nameof(hotspot.splitPopupDataModel.popUpSetting.popUpPosition)], new GUIContent("Position", "Where should the pop-up appear on screen."));
            if (hotspot.splitPopupDataModel.popUpSetting.popUpPosition == PopUpPosition.Custom)
            {
                EditorGUILayout.PropertyField(splitPopUpDataModelDictionary[nameof(hotspot.splitPopupDataModel.popUpSetting.popUpPositionOffset)], new GUIContent("Offset", "The offset in pixels from the hotspot position."));
            }
            EditorGUILayout.Space();

            //Close Button
            EditorGUILayout.PropertyField(splitPopUpDataModelDictionary[nameof(hotspot.splitPopupDataModel.popUpSetting.overrideDefaultCloseButton)], new GUIContent("Override Default Close Button"), true);
            if (hotspot.splitPopupDataModel.popUpSetting.overrideDefaultCloseButton)
            {
                EditorGUILayout.PropertyField(splitPopUpDataModelDictionary[nameof(hotspot.splitPopupDataModel.popUpSetting.closeButton)], new GUIContent("Close Button"), true);
            }


            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Text Settings", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;

            //TITLE
            EditorGUILayout.PropertyField(splitPopUpDataModelDictionary[nameof(hotspot.splitPopupDataModel.popUpSetting.title)], new GUIContent("Title"), true);

            //BODY
            EditorGUILayout.PropertyField(splitPopUpDataModelDictionary[nameof(hotspot.splitPopupDataModel.popUpSetting.body)], new GUIContent("Body"), true);

            //Background
            EditorGUILayout.PropertyField(splitPopUpDataModelDictionary[nameof(hotspot.splitPopupDataModel.popUpSetting.textBackground)], new GUIContent("Text Background"), true);


            //PADDING
            EditorGUILayout.PropertyField(splitPopUpDataModelDictionary[nameof(hotspot.splitPopupDataModel.popUpSetting.padding)], new GUIContent("Padding", "Padding around the text"), true);


            EditorGUILayout.Space();
            EditorGUI.indentLevel--;
            EditorGUILayout.LabelField("Media Settings", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;

            //MEDIA TYPE

            //Media Position
            EditorGUILayout.PropertyField(splitPopUpDataModelDictionary[nameof(hotspot.splitPopupDataModel.popUpSetting.mediaPosition)], new GUIContent("Media Position"));

            EditorGUILayout.PropertyField(splitPopUpDataModelDictionary[nameof(hotspot.splitPopupDataModel.popUpSetting.mediaMask)], new GUIContent("Media Mask"));

            EditorGUILayout.PropertyField(splitPopUpDataModelDictionary[nameof(hotspot.splitPopupDataModel.popUpSetting.mediaType)], new GUIContent("MediaType"), true);

            //IMAGE
            if (hotspot.splitPopupDataModel.popUpSetting.mediaType == SplitPopUpDataModel.SplitPopUpSetting.MediaType.Image)
            {
                EditorGUILayout.PropertyField(splitPopUpDataModelDictionary[nameof(hotspot.splitPopupDataModel.popUpSetting.image)], new GUIContent("Image"), true);
            }
            //VIDEO
            else
            {
                EditorGUILayout.PropertyField(splitPopUpDataModelDictionary[nameof(hotspot.splitPopupDataModel.popUpSetting.video)], new GUIContent("Video"), true);
                EditorGUILayout.PropertyField(splitPopUpDataModelDictionary[nameof(hotspot.splitPopupDataModel.popUpSetting.loopVideo)], new GUIContent("Loop"));
            }

            EditorGUI.indentLevel--;


            EditorGUILayout.LabelField("General Settings", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;

            //SEPARATION
            EditorGUILayout.PropertyField(splitPopUpDataModelDictionary[nameof(hotspot.splitPopupDataModel.popUpSetting.separation)], new GUIContent("Separation", "Gap between image and text."));

            //SIZE
            EditorGUILayout.PropertyField(splitPopUpDataModelDictionary[nameof(hotspot.splitPopupDataModel.popUpSetting.sizeOption)], new GUIContent("Popup Mode"));

            if (hotspot.splitPopupDataModel.popUpSetting.sizeOption == SizeOption.FixedPopupSize)
            {
                EditorGUILayout.PropertyField(splitPopUpDataModelDictionary[nameof(hotspot.splitPopupDataModel.popUpSetting.size)], new GUIContent("Popup Size"));
                EditorGUILayout.PropertyField(splitPopUpDataModelDictionary[nameof(hotspot.splitPopupDataModel.popUpSetting.fixedPopupSizeImageOffset)], new GUIContent("Image Offset"));
            }
            else if (hotspot.splitPopupDataModel.popUpSetting.sizeOption == SizeOption.FixedPercentage)
            {
                EditorGUILayout.PropertyField(splitPopUpDataModelDictionary[nameof(hotspot.splitPopupDataModel.popUpSetting.percentage)], new GUIContent("Percentage"));
            }

            EditorGUI.indentLevel--;
        }

        void SceneLink()
        {
            EditorGUILayout.PropertyField(sceneLinkDataModelDictionary[nameof(hotspot.sceneLinkDataModel.popUpSetting.linkName)], new GUIContent("Scene", "Which scene to link to."));
            EditorGUILayout.PropertyField(sceneLinkDataModelDictionary[nameof(hotspot.sceneLinkDataModel.popUpSetting.fadeOut)], new GUIContent("Fade Out", "Should the scene fade out before changing scene?"));
            if (hotspot.sceneLinkDataModel.popUpSetting.fadeOut)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(sceneLinkDataModelDictionary[nameof(hotspot.sceneLinkDataModel.popUpSetting.fadeOutDuration)], new GUIContent("Fade Out Duration"));
                EditorGUILayout.PropertyField(sceneLinkDataModelDictionary[nameof(hotspot.sceneLinkDataModel.popUpSetting.fadeColor)], new GUIContent("Fade Colour"));
                EditorGUILayout.PropertyField(sceneLinkDataModelDictionary[nameof(hotspot.sceneLinkDataModel.popUpSetting.fadeOutAudio)], new GUIContent("Fade Audio Out"));
                EditorGUI.indentLevel--;
            }
        }

        void ActivateAndHideObjects()
        {
            EditorGUILayout.PropertyField(objectsToHide, true);
            EditorGUILayout.PropertyField(objectsToReveal, true);
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
        }

        void GetChildren(SerializedProperty serializedProperty, Dictionary<string, SerializedProperty> keyValuePairs)
        {
            SerializedProperty currentProperty = serializedProperty.Copy();
            SerializedProperty _currentProperty = serializedProperty.Copy();

            SerializedProperty nextSiblingProperty = serializedProperty.Copy();
            {
                nextSiblingProperty.Next(false);
            }

            if (currentProperty.Next(true))
            {

                do
                {
                    if (SerializedProperty.EqualContents(currentProperty, nextSiblingProperty))
                    {
                        return;
                    }


                    SerializedProperty sp = _currentProperty.FindPropertyRelative(currentProperty.name);

                    if (sp != null)
                        keyValuePairs.Add(currentProperty.name, sp);
                }
                while (currentProperty.Next(false));
            }
        }
    }

#endif
}