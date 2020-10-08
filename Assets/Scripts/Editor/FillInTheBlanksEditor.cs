using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


namespace Immersive.FillInTheBlank
{
    [CustomEditor(typeof(FillInTheBlanksData)), CanEditMultipleObjects]
    public class FillInTheBlanksEditor : Editor
    {
        static SerializedProperty fillInTheBlanks;
        static SerializedProperty spellings;
        static SerializedProperty missingLetters;

        private void OnEnable()
        {
            fillInTheBlanks = serializedObject.FindProperty("fillInTheBlanksList");
            spellings = serializedObject.FindProperty("spellings");
            missingLetters = serializedObject.FindProperty("missingLetters");

            EditorList.OnMoveArrayElement += MoveArrayElement;
            EditorList.OnInsertArrayElement += InsertArrayElementAtIndex;
            EditorList.OnDeleteArrayElement += DeleteArrayElementAtIndex;
        }

        private void OnDisable()
        {
            EditorList.OnMoveArrayElement -= MoveArrayElement;
            EditorList.OnInsertArrayElement -= InsertArrayElementAtIndex;
            EditorList.OnDeleteArrayElement -= DeleteArrayElementAtIndex;
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

        static void MoveArrayElement(int from, int to)
        {
            spellings.MoveArrayElement(from,to);
            missingLetters.MoveArrayElement(from, to);
        }

        void InsertArrayElementAtIndex(int index)
        {
            spellings.InsertArrayElementAtIndex(index);
            missingLetters.InsertArrayElementAtIndex(index);
        }

        void DeleteArrayElementAtIndex(int index)
        {

            int oldSize = spellings.arraySize;
            spellings.DeleteArrayElementAtIndex(index);
            missingLetters.DeleteArrayElementAtIndex(index);

            if (spellings.arraySize == oldSize)
            {
                spellings.DeleteArrayElementAtIndex(index);
                missingLetters.DeleteArrayElementAtIndex(index);
            }
        }
    }
}