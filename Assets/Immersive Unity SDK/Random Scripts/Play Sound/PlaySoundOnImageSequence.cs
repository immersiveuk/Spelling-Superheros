/* Copyright (C) Immersive Interactive, Ltd - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Luke Bissell <luke@immersive.co.uk>, Nov 2019
 */

using Com.Immersive.Cameras;
using Com.Immersive.Hotspots;
using UnityEngine;
using UnityEngine.Serialization;

/// <summary>
/// The component allows you to add sounds which will play when different images in the image sequence are displayed.
/// If you don't want a sound to be displayed for a given image, leave that element empty.
/// </summary>
public class PlaySoundOnImageSequence : MonoBehaviour, ISequencePopUpIndexChangeHandler
{
    public AudioClip[] clips;
    public float volume = 1;

    [FormerlySerializedAs("playAudioAsOneShot")]
    [Tooltip("If true audio will play as a one shot audio source and will not stop any other audio from playing.")]
    public bool allowOverlappingAudio = false;

    public void IndexChanged(int newIndex)
    {
        if (newIndex < clips.Length)
        {
            var clip = clips[newIndex];
            if (clip != null)
            {
                PlayAudio(clip);
            }
        }
    }

    private void PlayAudio(AudioClip clip)
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
}
