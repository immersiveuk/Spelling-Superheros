using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


namespace Immersive.FillInTheBlank
{
    [CustomEditor(typeof(FillInTheBlanksList)), CanEditMultipleObjects]
    public class FillInTheBlanksEditor : Editor
    {
        static SerializedProperty fillInTheBlanks;
        static SerializedProperty spellings;
        static SerializedProperty missingLetters;

        private void OnEnable()
        {
            fillInTheBlanks = serializedObject.FindProperty("fillInTheBlanksData");
            spellings = serializedObject.FindProperty("spellings");
            missingLetters = serializedObject.FindProperty("missingLetters");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorList.Show(fillInTheBlanks, EditorListOption.Buttons);
            EditorGUILayout.Space();
            EditorList.Show(spellings, EditorListOption.ListLabel);
            EditorGUILayout.Space();
            EditorList.Show(missingLetters, EditorListOption.ListLabel);
            serializedObject.ApplyModifiedProperties();
        }

        public static void MoveArrayElement(int from, int to)
        {
            spellings.MoveArrayElement(from,to);
            missingLetters.MoveArrayElement(from, to);
        }

        public static void InsertArrayElementAtIndex(int index)
        {
            spellings.InsertArrayElementAtIndex(index);
            missingLetters.InsertArrayElementAtIndex(index);
        }

        public static void DeleteArrayElementAtIndex(int index)
        {
            spellings.DeleteArrayElementAtIndex(index);
            missingLetters.DeleteArrayElementAtIndex(index);
        }
    }
}