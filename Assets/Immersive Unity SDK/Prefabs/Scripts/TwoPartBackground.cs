/* Copyright (C) Immersive Interactive, Ltd - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Luke Bissell <luke@immersive.co.uk>, July 2019
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Automatically positions a background that has been split into two parts.
/// </summary>
[ExecuteInEditMode]
public class TwoPartBackground : MonoBehaviour
{
    private Sprite _left = null;
    private Sprite _right = null;
    public Sprite backgroundLeft;
    public Sprite backgroundRight;

    private SpriteRenderer leftRenderer;
    private SpriteRenderer rightRenderer;


    private void Start()
    {
        leftRenderer = transform.GetChild(0).GetComponent<SpriteRenderer>();
        rightRenderer = transform.GetChild(1).GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        if (_left != backgroundLeft)
        {
            _left = backgroundLeft;
            PositionLeftBackground();
        }
        else if (_right != backgroundRight)
        {
            _right = backgroundRight;
            PositionRightBackground();
        }
    }

    private void PositionLeftBackground()
    {
        if (leftRenderer == null)
        {
            Debug.LogError("Left Sprite Renderer is missing.");
            return;
        }

        if (backgroundLeft)
        {
            //Position Sprite
            var leftOffset = -(backgroundLeft.rect.width / backgroundLeft.pixelsPerUnit) / 2;
            leftRenderer.transform.localPosition = new Vector3(leftOffset, 0, 0);
        }
        //Set sprite
        leftRenderer.sprite = backgroundLeft;
    }

    private void PositionRightBackground()
    {
        if (rightRenderer == null)
        {
            Debug.LogError("Right Sprite Renderer is missing.");
            return;
        }

        if (backgroundRight)
        {
            //Position Sprite
            var rightOffset = (backgroundRight.rect.width / backgroundRight.pixelsPerUnit) / 2;
            rightRenderer.transform.localPosition = new Vector3(rightOffset, 0, 0);
        }
        //Set sprite
        rightRenderer.sprite = backgroundRight;
    }
}
