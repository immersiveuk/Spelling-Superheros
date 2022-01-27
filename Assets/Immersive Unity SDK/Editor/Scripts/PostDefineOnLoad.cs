using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;

public class PostDefineOnLoad
{
    //TODO: Create list of possible defines and rework the functionality to loop through said list
    private static ListRequest _request;
    
    [InitializeOnLoadMethod]
    private static void OnLoad()
    {
        _request = Client.List();
        EditorApplication.update += LoadProgress;
    }

    private static void LoadProgress()
    {
        if (_request.IsCompleted)
        {
            if (_request.Result != null)
            {
                List<string> packageNames = new List<string>();
                foreach (var result in _request.Result)
                {
                    packageNames.Add(result.name);
                }

                if (packageNames.Any(x => x == "com.immersiveinteractive.immersivestate"))
                {
                    SetDefine(BuildTargetGroup.Standalone, "UNITY_ATOMS_AVAILABLE");
                }
            }
            else
            {
                ClearDefine(BuildTargetGroup.Standalone, "UNITY_ATOMS_AVAILABLE");
            }

            EditorApplication.update -= LoadProgress;
        }
    }
    
    private static void SetDefine(BuildTargetGroup targetGroup, string definition)
    {
        string currentDefines = PlayerSettings.GetScriptingDefineSymbolsForGroup(targetGroup);

        if (currentDefines.Contains(definition)) return;

        if (!currentDefines.EndsWith(";"))
        {
            definition = definition.Insert(0, ";");
        }

        currentDefines += definition;
        
        PlayerSettings.SetScriptingDefineSymbolsForGroup(targetGroup, currentDefines);
    }

    private static void ClearDefine(BuildTargetGroup targetGroup, string definition)
    {
        string currentDefines = PlayerSettings.GetScriptingDefineSymbolsForGroup(targetGroup);

        if (!currentDefines.Contains(definition)) return;

        int index = currentDefines.IndexOf(definition);
        if (index == -1) return;
        
        string updatedDefines = currentDefines.Remove(index, definition.Length);

        if (string.IsNullOrEmpty(updatedDefines))
        {
            PlayerSettings.SetScriptingDefineSymbolsForGroup(targetGroup, string.Empty);
            return;
        }
        
        PlayerSettings.SetScriptingDefineSymbolsForGroup(targetGroup, updatedDefines);
    }
}
