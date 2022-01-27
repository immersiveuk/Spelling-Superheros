using Immersive.UserEditable.Properties;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Immersive.UserEditable
{
    [RequireComponent(typeof(PlaySoundOnHotspotActivated))]
    [UserEditable(typeof(PlaySoundOnHotspotActivated))]
    public class UserEditablePlaySoundOnHotspotActivated : UserEditableComponent<PlaySoundOnHotspotActivated>
    {
        [HideInInspector, SerializeField]
        private UserEditableAudioProperty audioProperty = new UserEditableAudioProperty("Audio");

        private void OnEnable()
        {
            audioProperty.ValueSet += AudioChanged;
        }

        public override List<UserEditableProperty> UserEditableProperties
        {
            get
            {
                var properties = new List<UserEditableProperty>();
                properties.Add(audioProperty);
                return properties;
            }
        }

        protected override void SetDefaultPropertyValues() { }

        private void AudioChanged()
        {
            Target.clip = audioProperty.Value;
        }
    }
}