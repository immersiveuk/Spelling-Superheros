using System.Collections;
using System.Collections.Generic;
using Com.Immersive.Hotspots;
using UnityEngine;

public interface IHotspotVisibilityHandler
{
    HotspotController parentController { get; set; }
    
    void HotspotsVisible();
    void HotspotsHidden();
}
