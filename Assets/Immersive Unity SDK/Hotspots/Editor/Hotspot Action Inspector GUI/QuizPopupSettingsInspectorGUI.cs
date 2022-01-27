using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Com.Immersive.Hotspots
{
    public class QuizPopupSettingsInspectorGUI : PopUpSettingsInspectorGUI<QuizPopUpDataModel.QuizPopUpSetting>
    {
        public SerializedProperty question;
        public SerializedProperty background;
        public SerializedProperty options;
        public SerializedProperty result;

        //Results Properties
        private SerializedProperty correctAnswer;
        private SerializedProperty incorrectAnswer;
        private SerializedProperty correctAudio;
        private SerializedProperty incorrectAudio;

        private SerializedProperty tryAgainText;

        public QuizPopupSettingsInspectorGUI(SerializedProperty popupSettingsSerializedProp, QuizPopUpDataModel.QuizPopUpSetting quizPopUpSetting) : base(popupSettingsSerializedProp, quizPopUpSetting)
        {
            question = popupSettingsSerializedProp.FindPropertyRelative(nameof(question));
            background = popupSettingsSerializedProp.FindPropertyRelative(nameof(background));
            options = popupSettingsSerializedProp.FindPropertyRelative(nameof(options));
            result = popupSettingsSerializedProp.FindPropertyRelative(nameof(result));

            correctAnswer = result.FindPropertyRelative(nameof(correctAnswer));
            incorrectAnswer = result.FindPropertyRelative(nameof(incorrectAnswer));
            correctAudio = result.FindPropertyRelative(nameof(correctAudio));
            incorrectAudio = result.FindPropertyRelative(nameof(incorrectAudio));
            tryAgainText = popupSettingsSerializedProp.FindPropertyRelative(nameof(tryAgainText));
        }

        protected override void DrawMainContentSettings()
        {
            DrawPropertyInBox(question, new GUIContent("Question"), true);
            EditorGUILayout.Space();

            //Answer
            DrawPropertyInBox(options, new GUIContent("Answers"), true);
            EditorGUILayout.Space();

            //Result Property
            DrawResults();
        }

        protected override void DrawSettings()
        {
            //Background
            EditorGUILayout.PropertyField(background, new GUIContent("Background"), true);
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            DrawDefaultSizeSettings();
        }

        private void DrawResults()
        {
            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.LabelField("On Result", EditorStyles.boldLabel);

            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.PropertyField(correctAnswer, new GUIContent("Correct"), true);
            EditorGUILayout.PropertyField(correctAudio, new GUIContent("Audio"));
            EditorGUILayout.EndVertical();

            EditorGUILayout.Space();

            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.PropertyField(incorrectAnswer, new GUIContent("Incorrect"), true);
            EditorGUILayout.PropertyField(incorrectAudio, new GUIContent("Audio"));
            EditorGUILayout.PropertyField(tryAgainText, new GUIContent("Try Again Text"));
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndVertical();
        }
    }
}
