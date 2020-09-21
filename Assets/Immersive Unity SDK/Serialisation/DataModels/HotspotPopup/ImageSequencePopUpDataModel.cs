using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using UnityEngine;
using static CustomJsonConverter;

namespace Com.Immersive.Hotspots
{
    [System.Serializable]
    public class ImageSequencePopUpDataModel
    {
        public enum ControlPanelStyle { Full, ForwardAndClose, ForwardOnly, ForwardAndBack };

        [System.Serializable]
        public class ImageSequencePopUpSetting : PopUpSetting
        {
            [JsonConverter(typeof(StringImageConverter))]// Parse Json object into Image property
            public ImageProperty border;

            [JsonConverter(typeof(StringImageListConverter))]// parse Json object into list of Image property
            public List<ImageProperty> backgroundSprites = new List<ImageProperty>();
            public bool customButtons;

            [JsonConverter(typeof(StringEnumConverter))]// parse Json object into enum value
            public ControlPanelStyle controlPanelStyle;

            [JsonConverter(typeof(StringImageConverter))]// parse Json object into Image property
            public ImageProperty nextButton;

            [JsonConverter(typeof(StringImageConverter))]// parse Json object into Image property
            public ImageProperty previousButton;
        }
       
        public HotspotDataModel hotspotSetting;
        public ImageSequencePopUpSetting popUpSetting;
    }
}