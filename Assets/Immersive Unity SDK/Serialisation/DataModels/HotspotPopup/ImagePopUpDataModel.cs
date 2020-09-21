using Com.Immersive.Hotspots;
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
        public class ImagePopUpSetting : PopUpSetting
        {
            [JsonConverter(typeof(StringImageConverter))]// Parse Json object into Image property
            public ImageProperty background;

            [JsonConverter(typeof(StringImageConverter))]// Parse Json object into Image property
            public ImageProperty border;
            
            public bool maintainAspectRatio = true;            
        }

        public HotspotDataModel hotspotSetting;
        public ImagePopUpSetting popUpSetting;
    }
}