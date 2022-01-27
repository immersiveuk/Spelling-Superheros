#if SDK1_EXTRAS
using Immersive.UserEditable.Properties;
using System.Collections.Generic;
using UnityEngine;

namespace Immersive.UserEditable
{
    [RequireComponent(typeof(PlayAudioOnSequencePopUpIndexChanged))]
    [UserEditable(typeof(PlayAudioOnSequencePopUpIndexChanged))]
    public class UserEditablePlaySoundOnSequencePopUpIndexChanged : UserEditableComponent<PlayAudioOnSequencePopUpIndexChanged>
    {
        [HideInInspector, SerializeField]
        private List<UserEditableAudioProperty> audioProperties = new List<UserEditableAudioProperty>();

        private void OnEnable()
        {
            
            for (int i = 0; i < audioProperties.Count; i++)
            {
                int index = i;
                audioProperties[i].ValueSet = () =>
                    {
                        Target.clips[index] = audioProperties[index].Value;
                    };
            }
        }

        public override void UpdateDynamicallyCreatedProperties()
        {
            SetPropertyListLength(audioProperties, Target.clips.Count,
                (index) => new UserEditableAudioProperty($"(Slide {index + 1}) Audio"));
        }

        public override List<UserEditableProperty> UserEditableProperties
        {
            get
            {
                List<UserEditableProperty> properties = new List<UserEditableProperty>();
                foreach (var property in audioProperties)
                    properties.Add(property);
                
                return properties;
            }
        }

        protected override void SetDefaultPropertyValues() {}
    }

}
#endif