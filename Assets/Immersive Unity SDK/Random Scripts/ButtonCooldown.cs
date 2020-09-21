/* Copyright (C) Immersive Interactive, Ltd - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Luke Bissell <luke@immersive.co.uk>, Nov 2019
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Disables a button for provided length of time.
/// </summary>
public class ButtonCooldown : MonoBehaviour
{
    public float cooldownDuration = 0.5f;
    private float cooldownTimeRemaining;

    private Button button;

    // Start is called before the first frame update
    void Start()
    {
        button = GetComponent<Button>();
        button.interactable = false;

        cooldownTimeRemaining = cooldownDuration;
    }

    // Update is called once per frame
    void Update()
    {
        cooldownTimeRemaining -= Time.deltaTime;
        if (cooldownTimeRemaining < 0)
        {
            button.interactable = true;
            Destroy(this);
        }
    }
}
