using Immersive.Enumerations;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static CustomJsonConverter;

namespace Com.Immersive.Hotspots
{
    //Serializable class to set the property for text
    [System.Serializable]
    public class FontProperty
    {
        public TMP_FontAsset font;

        [ColorUsage(false)]
        [JsonConverter(typeof(StringColorConverter))]
        public Color color = Color.gray;

        public HorizontalAlignment alignment;

        public int size = 50;
    }
}