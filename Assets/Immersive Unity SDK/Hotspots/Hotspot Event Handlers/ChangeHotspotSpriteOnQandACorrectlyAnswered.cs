/* Copyright (C) Immersive Interactive, Ltd - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Luke Bissell <luke@immersive.co.uk>, Dec 2019
 */

 using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeHotspotSpriteOnQandACorrectlyAnswered : MonoBehaviour, IQuestionAnsweredHandler
{
    public Sprite newSprite;

    private SpriteRenderer spriteRenderer;

    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null) Debug.LogError("No SpriteRenderer attached to object "+gameObject.name+".");
    }

    public void QuestionAnswered(bool isAnswerCorrect)
    {
        if (isAnswerCorrect)
        {
            spriteRenderer.sprite = newSprite;
        }
    }

}
