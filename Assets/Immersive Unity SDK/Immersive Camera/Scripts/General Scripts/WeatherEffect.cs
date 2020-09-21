/* Copyright (C) Immersive Interactive, Ltd - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Luke Bissell <luke@immersive.co.uk>, July 2019
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Com.Immersive.Cameras
{
    /// <summary>
    /// Adds a provided weather effect to the associated Immersive Camera.
    /// </summary>
    public class WeatherEffect : MonoBehaviour
    {
        public GameObject effectPrefab;
        public AudioSource effectSound;

        public bool effectOnOff = false;
        private bool _effectOnOff = false;

        private AbstractImmersiveCamera immersiveCamera;

        // Start is called before the first frame update
        void Start()
        {
            if (!effectPrefab)
            {
                Debug.LogError("AddWeatherEffect: Weather Effect Prefab has not been set.");
                return;
            }

            immersiveCamera = GetComponent<AbstractImmersiveCamera>();
            if (!immersiveCamera)
            {
                Debug.LogError("AddWeatherEffect: No ImmersiveCamera attached to GameObject.");
                return;
            }

            immersiveCamera.SetupWeatherEffects(effectPrefab);
        }

        // Update is called once per frame
        void Update()
        {

            // Turn Effect on and off.
            if (effectOnOff != _effectOnOff)
            {
                _effectOnOff = effectOnOff;
                if (immersiveCamera)
                {
                    if (effectOnOff) immersiveCamera.TurnOnWeatherEffect();
                    else immersiveCamera.TurnOffWeatherEffect();
                }

                if (effectSound)
                {
                    if (effectOnOff) effectSound.Play();
                    else effectSound.Pause();
                }
            }

        }
    }
}
