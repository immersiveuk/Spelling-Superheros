using Com.Immersive.Cameras.PostProcessing;
using Com.Immersive.Hotspots;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutroSequence : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        DarkenBackground.CurrentDarkenBackground.TurnOn(0.7f);
        HotspotController.DisableHotspotForAllControllers();

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
