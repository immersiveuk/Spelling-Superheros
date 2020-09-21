/* Copyright (C) Immersive Interactive, Ltd - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Luke Bissell <luke@immersive.co.uk>, July 2019
 */

using Com.Immersive.Cameras;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This component will add rotation buttons to when there are two or fewer walls.
/// The buttons call the RotateCameraLeft and RotateCameraRight methods which
/// must be implemented in any Subclass of GenericImmersiveCamera.
/// </summary>
public class RotationButtonController : MonoBehaviour
{
    //Prefabs which provide the left and right buttons
    public GameObject moveLeftButtonPrefab;
    public GameObject moveRightButtonPrefab;

    private AbstractImmersiveCamera immersiveCamera;
    private CameraPosition camPos = CameraPosition.Left;

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

        //If more than two walls no need for rotation buttons.
        if (immersiveCamera.WallCount > 2) return;

        // 1. Work out the start CameraPosition and physical camera position.
        if (immersiveCamera.WallCount == 1) camPos = CameraPosition.Center;
        else camPos = immersiveCamera.HasLeftWall ? CameraPosition.Left : CameraPosition.Right;

        // 2. Handle Single Wall case
        if (immersiveCamera.WallCount == 1)
        {

            // 2.1. Find Canvas which is in use.
            Canvas canvas = null;
            switch (immersiveCamera.surfaces[0].position)
            {
                case SurfacePosition.Left: canvas = immersiveCamera.leftUICamera.GetComponent<UICamera>().canvas; break;
                case SurfacePosition.Center: canvas = immersiveCamera.centerUICamera.GetComponent<UICamera>().canvas; break;
                case SurfacePosition.Right: canvas = immersiveCamera.rightUICamera.GetComponent<UICamera>().canvas; break;
            }

            // 2.2. Instantiate and setup Rotate Left and Right button.
            moveLeftButton = Instantiate(moveLeftButtonPrefab, canvas.transform);
            moveRightButton = Instantiate(moveRightButtonPrefab, canvas.transform);
        }

        // 3. Handle Double Wall case
        if (immersiveCamera.WallCount == 2)
        {
            // 3.1. Find Center Canvas
            var centerCanvas = immersiveCamera.centerUICamera.GetComponent<UICamera>().canvas;

            // 3.2. Hand Left and Center Wall case.
            if (immersiveCamera.HasLeftWall)
            {
                // 3.2.1. Find Left Canvas
                var leftCanvas = immersiveCamera.leftUICamera.GetComponent<UICamera>().canvas;

                // 3.2.2. Instantiate and setup Rotate Left and Right button.
                moveLeftButton = Instantiate(moveLeftButtonPrefab, leftCanvas.transform);
                moveRightButton = Instantiate(moveRightButtonPrefab, centerCanvas.transform);
            }

            // 3.3. Hand Right and Center Wall case.
            else
            {
                // 3.3.1. Find Right Canvas
                var rightCanvas = immersiveCamera.rightUICamera.GetComponent<UICamera>().canvas;

                // 3.3.2. Instantiate and setup Rotate Left and Right button.
                moveLeftButton = Instantiate(moveLeftButtonPrefab, centerCanvas.transform);
                moveRightButton = Instantiate(moveRightButtonPrefab, rightCanvas.transform);
            }
        }

        // 4. Pass references to instantiated buttons.
        moveLeftButton.GetComponent<MoveLeftTrigger>().immersiveCamera = immersiveCamera;
        moveLeftButton.GetComponent<MoveLeftTrigger>().rotationButtonsController = this;
        moveRightButton.GetComponent<MoveRightTrigger>().immersiveCamera = immersiveCamera;
        moveRightButton.GetComponent<MoveRightTrigger>().rotationButtonsController = this;

        // 5. Enable and disable buttons.
        EnableDisableRotationButtons();
    }


    //==============================================================
    // HANDLES CAMERA ROTATION
    // Enables and disables the correct buttons based on the camera positions.
    //==============================================================

    public void RotateCameraLeft()
    {
        if (immersiveCamera.WallCount == 1)
        {
            if (camPos == CameraPosition.Center) camPos = CameraPosition.Left;
            else if (camPos == CameraPosition.Right) camPos = CameraPosition.Center;
            else return;
        }
        if (immersiveCamera.WallCount == 2)
        {
            if (camPos == CameraPosition.Right) camPos = CameraPosition.Left;
            else return;
        }
    }

    public void RotateCameraRight()
    {
        if (immersiveCamera.WallCount == 1)
        {
            if (camPos == CameraPosition.Center) camPos = CameraPosition.Right;
            else if (camPos == CameraPosition.Left) camPos = CameraPosition.Center;
            else return;
        }
        if (immersiveCamera.WallCount == 2)
        {
            if (camPos == CameraPosition.Left) camPos = CameraPosition.Right;
            else return;
        }
    }


    // Enables and disables rotation buttons based on the camera position.
    public void EnableDisableRotationButtons()
    {
        switch (camPos)
        {
            case CameraPosition.Left:
                moveLeftButton.SetActive(false);
                moveRightButton.SetActive(true);
                break;
            case CameraPosition.Center:
                moveLeftButton.SetActive(true);
                moveRightButton.SetActive(true);
                break;
            case CameraPosition.Right:
                moveLeftButton.SetActive(true);
                moveRightButton.SetActive(false);
                break;
        }
    }

    //==============================================================
    // DATA STRUCTURES
    //==============================================================

    //The posisiton of the camera when rotation buttons are visible
    private enum CameraPosition
    {
        Left,
        Right,
        Center
    }

}
