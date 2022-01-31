using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Immersive.UserEditable.Properties;
using System;
using TMPro;
using Immersive.FillInTheBlank;

namespace Immersive.UserEditable
{
    public class UserEditableFillInTheBlank : UserEditableComponent
    {
        [HideInInspector, SerializeField] protected List<UserEditableSpellingProperty> spellingProperties = new List<UserEditableSpellingProperty>();

        public override List<UserEditableProperty> UserEditableProperties => GenerateUserEditablePropertiesFromGroups();

        FillInTheBlanksData fillInTheBlanksData;

        protected override UserEditableGroup[] CreateGroups()
        {
            List<UserEditableGroup> slideGroups = new List<UserEditableGroup>();

            foreach (var sp in spellingProperties)
            {
                slideGroups.AddRange(sp.CreateGroups());
            }

            return slideGroups.ToArray();
        }

        private void OnEnable()
        {
            SetupOnValueSetMethods();
        }

        protected override void SetDefaultPropertyValues()
        {
            fillInTheBlanksData = GetComponent<FillInTheBlanksData>();

            for (int i=0; i< fillInTheBlanksData.fillInTheBlanksList.spellings.Count; i++)
            {
                spellingProperties[i].SetDefaultPropertyValues(fillInTheBlanksData.fillInTheBlanksList.spellings[i]);
            }
        }

        public void SetupOnValueSetMethods()
        {
            fillInTheBlanksData = GetComponent<FillInTheBlanksData>();

            for (int i = 0; i < fillInTheBlanksData.fillInTheBlanksList.spellings.Count; i++)
            {
                spellingProperties[i].OnValueSetUpdateSpellingProperty(fillInTheBlanksData.fillInTheBlanksList.spellings[i]);                
            }
        }

        public override void UpdateDynamicallyCreatedProperties()
        {
            fillInTheBlanksData = GetComponent<FillInTheBlanksData>();

            SetPropertyListLength(spellingProperties, fillInTheBlanksData.fillInTheBlanksList.spellings.Count, NewSlide);

            for (int i = 0; i < spellingProperties.Count; i++)
            {
                spellingProperties[i].UpdateDynamicallyCreatedProperties(fillInTheBlanksData.fillInTheBlanksList.spellings[i]);
            }
        }

        protected UserEditableSpellingProperty NewSlide(int index) => new UserEditableSpellingProperty();
    }
}
