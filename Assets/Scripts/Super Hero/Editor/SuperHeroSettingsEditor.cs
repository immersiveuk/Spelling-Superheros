using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Immersive.SuperHero
{
    [CustomEditor(typeof(SuperHeroSettings)), CanEditMultipleObjects]
    public class SuperHeroSettingsEditor : Editor
    {
        SerializedProperty headsProperty;
        SerializedProperty bodiesProperty;
        SerializedProperty legsProperty;

        private void OnEnable()
        {
            headsProperty = serializedObject.FindProperty("heads");
            bodiesProperty = serializedObject.FindProperty("bodies");
            legsProperty = serializedObject.FindProperty("legs");
        }

        private void OnDisable()
        {

        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorList.Show(headsProperty, EditorListOption.Buttons | EditorListOption.ListLabel);
            EditorList.Show(bodiesProperty, EditorListOption.Buttons | EditorListOption.ListLabel);
            EditorList.Show(legsProperty, EditorListOption.Buttons | EditorListOption.ListLabel);

            serializedObject.ApplyModifiedProperties();
        }

        void MoveArrayElement(int from, int to)
        {

            headsProperty.MoveArrayElement(from, to);
            bodiesProperty.MoveArrayElement(from, to);
            legsProperty.MoveArrayElement(from, to);
        }

        void InsertArrayElementAtIndex(int index)
        {
            Debug.Log("Add");
            headsProperty.InsertArrayElementAtIndex(index);
            bodiesProperty.InsertArrayElementAtIndex(index);
            legsProperty.InsertArrayElementAtIndex(index);
        }

        void DeleteArrayElementAtIndex(int index)
        {
            int oldSizeHeads = headsProperty.arraySize;
            int oldSizeBodies = bodiesProperty.arraySize;
            int oldSizeLegs = legsProperty.arraySize;

            headsProperty.DeleteArrayElementAtIndex(index);
            bodiesProperty.DeleteArrayElementAtIndex(index);
            legsProperty.DeleteArrayElementAtIndex(index);

            if (headsProperty.arraySize == oldSizeHeads)
            {
                headsProperty.DeleteArrayElementAtIndex(index);
            }

            if (bodiesProperty.arraySize == oldSizeBodies)
            {
                bodiesProperty.DeleteArrayElementAtIndex(index);
            }

            if (legsProperty.arraySize == oldSizeLegs)
            {
                legsProperty.DeleteArrayElementAtIndex(index);
            }
        }
    }
}