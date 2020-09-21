using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class DelayAudioPlayback : MonoBehaviour
{
    public float delay = 1;

    private AudioSource audioSource;

    private float startAudioTime;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.Stop();
        startAudioTime = Time.time + delay;
    }

    // Update is called once per frame
    void Update()
    {
        if (startAudioTime > 0 && Time.time >= startAudioTime)
        {
            audioSource.Play();
            startAudioTime = -1;
        }
    }
}
