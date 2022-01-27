/* Copyright (C) Immersive Interactive, Ltd - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Luke Bissell <luke@immersive.co.uk>, Nov 2019
 */
using Com.Immersive.Cameras;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// A script which will go to a different scene depending upon whether the room is Standard, Wide or Wide Front.
/// </summary>
public class ChooseSceneBasedOnLayout : MonoBehaviour
{
    [SceneSelector]
    public string standardSceneName;
    [SceneSelector]
    public string wideFrontSceneName;
    [SceneSelector]
    public string wideSceneName;

    // Start is called before the first frame update
    void Start()
    {
        var walls = AbstractImmersiveCamera.CurrentImmersiveCamera.walls;
        float maxSideAspectRatio = 0;
        float centerAspectRatio = 0;

        foreach(var wall in walls)
        {
            if (wall.position == SurfacePosition.Center)
            {
                centerAspectRatio = wall.rect.width / wall.rect.height;
            }
            else if (wall.position == SurfacePosition.Left || wall.position == SurfacePosition.Right)
            {
                var aspectRatio = wall.rect.width / wall.rect.height;
                if (aspectRatio > maxSideAspectRatio) maxSideAspectRatio = aspectRatio;
            }
        }

        //Standard
        if (centerAspectRatio < 2 && maxSideAspectRatio < 2)
        {
            SceneManager.LoadScene(standardSceneName);
            return;
        }

        //Wide Front
        if (centerAspectRatio >= 2 && maxSideAspectRatio < 2 && maxSideAspectRatio != 0)
        {
            SceneManager.LoadScene(wideFrontSceneName);
            return;
        }

        //Wide
        if (centerAspectRatio >= 2 && (maxSideAspectRatio >= 2 || maxSideAspectRatio == 0))
        {
            SceneManager.LoadScene(wideSceneName);
            return;
        }

        SceneManager.LoadScene(standardSceneName);

    }

}
