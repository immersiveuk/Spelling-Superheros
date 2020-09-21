using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Com.Immersive.WipeToReveal
{
    /// <summary>
    /// This is an interface that when implemented by a MonoBehaviour Script in Unity, and that script is applied to a Wipe To Reveal object, the WipeComplete Method will be called when the Wipe is completed.
    /// </summary>
    public interface IOnWipeEventHandler
    {
        /// <summary>
        /// This method is called when the Wipe is complete and has reached the Wipe Percentage.
        /// </summary>
        void WipeComplete();

        /// <summary>
        /// This method is called every frame in which a Wipe is occuring.
        /// </summary>
        /// <param name="phase">The TouchPhase of the touch causing the Wipe.</param>
        /// <param name="position">The position in local space of the the touch. Values between 0 and 1.</param>
        /// <param name="currentPercentage">The current wipe percentage.</param>
        void WipeOccuring(TouchPhase phase, Vector2 position, float currentPercentage);
    }
}
