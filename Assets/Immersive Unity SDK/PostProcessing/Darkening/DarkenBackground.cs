/* Copyright (C) Immersive Interactive, Ltd - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Luke Bissell <luke@immersive.co.uk>, Nov 2019
 */

using Com.Immersive.Cameras;
using Com.Immersive.Cameras.PostProcessing;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Com.Immersive.Cameras.PostProcessing
{
    public class DarkenBackground : MonoBehaviour
    {
        public static DarkenBackground CurrentDarkenBackground;


        public bool active = false;
        private bool _active = false;

        private float intensity = 0;

        private List<ApplyFadeInOutMaterial> darkenScripts;

        public Material darkenMaterials;

        private void Start()
        {
            CurrentDarkenBackground = this;
            AddEffectToEachCamera();
        }

        private void AddEffectToEachCamera()
        {
            //Add fade out script to each camera.
            //Use UI camera so its the top camera that fades out.
            var cameras = AbstractImmersiveCamera.CurrentImmersiveCamera.cameras;
            darkenScripts = new List<ApplyFadeInOutMaterial>();
            foreach (Camera cam in cameras)
            {
                var fadeScript = cam.gameObject.AddComponent<ApplyFadeInOutMaterial>() as ApplyFadeInOutMaterial;
                fadeScript.SetMaterial(darkenMaterials);
                darkenScripts.Add(fadeScript);
            }
        }

        public void TurnOn(float intensity)
        {
            this.intensity = intensity;
            active = true;
            foreach (var darkScript in darkenScripts)
            {
                darkScript.SetFadeLevel(intensity);
            }
        }

        public void TurnOff()
        {
            active = false;
        }


        private void Update()
        {
            //Activate and Deactivate
            if (active != _active)
            {
                _active = active;
                foreach (var fadeScript in darkenScripts)
                {
                    fadeScript.active = active;
                }
            }
        }
    }
}