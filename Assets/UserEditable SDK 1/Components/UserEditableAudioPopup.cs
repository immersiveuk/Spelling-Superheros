using System.Collections.Generic;
using Com.Immersive.Hotspots;
using Immersive.UserEditable.Enumerations;
using Immersive.UserEditable.Properties;
using UnityEngine;

namespace Immersive.UserEditable
{
    [UserEditable(typeof(HotspotScript))]
    public class UserEditableAudioPopup : UserEditableHotspot
    {
        [HideInInspector, SerializeField] private UserEditableAudioProperty audioProperty = new UserEditableAudioProperty("Popup Audio");
        [HideInInspector, SerializeField] private UserEditableImageProperty imageProperty = new UserEditableImageProperty("Audio Thumbnail");

        [SerializeField] private AudioFlags audioFlags;

        protected override void Enable()
        {
            audioProperty.ValueSet = OnAudioPropertyChanged;
            imageProperty.ValueSet = OnImagePropertyChanged;
        }

        protected override void SetDefaults()
        {
            // Cant actually set the defaults for assets
        }

        public override List<UserEditableProperty> UserEditableProperties
        {
            get
            {
                List<UserEditableProperty> properties = new List<UserEditableProperty>();
                
                properties.Add(audioProperty);

                if (audioFlags.HasFlag(AudioFlags.Thumbnail) || hotspotScript.audioPopUpDataModel.popUpSetting.useThumbnail)
                {
                    properties.Add(imageProperty);
                }

                return properties;
            }
        }

        private void OnAudioPropertyChanged()
        {
            hotspotScript.audioPopUpDataModel.popUpSetting.audioClip = audioProperty.Value;
        }

        private void OnImagePropertyChanged()
        {
            if (!audioFlags.HasFlag(AudioFlags.Thumbnail) || !hotspotScript.audioPopUpDataModel.popUpSetting.useThumbnail) return;

            hotspotScript.audioPopUpDataModel.popUpSetting.thumbnail.sprite = imageProperty.Value;
        }
    }
}