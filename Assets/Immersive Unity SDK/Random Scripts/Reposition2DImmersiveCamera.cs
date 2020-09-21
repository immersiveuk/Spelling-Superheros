/* Copyright (C) Immersive Interactive, Ltd - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Luke Bissell <luke@immersive.co.uk>, Dec 2019
 */

using Com.Immersive.Cameras;
using Com.Immersive.Hotspots;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Com.Immersive.Cameras.ImmersiveCamera2D;

/// <summary>
/// Script which repositions the current 2D Immersive Camera.
/// It can also change the CAG type.
/// </summary>
public class Reposition2DImmersiveCamera : MonoBehaviour, IInteractableObject
{
    public enum MoveTrigger { MoveAtStart, MoveOnTouched, HotspotTouched, APIOnly };
    public MoveTrigger moveTrigger = MoveTrigger.APIOnly;

    public CAGType cagType;
    public Vector3 translation;

    public void Move()
    {
        if (AbstractImmersiveCamera.CurrentImmersiveCamera is ImmersiveCamera2D)
        {
            ImmersiveCamera2D immersiveCam = AbstractImmersiveCamera.CurrentImmersiveCamera as ImmersiveCamera2D;
            immersiveCam.Translate(translation, cagType);
        }
        else
        {
            Debug.LogError("No 2D Immersive Camera in Scene.");
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        if (moveTrigger == MoveTrigger.MoveAtStart) Move();
    }

    public void OnRelease()
    {
        if (moveTrigger == MoveTrigger.MoveOnTouched) Move();
        if (moveTrigger == MoveTrigger.HotspotTouched)
        {
            var hotspot = GetComponent<HotspotScript>();
            if (hotspot.IsInteractable) Move();
        }
    }


    public void OnPress() { }
    public void OnTouchEnter() { }
    public void OnTouchExit() { }
}
