using Com.Immersive.Hotspots;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveAndLoadHotspotsToJSON : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}


[Serializable]
public class SerializedHotspotController
{
    public SerializedHotspotController(HotspotController hotspotController)
    {
        
    }

    //public SerializedHotspot
}

[Serializable]
public abstract class SerializedHotspotOrBatch
{
    public SerializedHotspotOrBatch(GameObject hotspotOrBatch)
    {
        name = hotspotOrBatch.name;
    }

    public string name;
}



[Serializable]
public class SerializedHotspot
{
    public SerializedHotspot(GameObject hotspotOrBatch)
    {
        position = hotspotOrBatch.transform.position;
        name = hotspotOrBatch.name;

        if (hotspotOrBatch.GetComponent<HotspotBatch>())
        {
            type = HotspotType.Batch;
        }
        else if (hotspotOrBatch.GetComponent<HotspotScript>()) { }
    }

    public Vector3 position;
    public string name;
    public HotspotType type; 
}



public enum HotspotType
{
    Basic,
    Image,
    Batch,
    Region,
    Text,
    Invisible
}