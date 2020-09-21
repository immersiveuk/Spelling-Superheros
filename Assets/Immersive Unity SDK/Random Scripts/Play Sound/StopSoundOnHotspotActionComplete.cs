/* Copyright (C) Immersive Interactive, Ltd - All Rights Reserved
* Unauthorized copying of this file, via any medium is strictly prohibited
* Proprietary and confidential
* Written by Luke Bissell <luke@immersive.co.uk>, Nov 2019
*/

using Com.Immersive.Cameras;
using Com.Immersive.Hotspots;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StopSoundOnHotspotActionComplete : MonoBehaviour, IHotspotActionCompleteHandler
{
    public void HotspotActionComplete()
    {
        AbstractImmersiveCamera.StopAudio();
    }
}
