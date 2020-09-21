using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using static CustomJsonConverter;

public enum SizeOption
{
    FixedPopupSize,
    FixedContentSize,
    FixedPercentage
}

namespace Com.Immersive.Hotspots
{
    //generic class to set image property for any popup
    [System.Serializable]
    public class ImageProperty
    {
        [HideInInspector]
        public string imageUrl;

        public Sprite sprite;

        [JsonConverter(typeof(StringColorConverter))]
        public Color color = Color.white;

        [JsonConverter(typeof(StringEnumConverter))]
        public Image.Type type = Image.Type.Simple;
    }

    [System.Serializable]
    public class VideoProperty
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public VideoSource videoSource;        //URL or VideoClip

        public string videoURL;

        [JsonConverter(typeof(IgnoreSerialization))]
        public VideoClip videoClip;
    }

    [System.Serializable]
    public class AudioProperty
    {
        [HideInInspector]
        public string audioUrl;

        public AudioClip audioClip = null;

        [Range(0, 1)]
        public float audioVolume = 1;
    }

    //Serializable class to set the property for text
    [System.Serializable]
    public class TextProperty
    {
        [TextArea]
        public string text = "Default Text";

        [HideInInspector]
        public string fontName;

        public TMP_FontAsset font;

        [ColorUsage(false)]
        [JsonConverter(typeof(StringColorConverter))]
        public Color color = Color.gray;

        public int size = 50;
    }

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
    }

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

    //To create JSON for Image property
    public class ImagePropertyJSON
    {
        [HideInInspector]
        public string imageUrl;

        [JsonConverter(typeof(StringColorConverter))]
        public Color color = Color.white;

        [JsonConverter(typeof(StringEnumConverter))]
        public Image.Type type = Image.Type.Simple;

        public ImagePropertyJSON(ImageProperty property)
        {
            this.imageUrl = property.imageUrl;
            this.color = property.color;
            this.type = property.type;
        }
    }

    //To create JSON for Text property
    public class TextPropertyJSON
    {       
        public string text;        
        public string fontName;

        [JsonConverter(typeof(StringColorConverter))]
        public Color color = Color.white;

        public int size;

        public TextPropertyJSON(TextProperty property)
        {
            this.text = property.text;
            this.fontName = property.fontName;
            this.color = property.color;
            this.size = property.size;
        }
    }

    //To create JSON for Audio property
    public class AudioPropertyJSON
    {
        [HideInInspector]
        public string audioUrl;

        [Range(0, 1)]
        public float audioVolume = 1;

        public AudioPropertyJSON(AudioProperty property)
        {
            this.audioUrl = property.audioUrl;
            this.audioVolume = property.audioVolume;            
        }
    }

    //To create JSON for Option property
    public class OptionsPropertyJSON
    {        
        public string fontName;
     
        [JsonConverter(typeof(StringColorConverter))]
        public Color color = Color.white;

        public int size;
        public List<string> options = new List<string>();
        public string correctAnswer;

        public OptionsPropertyJSON(OptionsProperty property)
        {
            this.fontName = property.fontName;
            this.color = property.color;
            this.size = property.size;
            this.options = property.options;
            this.correctAnswer = property.correctAnswer;
        }
    }

    //To create JSON for Result property
    public class QuizResultPropertyJSON
    {
        [JsonConverter(typeof(StringTextConverter))]
        public TextProperty correctAnswer;

        [JsonConverter(typeof(StringTextConverter))]
        public TextProperty incorrectAnswer;

        [HideInInspector]
        public string correctAudioURL;

        [HideInInspector]
        public string incorrectAudioURL;

        public QuizResultPropertyJSON(QuizResultProperty property)
        {
            this.correctAnswer = property.correctAnswer;
            this.incorrectAnswer = property.incorrectAnswer;

            this.correctAudioURL = property.correctAudioURL;
            this.incorrectAudioURL = property.incorrectAudioURL;
        }
    }
}