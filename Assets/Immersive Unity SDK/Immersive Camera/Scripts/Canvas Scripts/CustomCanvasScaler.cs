/* Copyright (C) Immersive Interactive, Ltd - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Luke Bissell <luke@immersive.co.uk>, July 2019
 */

using UnityEngine;
using UnityEngine.UI;


/// <summary>
/// The normal canvas scaler doesn't work with custon camera rects or with render targets.
/// </summary>
public class CustomCanvasScaler : CanvasScaler
{
    // The log base doesn't have any influence on the results whatsoever, as long as the same base is used everywhere.
    public const float kLogBase = 2;

    private Canvas canvas;

    protected override void OnEnable()
    {
        canvas = GetComponent<Canvas>();
        base.OnEnable();
    }

    protected override void HandleScaleWithScreenSize()
    {
        Vector2 screenSize = new Vector2(1920, 1080);
        Camera cam = canvas.worldCamera;
        //Virtual Room Mode
        if (cam.targetTexture != null)
        {
            screenSize = new Vector2(cam.targetTexture.width, cam.targetTexture.height);
        }
        //Spanning and target Display mode
        else
        {
            if (Display.displays.Length > cam.targetDisplay)
            {
                Display display = Display.displays[cam.targetDisplay];
                screenSize = new Vector2(display.renderingWidth, display.renderingHeight);
            }
        }

        if (canvas.renderMode == RenderMode.ScreenSpaceCamera && canvas.worldCamera != null)
        {
            screenSize.x *= canvas.worldCamera.rect.width;
            screenSize.y *= canvas.worldCamera.rect.height;
        }

        float scaleFactor = 0;
        switch (m_ScreenMatchMode)
        {
            case ScreenMatchMode.MatchWidthOrHeight:
                {
                    // We take the log of the relative width and height before taking the average.
                    // Then we transform it back in the original space.
                    // the reason to transform in and out of logarithmic space is to have better behavior.
                    // If one axis has twice resolution and the other has half, it should even out if widthOrHeight value is at 0.5.
                    // In normal space the average would be (0.5 + 2) / 2 = 1.25
                    // In logarithmic space the average is (-1 + 1) / 2 = 0
                    float logWidth = Mathf.Log(screenSize.x / m_ReferenceResolution.x, kLogBase);
                    float logHeight = Mathf.Log(screenSize.y / m_ReferenceResolution.y, kLogBase);
                    float logWeightedAverage = Mathf.Lerp(logWidth, logHeight, m_MatchWidthOrHeight);
                    scaleFactor = Mathf.Pow(kLogBase, logWeightedAverage);
                    break;
                }
            case ScreenMatchMode.Expand:
                {
                    scaleFactor = Mathf.Min(screenSize.x / m_ReferenceResolution.x, screenSize.y / m_ReferenceResolution.y);
                    break;
                }
            case ScreenMatchMode.Shrink:
                {
                    scaleFactor = Mathf.Max(screenSize.x / m_ReferenceResolution.x, screenSize.y / m_ReferenceResolution.y);
                    break;
                }
        }
        SetScaleFactor(scaleFactor);
        SetReferencePixelsPerUnit(m_ReferencePixelsPerUnit);
    }
}
