using Immersive.Properties;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using static CustomJsonConverter;

namespace Com.Immersive.Hotspots
{
    [System.Serializable]
    public abstract class PopUpSequenceSettings : PopUpSettings
    {
        public AudioProperty indexChangedAudioClip;

        [FormerlySerializedAs("customButtons")]
        public bool useCustomButtons;

        [JsonConverter(typeof(StringEnumConverter))]// parse Json object into enum value
        public ControlPanelStyle controlPanelStyle;

        [JsonConverter(typeof(StringImageConverter))]// parse Json object into Image property
        public ImageProperty nextButton;

        [JsonConverter(typeof(StringImageConverter))]// parse Json object into Image property
        public ImageProperty previousButton;

        public bool ShouldGlowToNextButton => addGlowToButtons && !ControlPanelAlwaysShowsClose;
        private bool ControlPanelAlwaysShowsClose => controlPanelStyle == ControlPanelStyle.Full || controlPanelStyle == ControlPanelStyle.ForwardAndClose;

        public enum ControlPanelStyle { Full, ForwardAndClose, ForwardOnly, ForwardAndBack };

        public abstract int Count { get; }
    }

}