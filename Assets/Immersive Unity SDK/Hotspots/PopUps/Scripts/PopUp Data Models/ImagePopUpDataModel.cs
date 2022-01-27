using Com.Immersive.Hotspots;
using Immersive.Properties;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using UnityEngine;
using static CustomJsonConverter;

namespace Com.Immersive.Hotspots
{
    [System.Serializable]
    public class ImagePopUpDataModel
    {
        [System.Serializable]
        public class ImagePopUpSetting : PopUpSettings
        {
            [JsonConverter(typeof(StringImageConverter))]// Parse Json object into Image property
            public ImageProperty background;

            [JsonConverter(typeof(StringImageConverter))]// Parse Json object into Image property
            public ImageProperty border;

            [JsonConverter(typeof(StringImageConverter))]
            public ImageProperty mediaMask;

            public bool maintainAspectRatio = true;            
        }

        public HotspotDataModel hotspotSetting;
        public ImagePopUpSetting popUpSetting;
    }
}