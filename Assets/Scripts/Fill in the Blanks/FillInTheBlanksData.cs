using Immersive.FillInTheBlank;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Com.Immersive.Cameras;
using Newtonsoft.Json;
using Immersive.SuperHero;

namespace Immersive.FillInTheBlank
{
    public enum LetterCase { Upper, Lower, Capital, EditorSetting}
    public class FillInTheBlanksData : MonoBehaviour
    {
        [Space(10)]
        public LetterCase letterCase;        

        public delegate void ResultAction(bool result);
        public event ResultAction OnResultAction;

        public delegate void SpellingSelected(FillInTheBlanksSpelling spelling);
        public event SpellingSelected OnSpellingSelected;

        [Space(20)]
        public FillInTheBlanksModel fillInTheBlanksList;
        public List<FillInTheBlanksSpelling> spellings;
        public List<FillInTheBlanksMissingLetter> missingLetters;

        [HideInInspector]
        public FillInTheBlanksMissingLetter.MissingLettersStats missingLettersStats = FillInTheBlanksMissingLetter.MissingLettersStats.CanPlace;

        private int questionNo = 0;
        private FillInTheBlanksSpelling currentSpelling;

        [ContextMenu("JSON")]
        void CreateJson()
        {
            Debug.Log(JsonConvert.SerializeObject(fillInTheBlanksList));
        }

        private void Awake()
        {
            fillInTheBlanksList.SetOnChangedEvent(SetLayout);
        }

        private void Start()
        {
            //SetLayout();
        }

        /// <summary>
        /// Set Layout of Spelling and Missing Layout based on <FillInTheBlanksData>
        /// </summary>
        public void SetLayout()
        {
            Debug.Log("it should call last");
            SetSpellings();
            SetMissingLetters();
            SelectFirstSpelling();
        }

        void SetSpellings()
        {
            for (int i = 0; i < spellings.Count; i++)
            {
                fillInTheBlanksList.spellings[i].FormateSpelling(letterCase);
                spellings[i].SetText(fillInTheBlanksList.spellings[i]);
            }
        }

        void SetMissingLetters()
        {
            List<SpellingSettings> lettrsToShuffle = new List<SpellingSettings>();
            lettrsToShuffle.AddRange(fillInTheBlanksList.spellings);
            lettrsToShuffle.Shuffle();

            for (int i = 0; i < missingLetters.Count; i++)
            {                
                missingLetters[i].SetText(lettrsToShuffle[i], this, OnResultCallback);
            }
        }

        public string FormateWord(string word)
        {
            switch (letterCase)
            {
                case LetterCase.Upper:
                    word = word.ToUpper();
                    break;
                case LetterCase.Lower:
                    Debug.Log(word);
                    word = word.ToLower();
                    Debug.Log(word);
                    break;
                case LetterCase.Capital:
                    word = word[0].ToString().ToUpper() + word.Substring(1).ToLower();
                    break;
            }

            return word;
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

        void SelectFirstSpelling()
        {
            currentSpelling = spellings[0];
            currentSpelling.OnSelect();

            OnSpellingSelected(spellings[0]);
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

            if (currentSpelling)
            {
                currentSpelling.OnSolved();
            }

            if (questionNo >= spellings.Count)
            {
                GetComponent<FillInTheBlanksWall>().OnComplete();
                return;
            }

            currentSpelling = spellings[questionNo];
            currentSpelling.OnSelect();

            OnSpellingSelected(spellings[questionNo]);

            questionNo++;
        }
    }
}
