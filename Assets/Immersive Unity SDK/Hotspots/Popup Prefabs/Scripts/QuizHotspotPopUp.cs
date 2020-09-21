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
    public class QuizHotspotPopUp : HotspotPopUp
    {
        public TextMeshProUGUI questionText;
        public TextMeshProUGUI txtResult;
        public TextMeshProUGUI tryAgainButton;

        public Image imageBackground, imageCloseButton;
        public GameObject answerPrefab;
        public GameObject resultPanel;

        private string correctAnswer;

        private QuizResultProperty resultProperty;

        private IQuestionAnsweredHandler[] questionAnsweredHandlers;

        public RectTransform optionsRect;

        private void Start()
        {
            //This ensures it is off screen until it the size is calculated and it can be properly placed.
            rectTransform.anchoredPosition = new Vector2(0, -10000);

            //Get Question Answered Handlers
            //CHECK IF IT WORKS WITH DELETE ON ACTION
            questionAnsweredHandlers = _spawningHotspot.GetComponents<IQuestionAnsweredHandler>();
        }

        Vector2 size;
        public void Init(QuizPopUpDataModel popupDataModel)
        {
            size = popupDataModel.popUpSetting.size;

            this.resultProperty = popupDataModel.popUpSetting.result;
            correctAnswer = popupDataModel.popUpSetting.options.correctAnswer;

            //Set text property for question
            SetTextProperty(questionText, popupDataModel.popUpSetting.question);

            foreach (var obj in optionsRect.GetComponentsInChildren<TextMeshProUGUI>())
            {
                DestroyImmediate(obj.gameObject);
            }

            for (int i = 0; i < popupDataModel.popUpSetting.options.options.Count; i++)
            {
                GameObject obj = Instantiate(answerPrefab, optionsRect, false);
                obj.SetActive(true);

                obj.GetComponent<TextMeshProUGUI>().font = popupDataModel.popUpSetting.options.font;
                obj.GetComponent<TextMeshProUGUI>().color = popupDataModel.popUpSetting.options.color;
                obj.GetComponent<TextMeshProUGUI>().text = popupDataModel.popUpSetting.options.options[i];

                string option = popupDataModel.popUpSetting.options.options[i];

                obj.GetComponent<Button>().onClick.AddListener(delegate
                {
                    OnResult(option);
                });
            }

            SetCloseButtonImage(imageCloseButton, popupDataModel.popUpSetting); //Set Close Button
            SetImageProperty(imageBackground, popupDataModel.popUpSetting.background, ImageEnum.None);//Set image property for Background
            SetImageProperty(resultPanel.GetComponent<Image>(), popupDataModel.popUpSetting.background, ImageEnum.None);//Set image property for Image sequance

            //Set size 
            StartCoroutine(SetSize(popupDataModel, popupDataModel.popUpSetting.sizeOption));
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
        IEnumerator SetSize(QuizPopUpDataModel quizPopUpDataModel, SizeOption sizeOption)
        {
            contentRect.GetComponent<ContentSizeFitter>().enabled = false;

            contentRect.GetComponent<VerticalLayoutGroup>().padding = quizPopUpDataModel.popUpSetting.padding;
            resultPanel.GetComponent<VerticalLayoutGroup>().padding = quizPopUpDataModel.popUpSetting.padding;


            switch (sizeOption)
            {
                case SizeOption.FixedPopupSize: //font size will be reset according to the popup size

                    SetContentSize(size); //Set popup size before setting text size

                    questionText.enableAutoSizing = false;
                    questionText.fontSize = quizPopUpDataModel.popUpSetting.question.size;
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

                case SizeOption.FixedContentSize: //font size will be fixed but popup size will be changed accordingly
                    contentRect.GetComponent<ContentSizeFitter>().enabled = true;

                    questionText.fontSize = quizPopUpDataModel.popUpSetting.question.size;

                    foreach (var obj in contentRect.GetComponentsInChildren<Button>())
                    {
                        obj.GetComponent<TextMeshProUGUI>().fontSize = quizPopUpDataModel.popUpSetting.options.size;
                    }

                    yield return new WaitForEndOfFrame();
                    SetContentSize(contentRect.GetComponent<RectTransform>().sizeDelta); //Set popup size after calculating text size

                    break;

                case SizeOption.FixedPercentage: //popup size will be based on Percentage

                    var rect = transform.GetComponentInParent<Canvas>().GetComponent<RectTransform>().sizeDelta;

                    float height = rect.y * ((float)quizPopUpDataModel.popUpSetting.percentage / 100.0f);
                    float width = (rect.x / rect.y) * height - 100;//100 referes control panel width

                    size = new Vector2(width, height);

                    StartCoroutine(SetSize(quizPopUpDataModel, SizeOption.FixedPopupSize)); // call SetSize to fit text into new size of popup

                    break;
            }

            yield return new WaitForEndOfFrame();

            PlacePopUp(rectTransform);

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

            //CORRECT
            if (correctAnswer.Equals(value))
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
                tryAgainButton.font = resultProperty.incorrectAnswer.font;
                tryAgainButton.color = resultProperty.incorrectAnswer.color;

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