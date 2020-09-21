/* Copyright (C) Immersive Interactive, Ltd - All Rights Reserved
* Unauthorized copying of this file, via any medium is strictly prohibited
* Proprietary and confidential
* Written by Luke Bissell <luke@immersive.co.uk>, Nov 2019
*/

using Com.Immersive.Cameras;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class PlaySoundOnTouch : MonoBehaviour, IInteractableObject
{
    public AudioClip clip;
    [Range(0, 1)]
    public float volume = 1;

    [FormerlySerializedAs("playAudioAsOneShot")]
    [Tooltip("If true audio will play as a one shot audio source and will not stop any other audio.")]
    public bool allowOverlappingAudio = false;

    public TouchType touchType;

    public void OnRelease()
    {
        if (touchType == TouchType.Released) PlayAudio();
    }

    public void OnPress()
    {
        if (touchType == TouchType.Pressed) PlayAudio();
    }

    public void OnTouchEnter()
    {
        if (touchType == TouchType.Entered) PlayAudio();
    }

    public void OnTouchExit()
    {
        if (touchType == TouchType.Exited) PlayAudio();
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
