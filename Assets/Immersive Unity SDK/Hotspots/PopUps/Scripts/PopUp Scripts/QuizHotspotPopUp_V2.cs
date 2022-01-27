/* Copyright (C) Immersive Interactive, Ltd - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Luke Bissell <luke@immersive.co.uk>, July 2019
 */

using Com.Immersive.Cameras;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
//using static Com.Immersive.Hotspots.QuizPopUpDataModel_V2;

namespace Com.Immersive.Hotspots
{
    public class QuizHotspotPopUp_V2 : HotspotPopUp<QuizPopUpSetting_V2>
    {
        enum ResultStats { Correct, Incorrect, TimesUp }

        public RectTransform optionsRect;

        [Header("Panels")]
        public GameObject questionPanel;
        public GameObject correctPanel;
        public GameObject incorrectPanel;
        public GameObject timesUpPanel;
        public GameObject resultPanel;

        [Header("Texts")]
        public TextMeshProUGUI questionText;
        public TextMeshProUGUI questionNumberText;
        public TextMeshProUGUI questionNumberTotalText;
        public TextMeshProUGUI timeRemainingText;
        public TextMeshProUGUI incorrectMessageText;
        public TextMeshProUGUI timesUpMessageText;

        [Header("Button Stats Sprites")]
        public Sprite normalStatsSprite;
        public Sprite correctStatsSprite;
        public Sprite incorrectStatsSprite;

        public Image timerFill;

        [ColorUsage(false)] public Color emptyColor = Color.red;
        [ColorUsage(false)] public Color fullColor = Color.green;

        private int questionNumber;
        private string correctAnswer;
        private Button correctAnswerButton;

        private IQuestionAnsweredHandler[] questionAnsweredHandlers;
        private IQuestionsAnsweredHandler[] questionsAnsweredHandlers;
        private IQuestionsAnsweredHandler questionsAnsweredHandler;

        private Button[] optionButtons;

        List<QuizPopUpSetting_V2.QuizPopup> quizPages;
        QuizPopUpSetting_V2.QuizPopup currentQuestion;

        protected override void SetupPopUpFromSettings(QuizPopUpSetting_V2 popUpSettings)
        {
            questionNumber = 0;
            popUpSettings.size = contentRect.sizeDelta;
            controlPanelRect.gameObject.SetActive(!popUpSettings.disableCloseButton);

            quizPages = new List<QuizPopUpSetting_V2.QuizPopup>();
            quizPages.AddRange(popUpSettings.questions);

            if (popUpSettings.randomiseQuestions)
                quizPages.Shuffle();

            SetQuestion();
        }

        protected override void RetrievePopUpEventHandlers()
        {
            questionAnsweredHandlers = GetPopUpEventHandlers<IQuestionAnsweredHandler>();
            questionsAnsweredHandlers = GetPopUpEventHandlers<IQuestionsAnsweredHandler>();
            questionsAnsweredHandler = this.GetComponent<IQuestionsAnsweredHandler>(); //to display result screen on popup at end of quiz
        }


        void SetQuestion()
        {
            currentQuestion = quizPages[questionNumber];

            questionNumberText.text = "Question : " + (questionNumber + 1);
            questionNumberTotalText.text = "" + (questionNumber + 1) + "/" + popUpSettings.questions.Count;

            correctAnswer = currentQuestion.options.correctAnswer;

            //Set text property for question
            questionText.text = currentQuestion.question.GenerateTMPStyledText();

            optionButtons = optionsRect.transform.GetComponentsInChildren<Button>();

            List<string> options = new List<string>();
            options.AddRange(currentQuestion.options.options);

            if (popUpSettings.randomiseOption)
                options.Shuffle();

            for (int i = 0; i < options.Count; i++)
            {
                string option = options[i];
                Button button = optionButtons[i];

                button.GetComponentInChildren<TextMeshProUGUI>().text = option;

                if (option.Equals(correctAnswer))
                    correctAnswerButton = button;

                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(delegate
                {
                    OnResult(option, button);
                });
            }

            questionNumber++;

            StopAllCoroutines();

            StartTimer();
            StartCoroutine(SetPosition());
        }

        IEnumerator SetPosition()
        {
            yield return new WaitForEndOfFrame();
            PositionHotspot();
            //SetContentSizeAndPositionHotspot(popUpSettings.size);
        }

        /// <summary>
        /// Display the result based on option clicked
        /// </summary>
        /// <param name="value"></param>
        public void OnResult(string option, Button button)
        {
            for (int i = 0; i < optionButtons.Length; i++)
            {
                optionButtons[i].interactable = false;
            }

            StopAllCoroutines();
            StartCoroutine(OnResultWait(correctAnswer.Equals(option) ? ResultStats.Correct : ResultStats.Incorrect, button));
        }

        IEnumerator OnResultWait(ResultStats resultStats, Button button)
        {
            switch (resultStats)
            {
                case ResultStats.Correct:
                    //Question Answered Handlers
                    PassInfoToQuestionAnsweredHandlers(true);

                    AbstractImmersiveCamera.PlayAudio(popUpSettings.correctClip);

                    button.image.sprite = correctStatsSprite;
                    yield return new WaitForSeconds(0.2f);

                    questionPanel.SetActive(false);
                    correctPanel.SetActive(true);
                    break;

                case ResultStats.Incorrect:
                    //Question Answered Handlers
                    PassInfoToQuestionAnsweredHandlers(false);
                    AbstractImmersiveCamera.PlayAudio(popUpSettings.incorrectClip);

                    button.image.sprite = incorrectStatsSprite;

                    yield return new WaitForSeconds(0.5f);

                    correctAnswerButton.image.sprite = correctStatsSprite;

                    yield return new WaitForSeconds(1.0f);
                    incorrectMessageText.text = "Correct Answer is: " + correctAnswer;

                    questionPanel.SetActive(false);
                    incorrectPanel.SetActive(true);
                    break;

                case ResultStats.TimesUp:
                    //Question Answered Handlers
                    PassInfoToQuestionAnsweredHandlers(false);

                    correctAnswerButton.image.sprite = correctStatsSprite;

                    yield return new WaitForSeconds(1.0f);
                    timesUpMessageText.text = "Correct Answer is: " + correctAnswer;

                    questionPanel.SetActive(false);
                    timesUpPanel.SetActive(true);
                    break;

            }
        }

        void ResetQuestionPanel()
        {
            questionPanel.SetActive(true);
            correctPanel.SetActive(false);
            incorrectPanel.SetActive(false);
            timesUpPanel.SetActive(false);
            resultPanel.SetActive(false);

            foreach (var button in optionButtons)
            {
                button.image.sprite = normalStatsSprite;
                button.interactable = true;
            }
        }

        public void ButtonContinue()
        {
            ResetQuestionPanel();

            if (questionNumber < popUpSettings.questions.Count)
            {
                SetQuestion();
            }
            else
            {
                questionPanel.SetActive(false);
                resultPanel.SetActive(true);
            }
        }

        public override void ClosePopUp()
        {
            foreach (var handler in questionsAnsweredHandlers)
            {
                handler.QuizCompleted();
            }
            base.ClosePopUp();
        }

        private void PassInfoToQuestionAnsweredHandlers(bool isAnswerCorrect)
        {
            foreach (var handler in questionAnsweredHandlers)
            {
                handler.QuestionAnswered(isAnswerCorrect);
            }

            foreach (var handler in questionsAnsweredHandlers)
            {
                handler.QuestionAnswered(questionNumber - 1, isAnswerCorrect);
            }

            if (questionsAnsweredHandler != null)
                questionsAnsweredHandler.QuestionAnswered(questionNumber - 1, isAnswerCorrect);
        }

        void StartTimer()
        {
            StartCoroutine(Timer());
        }

        IEnumerator Timer()
        {
            float timerValue = 0;
            float time = 0;
            bool timesUpClip = false;

            while (timerValue < 1)
            {
                if (!incorrectPanel.activeSelf)
                {
                    time += Time.deltaTime;
                    timerValue = time / popUpSettings.duration;
                    timerFill.fillAmount = 1 - Mathf.Lerp(0, 1, timerValue);
                    timeRemainingText.text = (popUpSettings.duration - time).ToString("00") + " Sec";

                    timerFill.color = Color.Lerp(fullColor, emptyColor, timerValue);

                    if (popUpSettings.timesUpClip && ((popUpSettings.duration - time) - popUpSettings.timesUpClip.length <= 0 && !timesUpClip))
                    {
                        timesUpClip = true;
                        AbstractImmersiveCamera.PlayAudio(popUpSettings.timesUpClip);
                    }
                }
                yield return null;
            }

            StartCoroutine(OnResultWait(ResultStats.TimesUp, null));
        }
    }

    public static class IListExtensions
    {
        /// <summary>
        /// Shuffles the element order of the specified list.
        /// </summary>
        public static void Shuffle<T>(this IList<T> ts)
        {
            var count = ts.Count;
            var last = count - 1;
            for (var i = 0; i < last; ++i)
            {
                var r = UnityEngine.Random.Range(i, count);
                var tmp = ts[i];
                ts[i] = ts[r];
                ts[r] = tmp;
            }
        }
    }
}