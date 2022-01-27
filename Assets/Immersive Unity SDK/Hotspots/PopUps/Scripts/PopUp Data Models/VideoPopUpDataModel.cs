using System.Collections;
using System.Collections.Generic;
using System.IO;
using Immersive.Properties;
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
        public class VideoPopUpSetting : PopUpSettings
        {
            public VideoProperty video;

            [JsonConverter(typeof(StringImageConverter))]// Parse Json object into Image property
            public ImageProperty border;

            [JsonConverter(typeof(StringImageConverter))]
            public ImageProperty mediaMask;

            public bool closeAfterPlay = false;
            public bool loop = false;

            [JsonConverter(typeof(StringEnumConverter))]// parse Json object into enum value
            public ControlPanelStyle controlPanelStyle;

            public bool useCustomButtons = false;

            [JsonConverter(typeof(StringImageConverter))]
            public ImageProperty pauseButtonImage;
            [JsonConverter(typeof(StringImageConverter))]
            public ImageProperty playButtonImage;
            [JsonConverter(typeof(StringImageConverter))]
            public ImageProperty restartButtonImage;

        }

        public HotspotDataModel hotspotSetting;
        public VideoPopUpSetting popUpSetting;
    }
}