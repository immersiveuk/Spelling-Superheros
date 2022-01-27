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
    public class SplitPopUpDataModel
    {

        [System.Serializable]
        public class SplitPopUpSetting : PopUpSettings
        {
            public bool includeTitle;

            [JsonConverter(typeof(StringTextConverter))]
            public TextProperty title;

            [Range(0, 10)]
            public int space; //Space Line between Title and Body

            [JsonConverter(typeof(StringTextConverter))]
            public TextProperty body;

            [JsonConverter(typeof(StringImageConverter))]
            public ImageProperty textBackground;

            [JsonConverter(typeof(StringEnumConverter))]
            public MediaType mediaType = MediaType.Image;
            public enum MediaType { Image, Video };

            [JsonConverter(typeof(StringImageConverter))]
            public ImageProperty image;

            public VideoProperty video;

            [JsonConverter(typeof(StringImageConverter))]
            public ImageProperty mediaMask;

            public bool loopVideo = true;
            public bool videoControl = true;

            [Range(-0.5f,0.5f)]
            public float fixedPopupSizeImageOffset = 0;

            public int separation = 0;

            [JsonConverter(typeof(StringEnumConverter))]
            public MediaPosition mediaPosition = MediaPosition.Left;
            public enum MediaPosition { Left, Right };

            public bool isRightToLeftText = false;
        }

        public HotspotDataModel hotspotSetting;
        public SplitPopUpSetting popUpSetting;
    }
}