using Com.Immersive.Hotspots;
using Immersive.Properties;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static CustomJsonConverter;

namespace Com.Immersive.Hotspots
{
    [System.Serializable]
    public class MatchingPairPopUpSetting : PopUpSequenceSettings
    {     
        public enum OptionType
        {
            Left,
            Right
        }

        [System.Serializable]
        public class MatchingPair
        {
            [System.Serializable]
            public class Pair
            {
                public string leftPart;
                public string rightPart;
            }

            [JsonConverter(typeof(StringTextConverter))] // parse Json object into Text property
            public TextProperty question;

            public List<Pair> pairs = new List<Pair>();
        }

        public List<MatchingPair> matchingPairQuestions = new List<MatchingPair>();

        public bool disableCloseButton;
        public bool enableTimer;
        public int duration;
        public AudioClip correctClip;
        public AudioClip incorrectClip;
        public AudioClip timesUpClip;

        public override int Count => matchingPairQuestions.Count;
    }
}