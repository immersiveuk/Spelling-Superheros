using System.Collections.Generic;
using Immersive.Properties;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using UnityEngine;
using static CustomJsonConverter;

namespace Com.Immersive.Hotspots
{
    [System.Serializable]
    public class ImageSequencePopUpDataModel
    {
        [System.Serializable]
        public class ImageSequencePopUpSetting : PopUpSequenceSettings 
        {
            [JsonConverter(typeof(StringImageConverter))]// Parse Json object into Image property
            public ImageProperty border;

            [JsonConverter(typeof(StringImageListConverter))]// parse Json object into list of Image property
            public List<ImageProperty> backgroundSprites = new List<ImageProperty>();

            [JsonConverter(typeof(StringImageConverter))]
            public ImageProperty mediaMask;

            public override int Count => backgroundSprites.Count;
        }

        public HotspotDataModel hotspotSetting;
        public ImageSequencePopUpSetting popUpSetting;
    }
}