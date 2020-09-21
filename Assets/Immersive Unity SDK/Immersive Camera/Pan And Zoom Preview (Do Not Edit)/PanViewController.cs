using Com.Immersive.Cameras;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanViewController : MonoBehaviour
{
    //Used to save camera position between scenes.
    public static bool isFirstScene = true;
    private static Vector3 camPosition;
    private static float camSize = 0.5f;

    public Transform screens;
    public Camera cam;

    public Material screenMaterial;

    private float maxOrthographicWidth = 1;
    private float maxOrthographicHeight = 1;
    private float minOrthographicSize = 0.5f;
    private float maxXPosition = 1;
    private float minXPosition = 1;
    private float zoomedOutHeight = 1;
    //private float zoomedOutWidth = 1;
    private float zoomedOutYPosition = 0;
    private float zoomedOutXPosition = 0;

    private float maxYPosition = 1;
    private float minYPosition = 1;

    private Vector3 previousMousePosition = Vector3.zero;


    private const float screenSeperation = 0.01f;
   
    /// <summary>
    /// Zoom in the pan camera.
    /// </summary>
    /// <param name="zoom">A negative value will zoom out and positive value will zoom in.</param>
    public void Zoom(float zoom)
    {
        var aspectRatio = (float)cam.pixelWidth / (float)cam.pixelHeight;

        cam.orthographicSize *= 1 + (-1 * zoom);
        ConstrainCamPosition();

    }

    /// <summary>
    /// Pan the camera.
    /// </summary>
    /// <param name="mouseDelta">How many screen pixels to pan by.</param>
    public void Pan(Vector3 mouseDelta)
    {
        var translation = mouseDelta / cam.pixelHeight;
        translation *= cam.orthographicSize * 2;
        cam.transform.Translate(translation);

        ConstrainCamPosition();
    }

    private void ConstrainCamPosition()
    {

        var aspectRatio = (float)cam.pixelWidth / (float)cam.pixelHeight;

        ////Constrain max size
        if (cam.orthographicSize > maxOrthographicWidth / aspectRatio && cam.orthographicSize > maxOrthographicHeight)
        {
            cam.orthographicSize = Mathf.Max(maxOrthographicWidth / aspectRatio, maxOrthographicHeight);
        }

        //Constrain min size
        if (cam.orthographicSize < minOrthographicSize)
        {
            cam.orthographicSize = minOrthographicSize;
        }


        Vector3 position = cam.transform.localPosition;

        if (cam.orthographicSize * aspectRatio > maxOrthographicWidth)
        {
            position.x = zoomedOutXPosition;
        }
        else
        {
            //Constrain Right Bound
            var camRightPos = cam.transform.localPosition.x + cam.orthographicSize * aspectRatio;
            if (camRightPos > maxXPosition)
            {
                position.x = maxXPosition - cam.orthographicSize * aspectRatio;
            }

            //Constrain Left Bound
            var camLeftPos = cam.transform.localPosition.x - cam.orthographicSize * aspectRatio;
            if (camLeftPos < minXPosition)
            {
                position.x = minXPosition + cam.orthographicSize * aspectRatio;
            }
        }

        //Constrain Y Position.
        if (cam.orthographicSize > zoomedOutHeight)
        {
            position.y = zoomedOutYPosition;
        }
        else
        {
            var camTopPos = cam.transform.localPosition.y + cam.orthographicSize;
            if (camTopPos > maxYPosition)
            {
                position.y = maxYPosition - cam.orthographicSize;
            }

            var camBottomPos = cam.transform.localPosition.y - cam.orthographicSize;
            if (camBottomPos < minYPosition)
            {
                position.y = minYPosition + cam.orthographicSize;
            }
        }

        cam.transform.localPosition = position;
    }

    // Update is called once per frame
    void Update()
    {
        //If mouse over screen then Zoom.
        Rect screenRect = new Rect(0, 0, Screen.width, Screen.height);
        if (screenRect.Contains(Input.mousePosition))
        {
            Zoom(Input.GetAxis("Mouse ScrollWheel"));

        }

        //ROTATE
        if (Input.GetMouseButtonDown(1))
        {
            previousMousePosition = Input.mousePosition;
        }
        //else 
        if (Input.GetMouseButton(1))
        {
            Vector3 mouseDelta = previousMousePosition - Input.mousePosition;
            Pan(mouseDelta);
            previousMousePosition = Input.mousePosition;
        }

        camPosition = cam.transform.localPosition;
        camSize = cam.orthographicSize;
    }

    public void LayoutSurfaces(List<SurfaceInfo> surfacesInfo, List<RenderTexture> renderTextures)
    {
        // 1. Calculate the width of the central screen and maximum width of a side wall.
        float centerWidth = 0;
        float rightWidth = 0;

        foreach (SurfaceInfo surfaceInfo in surfacesInfo)
        {
            switch (surfaceInfo.position)
            {
                case SurfacePosition.Center:
                case SurfacePosition.Back:
                    var newCenterWidth = surfaceInfo.aspectRatio;
                    centerWidth = Mathf.Max(centerWidth, newCenterWidth);
                    break;
                case SurfacePosition.Left: break;
                case SurfacePosition.Right:
                    rightWidth = surfaceInfo.aspectRatio;
                    break;
            }
        }


        //Keep track of surfaces which are in use
        List<SurfacePosition> surfacePositions = new List<SurfacePosition>();

        //Resize all screens
        for (int i = 0; i < surfacesInfo.Count; i++)
        {
            SurfaceInfo surfaceInfo = surfacesInfo[i];
            RenderTexture renderTexture = renderTextures[i];

            surfacePositions.Add(surfaceInfo.position);
            switch (surfaceInfo.position)
            {
                case SurfacePosition.Left:
                    var left = screens.GetChild(0);
                    var width = surfaceInfo.aspectRatio;
                    left.localPosition = new Vector3(-(centerWidth + width) / 2 - screenSeperation, 0, 0);
                    left.localScale = new Vector3(width, 1, 1);
                    SetScreenTexture(renderTexture, left);
                    break;
                case SurfacePosition.Center:
                    var center = screens.GetChild(1);
                    center.localScale = new Vector3(centerWidth, 1, 1);
                    SetScreenTexture(renderTexture, center);
                    break;
                case SurfacePosition.Right:
                    var right = screens.GetChild(2);
                    width = surfaceInfo.aspectRatio;
                    right.localPosition = new Vector3((centerWidth + width) / 2 + screenSeperation, 0, 0);
                    right.localScale = new Vector3(width, 1, 1);
                    SetScreenTexture(renderTexture, right);
                    break;
                case SurfacePosition.Back:
                    var back = screens.GetChild(3);
                    width = surfaceInfo.aspectRatio;
                    back.localPosition = new Vector3((centerWidth + width) / 2 + rightWidth + screenSeperation*2, 0, 0);
                    back.localScale = new Vector3(width, 1, 1);
                    SetScreenTexture(renderTexture, back);
                    break;
                case SurfacePosition.Floor:
                    var floor = screens.GetChild(4);
                    floor.localScale = new Vector3(surfaceInfo.aspectRatio, 1, 1);
                    floor.localPosition = new Vector3(0, -1 - screenSeperation, 0);
                    SetScreenTexture(renderTexture, floor);
                    break;
            }
        }

        //Disable Screens which are not in used
        screens.GetChild(0).gameObject.SetActive(surfacePositions.Contains(SurfacePosition.Left));
        screens.GetChild(1).gameObject.SetActive(surfacePositions.Contains(SurfacePosition.Center));
        screens.GetChild(2).gameObject.SetActive(surfacePositions.Contains(SurfacePosition.Right));
        screens.GetChild(3).gameObject.SetActive(surfacePositions.Contains(SurfacePosition.Back));
        screens.GetChild(4).gameObject.SetActive(surfacePositions.Contains(SurfacePosition.Floor));

        //Get Leftmost Screen
        var leftmostScreen = GetLeftmostScreen();

        //Get Rightmost Screen
        var rightMostScreen = GetRightmostScreen();

        var topMostScreen = GetTopScreen();

        //Get Bottom Screen
        var bottomMostScreen = screens.GetChild(screens.childCount-1);

        
        //CALCULATE POSITION BOUNDS.
        maxXPosition = (rightMostScreen.localPosition.x + rightMostScreen.localScale.x / 2 + screenSeperation);
        minXPosition = (leftmostScreen.localPosition.x - leftmostScreen.localScale.x / 2 - screenSeperation);

        maxYPosition = topMostScreen.localPosition.y + topMostScreen.localScale.y / 2 + screenSeperation;
        minYPosition = bottomMostScreen.localPosition.y - bottomMostScreen.localScale.y / 2 - screenSeperation;

        maxOrthographicWidth = maxXPosition - minXPosition;
        maxOrthographicWidth /= 2;

        maxOrthographicHeight = maxYPosition - minYPosition;
        maxOrthographicHeight /= 2;

        minOrthographicSize = 0.5f + screenSeperation;

        if (surfacePositions.Contains(SurfacePosition.Floor))
        {
            zoomedOutYPosition = -0.5f;
            zoomedOutHeight = 1 + screenSeperation;
        }
        else
        {
            zoomedOutYPosition = 0;
            zoomedOutHeight = 0.5f;
        }

        zoomedOutXPosition = (maxXPosition + minXPosition)/ 2;

        var aspectRatio = (float)cam.pixelWidth / (float)cam.pixelHeight;
        cam.orthographicSize = maxOrthographicWidth * aspectRatio;

        PositionCamera();

    }

    private void PositionCamera()
    {
        if (isFirstScene)
        {
            isFirstScene = false;
            ConstrainCamPosition();

        }
        else
        {
            cam.transform.localPosition = camPosition;
            cam.orthographicSize = camSize;
        }
    }

    private Transform GetLeftmostScreen()
    {
        Transform leftmostScreen = null;
        float leftScreenLeftPosition = 0;
        for (int i = 0; i < screens.childCount; i++)
        {
            var screen = screens.GetChild(i);
            if (!screen.gameObject.activeSelf) continue;

            var screenLeftPosition = screen.position.x - screen.localScale.x / 2;
            if (leftmostScreen == null) { leftmostScreen = screen; leftScreenLeftPosition = screenLeftPosition; continue; }
            if (screenLeftPosition < leftScreenLeftPosition) { leftmostScreen = screen; leftScreenLeftPosition = screenLeftPosition; }
        }
        return leftmostScreen;
    }

    private Transform GetRightmostScreen()
    {
        Transform rightmostScreen = null;
        float rightScreenRightPosition = 0;
        for (int i = 0; i < screens.childCount; i++)
        {
            var screen = screens.GetChild(i);
            if (!screen.gameObject.activeSelf) continue;

            var screenRightPosition = screen.position.x + screen.localScale.x / 2;
            if (rightmostScreen == null) { rightmostScreen = screen; rightScreenRightPosition = screenRightPosition; continue; }
            if (screen.position.x > rightmostScreen.position.x) { rightmostScreen = screen; rightScreenRightPosition = screenRightPosition; }
        }
        return rightmostScreen;
    }

    public Transform GetTopScreen()
    {
        for (int i = 0; i < screens.childCount-1; i++)
        {
            var screen = screens.GetChild(i);
            if (screen.gameObject.activeSelf) return screen;

        }
        return null;

    }

    private void SetScreenTexture(RenderTexture texture, Transform screen)
    {
        var material = new Material(screenMaterial);
        material.SetTexture("_MainTex", texture);

        screen.GetComponent<Renderer>().material = material;
    }
    

}
