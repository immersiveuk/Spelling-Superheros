using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Immersive.UserEditable.Properties
{
    [System.Serializable]
    public class UserEditableQuizV2Property
    {
        [HideInInspector, SerializeField]
        public UserEditableStringProperty questionTextProperty;

        [HideInInspector, SerializeField]
        public List<UserEditableStringProperty> optionsTextProperty;

        [HideInInspector, SerializeField]
        public UserEditableStringProperty correctAnswerProperty;

        public UserEditableQuizV2Property(int index, int optionsCount)
        {
            questionTextProperty = new UserEditableStringProperty($"({ index }) Question");
            this.optionsTextProperty = CreateOptionProperties(index, optionsCount);
            correctAnswerProperty = new UserEditableStringProperty($"({ index }) Correct Answer");
        }

        public List<UserEditableProperty> GetProperties()
        {
            List<UserEditableProperty> properties = new List<UserEditableProperty>();

            properties.Add(questionTextProperty);

            properties.AddRange(optionsTextProperty);

            properties.Add(correctAnswerProperty);

            return properties;
        }

        private List<UserEditableStringProperty> CreateOptionProperties(int index, int optionsCount)
        {
            List<UserEditableStringProperty> optionsTextProperty = new List<UserEditableStringProperty>();

            for (int i = optionsTextProperty.Count; i < optionsCount; i++)
            {
                optionsTextProperty.Add(new UserEditableStringProperty($"({ index }) Option {i + 1}"));
            }

            return optionsTextProperty;
        }
    }
}