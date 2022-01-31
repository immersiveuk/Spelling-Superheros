using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


namespace Immersive.FillInTheBlank
{
    
    [CustomEditor(typeof(FillInTheBlanksData)), CanEditMultipleObjects]
    public class FillInTheBlanksEditor : Editor
    {
        private SerializedProperty letterCase;
        static SerializedProperty fillInTheBlankProperty;
        static SerializedProperty spellingsCells;
        static SerializedProperty missingLettersCells;
        private SerializedProperty spellings;

        private const int minNumberOfSpellings = 4;
        private const int maxNumberOfSpellings = 4;

        private void OnEnable()
        {
            letterCase = serializedObject.FindProperty(nameof(letterCase));
            fillInTheBlankProperty = serializedObject.FindProperty("fillInTheBlanksList");
            spellingsCells = serializedObject.FindProperty("spellings");
            missingLettersCells = serializedObject.FindProperty("missingLetters");
            spellings = fillInTheBlankProperty.FindPropertyRelative(nameof(spellings));
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.Space();
            serializedObject.Update();

            EditorGUILayout.PropertyField(letterCase);

            EditorGUILayout.Space();
            DrawOptions();

            EditorList.Show(spellingsCells, EditorListOption.ListLabel);
            EditorGUILayout.Space();
            EditorList.Show(missingLettersCells, EditorListOption.ListLabel);

            serializedObject.ApplyModifiedProperties();
        }

        protected void DrawOptions()
        {
            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();

            //EditorGUI.BeginDisabledGroup(spellings.arraySize >= maxNumberOfSpellings);
            if (GUILayout.Button("+", GUILayout.Width(30)))
            {
                AddPairElement();
            }
            //EditorGUI.EndDisabledGroup();

            //EditorGUI.BeginDisabledGroup(spellings.arraySize <= minNumberOfSpellings);
            if (GUILayout.Button("-", GUILayout.Width(30)))
            {
                DeletePairElement();
            }
            //EditorGUI.EndDisabledGroup();

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.PropertyField(fillInTheBlankProperty, new GUIContent("Spellings"), true);
        }

        /// <summary>
        /// Add new pair to the list on Plus button click
        /// </summary>
        /// <param name="pairs"></param>
        void AddPairElement()
        {
            int newIndex = spellings.arraySize;
            spellings.InsertArrayElementAtIndex(newIndex);

            if (spellings.arraySize > spellingsCells.arraySize)
            {
                spellingsCells.InsertArrayElementAtIndex(spellingsCells.arraySize);
                spellingsCells.GetArrayElementAtIndex(spellingsCells.arraySize - 1).objectReferenceValue = null;
            }

            if (spellings.arraySize > missingLettersCells.arraySize)
            {
                missingLettersCells.InsertArrayElementAtIndex(missingLettersCells.arraySize);
                missingLettersCells.GetArrayElementAtIndex(spellingsCells.arraySize - 1).objectReferenceValue = null;
            }
        }


        /// <summary>
        /// Remove last pair on Minus button click 
        /// </summary>
        /// <param name="pairs"></param>
        void DeletePairElement()
        {
            if (spellings.arraySize > 0)
            {
                int newIndex = spellings.arraySize - 1;
                spellings.DeleteArrayElementAtIndex(newIndex);
            }

            if (spellings.arraySize < spellingsCells.arraySize)
                spellingsCells.DeleteArrayElementAtIndex(spellingsCells.arraySize - 1);

            if (spellings.arraySize < missingLettersCells.arraySize)
                missingLettersCells.DeleteArrayElementAtIndex(missingLettersCells.arraySize - 1);
        }
    }
}