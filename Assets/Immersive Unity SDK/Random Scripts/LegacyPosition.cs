/* Copyright (C) Immersive Interactive, Ltd - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Luke Bissell <luke@immersive.co.uk>, July 2019
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Takes in the X and Y pixel positions from the top left of the background and moves the object
/// to the correct position on screen.
/// </summary>
[ExecuteInEditMode]
public class LegacyPosition : MonoBehaviour
{
    public int X;
    public int Y;

    private SpriteRenderer spriteRenderer;

    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        if (spriteRenderer != null)
        {
            var spriteWidth = spriteRenderer.sprite.rect.width * transform.localScale.x;
            var spriteHeight = spriteRenderer.sprite.rect.height * transform.localScale.y;

            var xOffset = X - 5760 + spriteWidth / 2;
            var yOffset = Y - 540 + spriteHeight / 2;

            transform.localPosition = new Vector3(xOffset / 1080f, -(yOffset / 1080f), transform.localPosition.z);
        }

    }
}
