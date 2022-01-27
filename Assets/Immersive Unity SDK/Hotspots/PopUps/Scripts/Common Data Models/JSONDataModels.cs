using System.Collections.Generic;
using Immersive.Properties;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using UnityEngine;
using UnityEngine.UI;
using static CustomJsonConverter;

namespace Com.Immersive.Hotspots
{
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
            this.text = property.Text;
            this.fontName = property.FontName;
            this.color = property.Color;
            this.size = property.FontSize;
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