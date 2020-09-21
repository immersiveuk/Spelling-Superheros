using Com.Immersive.Cameras;
using Com.Immersive.Cameras.PostProcessing;
using Com.Immersive.Hotspots;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartupSequence : MonoBehaviour
{
    private void Start()
    {
        if(AbstractImmersiveCamera.CurrentImmersiveCamera.IsPlaceHotspotMode)
        {
            gameObject.SetActive(false);
        }
    }

    private bool isFirst = true;
    private void Update()
    {
        if (isFirst)
        {
            DarkenBackground.CurrentDarkenBackground.TurnOn(0.7f);
            HotspotController.DisableHotspotForAllControllers();
            isFirst = false;
        }
    }

    public void TurnOffDarkenedBackground()
    {
        DarkenBackground.CurrentDarkenBackground.TurnOff();
    }

    public void EnableHotspots()
    {
        HotspotController.EnableHotspotsForAllControllers();
    }

    public void DestroySelf()
    {
        Destroy(gameObject);
    }
}
