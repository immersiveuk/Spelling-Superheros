using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Immersive.FillInTheBlank
{
    [CustomPropertyDrawer(typeof(FillInTheBlanksModel))]
    public class FillInTheBlanksPropertyDrawer : PropertyDrawer
    {
        private float verticalSpace = 0;
        //Vector2Int[] missingLettersPosition = new Vector2Int[0];

        SerializedProperty missingLettersPairs, missingLettersPosition;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            verticalSpace = EditorGUIUtility.singleLineHeight * property.FindPropertyRelative("missingLettersPosition").arraySize;// EditorGUI.GetPropertyHeight(property.FindPropertyRelative("indexs"));
            return 5* EditorGUIUtility.singleLineHeight + verticalSpace;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            missingLettersPairs = property.FindPropertyRelative("missingLettersPairs");
            missingLettersPosition = property.FindPropertyRelative("missingLettersPosition"); ;

            var indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            string spelling = property.FindPropertyRelative("spelling").stringValue;
            string preview = spelling;

            var spellingRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
            EditorGUI.PropertyField(spellingRect, property.FindPropertyRelative("spelling"), GUIContent.none);

            var missingPairsSizeRect = new Rect(position.x, spellingRect.y + EditorGUIUtility.singleLineHeight + 5, position.width, EditorGUIUtility.singleLineHeight);
            EditorGUI.PropertyField(missingPairsSizeRect, missingLettersPairs, new GUIContent("Missing Letters Pairs:"));

            if (missingLettersPairs.intValue > spelling.Length /2)
                missingLettersPairs.intValue = spelling.Length /2;

            for (int i = 0; i < missingLettersPosition.arraySize; i++)
            {
                //Debug.Log(missingLettersPosition.Length);
                if (missingLettersPosition.GetArrayElementAtIndex(i).vector2IntValue.y + 1 >= spelling.Length)
                {
                    if (missingLettersPairs.intValue > i + 1)
                        missingLettersPairs.intValue = i + 1;
                    break;
                }
            }

            //missingLettersPosition = new Vector2Int[missingLettersPairs.intValue];
            missingLettersPosition.arraySize = missingLettersPairs.intValue;

            float contentWidth = position.width / 2;
            
            for (int i = 0; i < missingLettersPairs.intValue; i++)
            {
                SerializedProperty indexProperty = missingLettersPosition.GetArrayElementAtIndex(i);

                var sizeRect = new Rect(position.x, missingPairsSizeRect.y + EditorGUIUtility.singleLineHeight*(i+1) + 5, 90, EditorGUIUtility.singleLineHeight);
                EditorGUI.LabelField(sizeRect, "Start Index");

                sizeRect.x += 80; sizeRect.width = contentWidth / 4;
                DrawDropDown_X(missingLettersPosition, indexProperty, spelling, sizeRect, i);

                sizeRect.x += sizeRect.width+50; sizeRect.width = EditorGUIUtility.labelWidth;
                EditorGUI.LabelField(sizeRect, "End Index");

                sizeRect.x += 80; sizeRect.width = contentWidth / 4;
                DrawDropDown_Y(indexProperty, spelling, sizeRect, i);
            }


            for (int j = 0; j < missingLettersPosition.arraySize; j++)
            {
                for (int i = missingLettersPosition.GetArrayElementAtIndex(j).vector2IntValue.x; i <= missingLettersPosition.GetArrayElementAtIndex(j).vector2IntValue.y; i++)
                {
                    preview = preview.Remove(i, 1);
                    preview = preview.Insert(i, "_");
                }
            }

            var previewSizeRect = new Rect(position.x, missingPairsSizeRect.y + EditorGUIUtility.singleLineHeight + verticalSpace + 10, position.width - EditorGUIUtility.labelWidth, EditorGUIUtility.singleLineHeight);
            EditorGUI.LabelField(previewSizeRect, "Preview: " + preview);

            EditorGUI.indentLevel = indent;
        }

        void DrawDropDown_X(SerializedProperty parentProperty,SerializedProperty property, string stringValue, Rect position, int index)
        {
            if (stringValue.Length > 0 && property.vector2IntValue.x < stringValue.Length)
            {
                List<string> options = new List<string>();

                int temp = 0;

                if (index > 0)
                {
                    temp = parentProperty.GetArrayElementAtIndex(index - 1).vector2IntValue.y + 1;// missingLettersPosition[index - 1].y + 1;
                }

                for (int i = 0 + temp; i < stringValue.Length; i++)
                {
                    options.Add("" + i);
                }

                if (options.Count <= 0)
                    return;

                int choice = options.IndexOf("" + property.vector2IntValue.x);
                if (choice < 0)
                    choice = 0;

                choice = EditorGUI.Popup(position, choice, options.ToArray());

                property.vector2IntValue = new Vector2Int(int.Parse(options[choice]), property.vector2IntValue.y);
                //missingLettersPosition[index] = property.vector2IntValue;
            }
            else
            {
                //missingLettersPosition[index] = Vector2Int.zero;
            }
        }

        void DrawDropDown_Y(SerializedProperty property, string stringValue, Rect position, int index)
        {
            if (stringValue.Length > 0 && property.vector2IntValue.x < stringValue.Length)
            {
                List<string> options = new List<string>();

                for (int i = property.vector2IntValue.x; i < stringValue.Length; i++)
                {
                    options.Add("" + i);
                }

                int choice = options.IndexOf("" + property.vector2IntValue.y);

                if (choice < 0)
                    choice = 0;

                choice = EditorGUI.Popup(position, choice, options.ToArray());

                property.vector2IntValue = new Vector2Int(property.vector2IntValue.x, int.Parse(options[choice]));
                //missingLettersPosition[index] = property.vector2IntValue;
            }
            else
            {
                //missingLettersPosition[index] = Vector2Int.zero;
            }
        }
    }
}