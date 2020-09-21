/* Copyright (C) Immersive Interactive, Ltd - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Luke Bissell <luke@immersive.co.uk>, July 2019
 */

using Com.Immersive.Cameras;
using UnityEngine;

/// <summary>
/// This component will activate or disable one or more provided game object when pressed.
/// </summary>
public class ActivateAndDisableOnTouch : AbstractActivateAndDisable, IInteractableObject
{
    public float timeBeforeActive = 0;

    private bool active = false;
    private float timeRemaining;

    // Start is called before the first frame update
    void Start()
    {
        timeRemaining = timeBeforeActive;

        DisableActivateObjectsOnStart();
    }

    private void Update()
    {
        if (!active)
        {
            timeRemaining -= Time.deltaTime;
            if (timeRemaining < 0) active = true;
        }
    }

    public void OnRelease()
    {
        if (active)
        {
            ActivateAndDisable();
        }
    }

    public void OnPress() { }
    public void OnTouchEnter() { }
    public  void OnTouchExit() { }

}
