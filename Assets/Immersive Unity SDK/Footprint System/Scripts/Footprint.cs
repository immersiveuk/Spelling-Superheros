/* Copyright (C) Immersive Interactive, Ltd - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Luke Bissell <luke@immersive.co.uk>, Dec 2019
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Fades footprints as part of the footprint system.
/// </summary>
public class Footprint : MonoBehaviour
{

    public float fadeDuration = 5;
    public Foot foot;
    public Vector2 position;
    public enum Foot { Left, Right };


    private float timeRemaining;
    private SpriteRenderer spriteRend;

    // Start is called before the first frame update
    void Start()
    {
        timeRemaining = fadeDuration;
        if (spriteRend == null ) spriteRend = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        timeRemaining -= Time.deltaTime;

        if (timeRemaining < 0)
        {
            Destroy(gameObject);
            return;
        }

        var alpha = (timeRemaining / fadeDuration);
        var color = spriteRend.color;
        color.a = alpha;
        spriteRend.color = color;
    }

    public void Setup(Vector2 position, Foot foot, float fadeDuration, Sprite sprite, float size)
    {
        //Set position
        this.position = position;

        //Set Foot
        this.foot = foot;

        //Set fade duration
        this.fadeDuration = fadeDuration;

        //Set Sprite
        if (spriteRend == null) spriteRend = GetComponent<SpriteRenderer>();
        spriteRend.sprite = sprite;

        //Set Size
        transform.localScale = new Vector3(size, size, size);
    }
}
