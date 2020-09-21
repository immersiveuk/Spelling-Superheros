/* Copyright (C) Immersive Interactive, Ltd - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Luke Bissell <luke@immersive.co.uk>, Aug 2019
 */

using UnityEngine;

/// <summary>
/// Draws a transparent red square where the invisible hotspot is in the scene view.
/// </summary>
public class InvisibleHotspotGizmos : MonoBehaviour
{
    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(1, 0, 0, 0.5f);
        Gizmos.DrawCube(transform.position, transform.localScale);
    }
}
