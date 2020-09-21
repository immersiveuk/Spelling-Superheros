using Com.Immersive.Hotspots;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Script which should be applied to all hotspots that you want the ActivateAndDisableOnAllHotspotActivatedManager to watch in order to activate and disable a list of GameObjects.
/// </summary>
[RequireComponent(typeof(HotspotScript))]
public class ActivateAndDisableOnAllHotspotActivated : MonoBehaviour, IHotspotActionCompleteHandler
{
    private bool previouslyOpened = false;
    private ActivateAndDisableOnAllHotspotsActivatedManager manager;


    // Start is called before the first frame update
    void Start()
    {
        manager = ActivateAndDisableOnAllHotspotsActivatedManager.Instance;
        if (manager == null) Debug.LogError("No ActivateAndDisableOnAllHotspotsActivatedManager in Scene.");

        else manager.AddHotspot();
       
    }

    public void HotspotActionComplete()
    {
        if (!previouslyOpened)
        {
            if (manager) manager.RemoveHotspot();
            previouslyOpened = true;
        }
    }
}
