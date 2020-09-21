using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DontLightVirtualRoom : MonoBehaviour
{
    // Start is called before the first frame update
    void Awake()
    {
        var light = GetComponent<Light>();
        if (light == null)
        {
            Debug.LogError("No Light component on GameObject " + name + " with DontLightVirtualRoom component attached");
            return;
        }

        light.cullingMask = ~(1 << CreateVirtualRoomLayer.virtualRoomLayer);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
