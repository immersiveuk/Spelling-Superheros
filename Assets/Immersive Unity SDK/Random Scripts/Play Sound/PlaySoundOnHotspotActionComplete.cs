using Com.Immersive.Cameras;
using Com.Immersive.Hotspots;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class PlaySoundOnHotspotActionComplete : MonoBehaviour, IHotspotActionCompleteHandler
{
    public AudioClip clip;
    [Range(0, 1)]
    public float volume = 1;

    [FormerlySerializedAs("playAudioAsOneShot")]
    [Tooltip("If true audio will play as a one shot audio source and will not stop any other audio from playing.")]
    public bool allowOverlappingAudio = true;

    public void HotspotActionComplete()
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
