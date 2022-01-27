using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopUpPositionerCentredOnCanvasTop : PopUpPositioner
{
    public override ControlPanelSide DefaultControlPanelSide => ControlPanelSide.Right;

    protected override Vector2 Pivot => new Vector2(0.5f, 1);

    protected override Vector2 Anchor => new Vector2(0.5f, 1);

    protected override Vector2 Offset => new Vector2(0, -100);

    protected override bool ShouldContrainPopUpToSurface => false;

    public PopUpPositionerCentredOnCanvasTop(Canvas canvas) : base(canvas) { }
}
