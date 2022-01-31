using Immersive.FillInTheBlank;
using Immersive.UserEditable.Properties;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Immersive.UserEditable
{
    [System.Serializable]
    public class UserEditableMissingLettersPair : UserEditableCompositeProperty
    {
        [HideInInspector, SerializeField] public UserEditableIntProperty startIndexProperty;
        [HideInInspector, SerializeField] public UserEditableIntProperty endIndexProperty;

        public UserEditableMissingLettersPair(int pairIndex)
        {
            startIndexProperty = new UserEditableIntProperty($"Pair {pairIndex + 1} Start Index");
            endIndexProperty = new UserEditableIntProperty($"Pair {pairIndex + 1} End Index");
        }

        public List<UserEditableProperty> GetPropertiesList()
        {
            List<UserEditableProperty> properties = new List<UserEditableProperty>();

            properties.Add(startIndexProperty);
            properties.Add(endIndexProperty);

            return properties;
        }

        public void SetDefaultPropertyValues(MissingLettersPair pair)
        {
            startIndexProperty.SetDefaultValue(pair.startIndex);
            endIndexProperty.SetDefaultValue(pair.endIndex);
        }

        public void OnValueSetUpdateMissingPairProperty(MissingLettersPair pair, System.Action onValueSet = null)
        {
            startIndexProperty.OnValueSet = pair.SetStartIndex;
            endIndexProperty.OnValueSet = pair.SetEndIndex;

            onValueSet?.Invoke();
        }
    }
}
