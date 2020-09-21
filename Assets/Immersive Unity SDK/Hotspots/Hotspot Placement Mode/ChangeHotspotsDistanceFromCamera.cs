using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeHotspotsDistanceFromCamera : MonoBehaviour
{
    private PositionHotspots positioner;
    // Start is called before the first frame update
    void Start()
    {
        positioner = PositionHotspots.CurrentPositionHotspot;
    }

    public void DistanceChanged(float value)
    {
        positioner.distanceFromCamera = value;
    }
}
