#if UNITY_EDITOR

using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;
using UnityEngine;

public static class UserEditableSetup
{

    private static string[] packages =
    {
        "com.unity.nuget.newtonsoft-json",
        "com.unity.editorcoroutines",
        "https://github.com/immersiveuk/Immersive-Animation-Package.git",
        "https://github.com/immersiveuk/Immersive-Editor-Tools-Package.git",
        "https://github.com/immersiveuk/Immersive-Reusable-Components.git",
        "https://github.com/immersiveuk/Immersive-Asset-Management-Package.git?path=/com.immersiveinteractive.immersivejsonandassetmanagement"
    };

    static AddRequest request;

    static int index = 0;

    [MenuItem("Immersive Interactive/Add Legacy Packages")]
    public static void WOOO(MenuCommand command)
    {
        Go();
    }

    static void Go()
    {
        if (index < packages.Length)
        {
            request = Client.Add(packages[index]);
            index++;
            EditorApplication.update += Progress;
        }
        else
        {
            Debug.Log("Finished Adding Packages.");
        }
    }


    static void Progress()
    {
        if (request.IsCompleted)
        {
            if (request.Status == StatusCode.Success)
                Debug.Log($"Added {request.Result.name}");
            else if (request.Status >= StatusCode.Failure)
                Debug.Log($"Failed: {request.Error.message},");

            EditorApplication.update -= Progress;
            Go();
        }

    }

}
#endif