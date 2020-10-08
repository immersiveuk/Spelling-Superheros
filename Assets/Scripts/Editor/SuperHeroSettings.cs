using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Immersive.SuperHero
{
    [CreateAssetMenu(fileName = "New Super Hero Settings", menuName = "Super Hero/ Settings", order = 1)]
    public class SuperHeroSettings : ScriptableObject
    {
        public List<SuperHeroParts> superHeroHeads;
        public List<SuperHeroParts> superHeroBodies;
        public List<SuperHeroParts> superHeroLegs;
    }

    [CustomEditor(typeof(SuperHeroSettings)), CanEditMultipleObjects]
    public class SuperHeroSettingsEditor : Editor
    {
        SerializedProperty superHeroHeadsProperty;
        SerializedProperty superHeroBodiesProperty;
        SerializedProperty superHeroLegsProperty;

        private void OnEnable()
        {
            Debug.Log("OnEnable");
            superHeroHeadsProperty = serializedObject.FindProperty("superHeroHeads");
            superHeroBodiesProperty = serializedObject.FindProperty("superHeroBodies");
            superHeroLegsProperty = serializedObject.FindProperty("superHeroLegs");

            EditorList.OnMoveArrayElement += MoveArrayElement;
            EditorList.OnInsertArrayElement += InsertArrayElementAtIndex;
            EditorList.OnDeleteArrayElement += DeleteArrayElementAtIndex;
        }

        private void OnDisable()
        {
            Debug.Log("OnDisable");
            EditorList.OnMoveArrayElement -= MoveArrayElement;
            EditorList.OnInsertArrayElement -= InsertArrayElementAtIndex;
            EditorList.OnDeleteArrayElement -= DeleteArrayElementAtIndex;
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
