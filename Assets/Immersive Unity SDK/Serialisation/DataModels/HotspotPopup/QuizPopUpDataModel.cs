using System.Collections;
using System.Collections.Generic;
using Com.Immersive.Hotspots;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using UnityEngine;
using static CustomJsonConverter;

namespace Com.Immersive.Hotspots
{
    [System.Serializable]
    public class QuizPopUpDataModel
    {

        [System.Serializable]
        public class QuizPopUpSetting : PopUpSetting
        {
            [JsonConverter(typeof(StringTextConverter))] // parse Json object into Text property
            public TextProperty question;

            [JsonConverter(typeof(StringImageConverter))] // parse Json object into Image property
            public ImageProperty background;

            [JsonConverter(typeof(StringOptionsConverter))] // parse Jon object into Options property 
            public OptionsProperty options;

            [JsonConverter(typeof(StringResultConverter))] // parse Jon object into Options property 
            public QuizResultProperty result;
        }

        public HotspotDataModel hotspotSetting;
        public QuizPopUpSetting popUpSetting;
    }
}