/* Copyright (C) Immersive Interactive, Ltd - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Luke Bissell <luke@immersive.co.uk>, July 2019
 */

using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Com.Immersive.Hotspots
{
    public class SaveDiscardHotspotPositionCanvasScript : MonoBehaviour
    {

        public void SaveHotspotChanges()
        {
            PositionHotspots.CurrentPositionHotspot.SaveChanges();
        }

        public void DiscardHotspotChanges()
        {
            PositionHotspots.CurrentPositionHotspot.DiscardChanges();
        }
    }
}
