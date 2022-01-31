using Immersive.FillInTheBlank;
using Immersive.UserEditable.Properties;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Immersive.UserEditable
{
    [System.Serializable]
    public class UserEditableSpellingProperty : UserEditableCompositeProperty
    {
        [HideInInspector, SerializeField] public UserEditableStringProperty spellingValueProperty;
        [HideInInspector, SerializeField] public List<UserEditableMissingLettersPair> missingLettersPairs;

        public UserEditableSpellingProperty()
        {
            spellingValueProperty = new UserEditableStringProperty("Spelling");
            missingLettersPairs = new List<UserEditableMissingLettersPair>();
        }

        public List<UserEditableGroup> CreateGroups()
        {
            var spellingGroup = new UserEditableGroup($"{spellingValueProperty.Value} Spelling Settings");
            spellingGroup.inline = true;
            spellingGroup.Add(spellingValueProperty);

            foreach (var pair in missingLettersPairs)
            {
                spellingGroup.Add(pair.startIndexProperty);
                spellingGroup.Add(pair.endIndexProperty);
            }

            return new List<UserEditableGroup> { spellingGroup };
        }

        public void SetDefaultPropertyValues(SpellingSettings setting)
        {
            spellingValueProperty.SetDefaultValue(setting.spelling);

            for (int i = 0; i < missingLettersPairs.Count; i++)
            {
                missingLettersPairs[i].SetDefaultPropertyValues(setting.missingLettersPairs[i]);
            }
        }

        public void OnValueSetUpdateSpellingProperty(SpellingSettings property, Action onValueSet = null)
        {
            spellingValueProperty.OnValueSet = property.SetSpelling;

            for (int i = 0; i < missingLettersPairs.Count; i++)
            {
                missingLettersPairs[i].OnValueSetUpdateMissingPairProperty(property.missingLettersPairs[i], onValueSet);
            }
        }

        public void UpdateDynamicallyCreatedProperties(SpellingSettings setting)
        {
            SetPropertyListLength(missingLettersPairs, setting.missingLettersPairs.Count, NewSlide);
        }

        protected void SetPropertyListLength<T>(List<T> properties, int targetLength, Func<int, T> propertyCreator)
        {
            if (properties.Count > targetLength)
                properties.RemoveRange(targetLength, properties.Count - targetLength);

            for (int i = properties.Count; i < targetLength; i++)
                properties.Add(propertyCreator(i));
        }

        protected UserEditableMissingLettersPair NewSlide(int index) => new UserEditableMissingLettersPair(index);
    }
}