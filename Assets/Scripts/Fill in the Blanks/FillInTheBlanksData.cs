using Immersive.FillInTheBlank;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Com.Immersive.Cameras;

namespace Immersive.FillInTheBlank
{
    public class FillInTheBlanksData : MonoBehaviour
    {
        public delegate void ResultAction(bool result);
        public event ResultAction OnResultAction;

        public delegate void SpellingSelected(FillInTheBlanksSpelling spelling);
        public event SpellingSelected OnSpellingSelected;

        public List<FillInTheBlanksModel> fillInTheBlanksList;
        public List<FillInTheBlanksSpelling> spellings;
        public List<FillInTheBlanksMissingLetter> missingLetters;

        public FillInTheBlanksMissingLetter.MissingLettersStats missingLettersStats = FillInTheBlanksMissingLetter.MissingLettersStats.CanPlace;

        private int questionNo = 0;

        private void Start()
        {
            SetLayout();
        }

        /// <summary>
        /// Set Layout of Spelling and Missing Layout based on <FillInTheBlanksData>
        /// </summary>
        void SetLayout()
        {
            SetSpellings();
            SetMissingLetters();
            SelectNextSpelling();
        }

        void SetSpellings()
        {
            for (int i = 0; i < spellings.Count; i++)
            {
                spellings[i].SetText(fillInTheBlanksList[i]);
            }
        }

        void SetMissingLetters()
        {
            List<FillInTheBlanksModel> lettrsToShuffle = new List<FillInTheBlanksModel>();
            lettrsToShuffle.AddRange(fillInTheBlanksList);
            lettrsToShuffle.Shuffle();

            for (int i = 0; i < missingLetters.Count; i++)
            {
                missingLetters[i].SetText(lettrsToShuffle[i], this, OnResultCallback);
            }
        }

        /// <summary>
        /// Callback after click on Missing Letter with result
        /// </summary>
        /// <param name="result"></param>
        void OnResultCallback(bool result)
        {
            missingLettersStats = FillInTheBlanksMissingLetter.MissingLettersStats.CanPlace;

            OnResultAction(result);

            if (result)
            {
                SelectNextSpelling();
            }
        }

        /// <summary>
        /// Select next Spelling after correct answer
        /// </summary>
        public void SelectNextSpelling()
        {
            foreach (var obj in spellings)
            {
                obj.OnDeselect();
            }

            if (questionNo >= spellings.Count)
            {
                GetComponent<FillInTheBlanksWall>().OnComplete();
                return;
            }

            spellings[questionNo].OnSelect();

            OnSpellingSelected(spellings[questionNo]);

            questionNo++;
        }
    }
}
