using System;
using System.Collections.Generic;
using Com.Immersive.Hotspots;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using UnityEngine;
using UnityEngine.UI;
using static Com.Immersive.Hotspots.HotspotController;
using static CustomJsonConverter;

namespace Com.Immersive.Hotspots
{

    [Serializable]
    public class JsonDataModel
    {

    }

    /// <summary>
    /// Serializable class to parse Default settings of hotspots
    /// </summary>
    [Serializable]
    public class JsonDataModelDefault
    {
        public string Font;
        [JsonConverter(typeof(StringEnumConverter))]
        public HotspotRevealType HotspotRevealType;

        public string PopupBorderColour;
        public float PopupScale;
        public bool HighlightRegionHotspots;

        [JsonConverter(typeof(StringEnumConverter))]
        public HotspotEffects HotspotHighlighting;

        [JsonConverter(typeof(StringColorConverter))]
        public Color HotspotGlowColor;

        public bool AddShadowToHotspots;
        public bool AddShadowToPopups;
        public string PopupStyle;

        [JsonConverter(typeof(StringEnumConverter))]
        public PopUpPosition DefaultPopupPosition;

        [JsonConverter(typeof(StringEnumConverter))]
        public OnClickAction DefaultClickAction;

        [JsonConverter(typeof(StringEnumConverter))]
        public BackgroundScaleMode BackgroundScaling;

        [JsonConverter(typeof(StringEnumConverter))]
        public BackgroundScaleMode ForegroundScaling;

        [JsonConverter(typeof(StringEnumConverter))]
        public BackgroundType BackgroundType;

        [JsonConverter(typeof(StringColorConverter))]
        public Color TextColour;

        [JsonConverter(typeof(StringColorConverter))]
        public Color TextBackgroundColour;

        [JsonConverter(typeof(StringColorConverter))]
        public Color TextBorderColour;

        public bool singlePopUpOpenAtOnce;
        public string PopupSize;
        public string PopupAnimation;
        public bool BlurAndDesaturate;
        public int BlurInDuration;
        public int BlurOutDuration;
        public bool PlayAudioOnTouch;
        public int TouchAudioVolume;
        public int PopupBorderWidth;
        public int Volume;

        public string DefaultPopUpImage;
        public string DefaultBackArrowImage;
        public string DefaultForwardArrowImage;

        public string DefaultHotspotIcon;

        public string DefaultInformationIcon;
        public string DefaultPauseIcon;
        public string DefaultPlayIcon;
        public string DefaultReplayIcon;
        public string DefaultQuestionIcon;
        public string DefaultTickIcon;
        public string DefaultCloseHotspotImage;
        public string DefaultHotspotClickSound;
        public string DefaultCorrectAudio;
        public string DefaultIncorrectAudio;
        public string DefaultPopupOpenAudio;
        public string BackgroundImage;
        public string ForegroundImage;
        public string BackgroundVideo;
        public string Video360;
        public string BackgroundSound;
        public string PopupFrame;
        public string IntroDialog;
        public string EndDialog;

        public List<HotspotControllerDataModel> HotspotController = new List<HotspotControllerDataModel>();
    }

    /*
    /// <summary>
    /// Serializable class to parse list of Hotspot Controller
    /// </summary>
    [Serializable]
    public class JsonDataModelHotspot
    {
        public List<HotspotControllerDataModel> HotspotController;
    }
    */

    /// <summary>
    /// Serializable class to parse Hotspot controller
    /// </summary>
    [Serializable]
    public class HotspotControllerDataModel
    {
        //public bool addGlowAroundHotspots = false;
        //public HotspotRevealType revealType = HotspotRevealType.AllAtOnce;
        //public HotspotGlowSettings hotspotGlowSettings;

        public HotspotJson Hotspots = new HotspotJson();
    }


    /// <summary>
    /// Serializable class to parse list of individual hotspots popup
    /// </summary>
    [Serializable]
    public class HotspotJson
    {
        public List<ImagePopUpDataModel> imagePopUp = new List<ImagePopUpDataModel>();
        public List<ImageSequencePopUpDataModel> imageSequencePopUp = new List<ImageSequencePopUpDataModel>();
        public List<VideoPopUpDataModel> videoPopUp = new List<VideoPopUpDataModel>();
        public List<TextPopUpDataModel> textPopUp = new List<TextPopUpDataModel>();
        public List<QuizPopUpDataModel> quizPopUp = new List<QuizPopUpDataModel>();
        public List<AudioPopUpDataModel> audioPopUp = new List<AudioPopUpDataModel>();
        public List<SceneLinkDataModel> sceneLinkPopUp = new List<SceneLinkDataModel>();
        public List<SplitPopUpDataModel> splitPopUps = new List<SplitPopUpDataModel>();
    }

    /// <summary>
    /// Enum of Background scale mode
    /// </summary>
    public enum BackgroundScaleMode
    {
        Fill,
        Crop
    }

    /// <summary>
    /// Enum of background type
    /// </summary>
    public enum BackgroundType
    {
        None,
        Background,
        Foreground,
        BackgroundForeground,
        BackgroundVideo,
        Background360
    }

    /// <summary>
    /// Enum of 360 video type
    /// </summary>
    public enum Video360Type
    {
        Sphere,
        Cube
    }
}