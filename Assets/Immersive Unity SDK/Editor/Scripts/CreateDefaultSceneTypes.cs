using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

/// <summary>
/// These options will appear in the Project View's Create Menu.
/// </summary>
public static class CreateDefaultSceneTypes
{
    [MenuItem("Assets/Create/Immersive Interactive/2D Scene")]
    public static void New2DScene() => CreateNewScene("New2DImmersiveScene", "Default2DScene (DO NOT CHANGE)");

    [MenuItem("Assets/Create/Immersive Interactive/3D Scene")]
    public static void New3DScene() => CreateNewScene("New3DImmersiveScene", "Default3DScene (DO NOT CHANGE)");

    [MenuItem("Assets/Create/Immersive Interactive/Layout Selector Scene")]
    public static void NewLayoutSelectorScene() => CreateNewScene("LayoutSelectorScene", "DefaultLayoutSelectorScene (DO NOT CHANGE)");

    [MenuItem("Assets/Create/Immersive Interactive/Loading Scene")]
    public static void NewLoadingScene() => CreateNewScene("Loading Scene", "Default Loading Scene (DO NOT CHANGE)");

    [MenuItem("Assets/Create/Immersive Interactive/Argument Scene Selector Scene")]
    public static void NewArgumentSceneSelectorScene() => CreateNewScene("Argument Selector Scene", "Default Argument Scene Selector (DO NOT CHANGE)");
   
    
    private static void CreateNewScene(string newSceneName, string templateSceneName)
    {
        var targetFilePath = GetNewAssetPath(newSceneName + ".unity");
        targetFilePath = GetUniqueFileName(targetFilePath);

        try
        {
            var templateFilePath = GetTemplateAssetPath(templateSceneName);
            FileUtil.CopyFileOrDirectory(templateFilePath, targetFilePath);
            AssetDatabase.Refresh();
            Debug.Log($"New 2D Immersive Scene created at {targetFilePath}.");
        }
        catch (IOException)
        {
            Debug.LogError($"Couldn't create {newSceneName} as there is already a scene at file path {targetFilePath}.");
        }
    }
    private static string GetTemplateAssetPath(string fileName)
    {
        var guids = AssetDatabase.FindAssets(fileName);
        if (guids.Length == 1)
            return AssetDatabase.GUIDToAssetPath(guids[0]);
        return null;
    }

    private static string GetUniqueFileName(string fileName)
    {
        string extension = Path.GetExtension(fileName);

        int i = 1;
        while (File.Exists(fileName))
        {
            if (i == 1)
                fileName = fileName.Replace(extension, " " + ++i + extension);
            else
                fileName = fileName.Replace(" " + i + extension, " " + ++i + extension);
        }

        return fileName;

    }

    private static string GetNewAssetPath(string fileName)
    {
        string path = "Assets";

        foreach (Object obj in Selection.GetFiltered(typeof(Object), SelectionMode.Assets))
        {
            path = AssetDatabase.GetAssetPath(obj);
            if (!string.IsNullOrEmpty(path) && File.Exists(path))
            {
                path = Path.GetDirectoryName(path);
                break;
            }
        }

        return Path.Combine(path, fileName);
    }
}