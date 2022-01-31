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
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            float verticalSpace = 0;
            SerializedProperty spellingsProperty = property.FindPropertyRelative("spellings");

            for (int k = 0; k < spellingsProperty.arraySize; k++)
            {
                verticalSpace += EditorGUIUtility.singleLineHeight * (5 + spellingsProperty.GetArrayElementAtIndex(k).FindPropertyRelative("missingLettersPairs").arraySize);
            }
            return verticalSpace;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            SerializedProperty spellingsProperty = property.FindPropertyRelative("spellings");

            for (int k = 0; k < spellingsProperty.arraySize; k++)
            {
                SerializedProperty missingLettersPairs = spellingsProperty.GetArrayElementAtIndex(k).FindPropertyRelative("missingLettersPairs");

                string spelling = spellingsProperty.GetArrayElementAtIndex(k).FindPropertyRelative("spelling").stringValue;
                string preview = spelling;

                EnsureMissingLettersPairsLenght(missingLettersPairs, spelling);

                var indent = EditorGUI.indentLevel;
                EditorGUI.indentLevel = 0;

                var spellingRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
                EditorGUI.PropertyField(spellingRect, spellingsProperty.GetArrayElementAtIndex(k).FindPropertyRelative("spelling"), GUIContent.none);

                var buttonsSizeRect = new Rect(position.x, spellingRect.y + EditorGUIUtility.singleLineHeight + 5, position.width, EditorGUIUtility.singleLineHeight);

                DrawButtons(missingLettersPairs, spelling.Length, buttonsSizeRect);
                DrawPairsDropDOwn(missingLettersPairs, spelling, buttonsSizeRect);

                var previewSizeRect = new Rect(position.x, buttonsSizeRect.y + EditorGUIUtility.singleLineHeight + EditorGUIUtility.singleLineHeight * missingLettersPairs.arraySize + 10, position.width - EditorGUIUtility.labelWidth, EditorGUIUtility.singleLineHeight);
                EditorGUI.LabelField(previewSizeRect, "Preview: " + GetPreview(missingLettersPairs, preview));

                position.y = previewSizeRect.y + EditorGUIUtility.singleLineHeight * 2;
                EditorGUI.indentLevel = indent;
            }
        }

        void EnsureMissingLettersPairsLenght(SerializedProperty missingLettersPairs, string spelling)
        {
            if (string.IsNullOrEmpty(spelling) || missingLettersPairs.arraySize > spelling.Length / 2)
                missingLettersPairs.ClearArray();

            if (!string.IsNullOrEmpty(spelling) && missingLettersPairs.arraySize == 0)
            {
                missingLettersPairs.InsertArrayElementAtIndex(0);
            }

            for (int i = 0; i < missingLettersPairs.arraySize; i++)
            {
                if (missingLettersPairs.GetArrayElementAtIndex(i).FindPropertyRelative("endIndex").intValue + 1 >= spelling.Length)
                {
                    if (missingLettersPairs.arraySize > i + 1)
                        DeletePairElement(missingLettersPairs);
                    break;
                }
            }
        }

        void DrawPairsDropDOwn(SerializedProperty missingLettersPairs, string spelling, Rect position)
        {
            for (int i = 0; i < missingLettersPairs.arraySize; i++)
            {
                SerializedProperty indexProperty = missingLettersPairs.GetArrayElementAtIndex(i);

                var sizeRect = new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight * (i + 1) + 5, 90, EditorGUIUtility.singleLineHeight);
                EditorGUI.LabelField(sizeRect, "Start Index");

                sizeRect.x += 80; sizeRect.width = position.width / 4;
                DrawDropDown_X(missingLettersPairs, indexProperty, spelling, sizeRect, i);

                sizeRect.x += sizeRect.width + 50; sizeRect.width = EditorGUIUtility.labelWidth;
                EditorGUI.LabelField(sizeRect, "End Index");

                sizeRect.x += 80; sizeRect.width = position.width / 4;
                DrawDropDown_Y(indexProperty, spelling, sizeRect, i);
            }
        }

        string GetPreview(SerializedProperty missingLettersPairs, string spelling)
        {
            string preview = spelling;

            for (int j = 0; j < missingLettersPairs.arraySize; j++)
            {
                for (int i = missingLettersPairs.GetArrayElementAtIndex(j).FindPropertyRelative("startIndex").intValue; i <= missingLettersPairs.GetArrayElementAtIndex(j).FindPropertyRelative("endIndex").intValue; i++)
                {
                    preview = preview.Remove(i, 1);
                    preview = preview.Insert(i, "_");
                }
            }

            return preview;
        }

        void DrawDropDown_X(SerializedProperty parentProperty, SerializedProperty property, string stringValue, Rect position, int index)
        {
            if (stringValue.Length > 0 && property.FindPropertyRelative("startIndex").intValue < stringValue.Length)
            {
                List<string> options = new List<string>();

                int temp = 0;

                if (index > 0)
                {
                    temp = parentProperty.GetArrayElementAtIndex(index - 1).FindPropertyRelative("endIndex").intValue + 1;
                }

                for (int i = 0 + temp; i < stringValue.Length; i++)
                {
                    options.Add("" + i);
                }

                if (options.Count <= 0)
                    return;

                int choice = options.IndexOf("" + property.FindPropertyRelative("startIndex").intValue);
                if (choice < 0)
                    choice = 0;

                choice = EditorGUI.Popup(position, choice, options.ToArray());

                property.FindPropertyRelative("startIndex").intValue = int.Parse(options[choice]);// new Vector2Int(int.Parse(options[choice]), property.vector2IntValue.y);
            }
        }

        void DrawDropDown_Y(SerializedProperty property, string stringValue, Rect position, int index)
        {
            if (stringValue.Length > 0 && property.FindPropertyRelative("startIndex").intValue < stringValue.Length)
            {
                List<string> options = new List<string>();

                for (int i = property.FindPropertyRelative("startIndex").intValue; i < stringValue.Length; i++)
                {
                    options.Add("" + i);
                }

                int choice = options.IndexOf("" + property.FindPropertyRelative("endIndex").intValue);

                if (choice < 0)
                    choice = 0;

                choice = EditorGUI.Popup(position, choice, options.ToArray());

                property.FindPropertyRelative("endIndex").intValue = int.Parse(options[choice]);
            }
        }

        void DrawButtons(SerializedProperty pairs, int spellingLength, Rect rect)
        {
            rect.x = rect.width - 10;
            rect.width = 30;

            EditorGUI.BeginDisabledGroup(pairs.arraySize <= 1);
            if (GUI.Button(rect, "-"))
            {
                DeletePairElement(pairs);
            }
            rect.x -= 35;
            EditorGUI.EndDisabledGroup();

            //GUI.enabled = pairs.arraySize < spellingLength / 2;
            EditorGUI.BeginDisabledGroup(pairs.arraySize >= spellingLength / 2);
            if (GUI.Button(rect, "+"))
            {
                AddPairElement(pairs);
            }
            EditorGUI.EndDisabledGroup();
        }

        /// <summary>
        /// Add new pair to the list on Plus button click
        /// </summary>
        /// <param name="pairs"></param>
        void AddPairElement(SerializedProperty pairs)
        {
            int newIndex = pairs.arraySize;
            pairs.InsertArrayElementAtIndex(newIndex);
        }


        /// <summary>
        /// Remove last pair on Minus button click 
        /// </summary>
        /// <param name="pairs"></param>
        void DeletePairElement(SerializedProperty pairs)
        {
            if (pairs.arraySize > 1)
            {
                int newIndex = pairs.arraySize - 1;
                pairs.DeleteArrayElementAtIndex(newIndex);
            }
        }
    }
}