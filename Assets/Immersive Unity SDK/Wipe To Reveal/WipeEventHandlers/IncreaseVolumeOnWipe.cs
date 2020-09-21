using Com.Immersive.WipeToReveal;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Increases the volume of an AudioSource from a minimum to maximum volume as a Wipe To Reveal is wiped.
/// </summary>
[RequireComponent(typeof(WipeManager))]
public class IncreaseVolumeOnWipe : MonoBehaviour, IOnWipeEventHandler
{
    [Header("Settings")]
    [Range(0, 1)][SerializeField] float minVolume = 0;
    [Range(0, 1)][SerializeField] float maxVolume = 1;


    [Header("Referenced Objects")]
    [SerializeField] AudioSource audioSource = null;

    private float wipePercentage;

    private void OnValidate()
    {
        if (minVolume > maxVolume)
            minVolume = maxVolume;
    }

    void Start()
    {
        if (minVolume == 0)
            audioSource.Pause();
        audioSource.volume = minVolume;
        wipePercentage = GetComponent<WipeManager>().wipeSettings.wipePercentage;
    }

    public void WipeComplete()
    {
        audioSource.volume = maxVolume;
    }

    public void WipeOccuring(TouchPhase phase, Vector2 position, float currentPercentage)
    {
        if (!audioSource.isPlaying)
            audioSource.Play();
        float t = Mathf.InverseLerp(0, wipePercentage, currentPercentage);
        var volume = Mathf.Lerp(minVolume, maxVolume,  t);
        audioSource.volume = volume;
    }
}
