using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Script which will activate and disable the provided GameObjects when all hotspots in the scene with the script ActivateAndDisableOnAllHotspotActivated have been opened.
/// </summary>
public class ActivateAndDisableOnAllHotspotsActivatedManager : AbstractActivateAndDisable
{
    public static ActivateAndDisableOnAllHotspotsActivatedManager Instance;

    public int RemainingHotspots { get; private set; }
    public int TotalHotspots { get; private set; }

    private void OnEnable() => Instance = this;
    private void OnDisable() => Instance = null;

    private void Start()
    {
        DisableActivateObjectsOnStart();
    }

    public void AddHotspot()
    {
        TotalHotspots++;
        RemainingHotspots++;
    }
    public void RemoveHotspot()
    {
        RemainingHotspots--;
        if (RemainingHotspots == 0)
        {
            ActivateAndDisable();
        }
    }
}
