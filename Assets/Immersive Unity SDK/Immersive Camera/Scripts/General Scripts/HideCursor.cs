/* Copyright (C) Immersive Interactive, Ltd - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Luke Bissell <luke@immersive.co.uk>, July 2019
 */

using UnityEngine;

/// <summary>
/// This script will hide the cursor after a given length of inactivity.
/// </summary>
public class HideCursor : MonoBehaviour
{
    public static bool EnforceHide = false;

    [Tooltip("The time after which the cursor will disappear.")]
    public float cursorHideTime = 2.5f;
    private Vector3 _mousePosition = Vector3.zero;
    private float _cursorTimeRemaining = 0; 

    // Update is called once per frame
    void Update()
    {
        if (EnforceHide)
        {
            Cursor.visible = false;
            return;
        }

        if (Input.mousePosition.x != _mousePosition.x ||
            Input.mousePosition.y != _mousePosition.y ||
            Input.mousePosition.z != _mousePosition.z)
        {
            _mousePosition = Input.mousePosition;
            _cursorTimeRemaining = cursorHideTime;
        }
        else
        {
            _cursorTimeRemaining -= Time.deltaTime;
        }
        Cursor.visible = _cursorTimeRemaining > 0;
    }
}
