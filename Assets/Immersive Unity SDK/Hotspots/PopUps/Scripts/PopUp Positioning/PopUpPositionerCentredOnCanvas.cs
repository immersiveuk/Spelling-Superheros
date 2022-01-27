using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopUpPositionerCentredOnCanvas : PopUpPositioner
{
    public override ControlPanelSide DefaultControlPanelSide => ControlPanelSide.Right;

    protected override Vector2 Pivot => new Vector2(0.5f, 0.5f);

    protected override Vector2 Anchor => new Vector2(0.5f, 0.5f);

    protected override Vector2 Offset => Vector2.zero;

    protected override bool ShouldContrainPopUpToSurface => false;

    public PopUpPositionerCentredOnCanvas(Canvas canvas) : base(canvas) { }
}
