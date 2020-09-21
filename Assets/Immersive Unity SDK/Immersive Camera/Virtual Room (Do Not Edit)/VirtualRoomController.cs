/* Copyright (C) Immersive Interactive, Ltd - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Luke Bissell <luke@immersive.co.uk>, July 2019
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;

namespace Com.Immersive.Cameras
{
    public class VirtualRoomController : MonoBehaviour
    {
        //Used to save camera position between scenes.
        public static bool isFirstScene = true;
        public static Vector3 camPosition;
        public static Quaternion camRotation;

        //The height of the walls in meters
        public float wallHeight = 3f;
        public Camera cam;
        public Light roomLight;
        public float rotateSpeed = 3;

        public Material screenMaterial;

        private Transform screens;
        private Transform walls;

        private void Awake()
        {
            screens = transform.GetChild(0);
            walls = transform.GetChild(1);

            SetLayer(gameObject);
            cam.cullingMask = 1 << CreateVirtualRoomLayer.virtualRoomLayer;
            roomLight.cullingMask = 1 << CreateVirtualRoomLayer.virtualRoomLayer;

        }

        private void SetLayer(GameObject go)
        {
            go.layer = CreateVirtualRoomLayer.virtualRoomLayer;
            for (int i = 0; i < go.transform.childCount; i++)
            {
                SetLayer(go.transform.GetChild(i).gameObject);
            }
        }

        public void LayoutSurfaces(List<SurfaceInfo> surfacesInfo, List<RenderTexture> renderTextures)
        {

            // 1. Calculate the width of the central screen and maximum width of a side wall.
            float centerWidth = 0;
            float sideWidth = 0;

            foreach (SurfaceInfo surfaceInfo in surfacesInfo)
            {
                switch (surfaceInfo.position)
                {
                    case SurfacePosition.Center:
                    case SurfacePosition.Back:
                        var newCenterWidth = surfaceInfo.aspectRatio * wallHeight;
                        centerWidth = Mathf.Max(centerWidth, newCenterWidth);
                        break;
                    case SurfacePosition.Left:
                    case SurfacePosition.Right:
                        var newSideWidth = surfaceInfo.aspectRatio * wallHeight;
                        sideWidth = Mathf.Max(sideWidth, newSideWidth);
                        break;
                }
            }

            if (sideWidth == 0) sideWidth = centerWidth;
            if (centerWidth == 0) centerWidth = sideWidth;

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
                        var width = surfaceInfo.aspectRatio * wallHeight;
                        left.localPosition = new Vector3(-centerWidth / 2, 0, (sideWidth - width) / 2);
                        left.localScale = new Vector3(width, wallHeight, 1);
                        SetScreenTexture(renderTexture, left);
                        break;
                    case SurfacePosition.Center:
                        var center = screens.GetChild(1);
                        center.localPosition = new Vector3(0, 0, sideWidth / 2f);
                        center.localScale = new Vector3(centerWidth, wallHeight, 1);
                        SetScreenTexture(renderTexture, center);
                        break;
                    case SurfacePosition.Right:
                        var right = screens.GetChild(2);
                        width = surfaceInfo.aspectRatio * wallHeight;
                        right.localPosition = new Vector3(centerWidth / 2, 0, (sideWidth - width) / 2);
                        right.localScale = new Vector3(width, wallHeight, 1);
                        SetScreenTexture(renderTexture, right);
                        break;
                    case SurfacePosition.Back:
                        var back = screens.GetChild(3);
                        width = surfaceInfo.aspectRatio * wallHeight;
                        back.localPosition = new Vector3(0, 0, -sideWidth / 2);
                        back.localScale = new Vector3(width, wallHeight, 1);
                        SetScreenTexture(renderTexture, back);
                        break;
                    case SurfacePosition.Floor:
                        var floor = screens.GetChild(4);
                        var height = centerWidth / surfaceInfo.aspectRatio;
                        floor.localPosition = new Vector3(0, -wallHeight / 2, (sideWidth - height) / 2);
                        floor.localScale = new Vector3(centerWidth, height, 1);
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

            //Resize Walls
            var leftWall = walls.GetChild(0);
            leftWall.localPosition = new Vector3(-centerWidth / 2, 0, 0);
            leftWall.localScale = new Vector3(sideWidth, wallHeight, 1);

            var centerWall = walls.GetChild(1);
            centerWall.localPosition = new Vector3(0, 0, sideWidth/2);
            centerWall.localScale = new Vector3(centerWidth, wallHeight, 1);

            var rightWall = walls.GetChild(2);
            rightWall.localPosition = new Vector3(centerWidth / 2, 0, 0);
            rightWall.localScale = new Vector3(sideWidth, wallHeight, 1);

            var backWall = walls.GetChild(3);
            backWall.localPosition = new Vector3(0, 0, -sideWidth / 2);
            backWall.localScale = new Vector3(centerWidth, wallHeight, 1);

            var floorSurface = walls.GetChild(4);
            floorSurface.localPosition = new Vector3(0, -wallHeight / 2, 0);
            floorSurface.localScale = new Vector3(centerWidth, sideWidth, 1);

            var ceilingSurface = walls.GetChild(5);
            ceilingSurface.localPosition = new Vector3(0, wallHeight / 2, 0);
            ceilingSurface.localScale = new Vector3(centerWidth, sideWidth, 1);

            //Position Camera
            if (isFirstScene)
            {
                isFirstScene = false;
                cam.transform.localPosition = new Vector3(0, -wallHeight / 2 + 1, 0);
            }
            else {
                cam.transform.position = camPosition;
                cam.transform.rotation = camRotation;
            }


            //Move light just above the room
            roomLight.transform.localPosition = new Vector3(0, wallHeight / 2 + 0.01f, 0);
            roomLight.range = centerWidth*20f;
        }

        private void SetScreenTexture(RenderTexture texture, Transform screen)
        {
            var material = new Material(screenMaterial);
            material.SetTexture("_MainTex", texture);

            screen.GetComponent<Renderer>().material = material;
        }

        // Update is called once per frame
        void Update()
        {

            //ROTATE
            if (Input.GetMouseButtonDown(1))
            {
                Cursor.lockState = CursorLockMode.Locked;
                HideCursor.EnforceHide = true;
            }
            else if (Input.GetMouseButton(1))
            {
                cam.transform.Rotate(new Vector3(0, Input.GetAxis("Mouse X") * rotateSpeed), Space.World);
            }
            else if (Input.GetMouseButtonUp(1))
            {
                Cursor.lockState = CursorLockMode.None;
                HideCursor.EnforceHide = false;
            }

            //Right
            if (Input.GetKey(KeyCode.D))
            {
                cam.transform.Translate(new Vector3(0.03f, 0, 0));
            }
            //Left
            if (Input.GetKey(KeyCode.A))
            {
                cam.transform.Translate(new Vector3(-0.03f, 0, 0));
            }
            //Forwards
            if (Input.GetKey(KeyCode.W))
            {
                cam.transform.Translate(new Vector3(0, 0, 0.03f));
            }
            //Backwards
            if (Input.GetKey(KeyCode.S))
            {
                cam.transform.Translate(new Vector3(0, 0, -0.03f));
            }

            camPosition = cam.transform.position;
            camRotation = cam.transform.rotation;
        }
    }
}
