using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TwoPartBackground))]
public class TwoPartBackgroundEditor : Editor
{
    private SerializedProperty leftSprite;
    private SerializedProperty rightSprite;
    
    private SerializedProperty leftSpriteRenderer;
    private SerializedProperty rightSpriteRenderer;

    private SerializedProperty material;
    private SerializedProperty includesBackWall;

    private TwoPartBackground splitBackground;

    private int currentTab = 0;

    private void OnEnable()
    {
        splitBackground = target as TwoPartBackground;

        leftSprite = serializedObject.FindProperty(nameof(leftSprite));
        rightSprite = serializedObject.FindProperty(nameof(rightSprite));
        leftSpriteRenderer = serializedObject.FindProperty(nameof(leftSpriteRenderer));
        rightSpriteRenderer = serializedObject.FindProperty(nameof(rightSpriteRenderer));
        material = serializedObject.FindProperty(nameof(material));
        includesBackWall = serializedObject.FindProperty(nameof(includesBackWall));

        Undo.undoRedoPerformed += splitBackground.UpdateSettings;
    }

    private void OnDisable()
    {
        Undo.undoRedoPerformed -= splitBackground.UpdateSettings;
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        currentTab = GUILayout.Toolbar(currentTab, new string[] { "Settings", "Referenced Objects" });

        switch (currentTab)
        {
            case 0:
                DrawSettings();
                break;
            case 1:
                DrawReferences();
                break;
        }

        bool changed = serializedObject.ApplyModifiedProperties();

        if (changed)
            splitBackground.UpdateSettings();

        DrawWarnings();
    }

    private void DrawSettings()
    {
        EditorGUILayout.LabelField("Sprites", EditorStyles.boldLabel);
        EditorGUI.indentLevel++;
        EditorGUILayout.PropertyField(leftSprite, new GUIContent("Left"));
        EditorGUILayout.PropertyField(rightSprite, new GUIContent("Right"));
        EditorGUI.indentLevel--;

        EditorGUILayout.Space();

        EditorGUILayout.PropertyField(material);
        EditorGUILayout.PropertyField(includesBackWall, new GUIContent("Offset for Back Wall", "Select this if the background is designed to work with a back wall."));
    }

    private void DrawReferences()
    {
        EditorGUILayout.LabelField("Sprite Renderers", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(leftSpriteRenderer, new GUIContent("Left"));
        EditorGUILayout.PropertyField(rightSpriteRenderer, new GUIContent("Right"));
    }

    private void DrawWarnings()
    {
        if (leftSpriteRenderer.objectReferenceValue == null ||
            rightSpriteRenderer.objectReferenceValue == null)
        {
            EditorGUILayout.HelpBox("Left and Right SpriteRenderers must be set in Referenced Objects", MessageType.Error, true);
        }
    }
}
