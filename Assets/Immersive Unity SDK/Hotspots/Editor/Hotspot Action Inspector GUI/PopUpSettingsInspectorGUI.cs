using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Com.Immersive.Hotspots
{
    public abstract class PopUpSettingsInspectorGUI<T> : HotspotActionInspectorGUI where T : PopUpSettings
    {
        protected T popUpSettings;

        protected SerializedProperty controlPanelWidth;
        protected SerializedProperty popUpPosition;
        protected SerializedProperty popUpPositionOffset;
        protected SerializedProperty controlPanelSide;
        protected SerializedProperty overrideDefaultCloseButton;
        protected SerializedProperty closeButton;
        protected SerializedProperty addGlowToButtons;
        protected SerializedProperty glowColor;

        //Size Options
        protected SerializedProperty sizeOption;
        protected SerializedProperty size;
        protected SerializedProperty percentage;
        protected SerializedProperty padding;

        public PopUpSettingsInspectorGUI(SerializedProperty popupSettingsSerializedProp, T popUpSettings)
        {
            this.popUpSettings = popUpSettings;

            controlPanelWidth = popupSettingsSerializedProp.FindPropertyRelative(nameof(controlPanelWidth));
            popUpPosition = popupSettingsSerializedProp.FindPropertyRelative(nameof(popUpPosition));
            popUpPositionOffset = popupSettingsSerializedProp.FindPropertyRelative(nameof(popUpPositionOffset));
            controlPanelSide = popupSettingsSerializedProp.FindPropertyRelative(nameof(controlPanelSide));
            overrideDefaultCloseButton = popupSettingsSerializedProp.FindPropertyRelative(nameof(overrideDefaultCloseButton));
            closeButton = popupSettingsSerializedProp.FindPropertyRelative(nameof(closeButton));
            addGlowToButtons = popupSettingsSerializedProp.FindPropertyRelative(nameof(addGlowToButtons));
            glowColor = popupSettingsSerializedProp.FindPropertyRelative(nameof(glowColor));


            sizeOption = popupSettingsSerializedProp.FindPropertyRelative(nameof(sizeOption));
            size = popupSettingsSerializedProp.FindPropertyRelative(nameof(size));
            percentage = popupSettingsSerializedProp.FindPropertyRelative(nameof(percentage));
            padding = popupSettingsSerializedProp.FindPropertyRelative(nameof(padding));
        }

        public override void OnInspectorGUI()
        {
            // Pop Up Position
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(popUpPosition, new GUIContent("Position", "Where should the pop-up appear on screen."));

            if (popUpSettings.popUpPosition == PopUpPosition.Custom)
            {
                EditorGUILayout.PropertyField(popUpPositionOffset, new GUIContent("Offset", "The offset in pixels from the hotspot position."));
            }
            EditorGUILayout.Space();

            DrawInABox(DrawControlPanelSettings, "Control Panel");

            EditorGUILayout.Space();

            DrawInABox(DrawMainContentSettings, "Main Content");

            EditorGUILayout.Space();

            DrawInABox(DrawSettings, "Settings");

            EditorGUILayout.Space();

            DrawExtras();
        }

        protected virtual void DrawAdditionalControlPanelSettings() { }
        protected virtual void DrawMainContentSettings() { }
        protected virtual void DrawSettings() { }

        // Any Editor GUI drawn here will appear at the bottom.
        protected virtual void DrawExtras() { }


        protected void DrawControlPanelSettings()
        {
            EditorGUILayout.PropertyField(controlPanelWidth, new GUIContent("Control Panel Width", "Width of Close Button"));
            EditorGUILayout.PropertyField(controlPanelSide, new GUIContent("Side"));

            EditorGUILayout.PropertyField(overrideDefaultCloseButton, new GUIContent("Override Default Close Button"), true);
            if (popUpSettings.overrideDefaultCloseButton)
                DrawWithIndent(closeButton, new GUIContent("Close Button"), true);

            //Glow
            EditorGUILayout.PropertyField(addGlowToButtons, new GUIContent("Glow Around Buttons"), true);
            if (popUpSettings.addGlowToButtons)
                DrawWithIndent(glowColor, new GUIContent("Glow Colour"), false);

            DrawAdditionalControlPanelSettings();
        }


        protected void DrawDefaultSizeSettings(bool includePadding = true)
        {
            EditorGUILayout.BeginVertical("box");

            //Size
            EditorGUILayout.LabelField("Size");
            EditorGUI.indentLevel++;

            EditorGUILayout.PropertyField(sizeOption, new GUIContent("Popup Mode"));

            if (popUpSettings.sizeOption == SizeOption.FixedPopupSize)
            {
                EditorGUILayout.PropertyField(size, new GUIContent("Popup Size"));
            }
            else if (popUpSettings.sizeOption == SizeOption.FixedPercentage)
            {
                EditorGUILayout.PropertyField(percentage, new GUIContent("Percentage"));
            }

            //Padding
            if (includePadding)
                EditorGUILayout.PropertyField(padding, new GUIContent("Padding"), true);

            EditorGUI.indentLevel--;

            EditorGUILayout.EndVertical();
        }

        protected void DrawPropertyInBox(SerializedProperty property, GUIContent content, bool includeChildren)
        {
            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.PropertyField(property, content, includeChildren);
            EditorGUILayout.EndVertical();
        }

        protected void DrawInABox(Action drawAction, string header)
        {
            EditorGUILayout.BeginVertical("box");
            int indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;
            
            EditorGUILayout.LabelField(header, EditorStyles.boldLabel);
            drawAction?.Invoke();

            EditorGUI.indentLevel = indent;
            EditorGUILayout.EndVertical();
        }

        protected void DrawWithIndent(SerializedProperty property, GUIContent content, bool includeChildren)
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(property, content, includeChildren);
            EditorGUI.indentLevel--;
        }
    }
}