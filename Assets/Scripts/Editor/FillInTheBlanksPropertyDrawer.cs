using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Immersive.FillInTheBlank
{
    [CustomPropertyDrawer(typeof(FillInTheBlanksData))]
    public class FillInTheBlanksPropertyDrawer : PropertyDrawer
    {
        private float verticalSpace = 0;
        Vector2Int[] missingIndexs = new Vector2Int[0];

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            verticalSpace = EditorGUIUtility.singleLineHeight * property.FindPropertyRelative("indexs").arraySize;// EditorGUI.GetPropertyHeight(property.FindPropertyRelative("indexs"));
            return 5* EditorGUIUtility.singleLineHeight + verticalSpace;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            string spelling = property.FindPropertyRelative("spelling").stringValue;
            string preview = spelling;

            var spellingRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
            EditorGUI.PropertyField(spellingRect, property.FindPropertyRelative("spelling"), GUIContent.none);

            var missingPairsSizeRect = new Rect(position.x, spellingRect.y + EditorGUIUtility.singleLineHeight + 5, position.width, EditorGUIUtility.singleLineHeight);
            EditorGUI.PropertyField(missingPairsSizeRect, property.FindPropertyRelative("missingPairs"), new GUIContent("Missing Pairs:"));

            if (property.FindPropertyRelative("missingPairs").intValue > spelling.Length /2)
                property.FindPropertyRelative("missingPairs").intValue = spelling.Length /2;

            for (int i = 0; i < missingIndexs.Length; i++)
            {
                if (missingIndexs[i].y + 1 >= spelling.Length)
                {
                    if (property.FindPropertyRelative("missingPairs").intValue > i + 1)
                        property.FindPropertyRelative("missingPairs").intValue = i + 1;
                    break;
                }
            }

            missingIndexs = new Vector2Int[property.FindPropertyRelative("missingPairs").intValue];
            property.FindPropertyRelative("indexs").arraySize = missingIndexs.Length;

            float contentWidth = position.width / 2;
            
            for (int i = 0; i < missingIndexs.Length; i++)
            {
                SerializedProperty indexProperty = property.FindPropertyRelative("indexs").GetArrayElementAtIndex(i);

                var sizeRect = new Rect(position.x, missingPairsSizeRect.y + EditorGUIUtility.singleLineHeight*(i+1) + 5, 90, EditorGUIUtility.singleLineHeight);
                EditorGUI.LabelField(sizeRect, "Start Index");

                sizeRect.x += 80; sizeRect.width = contentWidth / 4;
                DrawDropDown_X(indexProperty, spelling, sizeRect, i);

                sizeRect.x += sizeRect.width; sizeRect.width = EditorGUIUtility.labelWidth;
                EditorGUI.LabelField(sizeRect, "End Index");

                sizeRect.x += 80; sizeRect.width = contentWidth / 4;
                DrawDropDown_Y(indexProperty, spelling, sizeRect, i);
            }


            for (int j = 0; j < missingIndexs.Length; j++)
            {
                for (int i = missingIndexs[j].x; i <= missingIndexs[j].y; i++)
                {
                    preview = preview.Remove(i, 1);
                    preview = preview.Insert(i, "_");
                }
            }

            var previewSizeRect = new Rect(position.x, missingPairsSizeRect.y + EditorGUIUtility.singleLineHeight + verticalSpace + 5, position.width - EditorGUIUtility.labelWidth, EditorGUIUtility.singleLineHeight);
            EditorGUI.LabelField(previewSizeRect, "Preview: " + preview);

            EditorGUI.indentLevel = indent;
        }

        void DrawDropDown_X(SerializedProperty property, string stringValue, Rect position, int index)
        {
            if (stringValue.Length > 0 && missingIndexs[index].x<stringValue.Length)
            {
                List<string> options = new List<string>();

                int temp = 0;

                if (index > 0)
                {
                    temp = missingIndexs[index - 1].y + 1;
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
                missingIndexs[index] = property.vector2IntValue;
            }
            else
            {
                missingIndexs[index] = Vector2Int.zero;
            }
        }

        void DrawDropDown_Y(SerializedProperty property, string stringValue, Rect position, int index)
        {
            if (stringValue.Length > 0 && missingIndexs[index].x < stringValue.Length)
            {
                List<string> options = new List<string>();

                for (int i = missingIndexs[index].x; i < stringValue.Length; i++)
                {
                    options.Add("" + i);
                }

                int choice = options.IndexOf("" + property.vector2IntValue.y);

                if (choice < 0)
                    choice = 0;

                choice = EditorGUI.Popup(position, choice, options.ToArray());

                property.vector2IntValue = new Vector2Int(property.vector2IntValue.x, int.Parse(options[choice]));
                missingIndexs[index] = property.vector2IntValue;
            }
            else
            {
                missingIndexs[index] = Vector2Int.zero;
            }
        }
    }
}