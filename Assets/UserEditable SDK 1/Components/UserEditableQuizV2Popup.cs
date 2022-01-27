using System;
using Immersive.UserEditable.Properties;
using System.Collections.Generic;
using UnityEngine;
using Com.Immersive.Hotspots;

namespace Immersive.UserEditable
{

    [UserEditable(typeof(HotspotScript))]
    public class UserEditableQuizV2Popup : UserEditableHotspot
    {
        [SerializeField] private UserEditableFontData fontData;
        
        //only text
        [HideInInspector, SerializeField]
        private List<UserEditableQuizV2Property> userEditableQuizV2Properties = new List<UserEditableQuizV2Property>();

        public override List<UserEditableProperty> UserEditableProperties
        {
            get
            {
                List<UserEditableProperty> properties = new List<UserEditableProperty>();

                CreateQuestionsProperties();

                foreach (var quizV2Property in userEditableQuizV2Properties)
                {
                    properties.AddRange(quizV2Property.GetProperties());
                }                

                return properties;
            }
        }

        private void CreateQuestionsProperties()
        {
            int questionsCount = hotspotScript.quizPopUpDataModel_V2.questions.Count;

            if (userEditableQuizV2Properties.Count < questionsCount)
            {
                for (int i = userEditableQuizV2Properties.Count; i < questionsCount; i++)
                {
                    UserEditableQuizV2Property quizV2Property = new UserEditableQuizV2Property(i, hotspotScript.quizPopUpDataModel_V2.questions[i].options.options.Count);
                    userEditableQuizV2Properties.Add(quizV2Property);
                }
            }

            if (userEditableQuizV2Properties.Count > questionsCount)
            {
                userEditableQuizV2Properties.RemoveRange(questionsCount, userEditableQuizV2Properties.Count - questionsCount);
            }
        }

        protected override void Enable()
        {
            if (fontData != null)
            {
                FontLoaded();
                fontData.FontLoaded += FontLoaded;
            }

            int questionIndex = 0;

            foreach (var quiz in userEditableQuizV2Properties)
            {
                int index = questionIndex;
                quiz.questionTextProperty.ValueSet = () => OnQuestionTextChanged(index);
                quiz.correctAnswerProperty.ValueSet = () => OnCorrectAnswerTextChanged(index);

                for (int i = 0; i < quiz.optionsTextProperty.Count; i++)
                {
                    int optionIndex = i;
                    quiz.optionsTextProperty[i].ValueSet = () => OnOptionChanged(index, optionIndex);
                }

                questionIndex++;
            }
        }

        private void OnDestroy()
        {
            if (fontData != null)
            {
                fontData.FontLoaded -= FontLoaded;
            }
        }

        protected override void SetDefaults()
        {
            for (int i = 0; i < userEditableQuizV2Properties.Count; i++)
            {
                userEditableQuizV2Properties[i].questionTextProperty.SetDefaultValue(hotspotScript.quizPopUpDataModel_V2.questions[i].question.Text);

                SetDefaultPropertyValueOptions(i);

                userEditableQuizV2Properties[i].correctAnswerProperty.SetDefaultValue(hotspotScript.quizPopUpDataModel_V2.questions[i].options.correctAnswer);
            }
        }

        private void SetDefaultPropertyValueOptions(int index)
        {
            for (int i = 0; i < userEditableQuizV2Properties[index].optionsTextProperty.Count; i++)
            {
                userEditableQuizV2Properties[index].optionsTextProperty[i]
                    .SetDefaultValue(hotspotScript.quizPopUpDataModel_V2.questions[index].options.options[i]);
            }
        }

        #region Property Changed Callbacks

        private void OnQuestionTextChanged(int index)
        {
            hotspotScript.quizPopUpDataModel_V2.questions[index].question.Text = userEditableQuizV2Properties[index].questionTextProperty.Value;
        }       

        private void OnCorrectAnswerTextChanged(int index)
        {
            hotspotScript.quizPopUpDataModel_V2.questions[index].options.correctAnswer = userEditableQuizV2Properties[index].correctAnswerProperty.Value;
        }

        private void OnOptionChanged(int index, int option)
        {
            hotspotScript.quizPopUpDataModel_V2.questions[index].options.options[option] = userEditableQuizV2Properties[index].optionsTextProperty[option].Value;
        }        

        #endregion

        private void FontLoaded()
        {
            if (fontData != null && fontData.LoadedFont == null) return;

            hotspotScript.quizPopUpDataModel_V2.isRightToLeftText = fontData.UseRTL;
            hotspotScript.quizPopUpDataModel_V2.font = fontData.LoadedFont;
        }
    }
}