/* Copyright (C) Immersive Interactive, Ltd - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Luke Bissell <luke@immersive.co.uk>, July 2019
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Fades a sprite out over a given amount of time. 
/// Used to provide functionality to fade out a title card.
/// </summary>
public class IntroFadeOut : MonoBehaviour
{
    [Tooltip("Length of time for which title should be displayed before it starts to fade.")]
    public float introTime = 3;
    [Tooltip("Length of time over which the title will fade away.")]
    public float fadeOutTime = 3;

    private float timeRemaining = 0;
    private SpriteRenderer sprite;

    // Start is called before the first frame update
    void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
        timeRemaining = fadeOutTime + introTime;
    }

    // Update is called once per frame
    void Update()
    {
        timeRemaining -= Time.deltaTime;

        if (timeRemaining <= 0) Destroy(gameObject);

        if (timeRemaining < fadeOutTime)
        {
            //Change Alpha
            var tmpColor = sprite.color;
            tmpColor.a = timeRemaining / fadeOutTime;
            sprite.color = tmpColor;

        }

    }
}
