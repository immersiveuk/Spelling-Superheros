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
        int startChoinceIndex, endChoinceIndex;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            verticalSpace = EditorGUI.GetPropertyHeight(property.FindPropertyRelative("spelling")) + EditorGUIUtility.singleLineHeight;
            return 2 * EditorGUIUtility.singleLineHeight + verticalSpace;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            verticalSpace = EditorGUI.GetPropertyHeight(property.FindPropertyRelative("spelling"));

            //var labelRect = new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight, EditorGUIUtility.labelWidth, EditorGUIUtility.singleLineHeight);
            //EditorGUI.LabelField(labelRect, property.displayName);
            var textRect = new Rect(position.x + EditorGUIUtility.labelWidth, position.y, position.width - EditorGUIUtility.labelWidth, EditorGUI.GetPropertyHeight(property.FindPropertyRelative("spelling")));
            EditorGUI.PropertyField(textRect, property.FindPropertyRelative("spelling"), GUIContent.none, true);

            float contentWidth = position.width / 2;
            var sizeRect = new Rect(position.x + EditorGUIUtility.labelWidth, position.y + verticalSpace + 5, 90, EditorGUIUtility.singleLineHeight);
            EditorGUI.LabelField(sizeRect, "Start Index");

            sizeRect.x += 80; sizeRect.width = contentWidth / 4;
            string spelling = property.FindPropertyRelative("spelling").stringValue;
            DrawDropDown(property, "startIndex", spelling, sizeRect, 0, ref startChoinceIndex);

            sizeRect.x += sizeRect.width; sizeRect.width = EditorGUIUtility.labelWidth;
            EditorGUI.LabelField(sizeRect, "End Index");

            sizeRect.x += 80; sizeRect.width = contentWidth / 4;
            DrawDropDown(property, "endIndex", spelling, sizeRect, startChoinceIndex, ref endChoinceIndex);

            string preview = spelling;

            int startIndex = property.FindPropertyRelative("startIndex").intValue;
            int endIndex = property.FindPropertyRelative("endIndex").intValue;

            for (int i = startIndex; i <= endIndex; i++)
            {
                preview = preview.Remove(i, 1);
                preview = preview.Insert(i, "_");
            }

            var previewSizeRect = new Rect(position.x + EditorGUIUtility.labelWidth,sizeRect.y + verticalSpace + 5, position.width - EditorGUIUtility.labelWidth, EditorGUIUtility.singleLineHeight);
            EditorGUI.LabelField(previewSizeRect, "Preview: "+preview);

            EditorGUI.indentLevel = indent;
        }

        void DrawDropDown(SerializedProperty property, string propertyKey, string stringValue, Rect position, int startIndex, ref int ChoinceIndex)
        {
            if (stringValue.Length > 0 && startIndex < stringValue.Length)
            {
                List<string> options = new List<string>();

                for (int i = startIndex; i < stringValue.Length; i++)
                {
                    options.Add("" + i);
                }

                ChoinceIndex = options.IndexOf(""+property.FindPropertyRelative(propertyKey).intValue);

                if (ChoinceIndex < 0)
                    ChoinceIndex = 0;

                ChoinceIndex = EditorGUI.Popup(position, ChoinceIndex, options.ToArray());

                property.FindPropertyRelative(propertyKey).intValue = int.Parse(options[ChoinceIndex]);
            }
            else
            {
                startChoinceIndex = 0;
                endChoinceIndex = 0;
            }
        }
    }
}