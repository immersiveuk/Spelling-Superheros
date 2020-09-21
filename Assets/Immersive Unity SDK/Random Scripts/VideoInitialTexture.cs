/* Copyright (C) Immersive Interactive, Ltd - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Luke Bissell <luke@immersive.co.uk>, Aug 2019
 */
 
 using UnityEngine;

/// <summary>
/// Places a texture to an object before the video is played.
/// Use the first frame of the video to achieve seemless playback.
/// </summary>
public class VideoInitialTexture : MonoBehaviour
{
    public Texture initialTexture;

    private void Awake()
    {
        GetComponent<Renderer>().material.SetTexture("_MainTex", initialTexture);
    }
}
