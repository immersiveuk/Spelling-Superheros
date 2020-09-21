using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using UnityEngine;
using static CustomJsonConverter;

namespace Com.Immersive.Hotspots
{
    [System.Serializable]
    public class AudioPopUpDataModel
    {
        public enum ControlPanelStyle { Full, PlayPauseAndClose, RestartAndClose, OnlyClose };

        [System.Serializable]
        public class AudioPopUpSetting : PopUpSetting
        {
            [HideInInspector]
            public string audioClipURL;

            public AudioClip audioClip;

            public bool useThumbnail = false;

            [JsonConverter(typeof(StringImageConverter))]// parse Json object into Image property
            public ImageProperty thumbnail;

            public bool closeAfterPlay = false;
            public bool loop = false;

            [JsonConverter(typeof(StringEnumConverter))]// parse Json object into enum value
            public ControlPanelStyle controlPanelStyle;
        }

        public HotspotDataModel hotspotSetting;
        public AudioPopUpSetting popUpSetting;
    }
}