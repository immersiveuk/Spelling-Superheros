using System.Collections;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using UnityEngine;
using UnityEngine.Video;
using static CustomJsonConverter;

namespace Com.Immersive.Hotspots
{
    [System.Serializable]
    public class VideoPopUpDataModel
    {
        public enum ControlPanelStyle { Full, PlayPauseAndClose, RestartAndClose, OnlyClose };

        [System.Serializable]
        public class VideoPopUpSetting : PopUpSetting
        {
            public VideoProperty video;

            public bool closeAfterPlay = false;
            public bool loop = false;

            [JsonConverter(typeof(StringEnumConverter))]// parse Json object into enum value
            public ControlPanelStyle controlPanelStyle;
        }

        public HotspotDataModel hotspotSetting;
        public VideoPopUpSetting popUpSetting;
    }
}