using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class ActivateAndDisableOnAudioComplete : AbstractActivateAndDisable
{
    private AudioSource audioSource;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.Play();

        DisableActivateObjectsOnStart();
    }

    // Update is called once per frame
    void Update()
    {
        if (!audioSource.isPlaying) ActivateAndDisable();
    }
}