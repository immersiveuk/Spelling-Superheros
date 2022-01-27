using Com.Immersive.Hotspots;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PopUpPositioner
{
    public enum ControlPanelSide { Left, Right };

    public abstract ControlPanelSide DefaultControlPanelSide { get; }

    protected abstract Vector2 Pivot { get; }
    protected abstract Vector2 Anchor { get; }
    protected abstract Vector2 Offset { get; }
    protected abstract bool ShouldContrainPopUpToSurface { get; }

    protected Vector2 canvasSize;
    public Vector2 CanvasSize => canvasSize;

    public PopUpPositioner(Canvas canvas)
    {
        canvasSize = canvas.GetComponent<RectTransform>().sizeDelta;
    }

    public void PositionPopUp(RectTransform rectTransform)
    {
        Vector2 popUpSize = rectTransform.sizeDelta;
        rectTransform.pivot = Pivot;
        rectTransform.anchorMin = Anchor;
        rectTransform.anchorMax = Anchor;
        if (ShouldContrainPopUpToSurface)
            rectTransform.anchoredPosition = GetOffsetConstrainedToSurface(popUpSize);
        else
            rectTransform.anchoredPosition = Offset;

        //Debug.Log($"Positioning PopUp, Pivot = {Pivot}, Anchor = {Anchor}, OffSet = {Offset}.");
    }

    protected Vector2 GetOffsetConstrainedToSurface(Vector2 popUpSizePixels)
    {
        Vector2 popUpPositionPixels = GetCentrePositionPixels(popUpSizePixels);

        //Stop going over edge.
        // X-Axis
        if (popUpPositionPixels.x - popUpSizePixels.x / 2 < 0) popUpPositionPixels.x = popUpSizePixels.x / 2;
        if (popUpPositionPixels.x + popUpSizePixels.x / 2 > canvasSize.x) popUpPositionPixels.x = canvasSize.x - popUpSizePixels.x / 2;

        //Y-Axis
        if (popUpPositionPixels.y - popUpSizePixels.y / 2 < 0) popUpPositionPixels.y = popUpSizePixels.y / 2;
        if (popUpPositionPixels.y + popUpSizePixels.y / 2 > canvasSize.y) popUpPositionPixels.y = canvasSize.y - popUpSizePixels.y / 2;

        Vector2 newOffset = popUpPositionPixels - Anchor * canvasSize;
        return newOffset;
    }

    private Vector2 GetCentrePositionPixels(Vector2 popUpSize) => (Anchor * canvasSize) + Offset - (Pivot - new Vector2(0.5f, 0.5f)) * popUpSize;

    public float GetYOffsetFromCentre(Vector2 popUpSize) => canvasSize.y / 2 - GetCentrePositionPixels(popUpSize).y;
}