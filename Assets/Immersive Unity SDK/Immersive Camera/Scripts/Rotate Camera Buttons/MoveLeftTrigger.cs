/* Copyright (C) Immersive Interactive, Ltd - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Luke Bissell <luke@immersive.co.uk>, July 2019
 */

using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Com.Immersive.Cameras
{
    public class MoveLeftTrigger : EventTrigger
    {
        [HideInInspector]
        public AbstractImmersiveCamera immersiveCamera;
        [HideInInspector]
        public RotationButtonController rotationButtonsController;

        private const float disabledDuration = 0.2f;
        private bool IsEnabled => Time.time > disabledUntilTime;
        private float disabledUntilTime = 0;

        public override void OnPointerClick(PointerEventData eventData)
        {
          
            if (!IsEnabled)
                return;

            DisableTemporarily();

            if (immersiveCamera != null)
            {
                immersiveCamera.RotateCameraLeft();
            }
            if (rotationButtonsController != null)
            {
                rotationButtonsController.RotateCameraLeft();
                rotationButtonsController.EnableDisableRotationButtons();
            }
        }

        private void DisableTemporarily()
        {
            disabledUntilTime = Time.time + disabledDuration;
        }
    }
}