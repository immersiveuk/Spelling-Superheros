using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Com.Immersive
{
    public static class PropertyDrawerHelper
    {
        /// <summary>
        /// Splits a Rect into the a number of sections and returns an new rect which represents the section at the provided index.
        /// </summary>
        public static Rect GetRectSplit(Rect initialRect, int numberOfSection, int sectionIndex)
        {
            float subrectWidth = initialRect.width / numberOfSection;
            return new Rect(initialRect.x + subrectWidth * sectionIndex, initialRect.y, subrectWidth, initialRect.height);
        }
        
        /// <summary>
        /// Draws a Property with a label in the provided rect.
        /// </summary>
        public static void DisplayPropertyWithLabel(Rect position, SerializedProperty property, GUIContent label, int labelWidth, int spacing = 5)
        {
            EditorGUI.LabelField(position, label);
            position.x += labelWidth;
            position.width -= labelWidth + spacing;
            EditorGUI.PropertyField(position, property, GUIContent.none);
        }
    }
}