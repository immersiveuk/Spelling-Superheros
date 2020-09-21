using Com.Immersive.WipeToReveal;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Plays sound effects when the user wipes a WipeToReveal.
/// </summary>
public class PlaySoundsOnWipe : MonoBehaviour, IOnWipeEventHandler
{

    public AudioClip[] clips;
    public int previousWipeAverageNumber = 5;

    private AudioSource audioSource;
    private bool frameHasBeenWiped = false;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        previousWipeDeltas = new float[previousWipeAverageNumber];
    }

    private float prevFrameWipePercentage;
    private float[] previousWipeDeltas;
    private float wipeDeltasFloatingAverage;

    public void WipeOccuring(TouchPhase phase, Vector2 position, float currentPercentage)
    {
        //1. Note that frame has been wiped
        frameHasBeenWiped = true;

        //2. Calculate how much has been wiped.
        var wipeDelta = currentPercentage - prevFrameWipePercentage;
        prevFrameWipePercentage = currentPercentage;

        //3. Calculate the floating average of the wipeDeltas
        UpdateFloatingAverage(wipeDelta);

        //4. Set Volume
        audioSource.volume = wipeDeltasFloatingAverage;

        //5. Choose a clip and play
        if (!audioSource.isPlaying && phase != TouchPhase.Ended)
        {
            audioSource.clip = clips[Random.Range(0, clips.Length)];
            audioSource.Play();
        }
    }

    /// <summary>
    /// Calculates the floating average of the for the wipe deltas.
    /// </summary>
    /// <param name="newValue"></param>
    private void UpdateFloatingAverage(float newValue)
    {
        float count = 0;
        for (int i = 1; i < previousWipeDeltas.Length; i++)
        {
            count += previousWipeDeltas[i];
            previousWipeDeltas[i - 1] = previousWipeDeltas[i];
        }

        count += newValue;
        previousWipeDeltas[previousWipeDeltas.Length - 1] = newValue;

        wipeDeltasFloatingAverage = count / previousWipeDeltas.Length;
    }


    private void LateUpdate()
    {
        if (frameHasBeenWiped == false)
        {
            UpdateFloatingAverage(0);
            audioSource.volume = wipeDeltasFloatingAverage;
        }
        frameHasBeenWiped = false;
    }

    public void WipeComplete() { }

}
