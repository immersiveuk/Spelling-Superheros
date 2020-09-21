using Com.Immersive.Cameras;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class PlaySoundOnButtonClick : MonoBehaviour
{
    public AudioClip clip;
    [Range(0, 1)]
    public float volume = 1;

    [FormerlySerializedAs("playAudioAsOneShot")]
    [Tooltip("If true audio will play as a one shot audio source and will not stop any other audio.")]
    public bool allowOverlappingAudio = false;

    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Button>()?.onClick.AddListener(PlayAudio);
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
}
