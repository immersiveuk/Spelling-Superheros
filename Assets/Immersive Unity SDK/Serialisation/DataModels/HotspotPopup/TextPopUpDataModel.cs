using System;
using System.Collections;
using System.Collections.Generic;
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
        public class TextPopUpSetting : PopUpSetting
        {

            [JsonConverter(typeof(StringTextConverter))]// parse Json object into Text property
            public TextProperty title;

            [JsonConverter(typeof(StringTextConverter))]// parse Json object into Text property
            public TextProperty body;

            [JsonConverter(typeof(StringImageConverter))]// parse Json object into Image property
            public ImageProperty background;
        }

        public HotspotDataModel hotspotSetting;
        public TextPopUpSetting popUpSetting;
    }
}