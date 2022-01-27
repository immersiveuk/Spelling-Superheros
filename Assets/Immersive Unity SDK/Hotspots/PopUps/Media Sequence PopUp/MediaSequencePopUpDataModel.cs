using Immersive.Properties;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static CustomJsonConverter;

namespace Com.Immersive.Hotspots
{
    [System.Serializable]
    public class MediaSequencePopUpSetting : PopUpSequenceSettings 
    {      
        [System.Serializable]
        public class MediaPopUp
        {
            [JsonConverter(typeof(StringEnumConverter))]
            public MediaType mediaType = MediaType.Image;
            public enum MediaType { Image, Video };

            [JsonConverter(typeof(StringImageConverter))]
            public ImageProperty image;

            public VideoProperty video;
            public bool loopVideo = true;
        }

        public List<MediaPopUp> mediaPopups = new List<MediaPopUp>();

        [JsonConverter(typeof(StringImageConverter))]
        public ImageProperty mediaMask;

        [JsonConverter(typeof(StringImageConverter))]// Parse Json object into Image property
        public ImageProperty border;

        public override int Count => mediaPopups.Count;
    }
}
