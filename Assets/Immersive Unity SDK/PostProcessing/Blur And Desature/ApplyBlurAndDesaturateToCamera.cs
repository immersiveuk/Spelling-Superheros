/* Copyright (C) Immersive Interactive, Ltd - All Rights Reserved
* Unauthorized copying of this file, via any medium is strictly prohibited
* Proprietary and confidential
* Written by Luke Bissell <luke@immersive.co.uk>, Sept 2019
*/

using System;
using UnityEngine;

namespace Com.Immersive.Cameras.PostProcessing
{
    /// <summary>
    /// Apply a blur and desaturate filter to a camera.
    /// This is automatically will be done automatically for all Immersive Cameras.
    /// </summary>
    public class ApplyBlurAndDesaturateToCamera : MonoBehaviour
    {
        [NonSerialized]
        public bool active = false;

        [NonSerialized]
        public bool applyBlur = true;
        [NonSerialized]
        public bool applyDesaturate = true;

        private Material postProcessMaterial;

        //method which is automatically called by unity after the camera is done rendering
        void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            if (active && postProcessMaterial != null && (applyBlur || applyDesaturate))
            {
                var temporaryTexture = RenderTexture.GetTemporary(source.width, source.height);
                if (applyBlur && applyDesaturate)
                {
                    Graphics.Blit(source, temporaryTexture, postProcessMaterial, 0);
                    Graphics.Blit(temporaryTexture, destination, postProcessMaterial, 1);
                }
                else if (applyBlur) Graphics.Blit(source, destination, postProcessMaterial, 0);
                else if (applyDesaturate) Graphics.Blit(source, destination, postProcessMaterial, 1);
                RenderTexture.ReleaseTemporary(temporaryTexture);
            }
            else
            {
                Graphics.Blit(source, destination);
            }
        }

        //Set the Material to apply to screen.
        public void SetMaterial(Material postProcessMaterial)
        {
            this.postProcessMaterial = Instantiate(postProcessMaterial);
        }

        /// <summary>
        /// Sets the focal center points from which the blur and desaturate effect surround.
        /// If using Immersive Camera, call BlurAndDesaturate.SetFocalPoint instead as it will apply to all cameras.
        /// </summary>
        /// <param name="focalCenter1">Focal center 1 in Viewport space.</param>
        /// <param name="focalCenter1">Focal center 2 in Viewport space.</param>
        public void SetFocalCenters(Vector2 focalCenter1, Vector2 focalCenter2)
        {
            postProcessMaterial.SetFloat("_FocusPoint1X", focalCenter1.x);
            postProcessMaterial.SetFloat("_FocusPoint1Y", focalCenter1.y);
            postProcessMaterial.SetFloat("_FocusPoint2X", focalCenter2.x);
            postProcessMaterial.SetFloat("_FocusPoint2Y", focalCenter2.y);
        }


        /// <summary>
        /// Sets the intensity of the blur and desaturate filter.
        /// If using Immersive Camera, call BlurAndDesaturate.SetFocalPoint instead as it will apply to all cameras.
        /// </summary>
        /// <param name="intensity">Should be moved between 0(Off) and 3(Max).</param>
        public void SetIntensity(float intensity)
        {
            postProcessMaterial.SetFloat("_Intensity", intensity);
        }

        /// <summary>
        /// Sets the radius around the center point, in which no blurring or desaturating occurs.
        /// If using Immersive Camera, call BlurAndDesaturate.SetFocalPoint instead as it will apply to all cameras.
        /// </summary>
        /// <param name="radius">1 unit is equal to the height of the camera.</param>
        public void SetFocalRadius(float radius)
        {
            postProcessMaterial.SetFloat("_FocusRadius", radius);
        }
    }
}
