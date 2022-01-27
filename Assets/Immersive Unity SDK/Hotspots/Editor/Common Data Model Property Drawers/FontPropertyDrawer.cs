using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Com.Immersive.Hotspots
{
    /// <summary>
    /// Property drawer for Text
    /// </summary>
    [CustomPropertyDrawer(typeof(FontProperty))]
    public class FontPropertyDrawer : PropertyDrawer
    {
        private float verticalSpace = 0;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return 3 * EditorGUIUtility.singleLineHeight + verticalSpace;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            verticalSpace = EditorGUIUtility.singleLineHeight;

            var labelRect = new Rect(position.x, position.y, EditorGUIUtility.labelWidth, EditorGUIUtility.singleLineHeight);
            EditorGUI.LabelField(labelRect, property.displayName);
            /*
            var textRect = new Rect(EditorGUIUtility.labelWidth, position.y, position.width - EditorGUIUtility.labelWidth, EditorGUI.GetPropertyHeight(property.FindPropertyRelative("text")));
            EditorGUI.PropertyField(textRect, property.FindPropertyRelative("text"), GUIContent.none, true);
            */

            verticalSpace = 0;
            float contentWidth = position.width - EditorGUIUtility.labelWidth;
            var sizeRect = new Rect(EditorGUIUtility.labelWidth, position.y + verticalSpace + 5, contentWidth / 4, EditorGUIUtility.singleLineHeight);

            EditorGUI.LabelField(sizeRect, "Font Size");
            sizeRect.x += sizeRect.width;
            EditorGUI.PropertyField(sizeRect, property.FindPropertyRelative("size"), GUIContent.none);


            sizeRect.x += sizeRect.width;
            EditorGUI.LabelField(sizeRect, "Font");
            sizeRect.x += sizeRect.width - contentWidth * 0.1f;
            sizeRect.width += contentWidth * 0.1f;
            EditorGUI.PropertyField(sizeRect, property.FindPropertyRelative("font"), GUIContent.none);


            sizeRect = new Rect(EditorGUIUtility.labelWidth, position.y + verticalSpace + 10 + EditorGUIUtility.singleLineHeight, contentWidth / 4, EditorGUIUtility.singleLineHeight);

            EditorGUI.LabelField(sizeRect, "Alignment");
            sizeRect.x += sizeRect.width;
            EditorGUI.PropertyField(sizeRect, property.FindPropertyRelative("alignment"), GUIContent.none);


            sizeRect.x = position.width - contentWidth / 2;
            sizeRect.width = contentWidth / 2;
            EditorGUI.PropertyField(sizeRect, property.FindPropertyRelative("color"), GUIContent.none);
        }
    } 
}