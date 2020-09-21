using Com.Immersive.Cameras;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingScreen : MonoBehaviour
{
    [SceneSelector]
    public string standardSceneName;
    [SceneSelector]
    public string wideFrontSceneName;
    [SceneSelector]
    public string wideSceneName;

    [SerializeField] Image loadingImage = null;

    private AsyncOperation ao;

    private void Start()
    {
        StartCoroutine(LoadScene());
    }

    IEnumerator LoadScene()
    {
        yield return null;
        yield return new WaitForSeconds(2);

        ao = SceneManager.LoadSceneAsync(GetSceneName());
        ao.allowSceneActivation = false;

        while (!ao.isDone)
        {
            loadingImage.rectTransform.anchorMax = new Vector2(ao.progress, 1);

            // Check if the load has finished
            if (ao.progress >= 0.9f)
            {
                ao.allowSceneActivation = true;
            }

            yield return null;
        }
    }

    private string GetSceneName()
    {
        var walls = AbstractImmersiveCamera.CurrentImmersiveCamera.walls;
        float maxSideAspectRatio = 0;
        float centerAspectRatio = 0;

        foreach (var wall in walls)
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
            return standardSceneName;
        }

        //Wide Front
        if (centerAspectRatio >= 2 && maxSideAspectRatio < 2 && maxSideAspectRatio != 0)
        {
            return wideFrontSceneName;
        }

        //Wide
        if (centerAspectRatio >= 2 && (maxSideAspectRatio >= 2 || maxSideAspectRatio == 0))
        {
            return wideSceneName;
        }
        return null;
    }
}
