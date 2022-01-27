using System;
using System.Collections.Generic;
using Com.Immersive.Hotspots;
using Immersive.UserEditable.Enumerations;
using Immersive.UserEditable.Properties;
using UnityEngine;
using UnityEngine.UI;

namespace Immersive.UserEditable
{
    [UserEditable(typeof(HotspotScript))]
    public class UserEditableImagePopup : UserEditableHotspot
    {
        [HideInInspector, SerializeField] UserEditableCombinedImageProperty image = new UserEditableCombinedImageProperty("PopUp");
        [HideInInspector, SerializeField] UserEditableCombinedImageProperty border = new UserEditableCombinedImageProperty("PopUp Border");
        [HideInInspector, SerializeField] UserEditableCombinedImageProperty mask = new UserEditableCombinedImageProperty("PopUp Mask");

        [SerializeField] private ImageFlags imageFlags = ImageFlags.Image, borderFlags, maskFlags;

        public override List<UserEditableProperty> UserEditableProperties
        {
            get
            {
                List<UserEditableProperty> properties = new List<UserEditableProperty>();

                properties.Add(image, imageFlags);
                properties.Add(border, borderFlags);
                properties.Add(mask, maskFlags);

                return properties;
            }
        }

        protected override void Enable()
        {
            image.OnValueSetUpdateImageProperty(hotspotScript.imagePopUpDataModel.popUpSetting.background);
            border.OnValueSetUpdateImageProperty(hotspotScript.imagePopUpDataModel.popUpSetting.border);
            mask.OnValueSetUpdateImageProperty(hotspotScript.imagePopUpDataModel.popUpSetting.mediaMask);
        }

        protected override void SetDefaults()
        {
            image.SetDefaultValues(hotspotScript.imagePopUpDataModel.popUpSetting.background);
            border.SetDefaultValues(hotspotScript.imagePopUpDataModel.popUpSetting.border);
            mask.SetDefaultValues(hotspotScript.imagePopUpDataModel.popUpSetting.mediaMask);
        }
    }
}