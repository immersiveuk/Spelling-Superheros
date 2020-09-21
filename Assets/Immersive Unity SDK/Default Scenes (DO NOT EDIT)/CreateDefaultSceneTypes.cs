using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

/// <summary>
/// These options will appear in the Project View's Create Menu.
/// </summary>
public class CreateDefaultSceneTypes : MonoBehaviour
{
#if UNITY_EDITOR
    [MenuItem("Assets/Create/Immersive Interactive/2D Scene")]
    static void New2DScene()
    {
        var path = GetAssetPath("New2DImmersiveScene.unity");
        path = GetUniqueFileName(path);
        try
        {
            FileUtil.CopyFileOrDirectory("Assets/Immersive Unity SDK/Default Scenes (DO NOT EDIT)/Default2DScene (DO NOT CHANGE).unity", path);
            AssetDatabase.Refresh();
            print("New 2D Immersive Scene created at " + path + ".");
        }
        catch (IOException)
        {
            Debug.LogError("Couldn't create new 2D Scene as there is already a scene at file path " + path + ".");
        }
    }

    [MenuItem("Assets/Create/Immersive Interactive/3D Scene")]
    static void New3DScene()
    {
        var path = GetAssetPath("New3DImmersiveScene.unity");
        path = GetUniqueFileName(path);
        try
        {
            FileUtil.CopyFileOrDirectory("Assets/Immersive Unity SDK/Default Scenes (DO NOT EDIT)/Default3DScene (DO NOT CHANGE).unity", path);
            AssetDatabase.Refresh();
            print("New 3D Immersive Scene created at " + path + ".");
        }
        catch (IOException)
        {
            Debug.LogError("Couldn't create new 3D Scene as there is already a scene at file path " + path + ".");
        }

    }

    [MenuItem("Assets/Create/Immersive Interactive/Layout Selector Scene")]
    static void NewLayoutSelectorScene()
    {
        var path = GetAssetPath("LayoutSelectorScene.unity");
        path = GetUniqueFileName(path);
        try
        {
            FileUtil.CopyFileOrDirectory("Assets/Immersive Unity SDK/Default Scenes (DO NOT EDIT)/DefaultLayoutSelectorScene (DO NOT CHANGE).unity", path);
            AssetDatabase.Refresh();
            print("New Layout Selector Scene created at " + path + ".");
        }
        catch (IOException)
        {
            Debug.LogError("Couldn't create new Layout Selector Scene as there is already a scene at file path " + path + ".");
        }
    }

    [MenuItem("Assets/Create/Immersive Interactive/Loading Scene")]
    static void NewLoadingScene()
    {
        var path = GetAssetPath("Loading Scene.unity");
        path = GetUniqueFileName(path);
        try
        {
            FileUtil.CopyFileOrDirectory("Assets/Immersive Unity SDK/Default Scenes (DO NOT EDIT)/Default Loading Scene (DO NOT CHANGE).unity", path);
            AssetDatabase.Refresh();
            print("New Loading Scene created at " + path + ".");
        }
        catch (IOException)
        {
            Debug.LogError("Couldn't create new Loading Scene as there is already a scene at file path " + path + ".");
        }
    }

    static string GetUniqueFileName(string fileName)
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

    static string GetAssetPath(string fileName)
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

#endif
}