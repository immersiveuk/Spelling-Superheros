using Com.Immersive.Cameras;
using Mono.Options;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

namespace Immersive.FillInTheBlank
{
    public class FillInTheBlanksManager : MonoBehaviour
    {
        public delegate void SpellingSelected(FillInTheBlanksSpelling fillInTheBlanksData);
        public event SpellingSelected OnSpellingSelected;

        public FillInTheBlanksList fillInTheBlanksList;
        //public List<FillInTheBlanksData> fillInTheBlanksData;

        [Header("Sounds")]
        public AudioClip positiveClip;
        public AudioClip negativeClip;

        //public List<FillInTheBlanksSpelling> spellings;
        //public List<FillInTheBlanksMissingLetter> missingLetters;

        private int questionNo = 0;

        public static FillInTheBlanksMissingLetter.MissingLettersStats missingLettersStats = FillInTheBlanksMissingLetter.MissingLettersStats.CanPlace;

        void Start()
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
            for (int i = 0; i < fillInTheBlanksList.spellings.Count; i++)
            {
                fillInTheBlanksList.spellings[i].SetText(fillInTheBlanksList.fillInTheBlanksData[i]);
            }
        }

        void SetMissingLetters()
        {
            List<FillInTheBlanksData> lettrsToShuffle = new List<FillInTheBlanksData>();
            lettrsToShuffle.AddRange(fillInTheBlanksList.fillInTheBlanksData);
            lettrsToShuffle.Shuffle();

            for (int i = 0; i < fillInTheBlanksList.missingLetters.Count; i++)
            {
                fillInTheBlanksList.missingLetters[i].SetText(lettrsToShuffle[i], this, OnResultAction);
            }
        }

        /// <summary>
        /// Callback after click on Missing Letter with result
        /// </summary>
        /// <param name="result"></param>
        void OnResultAction(bool result)
        {
            missingLettersStats = FillInTheBlanksMissingLetter.MissingLettersStats.CanPlace;

            if (result)
            {
                SelectNextSpelling();
                AbstractImmersiveCamera.PlayAudio(positiveClip);
            }
            else
                AbstractImmersiveCamera.PlayAudio(negativeClip);
        }

        /// <summary>
        /// Select next Spelling after correct answer
        /// </summary>
        public void SelectNextSpelling()
        {
            foreach (var obj in fillInTheBlanksList.spellings)
            {
                obj.OnDeselect();
            }

            if (questionNo >= fillInTheBlanksList.spellings.Count)
                return;

            fillInTheBlanksList.spellings[questionNo].OnSelect();

            OnSpellingSelected(fillInTheBlanksList.spellings[questionNo]);

            questionNo++;
        }
    }
}