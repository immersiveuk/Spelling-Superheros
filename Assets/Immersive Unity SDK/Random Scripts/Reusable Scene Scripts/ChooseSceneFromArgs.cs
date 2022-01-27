using Com.Immersive.Cameras;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChooseSceneFromArgs : MonoBehaviour
{
    [SerializeField, SceneSelector] string defaultFirstScene = null;

    private const string arg = "-scene=";

    // Start is called before the first frame update
    void Start()
    {
        int sceneIndex = GetSceneIndexFromArgs(System.Environment.CommandLine);
        if (IsValidSceneIndex(sceneIndex))
            SceneManager.LoadScene(sceneIndex);
        else
            SceneManager.LoadScene(defaultFirstScene);
    }

    private int GetSceneIndexFromArgs(string args)
    {
        string[] parameters = ReadParameters.SplitParams(args);
        foreach (var parameter in parameters)
        {
            print("Param: " + parameter);
            if (parameter.ToLower().StartsWith(arg))
            {
                string sceneStr = parameter.Remove(0,arg.Length);
                sceneStr = ReadParameters.RemoveQuotationMarks(sceneStr);
                bool success = int.TryParse(sceneStr, out int sceneInt);
                if (!success)
                    return -1;

                return sceneInt;
            }
        }
        return -1;
    }

    private bool IsValidSceneIndex(int sceneIndex) => sceneIndex > 0 && sceneIndex < SceneManager.sceneCountInBuildSettings;
}
