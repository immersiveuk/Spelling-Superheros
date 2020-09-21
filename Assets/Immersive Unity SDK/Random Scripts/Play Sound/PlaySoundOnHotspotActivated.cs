/* Copyright (C) Immersive Interactive, Ltd - All Rights Reserved
* Unauthorized copying of this file, via any medium is strictly prohibited
* Proprietary and confidential
* Written by Luke Bissell <luke@immersive.co.uk>, Nov 2019
*/

using Com.Immersive.Cameras;
using Com.Immersive.Hotspots;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class PlaySoundOnHotspotActivated : MonoBehaviour, IInteractableObject
{
    public AudioClip clip;
    [Range(0, 1)]
    public float volume = 1;

    [FormerlySerializedAs("playAudioAsOneShot")]
    [Tooltip("If true audio will play as a one shot audio source and will not stop any other audio.")]
    public bool allowOverlappingAudio = false;

    public TouchType touchType;

    private IHotspot hotspot;
    private void Start()
    {
        hotspot = GetComponent<IHotspot>();
        if (hotspot == null) Debug.LogError("No Hotspot Component attached to PlaySoundOnHotspotActivate Object " + name + ".");
    }

    public void OnRelease()
    {
        if (touchType == TouchType.Released && hotspot.IsInteractable) PlayAudio();
    }

    public void OnPress()
    {
        if (touchType == TouchType.Pressed && hotspot.IsInteractable) PlayAudio();
    }

    public void OnTouchEnter()
    {
        if (touchType == TouchType.Entered && hotspot.IsInteractable) PlayAudio();
    }

    public void OnTouchExit()
    {
        if (touchType == TouchType.Exited && hotspot.IsInteractable) PlayAudio();
    }

    private void PlayAudio()
    {
        if (!allowOverlappingAudio)
        {
            AbstractImmersiveCamera.PlayAudio(clip, volume);
        }
        else
        {
            AudioSource.PlayClipAtPoint(clip, AbstractImmersiveCamera.CurrentImmersiveCamera.transform.position, volume);
        }
    }

    public enum TouchType { Released, Pressed, Entered, Exited}
}
