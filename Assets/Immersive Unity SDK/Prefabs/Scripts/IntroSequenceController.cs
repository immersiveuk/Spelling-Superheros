/* Copyright (C) Immersive Interactive, Ltd - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Luke Bissell <luke@immersive.co.uk>, July 2019
 */

 using Com.Immersive.Cameras;
using Com.Immersive.Hotspots;
using UnityEngine;

/// <summary>
/// Provides the functionality of an intro sequence which gives instructions for the scene ahead.
/// </summary>
public class IntroSequenceController : MonoBehaviour, IInteractableObject
{
    // Start is called before the first frame update
    void Start()
    {
        HotspotController.DisableHotspotForAllControllers();
    }

    public void OnRelease()
    {
        HotspotController.EnableHotspotsForAllControllers();

        if (transform.parent != null)
        {
            Destroy(transform.parent.gameObject);
        }
    }

    public void OnPress()  { }

    public void OnTouchEnter() { }

    public void OnTouchExit() { }
}
