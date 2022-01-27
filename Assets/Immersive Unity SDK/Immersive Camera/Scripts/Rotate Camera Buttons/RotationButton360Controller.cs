using Com.Immersive.Cameras;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This adds rotation buttons to 3D camera which allow you to rotate in 360 degrees.
/// </summary>
[RequireComponent(typeof(ImmersiveCamera3D))]
public class RotationButton360Controller : MonoBehaviour
{

    //Prefabs which provide the left and right buttons
    public GameObject moveLeftButtonPrefab;
    public GameObject moveRightButtonPrefab;

    [SerializeField] bool turnOnButtonAtStart = true;

    private AbstractImmersiveCamera immersiveCamera;

    //References to the left and right buttons.
    private GameObject moveLeftButton;
    private GameObject moveRightButton;

    // Start is called before the first frame update
    void Start()
    {
        immersiveCamera = GetComponent<AbstractImmersiveCamera>();
        if (immersiveCamera == null)
        {
            Debug.LogError("RotationButtonController: No Immersive Camera found on GameObject.");
            return;
        }
        
        PlaceRotationButtons();
    }
    /// <summary>
    /// Instantiates and places rotation buttons onto the correct canvas.
    /// </summary>
    private void PlaceRotationButtons()
    {
        
        Canvas leftButtonCanvas, rightButtonCanvas;

        //4 walls buttons appear centre canvas.
        if (immersiveCamera.WallCount == 4)
        {
            leftButtonCanvas = immersiveCamera.CentreCanvas;
            rightButtonCanvas = immersiveCamera.CentreCanvas;
        }
        //Less than 4 walls buttons appear on leftmost and rightmost canvas.
        else
        {
            leftButtonCanvas = immersiveCamera.uiCameras[0].GetComponent<UICamera>().canvas;
            if (immersiveCamera.floorCamera != null)
                rightButtonCanvas = immersiveCamera.uiCameras[immersiveCamera.uiCameras.Count - 2].GetComponent<UICamera>().canvas;
            else
                rightButtonCanvas = immersiveCamera.uiCameras[immersiveCamera.uiCameras.Count - 1].GetComponent<UICamera>().canvas;
        }


        moveLeftButton = Instantiate(moveLeftButtonPrefab, leftButtonCanvas.transform);
        moveRightButton = Instantiate(moveRightButtonPrefab, rightButtonCanvas.transform);

        moveLeftButton.GetComponent<MoveLeftTrigger>().immersiveCamera = immersiveCamera;
        moveRightButton.GetComponent<MoveRightTrigger>().immersiveCamera = immersiveCamera;

        if (turnOnButtonAtStart)
            EnableRotationButtons();
    }

    public void EnableRotationButtons()
    {
        if (moveLeftButton)
            moveLeftButton.SetActive(true);
        if (moveRightButton)
            moveRightButton.SetActive(true);
    }

    public void DisableRotationButtons()
    {
        if (moveLeftButton)
            moveLeftButton.SetActive(false);
        if (moveRightButton)
            moveRightButton.SetActive(false);
    }
}
