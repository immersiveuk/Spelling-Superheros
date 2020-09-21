/* Copyright (C) Immersive Interactive, Ltd - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Luke Bissell <luke@immersive.co.uk>, Nov 2019
 */
 
using UnityEngine;

/// <summary>
/// A component which is a subclass of HotspotActionCompleteHandler can be used to trigger an action when a hotspots function is complete. In the case of a Popup this will be when the Popup is closed. Subclassed should implement the function HotspotActionComplete(). This is the function which will be called on when the Hotspot, with which the component is attached, has its action complete. 
/// </summary>
namespace Com.Immersive.Hotspots
{
    public interface IHotspotActionCompleteHandler
    {
        void HotspotActionComplete();
    }
}