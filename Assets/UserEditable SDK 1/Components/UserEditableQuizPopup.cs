using System;
using System.Collections.Generic;
using Com.Immersive.Hotspots;
using Immersive.UserEditable.Enumerations;
using Immersive.UserEditable.Properties;
using UnityEngine;

namespace Immersive.UserEditable
{
    [UserEditable(typeof(HotspotScript))]
    public class UserEditableQuizPopup : UserEditableHotspot
    {
        [SerializeField] private UserEditableFontData fontData;
        
        [HideInInspector, SerializeField]
        private UserEditableStringProperty questionTextProperty = new UserEditableStringProperty("Question");

        [HideInInspector, SerializeField]
        private UserEditableFloatProperty questionSizeProperty = new UserEditableFloatProperty("Question Size");

        [HideInInspector, SerializeField]
        private UserEditableColorProperty questionColorProperty = new UserEditableColorProperty("Question Colour");

        [HideInInspector, SerializeField]
        private List<UserEditableStringProperty> optionsTextProperties = new List<UserEditableStringProperty>();

        [HideInInspector, SerializeField]
        private UserEditableFloatProperty optionSizeProperty = new UserEditableFloatProperty("Option Size");

        [HideInInspector, SerializeField]
        private UserEditableColorProperty optionColorProperty = new UserEditableColorProperty("Option Colour");

        [HideInInspector, SerializeField]
        private UserEditableStringProperty correctAnswerProperty = new UserEditableStringProperty("Correct Answer");

        [HideInInspector, SerializeField] private UserEditableStringProperty correctResultTextProperty =
            new UserEditableStringProperty("Correct Result Message");

        [HideInInspector, SerializeField] private UserEditableStringProperty incorrectResultTextProperty =
            new UserEditableStringProperty("Incorrect Result Message");

        [HideInInspector, SerializeField] private UserEditableAudioProperty correctResultAudioProperty =
            new UserEditableAudioProperty("Correct Result Audio");

        [HideInInspector, SerializeField] private UserEditableAudioProperty incorrectResultAudioProperty =
            new UserEditableAudioProperty("Incorrect Result Audio");

        [SerializeField]
        private UserEditableStringProperty tryAgainTextProperty =
            new UserEditableStringProperty("Try Again Text");

        [SerializeField] private TextFlags questionFlags;
        [SerializeField] private TextFlags optionFlags;
        
        [SerializeField, Tooltip("Enabling this will let user editables modify correct/incorrect result text")]
        private bool resultText;

        [SerializeField, Tooltip("Enabling this will let user editables modify correct/incorrect result audio")]
        private bool resultAudio;

        [SerializeField] bool includeTryAgainText = false;

        protected override void Enable()
        {
            if (fontData != null)
            {
                FontLoaded();
                fontData.FontLoaded += FontLoaded;
            }

            questionTextProperty.ValueSet = OnQuestionTextChanged;
            questionTextProperty.ValueSet = OnQuestionTextChanged;
            questionSizeProperty.ValueSet = OnQuestionSizeChanged;
            questionColorProperty.ValueSet = OnQuestionColorChanged;
            correctAnswerProperty.ValueSet = OnCorrectAnswerTextChanged;
            optionSizeProperty.ValueSet = OnOptionSizeChanged;
            optionColorProperty.ValueSet = OnOptionColorChanged;
            correctResultTextProperty.ValueSet = OnCorrectResultTextChanged;
            incorrectResultTextProperty.ValueSet = OnIncorrectResultTextChanged;
            correctResultAudioProperty.ValueSet = OnCorrectResultAudioChanged;
            incorrectResultAudioProperty.ValueSet = OnIncorrectResultAudioChanged;
            tryAgainTextProperty.OnValueSet = newValue
                => hotspotScript.quizPopUpDataModel.popUpSetting.tryAgainText = newValue;
            
            for (int i = 0; i < optionsTextProperties.Count; i++)
            {
                int index = i;
                optionsTextProperties[i].ValueSet = () => OnOptionChanged(index);
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

            if (hotspotScript.hotspotDataModel.actionType != ActionType.QuizPopup) return;

            questionTextProperty.SetDefaultValue(hotspotScript.quizPopUpDataModel.popUpSetting.question.Text);
            questionSizeProperty.SetDefaultValues(hotspotScript.quizPopUpDataModel.popUpSetting.question.FontSize);
            questionColorProperty.SetDefaultValue(hotspotScript.quizPopUpDataModel.popUpSetting.question.Color);

            SetDefaultPropertyValueOptions();

            optionSizeProperty.SetDefaultValues(hotspotScript.quizPopUpDataModel.popUpSetting.options.size);
            optionColorProperty.SetDefaultValue(hotspotScript.quizPopUpDataModel.popUpSetting.options.color);
            correctAnswerProperty.SetDefaultValue(hotspotScript.quizPopUpDataModel.popUpSetting.options
                .correctAnswer);

            correctResultTextProperty.SetDefaultValue(hotspotScript.quizPopUpDataModel.popUpSetting.result
                .correctAnswer.Text);
            incorrectResultTextProperty.SetDefaultValue(hotspotScript.quizPopUpDataModel.popUpSetting.result
                .incorrectAnswer.Text);

            tryAgainTextProperty.SetDefaultValue(hotspotScript.quizPopUpDataModel.popUpSetting.tryAgainText);
        }

        public override List<UserEditableProperty> UserEditableProperties
        {
            get
            {
                List<UserEditableProperty> properties = new List<UserEditableProperty>();

                if (hotspotScript.hotspotDataModel.actionType != ActionType.QuizPopup) return properties;
                
                properties.Add(questionTextProperty);
                if (questionFlags.HasFlag(TextFlags.Size)) properties.Add(questionSizeProperty);
                if (questionFlags.HasFlag(TextFlags.Color)) properties.Add(questionColorProperty);

                properties.AddRange(optionsTextProperties);

                if (optionFlags.HasFlag(TextFlags.Size)) properties.Add(optionSizeProperty);
                if (optionFlags.HasFlag(TextFlags.Color)) properties.Add(optionColorProperty);

                properties.Add(correctAnswerProperty);

                if (resultText)
                {
                    properties.Add(correctResultTextProperty);
                    properties.Add(incorrectResultTextProperty);
                }

                if (resultAudio)
                {
                    properties.Add(correctResultAudioProperty);
                    properties.Add(incorrectResultAudioProperty);
                }

                if (includeTryAgainText)
                    properties.Add(tryAgainTextProperty);

                return properties;
            }
        }

        #region Property Changed Callbacks

        private void OnQuestionTextChanged()
        {
            hotspotScript.quizPopUpDataModel.popUpSetting.question.Text = questionTextProperty.Value;
        }

        private void OnQuestionSizeChanged()
        {
            if (!questionFlags.HasFlag(TextFlags.Size)) return;

            hotspotScript.quizPopUpDataModel.popUpSetting.question.FontSize = (int) questionSizeProperty.Value;
        }

        private void OnQuestionColorChanged()
        {
            if (!questionFlags.HasFlag(TextFlags.Color)) return;

            hotspotScript.quizPopUpDataModel.popUpSetting.question.Color = questionColorProperty.Value;
        }

        private void OnCorrectAnswerTextChanged()
        {
            hotspotScript.quizPopUpDataModel.popUpSetting.options.correctAnswer = correctAnswerProperty.Value;
        }

        private void OnOptionChanged(int index)
        {
            if (index >= optionsTextProperties.Count || index < 0) return;

            hotspotScript.quizPopUpDataModel.popUpSetting.options.options[index] = optionsTextProperties[index].Value;
        }

        private void OnOptionSizeChanged()
        {
            if (!optionFlags.HasFlag(TextFlags.Size)) return;

            hotspotScript.quizPopUpDataModel.popUpSetting.options.size = (int) optionSizeProperty.Value;
        }

        private void OnOptionColorChanged()
        {
            if (!optionFlags.HasFlag(TextFlags.Color)) return;

            hotspotScript.quizPopUpDataModel.popUpSetting.options.color = optionColorProperty.Value;
        }

        private void OnCorrectResultTextChanged()
        {
            if (!resultText) return;

            hotspotScript.quizPopUpDataModel.popUpSetting.result.correctAnswer.Text = correctResultTextProperty.Value;
        }

        private void OnIncorrectResultTextChanged()
        {
            if (!resultText) return;

            hotspotScript.quizPopUpDataModel.popUpSetting.result.incorrectAnswer.Text =
                incorrectResultTextProperty.Value;
        }

        private void OnCorrectResultAudioChanged()
        {
            if (!resultAudio) return;

            hotspotScript.quizPopUpDataModel.popUpSetting.result.correctAudio = correctResultAudioProperty.Value;
        }

        private void OnIncorrectResultAudioChanged()
        {
            if (!resultAudio) return;

            hotspotScript.quizPopUpDataModel.popUpSetting.result.incorrectAudio = incorrectResultAudioProperty.Value;
        }

        #endregion

        public override void UpdateDynamicallyCreatedProperties()
        {
            int optionsCount = hotspotScript.quizPopUpDataModel.popUpSetting.options.options.Count;
            SetPropertyListLength(optionsTextProperties, optionsCount, (index) => new UserEditableStringProperty("Option " + (index + 1)));
        }

        private void SetDefaultPropertyValueOptions()
        {
            for (int i = 0; i < optionsTextProperties.Count; i++)
            {
                optionsTextProperties[i]
                    .SetDefaultValue(hotspotScript.quizPopUpDataModel.popUpSetting.options.options[i]);
            }
        }

        private void FontLoaded()
        {
            if (fontData != null && fontData.LoadedFont == null) return;

            hotspotScript.quizPopUpDataModel.popUpSetting.isRightToLeftText = fontData.UseRTL;
            hotspotScript.quizPopUpDataModel.popUpSetting.question.Font = fontData.LoadedFont;
            hotspotScript.quizPopUpDataModel.popUpSetting.options.font = fontData.LoadedFont;
            hotspotScript.quizPopUpDataModel.popUpSetting.result.correctAnswer.Font = fontData.LoadedFont;
            hotspotScript.quizPopUpDataModel.popUpSetting.result.incorrectAnswer.Font = fontData.LoadedFont;
        }
    }
}