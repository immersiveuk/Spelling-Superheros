using Com.Immersive.Hotspots;
using Immersive.UserEditable.Enumerations;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using static Com.Immersive.Hotspots.SplitSequencePopUpSetting.SplitPopUp;

namespace Immersive.UserEditable.Properties
{
    [System.Serializable]
    public class UserEditableSplitSequenceSlide
    {
        [HideInInspector, SerializeField] UserEditableTextProperty titleProperty;
        [HideInInspector, SerializeField] UserEditableTextProperty bodyProperty;

        [HideInInspector, SerializeField] UserEditableEnumProperty mediaTypeProperty;
        [HideInInspector, SerializeField] UserEditableCombinedImageProperty mediaImageProperty;
        [HideInInspector, SerializeField] UserEditableVideoProperty mediaVideoProperty;

        public UserEditableSplitSequenceSlide(int index)
        {
            string slideName = $"(Slide {index + 1})";
            titleProperty = new UserEditableTextProperty($"{slideName} Title");
            bodyProperty = new UserEditableTextProperty($"{slideName} Body");

            mediaTypeProperty = new UserEditableEnumProperty($"{slideName} Media Type");
            mediaImageProperty = new UserEditableCombinedImageProperty($"{slideName} Image Media");
            mediaVideoProperty = new UserEditableVideoProperty($"{slideName} Video Media");
        }

        public UserEditableGroup[] CreateGroups(int index, TextFlags titleFlags, TextFlags bodyFlags, ImageFlags imageFlags, bool includeMediaOptions, bool includeTitle)
        {
            List<UserEditableGroup> groups = new List<UserEditableGroup>();
            string slideName = $"(Slide {index + 1})";

            UserEditableGroup textGroup = new UserEditableGroup($"{slideName}");
            textGroup.inline = false;

            UserEditableGroup mediaTypeGroup = new UserEditableGroup($"{slideName} Media");
            UserEditableGroup imageGroup = new UserEditableGroup(false);
            imageGroup.SetConditional(mediaTypeProperty, MediaType.Image);
            
            UserEditableGroup videoGroup = new UserEditableGroup(false);
            videoGroup.SetConditional(mediaTypeProperty, MediaType.Video);

            if (includeTitle)
                textGroup.userEditableProperties.Add(titleProperty, titleFlags);

            textGroup.userEditableProperties.Add(bodyProperty, bodyFlags);
            groups.Add(textGroup);

            if (includeMediaOptions)
            {
                mediaTypeGroup.Add(mediaTypeProperty);
                imageGroup.userEditableProperties.Add(mediaImageProperty, imageFlags);
                videoGroup.Add(mediaVideoProperty);

                groups.Add(mediaTypeGroup);
                groups.Add(imageGroup);
                groups.Add(videoGroup);
            }


            return groups.ToArray();
        }

        public void OnValueSetUpdateSlide(SplitSequencePopUpSetting.SplitPopUp popUpSlide)
        {
            titleProperty.OnValueSetUpdateTextProperty(popUpSlide.title);
            bodyProperty.OnValueSetUpdateTextProperty(popUpSlide.body);
            mediaTypeProperty.ValueSet = () => popUpSlide.mediaType = (SplitSequencePopUpSetting.SplitPopUp.MediaType)mediaTypeProperty.Value;
            mediaImageProperty.OnValueSetUpdateImageProperty(popUpSlide.image);
            mediaVideoProperty.ValueSet = () => mediaVideoProperty.ApplyTo(popUpSlide.video);
        }

        public void SetDefaultValues(SplitSequencePopUpSetting.SplitPopUp popUpSlide)
        {
            titleProperty.SetDefaultValues(popUpSlide.title);
            bodyProperty.SetDefaultValues(popUpSlide.body);
            mediaTypeProperty.SetDefaultValue(popUpSlide.mediaType);
            mediaImageProperty.SetDefaultValues(popUpSlide.image);
        }
    }
}