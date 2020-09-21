/* Copyright (C) Immersive Interactive, Ltd - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Luke Bissell <luke@immersive.co.uk>, July 2019
 */

using Com.Immersive.Cameras;
using Com.Immersive.Hotspots;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HotspotsSerializer : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public static void GenerateControllerJSON(HotspotController controller)
    {
        var serializedController = SerializeController(controller);

        string json = JsonUtility.ToJson(serializedController);
        print(controller.transform.childCount + " Hotspots");

        print(json);
    }

    public static HotspotControllerSerialized SerializeController(HotspotController controller)
    {
        HotspotControllerSerialized serializedController = new HotspotControllerSerialized();
        serializedController.hotspotsAndBatches = new HotspotControllerChildSerializable[controller.transform.childCount];


        for (int i = 0; i < controller.transform.childCount; i++)
        {
            var child = controller.transform.GetChild(i);
            var batch = child.GetComponent<HotspotBatch>();
            var hotspot = child.GetComponent<HotspotScript>();
            //BATCH
            if (batch != null)
            {
                serializedController.hotspotsAndBatches[i] = SerializeHotspotBatch(batch);
            }
            //HOTSPOT
            else if (hotspot != null)
            {
                serializedController.hotspotsAndBatches[i] = SerializeHotspot(hotspot);
            }
        }



        return serializedController;
    }

    private static HotspotBatchSerialized SerializeHotspotBatch(HotspotBatch batch)
    {
        HotspotBatchSerialized serializedBatch = new HotspotBatchSerialized();

        serializedBatch.name = batch.name;
        serializedBatch.hotspots = new HotspotSerialized[batch.transform.childCount];

        for(int i = 0; i < batch.transform.childCount; i++)
        {
            var hotspot = batch.GetComponent<HotspotScript>();
            if (hotspot != null)
            {
                serializedBatch.hotspots[i] = SerializeHotspot(hotspot);
            }
        }


        return serializedBatch;
    } 

    private static HotspotSerialized SerializeHotspot(HotspotScript hotspot)
    {
        HotspotSerialized serializedHotspot = new HotspotSerialized();

        Vector3 screenPos = AbstractImmersiveCamera.CurrentImmersiveCamera.cameras[0].WorldToScreenPoint(hotspot.transform.position);

        serializedHotspot.position = new Vector2(screenPos.x, screenPos.y);
        serializedHotspot.name = hotspot.name;
        print(serializedHotspot.name);

        return serializedHotspot;
    } 
}


[Serializable]
public class HotspotControllerSerialized
{
    public HotspotControllerChildSerializable[] hotspotsAndBatches;
}

[Serializable]
public class HotspotBatchSerialized : HotspotControllerChildSerializable
{
    public HotspotSerialized[] hotspots;
}

[Serializable]
public class HotspotSerialized : HotspotControllerChildSerializable
{
    public Vector2 position;
}

[Serializable]
public class HotspotControllerChildSerializable
{
    public string name;
}
