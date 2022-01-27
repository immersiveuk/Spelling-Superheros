using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class RenameObjectPopUpWindow : PopupWindowContent
{
    private SerializedProperty property;
    private float width;
    private string newName = "";

    public RenameObjectPopUpWindow(SerializedProperty property, float width)
    {
        if (property.objectReferenceValue == null)
        {
            Debug.LogError("Cannot Rename a non UnityObject.");
            editorWindow.Close();
            return;
        }

        this.property = property;
        this.width = width;

        newName = property.objectReferenceValue.name;
    }

    public override Vector2 GetWindowSize()
    {
        return new Vector2(width, EditorGUIUtility.singleLineHeight + 2* EditorGUIUtility.standardVerticalSpacing);
    }


    public override void OnGUI(Rect rect)
    {
        EditorGUILayout.BeginHorizontal();
        EditorGUI.BeginChangeCheck();

        newName = EditorGUILayout.DelayedTextField(newName);

        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(property.objectReferenceValue, "Changed Name");
            property.objectReferenceValue.name = newName;
            AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(property.objectReferenceValue));
            AssetDatabase.SaveAssets();
            editorWindow.Close();
        }
        EditorGUILayout.EndHorizontal();
    }
}
