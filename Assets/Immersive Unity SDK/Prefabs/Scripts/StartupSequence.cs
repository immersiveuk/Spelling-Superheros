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

    public void EnableHotspots()
    {
        HotspotController.EnableHotspotsForAllControllers();
    }

    public void DestroySelf()
    {
        Destroy(gameObject);
    }
}
