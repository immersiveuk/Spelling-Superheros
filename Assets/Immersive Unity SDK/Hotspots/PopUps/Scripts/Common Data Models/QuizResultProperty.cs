using Immersive.Properties;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static CustomJsonConverter;

namespace Com.Immersive.Hotspots
{
    //Serializable class to set property for result text in Q&A hotspot
    [System.Serializable]
    public class QuizResultProperty
    {
        [JsonConverter(typeof(StringTextConverter))]
        public TextProperty correctAnswer;

        [JsonConverter(typeof(StringTextConverter))]
        public TextProperty incorrectAnswer;

        [Tooltip("If an audio clip is provided it will play when the correct answer is selected")]
        public AudioClip correctAudio;

        [HideInInspector]
        public string correctAudioURL;

        [Tooltip("If an audio clip is provided it will play when an in correct answer is selected")]
        public AudioClip incorrectAudio;

        [HideInInspector]
        public string incorrectAudioURL;
    }

}