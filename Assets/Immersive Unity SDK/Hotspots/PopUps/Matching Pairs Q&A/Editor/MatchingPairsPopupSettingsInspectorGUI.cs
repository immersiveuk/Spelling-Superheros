using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Com.Immersive.Hotspots
{
    public class MatchingPairsPopupSettingsInspectorGUI : PopUpSequenceSettingsInspectorGUI<MatchingPairPopUpSetting>
    {
        public SerializedProperty matchingPairQuestions;
        public SerializedProperty disableCloseButton;
        public SerializedProperty enableTimer;
        public SerializedProperty duration;        
        public SerializedProperty correctClip;
        public SerializedProperty incorrectClip;
        public SerializedProperty timesUpClip;

        public MatchingPairsPopupSettingsInspectorGUI(SerializedProperty popupSettingsSerializedProp, MatchingPairPopUpSetting matchingPairPopUpSetting) : base(popupSettingsSerializedProp, matchingPairPopUpSetting)
        {
            matchingPairQuestions = popupSettingsSerializedProp.FindPropertyRelative(nameof(matchingPairQuestions));
            disableCloseButton = popupSettingsSerializedProp.FindPropertyRelative(nameof(disableCloseButton));
            enableTimer = popupSettingsSerializedProp.FindPropertyRelative(nameof(enableTimer));
            duration = popupSettingsSerializedProp.FindPropertyRelative(nameof(duration));
            correctClip = popupSettingsSerializedProp.FindPropertyRelative(nameof(correctClip));
            incorrectClip = popupSettingsSerializedProp.FindPropertyRelative(nameof(incorrectClip));
            timesUpClip = popupSettingsSerializedProp.FindPropertyRelative(nameof(timesUpClip));
        }

        protected override SerializedProperty SequenceProperty => matchingPairQuestions;

        protected override void DrawAdditionalControlPanelSettings()
        {
            EditorGUILayout.PropertyField(disableCloseButton, new GUIContent("Disable Close Button"));
        }

        protected override void DrawSettings()
        {
            EditorGUILayout.PropertyField(enableTimer, new GUIContent("Enable Timer", "Enable timer for question to anwser"));

            if (popUpSettings.enableTimer)
                EditorGUILayout.PropertyField(duration, new GUIContent("Duration", "Duration of each question"));

            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(correctClip, new GUIContent("Correct Answer Audio"));
            EditorGUILayout.PropertyField(incorrectClip, new GUIContent("Incorrect Answer Audio"));
            EditorGUILayout.PropertyField(timesUpClip, new GUIContent("Time Running Out Audio"));
        }

        void DrawQuestion(SerializedProperty serializedProperty)
        {
            SerializedProperty question = serializedProperty.FindPropertyRelative("question");

            EditorGUILayout.PropertyField(question, new GUIContent("Question"), true);

            EditorGUILayout.Space();
            DrawOptions(serializedProperty);
        }

        void DrawOptions(SerializedProperty matchingPairPage)
        {
            SerializedProperty pairs = matchingPairPage.FindPropertyRelative("pairs");

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("+", GUILayout.Width(30)))
            {
                AddPairElement(pairs);
            }

            if (GUILayout.Button("-", GUILayout.Width(30)))
            {
                DeletePairElement(pairs);
            }

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.LabelField("Left Part", EditorStyles.boldLabel);
            EditorGUILayout.LabelField("Right Part", EditorStyles.boldLabel);            

            EditorGUILayout.EndHorizontal();

            for (int i = 0; i < pairs.arraySize; i++)
            {
                SerializedProperty pair = pairs.GetArrayElementAtIndex(i);

                SerializedProperty leftOption = pair.FindPropertyRelative("leftPart");
                SerializedProperty rightOption = pair.FindPropertyRelative("rightPart");

                EditorGUILayout.BeginHorizontal();

                leftOption.stringValue = EditorGUILayout.TextField(leftOption.stringValue);
                rightOption.stringValue = EditorGUILayout.TextField(rightOption.stringValue);

                EditorGUILayout.EndHorizontal();
            }
        }

        protected override void DrawSequenceElement(int index)
        {
            DrawQuestion(matchingPairQuestions.GetArrayElementAtIndex(index));
        }

        void AddPairElement(SerializedProperty pairs)
        {
            int newIndex = pairs.arraySize;
            pairs.InsertArrayElementAtIndex(newIndex);
        }

        void DeletePairElement(SerializedProperty pairs)
        {
            if (pairs.arraySize > 0)
            {
                int newIndex = pairs.arraySize-1;
                pairs.DeleteArrayElementAtIndex(newIndex);
            }
        }
    }
}
