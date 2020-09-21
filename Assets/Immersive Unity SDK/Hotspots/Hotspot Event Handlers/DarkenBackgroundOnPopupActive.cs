/* Copyright (C) Immersive Interactive, Ltd - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Luke Bissell <luke@immersive.co.uk>, Dec 2019
 */

using Com.Immersive.Cameras;
using Com.Immersive.Cameras.PostProcessing;
using Com.Immersive.Hotspots;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// When this component is added to to a Hotspot, the background (behind the popup) will be darkened, while the popup is active.
/// </summary>
public class DarkenBackgroundOnPopupActive : MonoBehaviour, IInteractableObject, IHotspotActionCompleteHandler
{
    [Range(0,1)]
    public float intensity = 0.7f;

    private HotspotScript hotspot;

    public void HotspotActionComplete()
    {
        DarkenBackground.CurrentDarkenBackground.TurnOff();
    }

    public void OnRelease()
    {
        if (hotspot.IsInteractable) DarkenBackground.CurrentDarkenBackground.TurnOn(intensity);
    }


    // Start is called before the first frame update
    void Start()
    {
        hotspot = GetComponent<HotspotScript>();
        if (hotspot == null)
        {
            Debug.LogError("No Hotspot Component Found.");
            Destroy(this);
            return;
        }
    }

    public void OnPress() { }
    public void OnTouchEnter() { }
    public void OnTouchExit() { }
}
