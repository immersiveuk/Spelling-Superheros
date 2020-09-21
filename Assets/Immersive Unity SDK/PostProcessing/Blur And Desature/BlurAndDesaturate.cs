/* Copyright (C) Immersive Interactive, Ltd - All Rights Reserved
* Unauthorized copying of this file, via any medium is strictly prohibited
* Proprietary and confidential
* Written by Luke Bissell <luke@immersive.co.uk>, Sept 2019
*/

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Com.Immersive.Cameras.PostProcessing
{

    /// <summary>
    /// Controls the blurring and desaturating for an entire Immersive Camera system.
    /// </summary>
    public class BlurAndDesaturate : MonoBehaviour
    {
        public static BlurAndDesaturate CurrentBlurAndDesaturate;

        public bool active = false;
        private bool _active = false;

        public bool applyBlur = true;
        public bool applyDesaturate = true;

        public static readonly float MaxIntensity = 3;
        private float currentIntensity = 0;

        public Material postProcessMaterial;

        // References to Immersive Camera constants that will be used repeatedly.
        private List<Camera> cameras;
        private List<SurfaceInfo> walls;
        private Rect resolution;
        private List<ApplyBlurAndDesaturateToCamera> perCameraBlurScripts;

        private void Awake()
        {
            CurrentBlurAndDesaturate = this;
        }

        // Start is called before the first frame update
        void Start()
        {
            cameras = AbstractImmersiveCamera.CurrentImmersiveCamera.cameras;
            walls = AbstractImmersiveCamera.CurrentImmersiveCamera.walls;
            resolution = AbstractImmersiveCamera.CurrentImmersiveCamera.resolution;

            //Add post process to each camera
            perCameraBlurScripts = new List<ApplyBlurAndDesaturateToCamera>();
            foreach (Camera camera in cameras)
            {
                var blurScript = camera.gameObject.AddComponent<ApplyBlurAndDesaturateToCamera>() as ApplyBlurAndDesaturateToCamera;
                blurScript.SetMaterial(postProcessMaterial);
                perCameraBlurScripts.Add(blurScript);
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (active != _active)
            {
                _active = active;
                foreach (var blurScript in perCameraBlurScripts)
                {
                    blurScript.active = active;
                }
            }

            foreach (var blurScript in perCameraBlurScripts)
            {
                blurScript.applyBlur = applyBlur;
                blurScript.applyDesaturate = applyDesaturate;
            }
        }

        /// <summary>
        /// Sets the center point from which the blur and desaturate effect surround.
        /// </summary>
        /// <param name="pos">The position in surface space.</param>
        /// <param name="camIndex">The index of the rendering camera.</param>
        public void SetFocalPoint(Vector2 pos, int camIndex)
        {
            print("Set focal point: " + pos);
            var cam = cameras[camIndex];

            //Scale size to provided surface resolution
            var scale = walls[camIndex].rect.width / cam.pixelWidth;
            pos *= scale;

            //The xPos relative to the left most surface in pixels
            var absolutePosX = walls[camIndex].rect.x + pos.x;

            for (int i = 0; i < walls.Count; i++)
            {
                //Position in pixels relative to current camera
                var posRelativeToCamX = absolutePosX - walls[i].rect.x;

                //If in 360, 4 wall space calculate a wraparound position which allows for a continues blur with no seem between the left and back wall.
                if (walls.Count == 4)
                {
                    var wrapAroundRightPosX = -walls[i].rect.x - (resolution.width - absolutePosX);
                    var wrapAroundLeftPosX = (resolution.width - walls[i].rect.x) + absolutePosX;

                    float wrapPosX = posRelativeToCamX - resolution.width;
                    if (Mathf.Abs(wrapAroundLeftPosX) < Mathf.Abs(wrapAroundRightPosX))
                    {
                        wrapPosX = wrapAroundLeftPosX;
                    }
                    else
                    {
                        wrapPosX = wrapAroundRightPosX;
                    }

                    var wrapViewPortPos = new Vector2(wrapPosX / walls[i].rect.width, pos.y / walls[i].rect.height);
                    var viewPortPos = new Vector2(posRelativeToCamX / walls[i].rect.width, pos.y / walls[i].rect.height);

                    perCameraBlurScripts[i].SetFocalCenters(viewPortPos, wrapViewPortPos);
                }
                //Non surround space.
                else
                {
                    var viewPortPos = new Vector2(posRelativeToCamX / walls[i].rect.width, pos.y / walls[i].rect.height);
                    perCameraBlurScripts[i].SetFocalCenters(viewPortPos, viewPortPos);
                }
            }
        }

        /// <summary>
        /// Sets the intensity of the blur and desaturate filter.
        /// </summary>
        /// <param name="intensity">Recommended between 0 and 3.</param>
        public void SetIntensity(float intensity)
        {
            currentIntensity = intensity;
            foreach (var blurScript in perCameraBlurScripts)
            {
                blurScript.SetIntensity(intensity);
            }
        }

        /// <summary>
        /// Sets the radius around the center point, in which no blurring or desaturating occurs.
        /// </summary>
        /// <param name="radius">1 unit is equal to the height of the camera.</param>
        public void SetFocalRadius(float radius)
        {
            foreach (var blurScript in perCameraBlurScripts)
            {
                blurScript.SetFocalRadius(radius);
            }
        }

        /// <summary>
        /// This will increase the intensity from 0 to Max Intensity over the provided duration.
        /// </summary>
        /// <param name="duration">Time over which to increase intensity</param>
        public void IncreaseIntensityToMaxOverTime(float duration)
        {
            StartCoroutine(IncreaseIntensityToMax(duration));
        }

        private IEnumerator IncreaseIntensityToMax(float duration)
        {
            float increment = (MaxIntensity - currentIntensity);
            if (duration != 0)
            {
                increment = ((MaxIntensity - currentIntensity) * Time.fixedDeltaTime) / duration;
            }

            while (currentIntensity < MaxIntensity)
            {
                currentIntensity += increment;
                if (currentIntensity > MaxIntensity) currentIntensity = MaxIntensity;
                SetIntensity(currentIntensity);
                yield return new WaitForEndOfFrame();
            }
        }

        /// <summary>
        /// Removes the Blur or Desaturate effect over the duration provided.
        /// </summary>
        /// <param name="duration">Duration to remove effect.</param>
        public void RemoveEffectOverTime(float duration)
        {
            StartCoroutine(DecreaseIntensityToZero(duration));
        }

        private IEnumerator DecreaseIntensityToZero(float duration)
        {
            float increment = currentIntensity;
            if (duration != 0)
            {
                increment = (currentIntensity * Time.fixedDeltaTime) / duration;
            }

            while (currentIntensity > 0)
            {
                currentIntensity -= increment;
                if (currentIntensity < 0) currentIntensity = 0;
                SetIntensity(currentIntensity);
                yield return new WaitForEndOfFrame();
            }
        }

        /// <summary>
        /// This will set the focal point and radius to focus on Transform provided.
        /// </summary>
        /// <param name="target">Transform to focus on.</param>
        public void FocusOnTransform(Transform target)
        {
            //Find Rendering Camera
            Camera cam = AbstractImmersiveCamera.CurrentImmersiveCamera.FindCameraLookingAtPosition(target.position);
            if (cam == null) Debug.LogError("BlurAndDesaturate.FocusOnTransform(): Transform provided is not visible.");

            //Find Cameras index
            var camIndex = AbstractImmersiveCamera.CurrentImmersiveCamera.GetIndexOfCamera(cam);

            //Find Viewport position of transform
            var pos = cam.WorldToViewportPoint(target.position);
            pos.x *= cam.pixelWidth;
            pos.y *= cam.pixelHeight;

            //Get the size of object if it has a renderer.
            var rend = target.GetComponent<Renderer>();
            float focalRadius;
            if (rend)
            {
                var radius = rend.bounds.extents.magnitude;
                focalRadius = radius;
            }
            else
            {
                focalRadius = 0.3f;
            }

            //Set Focal Point and Radius
            SetFocalPoint(pos, camIndex);
            SetFocalRadius(focalRadius);
        }
    }
}