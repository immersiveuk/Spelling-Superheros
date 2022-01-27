using System.Collections;
using System.Collections.Generic;
using Com.Immersive.Hotspots;
using Immersive.Properties;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using TMPro;
using UnityEngine;
using static CustomJsonConverter;

namespace Com.Immersive.Hotspots
{
    [System.Serializable]
    public class QuizPopUpSetting_V2 : PopUpSequenceSettings
    {
        [System.Serializable]
        public class QuizPopup
        {
            [JsonConverter(typeof(StringTextConverter))] // parse Json object into Text property
            public TextProperty question;

            [JsonConverter(typeof(StringOptionsConverter))] // parse Jon object into Options property 
            public OptionsProperty options;

            public QuizPopup()
            {
                options = new OptionsProperty();
                options.options = new List<string>(4);
            }
        }

        public List<QuizPopup> questions = new List<QuizPopup>();

        public bool disableCloseButton;
        public int duration;
        public bool randomiseQuestions;
        public bool randomiseOption;        
        public AudioClip correctClip;
        public AudioClip incorrectClip;
        public AudioClip timesUpClip;

        public bool isRightToLeftText = false;
        public TMP_FontAsset font;

        public override int Count => questions.Count;
    }
}