using Com.Immersive.Cameras;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// This script allows you to render a CubeMap of the scene and the CurrentImmersiveCamera's position.
/// Shortcut buttons are provided in the Menu Bar at Immersive Interactive > Pre-Render which allow you to render the entire cube or individual faces.
/// Rendered Images will be placed in Assets > Rendered Screens
/// </summary>
public class RenderCube : MonoBehaviour
{
    static int cubemapSize = 1920;
    static Camera cam;
    static RenderTexture renderTexture;


#if UNITY_EDITOR

    [MenuItem("Immersive Interactive/Pre-Render/Render Entire Cube")]
    static void RenderCubeFaces()
    {
        CreateCamera();
        CreateRenderTexture();
        RenderAllFaces();
        DestroyImmediate(cam);
        DestroyImmediate(renderTexture);
    }

    [MenuItem("Immersive Interactive/Pre-Render/Render Cube Front")]
    static void RenderCubeFront()
    {
        CreateCamera();
        CreateRenderTexture();
        RenderFrontFace();
        DestroyImmediate(cam);
        DestroyImmediate(renderTexture);
    }

    [MenuItem("Immersive Interactive/Pre-Render/Render Cube Back")]
    static void RenderCubeBack()
    {
        CreateCamera();
        CreateRenderTexture();
        RenderBackFace();
        DestroyImmediate(cam);
        DestroyImmediate(renderTexture);
    }

    [MenuItem("Immersive Interactive/Pre-Render/Render Cube Right")]
    static void RenderCubeRight()
    {
        CreateCamera();
        CreateRenderTexture();
        RenderRightFace();
        DestroyImmediate(cam);
        DestroyImmediate(renderTexture);
    }

    [MenuItem("Immersive Interactive/Pre-Render/Render Cube Left")]
    static void RenderCubeLeft()
    {
        CreateCamera();
        CreateRenderTexture();
        RenderLeftFace();
        DestroyImmediate(cam);
        DestroyImmediate(renderTexture);
    }

    [MenuItem("Immersive Interactive/Pre-Render/Render Cube Bottom")]
    static void RenderCubeBottom()
    {
        CreateCamera();
        CreateRenderTexture();
        RenderBottomFace();
        DestroyImmediate(cam);
        DestroyImmediate(renderTexture);
    }

    [MenuItem("Immersive Interactive/Pre-Render/Render Cube Top")]
    static void RenderCubeTop()
    {
        CreateCamera();
        CreateRenderTexture();
        RenderTopFace();
        DestroyImmediate(cam);
        DestroyImmediate(renderTexture);
    }


#endif

    private static void CreateCamera()
    {
        GameObject obj = new GameObject("CubemapCamera", typeof(Camera));
        obj.hideFlags = HideFlags.HideAndDontSave;
        cam = obj.GetComponent<Camera>();
        cam.farClipPlane = 100000;
        cam.fieldOfView = 90;
        cam.enabled = false;

    }

    private static void CreateRenderTexture()
    {
        renderTexture = new RenderTexture(cubemapSize, cubemapSize, 24);
        renderTexture.antiAliasing = 8;
    }


    private static void RenderAllFaces()
    {
        RenderFrontFace();
        RenderRightFace();
        RenderBackFace();
        RenderLeftFace();
        RenderTopFace();
        RenderBottomFace();
    }

    private static void RenderFrontFace()
    {
        cam.transform.position = AbstractImmersiveCamera.CurrentImmersiveCamera.transform.position;
        cam.transform.rotation = AbstractImmersiveCamera.CurrentImmersiveCamera.transform.rotation;
        RenderCurrentDirection("front");
    }

    private static void RenderRightFace()
    {
        cam.transform.position = AbstractImmersiveCamera.CurrentImmersiveCamera.transform.position;
        cam.transform.rotation = AbstractImmersiveCamera.CurrentImmersiveCamera.transform.rotation;
        cam.transform.Rotate(new Vector3(0, 90, 0));
        RenderCurrentDirection("right");
    }

    private static void RenderBackFace()
    {
        cam.transform.position = AbstractImmersiveCamera.CurrentImmersiveCamera.transform.position;
        cam.transform.rotation = AbstractImmersiveCamera.CurrentImmersiveCamera.transform.rotation;
        cam.transform.Rotate(new Vector3(0, 180, 0));
        RenderCurrentDirection("back");
    }

    private static void RenderLeftFace()
    {
        cam.transform.position = AbstractImmersiveCamera.CurrentImmersiveCamera.transform.position;
        cam.transform.rotation = AbstractImmersiveCamera.CurrentImmersiveCamera.transform.rotation;
        cam.transform.Rotate(new Vector3(0, -90, 0));
        RenderCurrentDirection("left");
    }

    private static void RenderTopFace()
    {
        cam.transform.position = AbstractImmersiveCamera.CurrentImmersiveCamera.transform.position;
        cam.transform.rotation = AbstractImmersiveCamera.CurrentImmersiveCamera.transform.rotation;
        cam.transform.Rotate(new Vector3(-90, 0, 0));
        RenderCurrentDirection("top");
    }

    private static void RenderBottomFace()
    {
        cam.transform.position = AbstractImmersiveCamera.CurrentImmersiveCamera.transform.position;
        cam.transform.rotation = AbstractImmersiveCamera.CurrentImmersiveCamera.transform.rotation;
        cam.transform.Rotate(new Vector3(90, 0, 0));
        RenderCurrentDirection("bottom");
    }

    private static void RenderCurrentDirection(string suffix)
    {
        cam.forceIntoRenderTexture = true;
        cam.targetTexture = renderTexture;

        cam.Render();
        SaveTexture(renderTexture, suffix);
    }

    private static void SaveTexture(RenderTexture rt, string suffix)
    {
        byte[] bytes = toTexture2D(rt).EncodeToPNG();

        var directoryPath = Application.dataPath + "/Rendered Screens";

        //If RenderScreen directory doesn't exist create it.
        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }

        System.IO.File.WriteAllBytes(directoryPath+"/"+SceneManager.GetActiveScene().name+"_SavedScreen_"+suffix+".png", bytes);

#if UNITY_EDITOR
        AssetDatabase.Refresh();
#endif
    }

    private static Texture2D toTexture2D(RenderTexture rTex)
    {
        Texture2D tex = new Texture2D(rTex.width, rTex.height, TextureFormat.RGB24, false);
        RenderTexture.active = rTex;
        tex.ReadPixels(new Rect(0, 0, rTex.width, rTex.height), 0, 0);
        tex.Apply();
        return tex;
    }


}
