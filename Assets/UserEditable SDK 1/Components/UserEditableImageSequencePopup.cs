using Com.Immersive.Hotspots;
using Immersive.UserEditable;
using Immersive.UserEditable.Enumerations;
using Immersive.UserEditable.Properties;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Com.Immersive.Hotspots.ImageSequencePopUpDataModel;

namespace Immersive.userEditable
{
    [UserEditable(typeof(HotspotScript))]
    public class UserEditableImageSequencePopup : UserEditableHotspot
    {
        [HideInInspector, SerializeField]
        List<UserEditableImageSequenceSlide> userEditableImageSequenceProperties = new List<UserEditableImageSequenceSlide>();

        [HideInInspector, SerializeField]
        UserEditableImageProperty backgroundImage = new UserEditableImageProperty("Background Image");

        private ImageSequencePopUpSetting imageSequencePopUpSetting;

        public override List<UserEditableProperty> UserEditableProperties
        {
            get
            {
                List<UserEditableProperty> properties = new List<UserEditableProperty>();

                properties.Add(backgroundImage);

                for (int i = 0; i < userEditableImageSequenceProperties.Count; i++)
                {
                    properties.AddRange(userEditableImageSequenceProperties[i].UserEditableProperties);
                }

                return properties;
            }
        }

        public override void UpdateDynamicallyCreatedProperties()
        {
            SetImagePropertyLength();
        }

        private void SetImagePropertyLength()
        {
            imageSequencePopUpSetting = hotspotScript.imageSequencePopUpDataModel.popUpSetting;

            int slideCount = imageSequencePopUpSetting.Count;
            SetPropertyListLength(userEditableImageSequenceProperties, slideCount,
                (index) => new UserEditableImageSequenceSlide(index));
        }

        protected override void Enable()
        {
            imageSequencePopUpSetting = hotspotScript.imageSequencePopUpDataModel.popUpSetting;

            for (int i = 0; i < userEditableImageSequenceProperties.Count; i++)
            {
                int index = i;
                var imagePopup = userEditableImageSequenceProperties[index];
                imagePopup.OnValueSetUpdateSlide(imageSequencePopUpSetting, index);
            }
        }

        protected override void SetDefaults()
        {

        }
    }
}

