/* Copyright (C) Immersive Interactive, Ltd - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Luke Bissell <luke@immersive.co.uk>, July 2019
 */

using Com.Immersive.Cameras;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Com.Immersive.Hotspots
{
    public class QuizHotspotPopUp : HotspotPopUp<QuizPopUpDataModel.QuizPopUpSetting>
    {
        [SerializeField] TextMeshProUGUI questionText = null;
        [SerializeField] TextMeshProUGUI txtResult = null;
        [SerializeField] TextMeshProUGUI tryAgainButton = null;

        [SerializeField] Image imageBackground = null;
        [SerializeField] GameObject answerPrefab = null;
        [SerializeField] GameObject resultPanel = null;
        [SerializeField] RectTransform optionsRect;

        private IQuestionAnsweredHandler[] questionAnsweredHandlers;

        protected override void SetupPopUpFromSettings(QuizPopUpDataModel.QuizPopUpSetting popUpSettings)
        {
            questionText.isRightToLeftText = popUpSettings.isRightToLeftText;
            txtResult.isRightToLeftText = popUpSettings.isRightToLeftText;

            tryAgainButton.GetComponent<TextMeshProUGUI>().text = popUpSettings.tryAgainText;
            tryAgainButton.GetComponent<TextMeshProUGUI>().isRightToLeftText = popUpSettings.isRightToLeftText;


            //Set text property for question
            SetTextProperty(questionText, popUpSettings.question);

            foreach (var obj in optionsRect.GetComponentsInChildren<TextMeshProUGUI>())
            {
                DestroyImmediate(obj.gameObject);
            }

            for (int i = 0; i < popUpSettings.options.options.Count; i++)
            {
                GameObject obj = Instantiate(answerPrefab, optionsRect, false);
                obj.SetActive(true);

                obj.GetComponent<TextMeshProUGUI>().font = popUpSettings.options.font;
                obj.GetComponent<TextMeshProUGUI>().color = popUpSettings.options.color;
                obj.GetComponent<TextMeshProUGUI>().text = popUpSettings.options.options[i];
                obj.GetComponent<TextMeshProUGUI>().isRightToLeftText = popUpSettings.isRightToLeftText;

                //Set border color
                obj.transform.GetChild(0).GetComponent<Image>().color = popUpSettings.options.color;

                resultPanel.transform.GetChild(1).GetChild(0).GetComponent<Image>().color = popUpSettings.options.color;

                string option = popUpSettings.options.options[i];

                obj.GetComponent<Button>().onClick.AddListener(delegate
                {
                    OnResult(option);
                });
            }

            SetImageProperty(imageBackground, popUpSettings.background, ImageEnum.None);//Set image property for Background
            SetImageProperty(resultPanel.GetComponent<Image>(), popUpSettings.background, ImageEnum.None);//Set image property for Image sequance

            //Set size 
            StartCoroutine(SetSize(popUpSettings, popUpSettings.sizeOption));
        }

        protected override void RetrievePopUpEventHandlers()
        {
            questionAnsweredHandlers = GetPopUpEventHandlers<IQuestionAnsweredHandler>();
        }

        /// <summary>
        /// Set the size of QandA popup based on editoer setting
        /// </summary>
        /// <param name="sizeOption">Size option (FixedPopupSize/FixedFontSize)</param>
        /// <param name="popUpsize">Size of the popup</param>
        /// <param name="fontSizeQuestion">Font size for question text</param>
        /// <param name="fontSizeOptions">Font size for option text</param>
        /// <param name="margin">Padding around the text</param>
        /// <returns></returns>
        IEnumerator SetSize(QuizPopUpDataModel.QuizPopUpSetting popUpSettings, SizeOption sizeOption)
        {
            contentRect.GetComponent<ContentSizeFitter>().enabled = false;

            contentRect.GetComponent<VerticalLayoutGroup>().padding = popUpSettings.padding;
            resultPanel.GetComponent<VerticalLayoutGroup>().padding = popUpSettings.padding;

            switch (sizeOption)
            {
                case SizeOption.FixedPopupSize: //font size will be reset according to the popup size

                    SetContentSize(popUpSettings.size); //Set popup size before setting text size

                    questionText.enableAutoSizing = false;
                    questionText.fontSize = popUpSettings.question.FontSize;
                    questionText.GetComponent<LayoutElement>().minHeight = questionText.fontSize;

                    float tmSize = 0;
                    TextMeshProUGUI[] txtOptions = optionsRect.GetComponentsInChildren<TextMeshProUGUI>();

                    //1. Enable Auto Size of all text
                    foreach (var tm in txtOptions)
                        tm.enableAutoSizing = true;

                    yield return new WaitForEndOfFrame();

                    //2. Get total size of all text after AutoSize enabled
                    foreach (var tm in txtOptions)
                        tmSize += tm.fontSize;

                    //3. Disable AutoSize of all text and set average size for all text
                    foreach (var tm in txtOptions)
                    {
                        tm.enableAutoSizing = false;
                        tm.fontSize = tmSize / txtOptions.Length;
                    }

                    break;

                case SizeOption.FixedPercentage: //popup size will be based on Percentage

                    var rect = transform.GetComponentInParent<Canvas>().GetComponent<RectTransform>().sizeDelta;

                    float height = rect.y * ((float)popUpSettings.percentage / 100.0f);
                    float width = (rect.x / rect.y) * height - 100;//100 referes control panel width

                    StartCoroutine(SetSize(popUpSettings, SizeOption.FixedPopupSize)); // call SetSize to fit text into new size of popup

                    break;
            }

            yield return new WaitForEndOfFrame();

            PositionHotspot();
            //PlacePopUp(rectTransform);

            var resultPanelRect = resultPanel.GetComponent<RectTransform>();
            resultPanelRect.anchoredPosition = contentRect.anchoredPosition;
            resultPanelRect.sizeDelta = contentRect.sizeDelta;
            resultPanelRect.anchorMax = contentRect.anchorMax;
            resultPanelRect.anchorMin = contentRect.anchorMin;
        }

        /// <summary>
        /// Display the result based on option clicked
        /// </summary>
        /// <param name="value"></param>
        public void OnResult(string value)
        {
            contentRect.gameObject.SetActive(false);
            resultPanel.SetActive(true);

            var resultProperty = popUpSettings.result;

            //CORRECT
            if (popUpSettings.options.IsCorrect(value))
            {
                //Set Text property for correct answer
                SetTextProperty(txtResult, resultProperty.correctAnswer);

                tryAgainButton.gameObject.SetActive(false);

                //Correct Audio
                if (resultProperty.correctAudio != null)
                {
                    AbstractImmersiveCamera.PlayAudio(resultProperty.correctAudio);
                }

                //Question Answered Handlers
                PassInfoToQuestionAnsweredHandlers(true);
            }
            //INCORRECT
            else
            {
                //Set Text property for incorrect answer
                SetTextProperty(txtResult, resultProperty.incorrectAnswer);

                tryAgainButton.gameObject.SetActive(true);
                tryAgainButton.font = resultProperty.incorrectAnswer.Font;
                tryAgainButton.color = resultProperty.incorrectAnswer.Color;

                //Correct Audio
                if (resultProperty.incorrectAudio != null)
                {
                    AbstractImmersiveCamera.PlayAudio(resultProperty.incorrectAudio);
                }

                //Question Answered Handlers
                PassInfoToQuestionAnsweredHandlers(false);
            }
        }

        public void OnTryAgain()
        {
            contentRect.gameObject.SetActive(true);
            resultPanel.SetActive(false);
        }

        private void PassInfoToQuestionAnsweredHandlers(bool isAnswerCorrect)
        {
            foreach (var handler in questionAnsweredHandlers)
            {
                handler.QuestionAnswered(isAnswerCorrect);
            }
        }
    }
}