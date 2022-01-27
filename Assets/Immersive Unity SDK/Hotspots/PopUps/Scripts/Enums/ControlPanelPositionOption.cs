using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Com.Immersive.Hotspots
{
    /// <summary>
    /// Used by PopUps for determining which side a Control Panel should appear.
    /// If default it is determined by the PopUpPositioner.
    /// </summary>
    public enum ControlPanelPositionOption
    {
        Default = 0,
        Left = 1,
        Right = 2
    }
}