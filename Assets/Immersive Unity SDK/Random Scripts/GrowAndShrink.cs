/* Copyright (C) Immersive Interactive, Ltd - All Rights Reserved
* Unauthorized copying of this file, via any medium is strictly prohibited
* Proprietary and confidential
* Written by Luke Bissell <luke@immersive.co.uk>, Nov 2019
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrowAndShrink : MonoBehaviour
{

    public float timePeriod = 3;
    public float minScale = 0.7f;
    public float maxScale = 1.3f;

    public bool maintainGlobalSync = true;

    private float time = 0;

    // Update is called once per frame
    void Update()
    {

        CalculateTime();

        var scaleValue = time / (timePeriod/2);
        if (scaleValue > 1) scaleValue = 2 - scaleValue;
        var scale = Mathf.Lerp(minScale, maxScale, scaleValue);

        transform.localScale = new Vector3(scale, scale, scale);
    }

    private void CalculateTime()
    {
        if (maintainGlobalSync)
        {
            time = Time.time % timePeriod;
        }
        else
        {
            time += Time.deltaTime;
            if (time > timePeriod) time -= timePeriod;
        }
    }
}
