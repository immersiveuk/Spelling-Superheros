using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static Com.Immersive.Hotspots.PopUpSequenceSettings ;

namespace Com.Immersive.Hotspots
{
    public abstract class PopUpSequenceSettingsInspectorGUI<T> : PopUpSettingsInspectorGUI<T> where T : PopUpSequenceSettings 
    {
        private SerializedProperty indexChangedAudioClip;
        private SerializedProperty useCustomButtons;
        private SerializedProperty nextButton;
        private SerializedProperty previousButton;

        private SerializedProperty controlPanelStyle;

        protected abstract SerializedProperty SequenceProperty { get; }

        private int index = 0;
        private bool IsPreviousButtonEnabled => index > 0;
        private bool IsNextButtonEnabled => index < SequenceProperty.arraySize - 1;
        protected abstract void DrawSequenceElement(int index);

        public PopUpSequenceSettingsInspectorGUI(SerializedProperty popupSettingsSerializedProp, T popUpSettings) : base(popupSettingsSerializedProp, popUpSettings)
        {
            this.popUpSettings = popUpSettings;

            indexChangedAudioClip = popupSettingsSerializedProp.FindPropertyRelative(nameof(indexChangedAudioClip));
            useCustomButtons = popupSettingsSerializedProp.FindPropertyRelative(nameof(useCustomButtons));
            controlPanelStyle = popupSettingsSerializedProp.FindPropertyRelative(nameof(controlPanelStyle));
            nextButton = popupSettingsSerializedProp.FindPropertyRelative(nameof(nextButton));
            previousButton = popupSettingsSerializedProp.FindPropertyRelative(nameof(previousButton));
        }

        protected override void DrawMainContentSettings()
        {
            EnsureThereIsAlwaysOneElement();

            DrawPageButtons();
            EditorGUILayout.BeginVertical("box");
            DrawSequenceElement(index);
            EditorGUILayout.EndVertical();
            DrawDeleteButton();
        }

        protected override void DrawSettings()
        {
            DrawIndexChangedAudioClip();
        }

        protected override void DrawAdditionalControlPanelSettings()
        {
            EditorGUILayout.PropertyField(controlPanelStyle, new GUIContent("Control Panel Style"));

            EditorGUILayout.PropertyField(useCustomButtons, new GUIContent("Use Custom Buttons"));
            if (popUpSettings.useCustomButtons)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(nextButton, new GUIContent("Next Button Sprite"), true);

                if (popUpSettings.controlPanelStyle == ControlPanelStyle.Full)
                    EditorGUILayout.PropertyField(previousButton, new GUIContent("Prev Button Sprite"), true);
                EditorGUI.indentLevel--;
            }
        }

        private void DrawDeleteButton()
        {
            if (GUILayout.Button("Delete"))
            {
                if (EditorUtility.DisplayDialog("Delete Element", "Are you sure you want to delete this element?", "Delete", "Cancel"))
                {
                    SequenceProperty.DeleteArrayElementAtIndex(index);
                    if (index >= SequenceProperty.arraySize)
                        index = SequenceProperty.arraySize - 1;
                }
            }
        }

        protected void DrawCustomButtonControls()
        {
            EditorGUILayout.PropertyField(useCustomButtons, new GUIContent("Use Custom Buttons"));
            if (popUpSettings.useCustomButtons)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(nextButton, new GUIContent("Next Button Sprite"), true);

                if (popUpSettings.controlPanelStyle == ControlPanelStyle.Full)
                    EditorGUILayout.PropertyField(previousButton, new GUIContent("Prev Button Sprite"), true);
                EditorGUI.indentLevel--;
            }
        }

        protected void DrawIndexChangedAudioClip()
        {
            EditorGUILayout.LabelField("Index Changed Audio Settings");
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(indexChangedAudioClip.FindPropertyRelative("audioClip"), new GUIContent("AudioClip"));
            EditorGUILayout.PropertyField(indexChangedAudioClip.FindPropertyRelative("audioVolume"), new GUIContent("Volume"));
            EditorGUI.indentLevel--;
        }

        private void EnsureThereIsAlwaysOneElement()
        {
            if (SequenceProperty.arraySize == 0)
                AddElement();
        }

        protected void DrawPageButtons()
        {
            EditorGUILayout.BeginHorizontal();

            DrawChangeIndexButton("Previous", IsPreviousButtonEnabled, () => index--);
            DrawChangeIndexButton("Next", IsNextButtonEnabled, () => index++);

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.LabelField($"{(index + 1)}/{SequenceProperty.arraySize}", EditorStyles.boldLabel);
            if (GUILayout.Button("+", GUILayout.Width(30)))
                AddElement();

            EditorGUILayout.EndHorizontal();
        }

        private void DrawChangeIndexButton(string title, bool isEnabled, Action changeIndexAction)
        {
            EditorGUI.BeginDisabledGroup(!isEnabled);
            if (GUILayout.Button(title))
            {
                changeIndexAction?.Invoke();
                GUI.FocusControl(null);
            }
            EditorGUI.EndDisabledGroup();
        }

        protected virtual void AddElement()
        {
            int newIndex = SequenceProperty.arraySize;
            SequenceProperty.InsertArrayElementAtIndex(newIndex);
            index = newIndex;
        }
    }
}