using Immersive.UserEditable;
using Immersive.UserEditable.Enumerations;
using Immersive.UserEditable.Properties;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Com.Immersive.Hotspots.ImageSequencePopUpDataModel;

namespace Immersive.UserEditable.Properties
{
    [System.Serializable]
    public class UserEditableImageSequenceSlide
    {
        [HideInInspector, SerializeField] UserEditableImageProperty imageProperty;

        public List<UserEditableProperty> UserEditableProperties
        {
            get
            {
                List<UserEditableProperty> properties = new List<UserEditableProperty>();

                properties.Add(imageProperty);

                return properties;
            }
        }

        public UserEditableImageSequenceSlide(int index)
        {
            string slideName = $"(Slide {index + 1})";

            imageProperty = new UserEditableImageProperty($"{slideName} Image");
        }

        public void OnValueSetUpdateSlide(ImageSequencePopUpSetting popUpSlide, int index)
        {
            imageProperty.ValueSet = () => popUpSlide.backgroundSprites[index].sprite = imageProperty.Value;
        }
    }
}
