using System;
using System.Collections;
using System.Collections.Generic;
using Immersive.Properties;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using UnityEditor;
using UnityEngine;
using static CustomJsonConverter;

namespace Com.Immersive.Hotspots
{
    [System.Serializable]
    public class TextPopUpDataModel
    {
        [System.Serializable]
        public class TextPopUpSetting : PopUpSettings
        {
            public bool includeTitle;

            [JsonConverter(typeof(StringTextConverter))]// parse Json object into Text property
            public TextProperty title;

            [Range(0,10)]
            public int space; //Space Line between Title and Body

            [JsonConverter(typeof(StringTextConverter))]// parse Json object into Text property
            public TextProperty body;

            [JsonConverter(typeof(StringImageConverter))]// parse Json object into Image property
            public ImageProperty background;

            public bool isRightToLeftText = false;
        }

        public HotspotDataModel hotspotSetting;
        public TextPopUpSetting popUpSetting;
    }
}