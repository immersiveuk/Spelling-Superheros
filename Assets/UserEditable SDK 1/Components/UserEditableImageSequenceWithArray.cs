using Com.Immersive.Hotspots;
using Immersive.Properties;
using Immersive.UserEditable.Properties;
using System;
using System.Collections.Generic;
using UnityEngine;
using static Com.Immersive.Hotspots.ImageSequencePopUpDataModel;

namespace Immersive.UserEditable
{
    [UserEditable(typeof(HotspotScript))]
    public class UserEditableImageSequenceWithArray : UserEditableHotspot
    {

        [SerializeField]
        UserEditableArray<Slide> slides = new UserEditableArray<Slide>("Slides");
        [SerializeField]
        Slide slideTemplate = new Slide(Color.red);

        public override List<UserEditableProperty> UserEditableProperties =>
            new List<UserEditableProperty> { slides };

        private ImageSequencePopUpSetting imageSequencePopUpSetting;

        protected override void Enable()
        {
            imageSequencePopUpSetting = hotspotScript.imageSequencePopUpDataModel.popUpSetting;

            slides.ValueSet = () =>
            {
                imageSequencePopUpSetting.backgroundSprites = new List<ImageProperty>();
                for (int i = 0; i < slides.Count; i++)
                {
                    var imageProperty = new ImageProperty();
                    imageProperty.color = slides[i].combinedImage.colorProperty;
                    imageProperty.sprite = slides[i].combinedImage.imageProperty;
                    imageSequencePopUpSetting.backgroundSprites.Add(imageProperty);
                }
            };
        }

        protected override void SetDefaults()
        {
            slides.SetTemplate(slideTemplate);
        }

        [Serializable]
        private class Slide : UserEditableArrayElement
        {
            public UserEditableCombinedImageProperty combinedImage = new UserEditableCombinedImageProperty("Image");

            [SerializeField] Color defaultColour;

            public Slide(Color defaultColour)
            {
                this.defaultColour = defaultColour;
            }

            public override List<UserEditableDiscreteProperty> UserEditableProperties
            {
                get
                {
                    List<UserEditableDiscreteProperty> properties = new List<UserEditableDiscreteProperty>();
                    properties.Add(combinedImage.imageProperty);
                    properties.Add(combinedImage.colorProperty);
                    return properties;
                }
            }

            public override void SetDefaultPropertyValues()
            {
                combinedImage.colorProperty.SetDefaultValue(defaultColour);
            }
        }
    }

}