using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Light))]
public class DontLightVirtualRoom : MonoBehaviour
{
    void Awake()
    {
        var light = GetComponent<Light>();
        light.cullingMask &= ~(1 << CreateVirtualRoomLayer.virtualRoomLayer);
    }
}
