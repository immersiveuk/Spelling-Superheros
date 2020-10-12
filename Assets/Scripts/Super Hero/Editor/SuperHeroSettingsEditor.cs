using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Immersive.SuperHero
{
    [CustomEditor(typeof(SuperHeroSettings)), CanEditMultipleObjects]
    public class SuperHeroSettingsEditor : Editor
    {
        SerializedProperty superHeroHeadsProperty;
        SerializedProperty superHeroBodiesProperty;
        SerializedProperty superHeroLegsProperty;

        private void OnEnable()
        {
            superHeroHeadsProperty = serializedObject.FindProperty("superHeroHeads");
            superHeroBodiesProperty = serializedObject.FindProperty("superHeroBodies");
            superHeroLegsProperty = serializedObject.FindProperty("superHeroLegs");
        }

        private void OnDisable()
        {

        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorList.Show(superHeroHeadsProperty, EditorListOption.Buttons | EditorListOption.ListLabel);
            EditorList.Show(superHeroBodiesProperty, EditorListOption.Buttons | EditorListOption.ListLabel);
            EditorList.Show(superHeroLegsProperty, EditorListOption.Buttons | EditorListOption.ListLabel);

            serializedObject.ApplyModifiedProperties();
        }

        void MoveArrayElement(int from, int to)
        {

            superHeroHeadsProperty.MoveArrayElement(from, to);
            superHeroBodiesProperty.MoveArrayElement(from, to);
            superHeroLegsProperty.MoveArrayElement(from, to);
        }

        void InsertArrayElementAtIndex(int index)
        {
            Debug.Log("Add");
            superHeroHeadsProperty.InsertArrayElementAtIndex(index);
            superHeroBodiesProperty.InsertArrayElementAtIndex(index);
            superHeroLegsProperty.InsertArrayElementAtIndex(index);
        }

        void DeleteArrayElementAtIndex(int index)
        {
            int oldSizeHeads = superHeroHeadsProperty.arraySize;
            int oldSizeBodies = superHeroBodiesProperty.arraySize;
            int oldSizeLegs = superHeroLegsProperty.arraySize;

            superHeroHeadsProperty.DeleteArrayElementAtIndex(index);
            superHeroBodiesProperty.DeleteArrayElementAtIndex(index);
            superHeroLegsProperty.DeleteArrayElementAtIndex(index);

            if (superHeroHeadsProperty.arraySize == oldSizeHeads)
            {
                superHeroHeadsProperty.DeleteArrayElementAtIndex(index);
            }

            if (superHeroBodiesProperty.arraySize == oldSizeBodies)
            {
                superHeroBodiesProperty.DeleteArrayElementAtIndex(index);
            }

            if (superHeroLegsProperty.arraySize == oldSizeLegs)
            {
                superHeroLegsProperty.DeleteArrayElementAtIndex(index);
            }
        }
    }
}