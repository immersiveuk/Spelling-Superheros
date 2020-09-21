/* Copyright (C) Immersive Interactive, Ltd - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Luke Bissell <luke@immersive.co.uk>, July 2019
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Causes a lights intensity to flickers.
/// </summary>
public class FlickerLight : MonoBehaviour
{
    //Settings
    public float intensityMin = 0.7f;
    public float intensityMax = 1f;

    public float flickeringRate = 1;

    //PRIVATE VARIABLES
    private Light flickeringLight;

    // Start is called before the first frame update
    void Start()
    {
        flickeringLight = GetComponent<Light>();
    }

    private float time = 0;

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime * flickeringRate;
        var noise = Mathf.PerlinNoise(time, 0);
        flickeringLight.intensity = intensityMin + (intensityMax - intensityMin) * noise;
    }
}
