using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopUpPositionerOffsetFromHotspot : PopUpPositioner
{
    private Vector2 anchor, offset;
    private ControlPanelSide controlPanelSide;

    public override ControlPanelSide DefaultControlPanelSide => controlPanelSide;

    protected override Vector2 Pivot => new Vector2(0.5f, 0.5f);

    protected override Vector2 Anchor => anchor;

    protected override Vector2 Offset => offset;

    protected override bool ShouldContrainPopUpToSurface => true;

    public ControlPanelSide GetDefaultControlPanelSide() => controlPanelSide;

    public PopUpPositionerOffsetFromHotspot(Vector3 hotspotWorldPos, Camera renderingCamera, Vector2 offset, Canvas canvas): base(canvas)
    {
        anchor = renderingCamera.WorldToViewportPoint(hotspotWorldPos);
        this.offset = offset;

        var xOffset = (anchor.x - 0.5f) * canvas.pixelRect.width + offset.x;
        controlPanelSide = xOffset < 0 ? ControlPanelSide.Right : ControlPanelSide.Left;
    }
}
