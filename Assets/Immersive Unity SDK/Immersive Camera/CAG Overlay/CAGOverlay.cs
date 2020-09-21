/* Copyright (C) Immersive Interactive, Ltd - All Rights Reserved
* Unauthorized copying of this file, via any medium is strictly prohibited
* Proprietary and confidential
* Written by Luke Bissell <luke@immersive.co.uk>, July 2019
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CAGOverlay : MonoBehaviour
{
    public Sprite cagISprite;
    public Sprite cagESprite;

    private SpriteRenderer spriteRenderer;

    private readonly float cagSpriteAspect = 1.6f;

    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();    
    }

    public void DisplayCAGI(float aspectRatio)
    {
        spriteRenderer.sprite = cagISprite;
        transform.localScale = new Vector3(aspectRatio / cagSpriteAspect, 1, 1);
    }

    public void DisplayCAGE(float aspectRatio)
    {
        spriteRenderer.sprite = cagESprite;
        transform.localScale = new Vector3(aspectRatio / cagSpriteAspect, 1, 1);
    }

    public void Hide()
    {
        spriteRenderer.sprite = null;
    }
}
