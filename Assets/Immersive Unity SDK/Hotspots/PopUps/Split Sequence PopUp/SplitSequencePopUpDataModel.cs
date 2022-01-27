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
    public class SplitSequencePopUpSetting : PopUpSequenceSettings 
    {
        [System.Serializable]
        public class SplitPopUp
        {
            public bool includeTitle;

            [JsonConverter(typeof(StringTextConverter))]
            public TextProperty title;

            [Range(0, 10)]
            public int space; //Space Line between Title and Body

            [JsonConverter(typeof(StringTextConverter))]
            public TextProperty body;
          

            [JsonConverter(typeof(StringEnumConverter))]
            public MediaType mediaType = MediaType.Image;
            public enum MediaType { Image, Video };

            [JsonConverter(typeof(StringImageConverter))]
            public ImageProperty image;

            public VideoProperty video;          
            public bool loopVideo = true;
            public bool videoControl = true;
        }

        public bool keepSameMedia;

        public List<SplitPopUp> splitPopups = new List<SplitPopUp>();

        [JsonConverter(typeof(StringImageConverter))]
        public ImageProperty textBackground;

        [JsonConverter(typeof(StringImageConverter))]
        public ImageProperty mediaMask;

        [JsonConverter(typeof(StringEnumConverter))]
        public MediaPosition mediaPosition = MediaPosition.Left;
        public enum MediaPosition { Left, Right };

        public int separation = 0;
        [Range(-0.5f, 0.5f)]
        public float fixedPopupSizeImageOffset = 0;
        public override int Count => splitPopups.Count;
    }
}