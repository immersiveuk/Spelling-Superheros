using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class SceneSelectorAttribute: PropertyAttribute
{
    string sceneName;
}

#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(SceneSelectorAttribute))]
public class SceneSelectorPropertyDrawer : PropertyDrawer
{

    private bool initialised = false;
    private string[] sceneNames;
    private int sceneIndex;

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        //verticalSpace = EditorGUI.GetPropertyHeight(property.FindPropertyRelative("sprite")) + EditorGUIUtility.singleLineHeight;
        
        return 2 * EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing/* + verticalSpace*/;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        if (!initialised)
        {
            sceneNames = GetScenesNames();
            sceneIndex = GetSceneIndex(property.stringValue);
        }

        var labelRect = new Rect(position.x, position.y, EditorGUIUtility.labelWidth, EditorGUIUtility.singleLineHeight);
        EditorGUI.LabelField(labelRect, new GUIContent(label.text, label.tooltip));
        
        var sceneSelectorRect = new Rect(EditorGUIUtility.labelWidth, position.y, position.width - EditorGUIUtility.labelWidth, EditorGUIUtility.singleLineHeight);

        var buttonRect = new Rect(EditorGUIUtility.labelWidth, position.y + EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing, position.width - EditorGUIUtility.labelWidth, EditorGUIUtility.singleLineHeight);

        var sceneIndexNew = EditorGUI.Popup(sceneSelectorRect, sceneIndex, sceneNames);
        if (sceneIndex != sceneIndexNew)
        {
            sceneIndex = sceneIndexNew;
            property.stringValue = sceneNames[sceneIndex];
        }

        if (GUI.Button(buttonRect, "Add Scenes to Build Settings"))
        {
            EditorApplication.ExecuteMenuItem("File/Build Settings...");

        }

    }

    private void Init()
    {
        sceneNames = GetScenesNames();

    }

    private string[] GetScenesNames()
    {
        int sceneCount = UnityEngine.SceneManagement.SceneManager.sceneCountInBuildSettings;
        string[] scenes = new string[sceneCount];
        for (int i = 0; i < sceneCount; i++)
        {
            scenes[i] = System.IO.Path.GetFileNameWithoutExtension(UnityEngine.SceneManagement.SceneUtility.GetScenePathByBuildIndex(i));
        }
        return scenes;
    }

    //Returns -1 if name is incorrect
    private int GetSceneIndex(string sceneName)
    {
        for (int i = 0; i < sceneNames.Length; i++)
        {
            if (sceneName == sceneNames[i]) return i;
        }
        return -1;
    }

}

#endif