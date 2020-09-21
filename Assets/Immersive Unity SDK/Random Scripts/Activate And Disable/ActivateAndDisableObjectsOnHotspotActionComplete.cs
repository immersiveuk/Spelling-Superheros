/* Copyright (C) Immersive Interactive, Ltd - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Luke Bissell <luke@immersive.co.uk>, Nov 2019
 */

using Com.Immersive.Hotspots;
using UnityEngine;

/// <summary>
/// A component that allows you to define a series of objects to activate or disable on the completion of the associated Hotspots action.
/// </summary>
public class ActivateAndDisableObjectsOnHotspotActionComplete : AbstractActivateAndDisable, IHotspotActionCompleteHandler
{
    private void Start()
    {
        DisableActivateObjectsOnStart();
    }

    void IHotspotActionCompleteHandler.HotspotActionComplete()
    {
        ActivateAndDisable();
    }
}
