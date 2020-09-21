using Com.Immersive.Cameras.PostProcessing;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DarkenBackgroundOnStart : MonoBehaviour
{
    [Range(0, 1)]
    public float intensity = 0.7f;

    public void Start()
    {
        DarkenBackground.CurrentDarkenBackground.TurnOn(intensity);
    }
}
