using Com.Immersive.WipeToReveal;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivateAndDisableOnWipeComplete : AbstractActivateAndDisable, IOnWipeEventHandler
{
    private void Start()
    {
        DisableActivateObjectsOnStart();
    }

    public void WipeComplete()
    {
        ActivateAndDisable();
    }

    public void WipeOccuring(TouchPhase phase, Vector2 position, float currentPercentage)
    {}

}
