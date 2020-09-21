using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SwitchScenesUsingUpDownKeys : MonoBehaviour
{
    [SceneSelector]
    public string NextScene;
    [SceneSelector]
    public string PreviousScene;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            SceneManager.LoadScene(NextScene);
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            SceneManager.LoadScene(PreviousScene);

        }
    }
}
