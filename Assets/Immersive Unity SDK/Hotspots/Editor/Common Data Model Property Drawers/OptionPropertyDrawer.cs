using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Com.Immersive.Hotspots
{
    /// <summary>
    /// Property drawer for Options
    /// </summary>
    [CustomPropertyDrawer(typeof(OptionsProperty))]
    public class OptionsPropertyDrawer : PropertyDrawer
    {
        private float verticalSpace = 0;
        int choiceIndex;

        private GUIContent fontSizeContent = new GUIContent("Font Size: ");
        private GUIContent fontContent = new GUIContent("Font: ");
        private GUIContent colourContent = new GUIContent("Colour: ");

        private const int minNumberOfOptions = 2;
        private const int maxNumberOfOptions = 6;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            verticalSpace = (property.FindPropertyRelative("options").arraySize + 2) * (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing);
            return 2 * (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing) + verticalSpace + 2 * EditorGUIUtility.standardVerticalSpacing; ;
        }

        private void DrawOptionsSizeProperty(Rect rect, SerializedProperty property)
        {
            int newSize = EditorGUI.IntField(rect, new GUIContent("Size"), property.intValue);
            if (newSize != property.intValue)
            {
                if (newSize < minNumberOfOptions)
                    newSize = minNumberOfOptions;
                if (newSize > maxNumberOfOptions)
                    newSize = maxNumberOfOptions;
                property.intValue = newSize;
            }
        }

        private Rect GetFirstLineRect(Rect position)
        {
            var lineRect = position;
            lineRect.height = EditorGUIUtility.singleLineHeight;
            return lineRect;
        }

        private Rect GetNextLineRect(Rect currentLineRect) => new Rect(new Vector2(currentLineRect.position.x, currentLineRect.position.y + EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing), currentLineRect.size);

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            Rect lineRect = GetFirstLineRect(position);

            //Draw Label
            EditorGUI.LabelField(lineRect, label);


            //Draw Answer Array
            SerializedProperty optionsArray = property.FindPropertyRelative("options");
            lineRect = GetNextLineRect(lineRect);
            EditorGUI.indentLevel++;

            DrawOptionsSizeProperty(lineRect, optionsArray.FindPropertyRelative("Array.size"));
            
            for (int i = 0; i < optionsArray.arraySize; i++)
            {
                lineRect = GetNextLineRect(lineRect);
                EditorGUI.PropertyField(lineRect, optionsArray.GetArrayElementAtIndex(i), new GUIContent($"Answer {i+1}"));
            }


            //Make List of Answers
            List<string> optionsList = new List<string>();
            for (int i = 0; i < property.FindPropertyRelative("options").arraySize; i++)
                optionsList.Add(property.FindPropertyRelative("options").GetArrayElementAtIndex(i).stringValue);


            if (optionsList.Count > 0)
            {
                var correctAnswerRect = new Rect(position.x, position.y + verticalSpace + EditorGUIUtility.standardVerticalSpacing * 2, position.width - EditorGUIUtility.labelWidth, EditorGUIUtility.singleLineHeight);

                choiceIndex = optionsList.IndexOf(property.FindPropertyRelative("correctAnswer").stringValue);
                choiceIndex = EditorGUI.Popup(correctAnswerRect, "Correct Answer", choiceIndex, optionsList.ToArray());

                if (choiceIndex != -1)
                    property.FindPropertyRelative("correctAnswer").stringValue = optionsList[choiceIndex];
            }

            position.y += verticalSpace + EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing * 3;

            Rect line2Rect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);

            PropertyDrawerHelper.DisplayPropertyWithLabel(PropertyDrawerHelper.GetRectSplit(line2Rect, 3, 0), property.FindPropertyRelative("size"), fontSizeContent, 80);
            PropertyDrawerHelper.DisplayPropertyWithLabel(PropertyDrawerHelper.GetRectSplit(line2Rect, 3, 1), property.FindPropertyRelative("font"), fontContent, 50);
            PropertyDrawerHelper.DisplayPropertyWithLabel(PropertyDrawerHelper.GetRectSplit(line2Rect, 3, 2), property.FindPropertyRelative("color"), colourContent, 50);

            EditorGUI.indentLevel--;
            EditorGUI.EndProperty();
        }
    } 
}