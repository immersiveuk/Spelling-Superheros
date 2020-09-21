using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using UnityEngine;
using static Com.Immersive.Hotspots.HotspotScript;
using static CustomJsonConverter;

namespace Com.Immersive.Hotspots
{
    [System.Serializable]
    public class PopUpSetting
    {
        [JsonConverter(typeof(StringEnumConverter))]// parse Json object into enum value
        public PopUpPosition popUpPosition = PopUpPosition.SurfaceCenter; //PopUp Setting

        public Vector2 popUpPositionOffset = new Vector2(0, 0); //PopUp Setting

        public bool overrideDefaultCloseButton;

        [JsonConverter(typeof(StringImageConverter))]// Parse Json object into Image property
        public ImageProperty closeButton;

        [JsonConverter(typeof(StringEnumConverter))]// parse Json object into enum value
        public SizeOption sizeOption;

        public Vector2 size = new Vector2(500,500);

        [Range(1, 100)]
        public int percentage;

        public RectOffset padding;
    }

    //==============================================================
    // DATA STRUCTURES
    //==============================================================

    public enum PopUpPosition
    {

        //                          +-+-+-+-+ +-+-+       
        //                          |Surface Center|        
        //                          +-+-+-+-+ +-+-+       
        //             ┌──────────────────────────────────────────┐
        //             │                                          │
        //             │                                          │
        //             │ ┌────────┐                               │
        //             │ │Hotspot │   ┌───────────┐               │
        //             │ └────────┘   │           │               │
        //             │              │           │               │
        //             │              │  Pop Up   │               │
        //             │              │           │               │
        //             │              └───────────┘               │
        //             │                                          │
        //             │                                          │
        //             │                                          │
        //             └──────────────────────────────────────────┘
        //                                                         
        //                          +-+-+-+-+ +-+-+       
        //                        |Surface Center Top|        
        //                          +-+-+-+-+ +-+-+       
        //             ┌──────────────────────────────────────────┐
        //             │              ┌───────────┐               │
        //             │              │           │               │
        //             │ ┌────────┐   │           │               │
        //             │ │Hotspot │   │  Pop Up   │               │
        //             │ └────────┘   │           │               │
        //             │              └───────────┘               │
        //             │                                          │
        //             │                                          │
        //             │                                          │
        //             │                                          │
        //             │                                          │
        //             │                                          │
        //             └──────────────────────────────────────────┘
        //                                                         
        //                         +-+-+ +-+-+-+-+             
        //                         |SameAsHotspot|          
        //                         +-+-+ +-+-+-+-+            
        //             ┌──────────────────────────────────────────┐
        //             │┌───────────┐                             │
        //             ││           │                             │
        //             ││           │                             │
        //             ││  Pop Up   │                             │
        //             ││           │                             │
        //             ││           │                             │
        //             │└───────────┘                             │
        //             │                                          │
        //             │                                          │
        //             │                                          │
        //             │                                          │
        //             │                                          │
        //             │                                          │
        //             └──────────────────────────────────────────┘
        //                                                         
        //                                                         
        //                +-+-+-+-+-+-+   ┌─────────────────────┐  
        //                  |Custom|      │   Pos: (200,-300)   │  
        //                +-+-+-+-+-+-+   └─────────────────────┘  
        //             ┌──────────────────────────────────────────┐
        //             │                                          │
        //             │                                          │
        //             │ ┌────────┐                               │
        //             │ │Hotsp┌──┴────────┐                      │
        //             │ └─────┤           │                      │
        //             │       │           │                      │
        //             │       │  Pop Up   │                      │
        //             │       │           │                      │
        //             │       │           │                      │
        //             │       └───────────┘                      │
        //             │                                          │
        //             │                                          │
        //             │                                          │
        //             └──────────────────────────────────────────┘

        SurfaceCenter,
        SurfaceCenterTop,
        SameAsHotspot,
        Custom
    }
}