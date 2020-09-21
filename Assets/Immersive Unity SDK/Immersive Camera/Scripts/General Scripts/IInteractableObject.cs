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
    public interface IInteractableObject
    {
        /// <summary>
        /// Handles what the object should do when touch is released.
        /// </summary>
        void OnRelease();

        /// <summary>
        /// Handles what the object should do when pressed but not yet released.
        /// </summary>
        /// <returns>Continue passing touch information to object.</returns>
        void OnPress();

        void OnTouchEnter();

        void OnTouchExit();
    }
}
