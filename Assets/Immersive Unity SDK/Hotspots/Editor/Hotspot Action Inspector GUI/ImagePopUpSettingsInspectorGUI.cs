using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Com.Immersive.Hotspots
{
    public class ImagePopupSettingsInspectorGUI : PopUpSettingsInspectorGUI<ImagePopUpDataModel.ImagePopUpSetting>
    {
        private SerializedProperty maintainAspectRatio;
        private SerializedProperty border;
        private SerializedProperty background;
        private SerializedProperty mediaMask;

        //Todo use Generics
        public ImagePopupSettingsInspectorGUI(SerializedProperty popupSettingsSerializedProp, ImagePopUpDataModel.ImagePopUpSetting imagePopUpSetting) : base(popupSettingsSerializedProp, imagePopUpSetting)
        {
            maintainAspectRatio = popupSettingsSerializedProp.FindPropertyRelative(nameof(maintainAspectRatio));
            border = popupSettingsSerializedProp.FindPropertyRelative(nameof(border));
            background = popupSettingsSerializedProp.FindPropertyRelative(nameof(background));
            mediaMask = popupSettingsSerializedProp.FindPropertyRelative(nameof(mediaMask));
        }

        protected override void DrawMainContentSettings()
        {
            EditorGUILayout.PropertyField(background, new GUIContent("Sprite"), true);
        }

        protected override void DrawSettings()
        {
            //Border
            EditorGUILayout.PropertyField(border, new GUIContent("Border"), true);
            EditorGUILayout.Space();

            //Mask
            EditorGUILayout.PropertyField(mediaMask, new GUIContent("Media Mask"), true);
            EditorGUILayout.Space();
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            ////Size
            EditorGUILayout.BeginVertical("box");

            EditorGUILayout.LabelField("Size", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;

            EditorGUILayout.PropertyField(sizeOption, new GUIContent("Popup Mode"));

            if (popUpSettings.background.sprite == null)
                EditorGUILayout.HelpBox("A Sprite Is Required To Set The Size", MessageType.Error);
            else
            {
                if (popUpSettings.sizeOption == SizeOption.FixedPopupSize)
                {
                    EditorGUILayout.BeginHorizontal();

                    DrawSize();

                    //Reset Resolution
                    if (GUILayout.Button("Reset Resolution", GUILayout.Width(150)))
                    {
                        size.vector2Value = popUpSettings.background.sprite.rect.size;
                    }

                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.PropertyField(maintainAspectRatio, new GUIContent("Maintain Aspect Ratio"));
                }
                else if (popUpSettings.sizeOption == SizeOption.FixedPercentage)
                {
                    EditorGUILayout.PropertyField(percentage, new GUIContent("Percentage"));
                }

                EditorGUILayout.PropertyField(padding, new GUIContent("Padding"), true);


            }
            EditorGUI.indentLevel--;
            EditorGUILayout.EndVertical();
        }

        private void DrawSize()
        {
            EditorGUI.BeginChangeCheck();
            Vector2 sizeBefore = size.vector2Value;
            EditorGUILayout.PropertyField(size, new GUIContent("Size"));
            bool changed = EditorGUI.EndChangeCheck();

            if (changed)
            {
                if (maintainAspectRatio.boolValue)
                {
                    Vector2 sizeAfter = size.vector2Value;
                    if (sizeBefore.x != sizeAfter.x)
                        sizeAfter.y = sizeBefore.y * (sizeAfter.x / sizeBefore.x);
                    else if (sizeBefore.y != sizeAfter.y)
                        sizeAfter.x = sizeBefore.x * (sizeAfter.y / sizeBefore.y);
                    size.vector2Value = sizeAfter;
                }
            }
        }
    } 
}