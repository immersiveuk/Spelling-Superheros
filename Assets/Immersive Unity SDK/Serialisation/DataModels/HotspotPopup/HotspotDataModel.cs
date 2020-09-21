using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using UnityEngine;
using static Com.Immersive.Hotspots.RegionHotspot;
using static CustomJsonConverter;

namespace Com.Immersive.Hotspots
{
    [System.Serializable]
    public class RegionHotspotDataModel : HotspotDataModel
    {
        [JsonConverter(typeof(StringColorConverter))]
        public Color regionHotspotColor = Color.white;

        [JsonConverter(typeof(StringEnumConverter))]
        public LineType lineType;

        [Range(0.1f, 5.0f)]
        public float lineThikness = 1;
    }

    [System.Serializable]
    public class ImageHotspotDataModel : HotspotDataModel
    {
        [JsonConverter(typeof(StringImageConverter))]
        public ImageProperty imageHotspotSprite = new ImageProperty();
    }

    [System.Serializable]
    public class TextHotspotDataModel : HotspotDataModel
    {
        [JsonConverter(typeof(StringTextConverter))]
        public TextProperty text = new TextProperty();

        [JsonConverter(typeof(StringImageConverter))]
        public ImageProperty image = new ImageProperty();

        public bool UseBackground = true;
    }

    [System.Serializable]
    public class HotspotDataModel
    {

        public Vector3 position;
        public Vector3 scale;

        [JsonConverter(typeof(StringEnumConverter))]// parse Json object into enum value
        public ActionType actionType;

        [JsonConverter(typeof(StringEnumConverter))]// parse Json object into enum value
        public OnClickAction clickAction;

        [JsonConverter(typeof(StringEnumConverter))]// parse Json object into enum value
        public HotspotType hotspotType;

        public bool playAudioOnAction = false; //HotspotDataModel

        [JsonConverter(typeof(StringAudioConverter))]
        public AudioProperty clickAudio;
    }

    public enum OnClickAction
    {
        Delete,
        Hide,
        Disable
    }

    public enum ActionType
    {
        ImagePopup,
        ImageSequencePopup,
        VideoPopup,
        TextPopup,
        QuizPopup,
        AudioPopup,
        SceneLink,
        ActivateAndHideObjects,
        SplitPopup
    }
}