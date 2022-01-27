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
    public class TextSequencePopUpSetting : PopUpSequenceSettings 
    {
        [System.Serializable]
        public class TextPopup
        {
            public bool includeTitle;

            [JsonConverter(typeof(StringTextConverter))]// parse Json object into Text property
            public TextProperty title;

            [Range(0, 10)]
            public int space; //Space Line between Title and Body

            [JsonConverter(typeof(StringTextConverter))]// parse Json object into Text property
            public TextProperty body;            
        }

        public List<TextPopup> textPopups = new List<TextPopup>();

        [JsonConverter(typeof(StringImageConverter))]// parse Json object into Image property
        public ImageProperty background;

        public bool includePageCount;

        [JsonConverter(typeof(StringTextConverter))]// parse Json object into Text property
        public FontProperty pageCountSettings;

        public override int Count => textPopups.Count;
    }
}
