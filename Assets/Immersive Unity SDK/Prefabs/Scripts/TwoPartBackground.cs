/* Copyright (C) Immersive Interactive, Ltd - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Luke Bissell <luke@immersive.co.uk>, July 2019
 */

using Com.Immersive.Cameras;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

/// <summary>
/// Automatically positions a background that has been split into two parts.
/// </summary>
[ExecuteInEditMode]
public class TwoPartBackground : MonoBehaviour
{
    [SerializeField, FormerlySerializedAs("backgroundLeft")] Sprite leftSprite = null;
    [SerializeField, FormerlySerializedAs("backgroundRight")] Sprite rightSprite = null;

    [SerializeField] SpriteRenderer leftSpriteRenderer = null;
    [SerializeField] SpriteRenderer rightSpriteRenderer = null;

    [SerializeField] Material material = null;
    [SerializeField] bool includesBackWall = false;

    private float GetSpriteWidthInWorldSpace(Sprite sprite) => sprite.rect.width / sprite.pixelsPerUnit;

    private float BackWallOffset
    {

        get
        {
            if (!includesBackWall)
                return 0;
            var immersiveCam = AbstractImmersiveCamera.CurrentImmersiveCamera;
            if (immersiveCam is ImmersiveCamera2D)
                return ((ImmersiveCamera2D)immersiveCam).TargetAspectRatioFloat;
            return 0;
        }
    }

    private void Start()
    {
        UpdateSettings();
    }

    public void UpdateSettings()
    {
        if (leftSpriteRenderer == null ||
            rightSpriteRenderer == null)
        {
            Debug.LogError("Left and Right SpriteRenderers must be set in Referenced Objects");
            return;
        }

        leftSpriteRenderer.sprite = leftSprite;
        rightSpriteRenderer.sprite = rightSprite;

        PositionRightSpriteRenderer();
        PositionLeftSpriteRenderer();

        if (material != null)
        {
            leftSpriteRenderer.sharedMaterial = material;
            rightSpriteRenderer.sharedMaterial = material;
        }
    }

    private void PositionLeftSpriteRenderer()
    {
        if (leftSprite)
        {
            var leftOffset = BackWallOffset - GetSpriteWidthInWorldSpace(leftSprite) / 2;
            leftSpriteRenderer.transform.localPosition = new Vector3(leftOffset, 0, 0);
        }
    }

    private void PositionRightSpriteRenderer()
    {
        if (rightSprite)
        {
            var rightOffset = BackWallOffset + GetSpriteWidthInWorldSpace(rightSprite) / 2;
            rightSpriteRenderer.transform.localPosition = new Vector3(rightOffset, 0, 0);
        }
    }
}
