using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopUpPositionerCentredOnHotspot : PopUpPositioner
{
    private Vector2 offset;
    private ControlPanelSide controlPanelSide;

    public override ControlPanelSide DefaultControlPanelSide => controlPanelSide;

    protected override Vector2 Pivot => new Vector2(0.5f, 0.5f);

    protected override Vector2 Anchor => Vector2.zero;

    protected override Vector2 Offset => offset;
    protected override bool ShouldContrainPopUpToSurface => true;


    public PopUpPositionerCentredOnHotspot(Vector3 hotspotWorldPos, Camera renderingCamera, Canvas canvas) : base(canvas) 
    {
        Vector2 viewportPosition = renderingCamera.WorldToViewportPoint(hotspotWorldPos);
        offset = new Vector2(viewportPosition.x * canvasSize.x, viewportPosition.y * canvasSize.y);
        controlPanelSide = viewportPosition.x < 0.5f ? ControlPanelSide.Right : ControlPanelSide.Left;
    }
}
