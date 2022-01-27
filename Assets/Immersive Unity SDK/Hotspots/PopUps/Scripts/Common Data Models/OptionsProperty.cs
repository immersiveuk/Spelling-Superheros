using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static CustomJsonConverter;

namespace Com.Immersive.Hotspots
{
    //Serializable class to set property for the options in questions popup
    [System.Serializable]
    public class OptionsProperty
    {
        [HideInInspector]
        public string fontName;
        public TMP_FontAsset font;

        [ColorUsage(false)]
        [JsonConverter(typeof(StringColorConverter))]
        public Color color = Color.white;

        public int size;
        public List<string> options = new List<string>();
        public string correctAnswer;

        public bool IsCorrect(string answer) => answer.Equals(correctAnswer);
    }

}