using System.Collections;
using System.Collections.Generic;
using Immersive.Properties;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using UnityEngine;
using UnityEngine.Serialization;
using static Com.Immersive.Hotspots.HotspotScript;
using static CustomJsonConverter;

namespace Com.Immersive.Hotspots
{
    [System.Serializable]
    public abstract class PopUpSettings
    {
        public int controlPanelWidth;
        [JsonConverter(typeof(StringEnumConverter))]// parse Json object into enum value
        public PopUpPosition popUpPosition = PopUpPosition.SurfaceCenter; //PopUp Setting

        public Vector2 popUpPositionOffset = new Vector2(0, 0); //PopUp Setting

        public ControlPanelPositionOption controlPanelSide = ControlPanelPositionOption.Default;

        public bool overrideDefaultCloseButton;

        public bool addGlowToButtons = false;
        [ColorUsage(false)]
        public Color glowColor = Color.white;

        [JsonConverter(typeof(StringImageConverter))]// Parse Json object into Image property
        public ImageProperty closeButton;

        [JsonConverter(typeof(StringEnumConverter))]// parse Json object into enum value
        public SizeOption sizeOption;

        public Vector2 size = new Vector2(500,500);

        [Range(1, 100)]
        public int percentage;

        [CustomPadding]
        public RectOffset padding;

        public PopUpPositioner GetPopUpPositioner(Camera cam, Canvas canvas, Vector3 worldPos)
        {
            switch (popUpPosition)
            {
                case PopUpPosition.SurfaceCenterTop:
                    return new PopUpPositionerCentredOnCanvasTop(canvas);
                case PopUpPosition.SurfaceCenter:
                    return new PopUpPositionerCentredOnCanvas(canvas);
                case PopUpPosition.SameAsHotspot:
                    return new PopUpPositionerCentredOnHotspot(worldPos, cam, canvas);
                case PopUpPosition.Custom:
                    return new PopUpPositionerOffsetFromHotspot(worldPos, cam, popUpPositionOffset, canvas);
                default:
                    Debug.LogError("Could create IPopUpPositioner for " + popUpPosition);
                    return null;
            }
        }
    }
}