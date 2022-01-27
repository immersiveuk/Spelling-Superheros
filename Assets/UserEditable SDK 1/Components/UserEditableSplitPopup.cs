using System;
using System.Collections.Generic;
using Com.Immersive.Hotspots;
using Immersive.UserEditable.Enumerations;
using Immersive.UserEditable.Properties;
using UnityEngine;

namespace Immersive.UserEditable
{
    [UserEditable(typeof(HotspotScript))]
    public class UserEditableSplitPopup : UserEditableHotspot
    {
        [SerializeField] private UserEditableFontData fontData;
        
        [SerializeField] private TextFlags titleFlags = TextFlags.Text, bodyFlags = TextFlags.Text;
        [SerializeField] bool showMediaOptions = true;
        [SerializeField] ImageFlags mediaImageFlags = ImageFlags.Image;

        [HideInInspector, SerializeField]
        UserEditableTextProperty titleProperty = new UserEditableTextProperty("Title");
        [HideInInspector, SerializeField]
        UserEditableTextProperty bodyProperty = new UserEditableTextProperty("Body");

        [HideInInspector, SerializeField] 
        UserEditableEnumProperty mediaTypeProperty = new UserEditableEnumProperty("Media Type");
        [HideInInspector, SerializeField] 
        UserEditableCombinedImageProperty mediaImageProperty = new UserEditableCombinedImageProperty("Image Media");
        [HideInInspector, SerializeField] 
        UserEditableVideoProperty mediaVideoProperty = new UserEditableVideoProperty("Video Media");

        public override List<UserEditableProperty> UserEditableProperties
        {
            get
            {
                List<UserEditableProperty> properties = new List<UserEditableProperty>();

                if (hotspotScript.splitPopupDataModel.popUpSetting.includeTitle)
                    properties.Add(titleProperty, titleFlags);

                properties.Add(bodyProperty, bodyFlags);

                if (showMediaOptions)
                {
                    properties.Add(mediaTypeProperty);
                    properties.Add(mediaImageProperty, mediaImageFlags);
                    properties.Add(mediaVideoProperty);
                }
                return properties;
            }
        }

        protected override void Enable()
        {
            if (fontData != null)
            {
                FontLoaded();
                fontData.FontLoaded += FontLoaded;
            }

            var popUpSettings = hotspotScript.splitPopupDataModel.popUpSetting;
            titleProperty.OnValueSetUpdateTextProperty(popUpSettings.title);
            bodyProperty.OnValueSetUpdateTextProperty(popUpSettings.body);
            mediaTypeProperty.ValueSet = () => popUpSettings.mediaType = (SplitPopUpDataModel.SplitPopUpSetting.MediaType)mediaTypeProperty.Value;
            mediaImageProperty.OnValueSetUpdateImageProperty(popUpSettings.image);
            mediaVideoProperty.ValueSet = () => mediaVideoProperty.ApplyTo(popUpSettings.video);
        }

        private void OnDestroy()
        {
            if (fontData != null)
            {
                fontData.FontLoaded -= FontLoaded;
            }
        }

        private void FontLoaded()
        {
            
            if (fontData.LoadedFont == null) return;

            hotspotScript.splitPopupDataModel.popUpSetting.isRightToLeftText = fontData.UseRTL;
            hotspotScript.splitPopupDataModel.popUpSetting.body.Font = fontData.LoadedFont;
            hotspotScript.splitPopupDataModel.popUpSetting.title.Font = fontData.LoadedFont;
        }
        
        protected override void SetDefaults()
        {
            var popUpSettings = hotspotScript.splitPopupDataModel.popUpSetting;
            titleProperty.SetDefaultValues(popUpSettings.title);
            bodyProperty.SetDefaultValues(popUpSettings.body);
            mediaTypeProperty.SetDefaultValue(popUpSettings.mediaType);
            mediaImageProperty.SetDefaultValues(popUpSettings.image);

        }

        public override void SetTooltips()
        {
            mediaTypeProperty.SetTooltip("Whether the PopUp should display and Image or a Video.");
            mediaVideoProperty.SetTooltip("Video to be displayed in PopUp.");
        }
    }
}