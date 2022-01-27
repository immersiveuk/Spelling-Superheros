using System.Collections;
using System.Collections.Generic;
using Com.Immersive.Hotspots;
using Immersive.UserEditable.Enumerations;
using Immersive.UserEditable.Properties;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

namespace Immersive.UserEditable
{
    [UserEditable(typeof(HotspotScript))]
    public class UserEditableVideoPopup : UserEditableHotspot
    {
        [HideInInspector, SerializeField]
        private UserEditableVideoProperty videoProperty = new UserEditableVideoProperty("Video Clip");

        [HideInInspector, SerializeField]
        private UserEditableImageProperty borderImageProperty = new UserEditableImageProperty("Video Border Image");

        [HideInInspector, SerializeField]
        private UserEditableEnumProperty borderTypeProperty = new UserEditableEnumProperty("Video Border Type");

        [HideInInspector, SerializeField]
        private UserEditableColorProperty borderColorProperty = new UserEditableColorProperty("Video Border Color");

        [HideInInspector, SerializeField]
        private UserEditableImageProperty maskImageProperty = new UserEditableImageProperty("Video Mask Image");

        [HideInInspector, SerializeField]
        private UserEditableEnumProperty maskTypeProperty = new UserEditableEnumProperty("Video Mask Type");

        [HideInInspector, SerializeField]
        private UserEditableColorProperty maskColorProperty = new UserEditableColorProperty("Video Mask Color");

        [SerializeField] private ImageFlags borderFlags;
        [SerializeField] private ImageFlags maskFlags;

        public override List<UserEditableProperty> UserEditableProperties
        {
            get
            {
                List<UserEditableProperty> properties = new List<UserEditableProperty>();

                properties.Add(videoProperty);

                if (borderFlags.HasFlag(ImageFlags.Image))
                {
                    properties.Add(borderImageProperty);
                    properties.Add(borderTypeProperty);
                }

                if (borderFlags.HasFlag(ImageFlags.Color))
                {
                    properties.Add(borderColorProperty);
                }

                if (maskFlags.HasFlag(ImageFlags.Image))
                {
                    properties.Add(maskImageProperty);
                    properties.Add(maskTypeProperty);
                }

                if (maskFlags.HasFlag(ImageFlags.Color))
                {
                    properties.Add(maskColorProperty);
                }

                return properties;
            }
        }

        protected override void Enable()
        {
            videoProperty.ValueSet = OnVideoPropertyChanged;
            borderImageProperty.ValueSet = OnBorderImagePropertyChanged;
            borderTypeProperty.ValueSet = OnBorderTypePropertyChanged;
            borderColorProperty.ValueSet = OnBorderColorPropertyChanged;
            maskImageProperty.ValueSet = OnMaskImagePropertyChanged;
            maskTypeProperty.ValueSet = OnMaskTypePropertyChanged;
            maskColorProperty.ValueSet = OnMaskColorPropertyChanged;
        }

        protected override void SetDefaults()
        {
            borderTypeProperty.SetDefaultValue(hotspotScript.videoPopUpDataModel.popUpSetting.border.type);
            borderColorProperty.SetDefaultValue(hotspotScript.videoPopUpDataModel.popUpSetting.border.color);
            maskTypeProperty.SetDefaultValue(hotspotScript.videoPopUpDataModel.popUpSetting.mediaMask.type);
            maskColorProperty.SetDefaultValue(hotspotScript.videoPopUpDataModel.popUpSetting.mediaMask.color);
        }

        private void OnVideoPropertyChanged()
        {
            hotspotScript.videoPopUpDataModel.popUpSetting.video.videoURL = videoProperty.URL;
            hotspotScript.videoPopUpDataModel.popUpSetting.video.videoSource = VideoSource.Url;
        }

        private void OnBorderImagePropertyChanged()
        {
            if (!borderFlags.HasFlag(ImageFlags.Image)) return;

            hotspotScript.videoPopUpDataModel.popUpSetting.border.sprite = borderImageProperty.Value;
        }

        private void OnBorderTypePropertyChanged()
        {
            if (!borderFlags.HasFlag(ImageFlags.Image)) return;

            hotspotScript.videoPopUpDataModel.popUpSetting.border.type = (Image.Type) borderTypeProperty.Value;
        }

        private void OnBorderColorPropertyChanged()
        {
            if (!borderFlags.HasFlag(ImageFlags.Color)) return;

            hotspotScript.videoPopUpDataModel.popUpSetting.border.color = borderColorProperty.Value;
        }

        private void OnMaskImagePropertyChanged()
        {
            if (!maskFlags.HasFlag(ImageFlags.Image)) return;

            hotspotScript.videoPopUpDataModel.popUpSetting.mediaMask.sprite = maskImageProperty.Value;
        }

        private void OnMaskTypePropertyChanged()
        {
            if (!maskFlags.HasFlag(ImageFlags.Image)) return;

            hotspotScript.videoPopUpDataModel.popUpSetting.mediaMask.type = (Image.Type) maskTypeProperty.Value;
        }

        private void OnMaskColorPropertyChanged()
        {
            if (!maskFlags.HasFlag(ImageFlags.Color)) return;

            hotspotScript.videoPopUpDataModel.popUpSetting.mediaMask.color = maskColorProperty.Value;
        }
    }
}