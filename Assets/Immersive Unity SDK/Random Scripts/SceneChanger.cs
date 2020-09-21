using Com.Immersive.Cameras.PostProcessing;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    public bool FadeOut = true;

    private string sceneName;

    public void ReloadScene ()
    {

        sceneName = SceneManager.GetActiveScene().name;
        if (FadeOut)
        {
            FadeInAndOut.CurrentFadeInAndOut.FadeOut(3, ChangeToScene);
        }
        else ChangeToScene();
    }

    public void ChangeScene(string sceneName)
    {
        this.sceneName = sceneName;
        if (FadeOut)
        {
            FadeInAndOut.CurrentFadeInAndOut.FadeOut(3, ChangeToScene);
        }
        else ChangeToScene();
    }

    private void ChangeToScene()
    {
        print("ChangeToScene: " + sceneName);
        SceneManager.LoadScene(sceneName);
    }
}
