using Com.Immersive.Cameras;
using Immersive.Animation;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Com.Immersive.Hotspots.MatchingPairPopUpSetting;

namespace Com.Immersive.Hotspots
{
    public class MatchingPairPopUp : HotspotPopUp<MatchingPairPopUpSetting>
    {
        [Header("Texts")]
        public TextMeshProUGUI questionText;
        public GameObject leftOptionPanel, rightOptionPanel;
        public MatchingPairOption matchingPairOption;
        public GameObject timerPanel, questionPanel, continuePanel, resultPanel, timeUpPanel;
        public List<LineRenderer> lines = new List<LineRenderer>();

        [Header("Timer")]
        public Image timerFill;
        public TextMeshProUGUI timeRemainingText;
        [ColorUsage(false)] public Color emptyColor = Color.red;
        [ColorUsage(false)] public Color fullColor = Color.green;

        public RectTransform positionar;

        private int questionNumber;
        private MatchingPair currentMatchingPair;

        List<MatchingPairOption> leftOptions = new List<MatchingPairOption>();
        List<MatchingPairOption> rightOptions = new List<MatchingPairOption>();

        protected override void SetupPopUpFromSettings(MatchingPairPopUpSetting popUpSettings)
        {
            questionNumber = 0;
            popUpSettings.size = contentRect.sizeDelta;
            closeButton.gameObject.SetActive(!popUpSettings.disableCloseButton);

            SetQuestion();            
            PositionHotspot();            
        }

        void SetQuestion()
        {
            currentMatchingPair = popUpSettings.matchingPairQuestions[questionNumber];
            //Set text property for question
            questionText.text = currentMatchingPair.question.GenerateTMPStyledText();

            SetOptions();
            StartTimer();

            questionNumber++;
        }

        void SetOptions()
        {
            leftOptions.Clear();
            rightOptions.Clear();

            for (int i = 0; i < currentMatchingPair.pairs.Count; i++)
            {
                MatchingPairOption leftOption = Instantiate(matchingPairOption, leftOptionPanel.transform, false);
                MatchingPairOption rightOption = Instantiate(matchingPairOption, rightOptionPanel.transform, false);

                leftOptions.Add(leftOption);
                rightOptions.Add(rightOption);

                leftOption.SetOption(currentMatchingPair.pairs[i], OptionType.Left, ButtonSelectLeftOption);
                rightOption.SetOption(currentMatchingPair.pairs[i], OptionType.Right, ButtonSelectRightOption);
            }

            Shuffle(leftOptions);
            Shuffle(rightOptions);
        }

        MatchingPairOption selectedLeftOption, selectedRightOption;

        void ButtonSelectLeftOption(MatchingPairOption option)
        {
            selectedLeftOption = option;
            CheckResult();
        }    

        void ButtonSelectRightOption(MatchingPairOption option)
        {
            selectedRightOption = option;
            CheckResult();
        }

        void CheckResult()
        {
            if (selectedLeftOption != null) selectedLeftOption.button.image.color = Color.gray;
            if (selectedRightOption != null) selectedRightOption.button.image.color = Color.gray;

            if (selectedLeftOption != null && selectedRightOption != null)
            {
                if (selectedLeftOption.pair.leftPart.Equals(selectedRightOption.pair.leftPart) && 
                    selectedLeftOption.pair.rightPart.Equals(selectedRightOption.pair.rightPart))
                {
                    OnCorrect();
                }
                else
                {
                    OnIncorrect();
                }

                selectedLeftOption = selectedRightOption = null;
            }
        }

        void OnCorrect()
        {
            selectedLeftOption.DrawLine(contentRect, selectedRightOption.target);

            selectedLeftOption.button.image.color = selectedRightOption.button.image.color = Color.green;
            selectedLeftOption.button.interactable = selectedRightOption.button.interactable = false;

            CheckGameOver();

            ResetOptions(leftOptions);
            ResetOptions(rightOptions);
        }
       

        void OnIncorrect()
        {
            selectedLeftOption.button.image.color = selectedRightOption.button.image.color = Color.red;

            StartCoroutine(ResetOptionsWait(0.5f));
        }

        IEnumerator ResetOptionsWait(float waitTime)
        {
            yield return new WaitForSeconds(waitTime);
            ResetOptions(leftOptions);
            ResetOptions(rightOptions);
        }

        void ResetOptions(List<MatchingPairOption> options)
        {
            for (int i = 0; i < options.Count; i++)
            {
                if (options[i].button.interactable)
                {
                    options[i].button.image.color = Color.white;
                }
            }
        }

        void CheckGameOver()
        {
            int count = 0;

            for (int i = 0; i < leftOptions.Count; i++)
            {
                if (leftOptions[i].button.interactable)
                    count++;
            }

            if (count == 0)
            {
                floatAnimator = null;
                questionPanel.SetActive(false);
                continuePanel.SetActive(true);
            }
        }

        public void ButtonContinue()
        {
            ResetQuestionPanel();

            if (questionNumber < popUpSettings.matchingPairQuestions.Count)
            {
                SetQuestion();
            }
            else
            {
                questionPanel.SetActive(false);
                resultPanel.SetActive(true);
            }
        }

        void ResetQuestionPanel()
        {
            questionPanel.SetActive(true);
            continuePanel.SetActive(false);
            timeUpPanel.SetActive(false);

            foreach (var obj in leftOptions) 
            {
                Destroy(obj.gameObject);
            }

            foreach (var obj in rightOptions)
            {
                Destroy(obj.gameObject);
            }
        }

        FloatAnimator floatAnimator;
        bool timesUpClip;
        void StartTimer()
        {
            if (!popUpSettings.enableTimer)
                timerPanel.SetActive(false);
            else
            {
                timerPanel.SetActive(true);
                floatAnimator = new FloatAnimator(popUpSettings.duration, 0, 1, EasingAnimations.Type.Linear);
            }
        }

        private void FixedUpdate()
        {
            if (floatAnimator != null)
                SetTimerValue();
        }

        void SetTimerValue()
        {
            float clampedValue = floatAnimator.GetCurrentValue();
            float totalTimePassed = popUpSettings.duration * clampedValue;

            timerFill.fillAmount = 1 - clampedValue;

            timeRemainingText.text = (popUpSettings.duration - totalTimePassed).ToString("00") + " Sec";

            timerFill.color = Color.Lerp(fullColor, emptyColor, clampedValue);
            

            if (popUpSettings.timesUpClip && ((popUpSettings.duration - totalTimePassed) - popUpSettings.timesUpClip.length <= 0 && !timesUpClip))
            {
                timesUpClip = true;
                AbstractImmersiveCamera.PlayAudio(popUpSettings.timesUpClip);
            }

            if (floatAnimator.IsFinished)
            {
                floatAnimator = null;
                questionPanel.SetActive(false);
                timeUpPanel.SetActive(true);
            }
        }

        public void Shuffle(List<MatchingPairOption> options)
        {
            for (int i = 0; i < options.Count; i++)
            {
                options[i].transform.SetSiblingIndex(Random.Range(0, options.Count));
            }
        }
    }
}