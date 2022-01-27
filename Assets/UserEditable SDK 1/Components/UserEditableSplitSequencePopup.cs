using Com.Immersive.Hotspots;
using Immersive.Enumerations;
using Immersive.UserEditable.Enumerations;
using Immersive.UserEditable.Properties;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Com.Immersive.Hotspots.SplitSequencePopUpSetting.SplitPopUp;

namespace Immersive.UserEditable
{
    [UserEditable(typeof(HotspotScript))]
    public class UserEditableSplitSequencePopup : UserEditableHotspot
    {
        [HideInInspector, SerializeField]
        List<UserEditableSplitSequenceSlide> userEditableSplitSequenceProperties = new List<UserEditableSplitSequenceSlide>();

        [HideInInspector, SerializeField] 
        UserEditableEnumProperty mediaTypeProperty = new UserEditableEnumProperty("Media Type");
        [HideInInspector, SerializeField] 
        UserEditableCombinedImageProperty mediaImageProperty = new UserEditableCombinedImageProperty("Media");
        [HideInInspector, SerializeField] 
        UserEditableVideoProperty mediaVideoProperty = new UserEditableVideoProperty("Media Video");

        [SerializeField] TextFlags titleFlags = TextFlags.Text, bodyFlags = TextFlags.Text;
        [SerializeField] bool showMediaOptions = true;
        [SerializeField] ImageFlags mediaImageFlags = ImageFlags.Image;

        private SplitSequencePopUpSetting splitSequencePopUpSetting;

        public override List<UserEditableProperty> UserEditableProperties => GenerateUserEditablePropertiesFromGroups();

        public override void UpdateDynamicallyCreatedProperties()
        {
            SetSplitPropertyLength();
        }

        protected override UserEditableGroup[] CreateGroups()
        {
            List<UserEditableGroup> groups = new List<UserEditableGroup>();

            splitSequencePopUpSetting = ((SplitSequenceHotspotPopUpSpawner)hotspotScript.customPopUpSpawner).PopUpSettings;
            
            bool isMediaUniversal = splitSequencePopUpSetting.keepSameMedia;
            if (isMediaUniversal && showMediaOptions)
            {
                UserEditableGroup mediaTypeGroup = new UserEditableGroup($"Media");
                UserEditableGroup imageGroup = new UserEditableGroup(false);
                imageGroup.SetConditional(mediaTypeProperty, MediaType.Image);
                UserEditableGroup videoGroup = new UserEditableGroup(false);
                videoGroup.SetConditional(mediaTypeProperty, MediaType.Video);

                mediaTypeGroup.Add(mediaTypeProperty);
                imageGroup.userEditableProperties.Add(mediaImageProperty, mediaImageFlags);
                videoGroup.Add(mediaVideoProperty);

                groups.Add(mediaTypeGroup);
                groups.Add(imageGroup);
                groups.Add(videoGroup);
            }

            for (int i = 0; i < userEditableSplitSequenceProperties.Count; i++)
            {
                var userEditable = userEditableSplitSequenceProperties[i];
                var slide = splitSequencePopUpSetting.splitPopups[i];

                var slideGroup = userEditable.CreateGroups(i, titleFlags, bodyFlags, mediaImageFlags, !isMediaUniversal && showMediaOptions, slide.includeTitle);

                groups.AddRange(slideGroup);
            }
            return groups.ToArray();
        }

        private void SetSplitPropertyLength()
        {

            splitSequencePopUpSetting = ((SplitSequenceHotspotPopUpSpawner)hotspotScript.customPopUpSpawner).PopUpSettings;

            int slideCount = splitSequencePopUpSetting.Count;
            SetPropertyListLength(userEditableSplitSequenceProperties, slideCount,
                (index) => new UserEditableSplitSequenceSlide(index));
        }

        protected override void Enable()
        {
            splitSequencePopUpSetting = ((SplitSequenceHotspotPopUpSpawner)hotspotScript.customPopUpSpawner).PopUpSettings;

            SplitSequencePopUpSetting.SplitPopUp initialSlide = splitSequencePopUpSetting.splitPopups[0];

            mediaTypeProperty.ValueSet = () => initialSlide.mediaType = (MediaType)mediaTypeProperty.Value;
            mediaImageProperty.OnValueSetUpdateImageProperty(initialSlide.image);
            mediaVideoProperty.ValueSet = () => mediaVideoProperty.ApplyTo(initialSlide.video);

            for (int i = 0; i < userEditableSplitSequenceProperties.Count; i++)
            {
                int index = i;
                var splitPopup = userEditableSplitSequenceProperties[index];

                splitPopup.OnValueSetUpdateSlide(splitSequencePopUpSetting.splitPopups[index]);
            }
        }

        protected override void SetDefaults()
        {
            splitSequencePopUpSetting = ((SplitSequenceHotspotPopUpSpawner)hotspotScript.customPopUpSpawner).PopUpSettings;

            bool isMediaUniversal = splitSequencePopUpSetting.keepSameMedia;

            if (isMediaUniversal)
            {
        
                mediaTypeProperty.SetDefaultValue(splitSequencePopUpSetting.splitPopups[0].mediaType);
                mediaImageProperty.SetDefaultValues(splitSequencePopUpSetting.splitPopups[0].image);
            }

            for (int i = 0; i < userEditableSplitSequenceProperties.Count; i++)
            {
                var slide = splitSequencePopUpSetting.splitPopups[i];
                userEditableSplitSequenceProperties[i].SetDefaultValues(slide);
            }
        }
    }
}