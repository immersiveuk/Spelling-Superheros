using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Com.Immersive.Hotspots;
using Immersive.Properties;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

public class CustomJsonConverter
{
    public class StringColorConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(string);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var sColor = (string)reader.Value;
            Color newColor;
            ColorUtility.TryParseHtmlString(sColor, out newColor);
            return newColor;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            Color Col = (Color)value;

            string sColo = ColorUtility.ToHtmlStringRGBA(Col);
            writer.WriteValue("#" + sColo);
        }
    }

    public class StringImageConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(string);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            ImageProperty imagePropertyJson = serializer.Deserialize<ImageProperty>(reader);

            ImageProperty newImage = new ImageProperty();
            newImage.color = imagePropertyJson.color;
            newImage.type = imagePropertyJson.type;
            newImage.imageUrl = imagePropertyJson.imageUrl;

            ////check if path is null use default image
            //if (string.IsNullOrEmpty(imagePropertyJson.imageUrl))
            //{
            //    switch (Path.GetExtension(reader.Path).Replace(".", ""))
            //    {
            //        case "closeButton":
            //            imagePropertyJson.imageUrl = TemplateSceneSetup._instance.jsonDataModelDefault.DefaultCloseHotspotImage;
            //            break;
            //        case "nextButton":
            //            imagePropertyJson.imageUrl = TemplateSceneSetup._instance.jsonDataModelDefault.DefaultForwardArrowImage;
            //            break;
            //        case "previousButton":
            //            imagePropertyJson.imageUrl = TemplateSceneSetup._instance.jsonDataModelDefault.DefaultBackArrowImage;
            //            break;
            //    }
            //}

            if (!string.IsNullOrEmpty(imagePropertyJson.imageUrl))
            {
                RuntimeLoading.Instance.LoadImage(imagePropertyJson.imageUrl, (Texture2D texture, Sprite sprite) =>
                {
                    newImage.sprite = sprite;
                });
            }

            return newImage;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            ImagePropertyJSON imagePropertyJSON = new ImagePropertyJSON((ImageProperty)value);

            var json = JsonConvert.SerializeObject(imagePropertyJSON);
            writer.WriteRawValue(json);
        }
    }

    public class StringImageListConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(string);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            List<ImageProperty> imagePropertyJson = serializer.Deserialize<List<ImageProperty>>(reader);
            List<ImageProperty> imageList = new List<ImageProperty>();

            for (int i = 0; i < imagePropertyJson.Count; i++)
            {

                ImageProperty img = new ImageProperty();
                img.color = imagePropertyJson[i].color;
                img.type = imagePropertyJson[i].type;
                img.imageUrl = imagePropertyJson[i].imageUrl;

                if (!string.IsNullOrEmpty(imagePropertyJson[i].imageUrl))
                {
                    RuntimeLoading.Instance.LoadImage(imagePropertyJson[i].imageUrl, (Texture2D texture, Sprite sprite) =>
                    {
                        img.sprite = sprite;
                    });
                }

                imageList.Add(img);
            }

            return imageList;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            List<ImagePropertyJSON> imagePropertyJSON = new List<ImagePropertyJSON>();
            List<ImageProperty> imageProperty = (List<ImageProperty>)value;

            for (int i = 0; i < imageProperty.Count; i++)
            {
                imagePropertyJSON.Add(new ImagePropertyJSON(imageProperty[i]));
            }

            var json = JsonConvert.SerializeObject(imagePropertyJSON);
            writer.WriteRawValue(json);
        }
    }

    public class StringTextConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(string);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            TextProperty textPropertyJson = serializer.Deserialize<TextProperty>(reader);

            TextProperty newText = new TextProperty();
            newText.Text = textPropertyJson.Text;
            newText.Color = textPropertyJson.Color;
            newText.FontSize = textPropertyJson.FontSize == 0 ? 50 : textPropertyJson.FontSize;

            return newText;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            TextPropertyJSON textPropertyJSON = new TextPropertyJSON((TextProperty)value);

            var json = JsonConvert.SerializeObject(textPropertyJSON);
            writer.WriteRawValue(json);
        }
    }

    public class StringAudioConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(string);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            AudioProperty audioPropertyJson = serializer.Deserialize<AudioProperty>(reader);

            AudioProperty newAudio = new AudioProperty();
            newAudio.audioUrl = audioPropertyJson.audioUrl;
            newAudio.audioVolume = audioPropertyJson.audioVolume;

            if (!string.IsNullOrEmpty(audioPropertyJson.audioUrl))
            {
                RuntimeLoading.Instance.LoadAudio(audioPropertyJson.audioUrl, (AudioClip clip) =>
                {
                    newAudio.audioClip = clip;
                });
            }

            return newAudio;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            AudioPropertyJSON audioPropertyJSON = new AudioPropertyJSON((AudioProperty)value);

            var json = JsonConvert.SerializeObject(audioPropertyJSON);
            writer.WriteRawValue(json);
        }
    }

    public class StringOptionsConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(string);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            OptionsProperty optionsPropertyJson = serializer.Deserialize<OptionsProperty>(reader);

            OptionsProperty newText = new OptionsProperty();
            newText.options = optionsPropertyJson.options;
            newText.correctAnswer = optionsPropertyJson.correctAnswer;
            newText.color = optionsPropertyJson.color;
            newText.size = optionsPropertyJson.size == 0 ? 50 : optionsPropertyJson.size;

            return newText;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            OptionsPropertyJSON optionsPropertyJSON = new OptionsPropertyJSON((OptionsProperty)value);

            var json = JsonConvert.SerializeObject(optionsPropertyJSON);
            writer.WriteRawValue(json);
        }
    }

    public class StringResultConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(string);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            QuizResultProperty quizResultProperty = serializer.Deserialize<QuizResultProperty>(reader);

            return quizResultProperty;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            QuizResultPropertyJSON resultPropertyJSON = new QuizResultPropertyJSON((QuizResultProperty)value);

            var json = JsonConvert.SerializeObject(resultPropertyJSON);
            writer.WriteRawValue(json);
        }
    }

    public class IgnoreSerialization : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(string);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            return null;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteValue("");
        }
    }

    public class HotspotDataModelConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(HotspotDataModel);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JObject jo = JObject.Load(reader);
            HotspotType hotspotType = (HotspotType)Enum.Parse(typeof(HotspotType), "" + jo["hotspotType"]);

            switch (hotspotType)
            {
                case HotspotType.Region:
                    return jo.ToObject<RegionHotspotDataModel>(serializer);
                case HotspotType.Image:
                    return jo.ToObject<ImageHotspotDataModel>(serializer);
                case HotspotType.Text:
                    return jo.ToObject<TextHotspotDataModel>(serializer);

                default:
                    return jo.ToObject<HotspotDataModel>(serializer);

            }
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            HotspotDataModel hotspotData = (HotspotDataModel)(value);
            var json = "";

            switch (hotspotData.hotspotType)
            {
                case HotspotType.Region:
                    json = JsonConvert.SerializeObject((RegionHotspotDataModel)hotspotData);
                    break;

                case HotspotType.Image:
                    json = JsonConvert.SerializeObject((ImageHotspotDataModel)hotspotData);
                    break;
                case HotspotType.Text:
                    json = JsonConvert.SerializeObject((TextHotspotDataModel)hotspotData);
                    break;

                default:
                    json = JsonConvert.SerializeObject(hotspotData);
                    break;

            }

            writer.WriteRawValue(json);
        }
    }
}
