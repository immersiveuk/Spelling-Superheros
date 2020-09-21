using System.Collections.Generic;
using Newtonsoft.Json;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using static CustomJsonConverter;

namespace Com.Immersive.Hotspots
{

    [CustomPropertyDrawer(typeof(ImageProperty))]
    public class ImagePropertyDrawer : PropertyDrawer
    {
        private float verticalSpace = 0;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            verticalSpace = EditorGUI.GetPropertyHeight(property.FindPropertyRelative("sprite")) + EditorGUIUtility.singleLineHeight;
            return verticalSpace +  EditorGUIUtility.standardVerticalSpacing*2;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            verticalSpace = EditorGUI.GetPropertyHeight(property.FindPropertyRelative("sprite"));

            var labelRect = new Rect(position.x, position.y, EditorGUIUtility.labelWidth, EditorGUIUtility.singleLineHeight);
            EditorGUI.LabelField(labelRect, property.displayName);

            var imageRect = new Rect(EditorGUIUtility.labelWidth, position.y, position.width - EditorGUIUtility.labelWidth, EditorGUIUtility.singleLineHeight);
            EditorGUI.PropertyField(imageRect, property.FindPropertyRelative("sprite"), GUIContent.none);

            var typeRect = new Rect(EditorGUIUtility.labelWidth, imageRect.y + verticalSpace + EditorGUIUtility.standardVerticalSpacing, 60, EditorGUIUtility.singleLineHeight);
            float temp = position.width / 2;

            EditorGUI.LabelField(typeRect, "Type");
            typeRect.x += typeRect.width;
            typeRect.width = temp / 2;
            EditorGUI.PropertyField(typeRect, property.FindPropertyRelative("type"), GUIContent.none);

            typeRect.x = position.width - temp / 3; // sizeRect.width;
            typeRect.width = temp / 3;
            EditorGUI.PropertyField(typeRect, property.FindPropertyRelative("color"), GUIContent.none);
        }
    }

    //In Unity Editor only possible to use Video Clips. URLs are for runtime loading.
    [CustomPropertyDrawer(typeof(VideoProperty))]
    public class VideoPropertyDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var labelRect = new Rect(position.x, position.y, EditorGUIUtility.labelWidth, EditorGUIUtility.singleLineHeight);
            EditorGUI.LabelField(labelRect, property.displayName);
            
            var videoRect = new Rect(EditorGUIUtility.labelWidth, position.y, position.width - EditorGUIUtility.labelWidth, EditorGUIUtility.singleLineHeight);

            EditorGUI.PropertyField(videoRect, property.FindPropertyRelative("videoClip"), GUIContent.none);
        }
    }

    /// <summary>
    /// Property drawer for Text
    /// </summary>
    [CustomPropertyDrawer(typeof(TextProperty))]
    public class TextPropertyDrawer : PropertyDrawer
    {
        private float verticalSpace = 0;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            verticalSpace = EditorGUI.GetPropertyHeight(property.FindPropertyRelative("text")) + EditorGUIUtility.singleLineHeight;
            return 2 * EditorGUIUtility.singleLineHeight + verticalSpace;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            verticalSpace = EditorGUI.GetPropertyHeight(property.FindPropertyRelative("text"));

            var labelRect = new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight, EditorGUIUtility.labelWidth, EditorGUIUtility.singleLineHeight);
            EditorGUI.LabelField(labelRect, property.displayName);

            var textRect = new Rect(EditorGUIUtility.labelWidth, position.y, position.width - EditorGUIUtility.labelWidth, EditorGUI.GetPropertyHeight(property.FindPropertyRelative("text")));
            EditorGUI.PropertyField(textRect, property.FindPropertyRelative("text"), GUIContent.none, true);

            float contentWidth = position.width / 3;
            var sizeRect = new Rect(EditorGUIUtility.labelWidth, position.y + verticalSpace + 5, 90, EditorGUIUtility.singleLineHeight);

            EditorGUI.LabelField(sizeRect, "Font Size");
            sizeRect.x += 60; sizeRect.width = contentWidth / 3;
            EditorGUI.PropertyField(sizeRect, property.FindPropertyRelative("size"), GUIContent.none);

            sizeRect.x += sizeRect.width;
            EditorGUI.LabelField(sizeRect, "Font");
            sizeRect.x += 30; sizeRect.width = contentWidth / 2;
            EditorGUI.PropertyField(sizeRect, property.FindPropertyRelative("font"), GUIContent.none);

            sizeRect.x = position.width - contentWidth / 2;
            sizeRect.width = contentWidth / 2;
            EditorGUI.PropertyField(sizeRect, property.FindPropertyRelative("color"), GUIContent.none);
        }
    }

    /// <summary>
    /// Property drawer for Options
    /// </summary>
    [CustomPropertyDrawer(typeof(OptionsProperty))]
    public class OptionsPropertyDrawer : PropertyDrawer
    {
        private float verticalSpace = 0;
        int choiceIndex;
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            verticalSpace = EditorGUI.GetPropertyHeight(property.FindPropertyRelative("options"))/* + EditorGUIUtility.singleLineHeight*/;
            return 3 * EditorGUIUtility.singleLineHeight + verticalSpace;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {

            var optionRect = new Rect(position.x, position.y /*+ EditorGUIUtility.singleLineHeight*/, position.width, EditorGUI.GetPropertyHeight(property.FindPropertyRelative("options")));
            property.FindPropertyRelative("options").isExpanded = true;
            EditorGUI.PropertyField(optionRect, property.FindPropertyRelative("options"), new GUIContent(property.displayName), true);

            EditorGUI.indentLevel++;

            List<string> optionsList = new List<string>();
            for (int i = 0; i < property.FindPropertyRelative("options").arraySize; i++)
            {
                optionsList.Add(property.FindPropertyRelative("options").GetArrayElementAtIndex(i).stringValue);
            }

            if (optionsList.Count > 0)
            {
                var correctAnswerRect = new Rect(position.x, position.y + verticalSpace + EditorGUIUtility.standardVerticalSpacing, position.width - EditorGUIUtility.labelWidth, EditorGUIUtility.singleLineHeight);

                choiceIndex = optionsList.IndexOf(property.FindPropertyRelative("correctAnswer").stringValue);
                choiceIndex = EditorGUI.Popup(correctAnswerRect, "Correct Answer", choiceIndex, optionsList.ToArray());

                property.FindPropertyRelative("correctAnswer").stringValue = optionsList[choiceIndex];
            }


            var labelRect = new Rect(position.x, position.y + verticalSpace + 2*EditorGUIUtility.standardVerticalSpacing + EditorGUIUtility.singleLineHeight, EditorGUIUtility.labelWidth, EditorGUIUtility.singleLineHeight);
            EditorGUI.LabelField(labelRect, "Text");

            float contentWidth = position.width / 3;

            EditorGUI.indentLevel--;

            //FONT
            var sizeRect = new Rect(EditorGUIUtility.labelWidth, labelRect.y, 90, EditorGUIUtility.singleLineHeight);

            EditorGUI.LabelField(sizeRect, "Font Size");
            sizeRect.x += 60; sizeRect.width = contentWidth / 3;
            EditorGUI.PropertyField(sizeRect, property.FindPropertyRelative("size"), GUIContent.none);

            sizeRect.x += sizeRect.width;
            EditorGUI.LabelField(sizeRect, "Font");
            sizeRect.x += 30; sizeRect.width = contentWidth / 2;
            EditorGUI.PropertyField(sizeRect, property.FindPropertyRelative("font"), GUIContent.none);

            sizeRect.x = position.width - contentWidth / 2;
            sizeRect.width = contentWidth / 2;
            EditorGUI.PropertyField(sizeRect, property.FindPropertyRelative("color"), GUIContent.none);


        }
    }
}