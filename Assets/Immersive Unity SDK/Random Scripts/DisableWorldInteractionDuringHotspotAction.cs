using Com.Immersive.Cameras;
using Com.Immersive.Hotspots;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableWorldInteractionDuringHotspotAction : MonoBehaviour, IHotspotActionCompleteHandler, IInteractableObject
{
    public void HotspotActionComplete()
    {
        AbstractImmersiveCamera.CurrentImmersiveCamera.worldInteractionOn = true;
    }

    public void OnRelease()
    {
        AbstractImmersiveCamera.CurrentImmersiveCamera.worldInteractionOn = false;
    }




    public void OnPress()
    {
    }


    public void OnTouchEnter()
    {
    }

    public void OnTouchExit()
    {
    }
}
