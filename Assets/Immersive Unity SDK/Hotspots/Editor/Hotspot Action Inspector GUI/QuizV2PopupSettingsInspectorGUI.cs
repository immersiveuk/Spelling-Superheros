using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Com.Immersive.Hotspots
{
    public class QuizV2PopupSettingsInspectorGUI : PopUpSequenceSettingsInspectorGUI<QuizPopUpSetting_V2>
    {
        public SerializedProperty questions;
        public SerializedProperty disableCloseButton;
        public SerializedProperty duration;
        public SerializedProperty randomiseQuestions;
        public SerializedProperty randomiseOption;
        public SerializedProperty correctClip;
        public SerializedProperty incorrectClip;
        public SerializedProperty timesUpClip;

        public QuizV2PopupSettingsInspectorGUI(SerializedProperty popupSettingsSerializedProp, QuizPopUpSetting_V2 quizPopUpSetting_V2) : base(popupSettingsSerializedProp, quizPopUpSetting_V2)
        {
            questions = popupSettingsSerializedProp.FindPropertyRelative(nameof(questions));
            disableCloseButton = popupSettingsSerializedProp.FindPropertyRelative(nameof(disableCloseButton));
            duration = popupSettingsSerializedProp.FindPropertyRelative(nameof(duration));
            randomiseQuestions = popupSettingsSerializedProp.FindPropertyRelative(nameof(randomiseQuestions));
            randomiseOption = popupSettingsSerializedProp.FindPropertyRelative(nameof(randomiseOption));
            correctClip = popupSettingsSerializedProp.FindPropertyRelative(nameof(correctClip));
            incorrectClip = popupSettingsSerializedProp.FindPropertyRelative(nameof(incorrectClip));
            timesUpClip = popupSettingsSerializedProp.FindPropertyRelative(nameof(timesUpClip));
        }

        protected override SerializedProperty SequenceProperty => questions;

        protected override void DrawSettings()
        {
            EditorGUILayout.PropertyField(disableCloseButton, new GUIContent("Disable Close Button"));
            EditorGUILayout.PropertyField(duration, new GUIContent("Time Limit", "Time Limit of each question"));

            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(randomiseQuestions, new GUIContent("Randomise Questions"));
            EditorGUILayout.PropertyField(randomiseOption, new GUIContent("Randomise Option"));

            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(correctClip, new GUIContent("Correct Answer Audio"));
            EditorGUILayout.PropertyField(incorrectClip, new GUIContent("Incorrect Answer Audio"));
            EditorGUILayout.PropertyField(timesUpClip, new GUIContent("Time Running Out Audio"));
        }

        void DrawQuestion(SerializedProperty questionPage)
        {
            SerializedProperty question = questionPage.FindPropertyRelative("question");

            EditorGUILayout.PropertyField(question, new GUIContent("Question"), true);

            EditorGUILayout.Space();
            DrawOptions(questionPage);
        }

        void DrawOptions(SerializedProperty questionPage)
        {
            SerializedProperty options = questionPage.FindPropertyRelative("options");
            EnsureOptionSize(options);



            EditorGUILayout.PropertyField(options, new GUIContent("Options"), true);
        }

        void EnsureOptionSize(SerializedProperty options)
        {
            if (options.FindPropertyRelative("options").arraySize < 4)
            {
                for (int i = options.FindPropertyRelative("options").arraySize; i < 4; i++)
                {
                    options.FindPropertyRelative("options").InsertArrayElementAtIndex(i);
                }
            }

            if (options.FindPropertyRelative("options").arraySize > 4)
            {
                for (int i = 4; i < options.FindPropertyRelative("options").arraySize; i++)
                {
                    options.FindPropertyRelative("options").DeleteArrayElementAtIndex(i);
                }
            }
        }

        protected override void DrawSequenceElement(int index)
        {
            DrawQuestion(questions.GetArrayElementAtIndex(index));
        }
    }
}
