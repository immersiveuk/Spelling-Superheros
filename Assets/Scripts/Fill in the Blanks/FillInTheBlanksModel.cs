using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Immersive.FillInTheBlank
{
    [System.Serializable]
    public class MissingLettersPair
    {
        Action OnChanged;

        public int startIndex;
        public int endIndex;

        public void SetOnChangedEvent(Action action)
        {
            this.OnChanged = action;
        }

        public void SetStartIndex(int newValue)
        {
            startIndex = newValue;
            OnChanged?.Invoke();
        }

        public void SetEndIndex(int newValue)
        {
            endIndex = newValue;
            OnChanged?.Invoke();
        }
    }

    [Serializable]
    public class SpellingSettings
    {
        Action OnChanged;

        public string spelling;

        [NonSerialized]
        public string missingLetters;

        public List<MissingLettersPair> missingLettersPairs = new List<MissingLettersPair>();

        public void FormateSpelling(LetterCase letterCase)
        {
            switch (letterCase)
            {
                case LetterCase.Upper:
                    spelling = spelling.ToUpper();
                    break;
                case LetterCase.Lower:
                    spelling = spelling.ToLower();
                    break;
                case LetterCase.Capital:
                    spelling = spelling[0].ToString().ToUpper() + spelling.Substring(1).ToLower();
                    break;
            }
        }

        public void SetSpelling(string newValue)
        {
            spelling = newValue;
            OnChanged?.Invoke();
        }

        public void SetOnChangedEvent(Action action)
        {
            this.OnChanged = action;
        }
    }

    [Serializable]
    public class FillInTheBlanksModel
    {
        public List<SpellingSettings> spellings = new List<SpellingSettings>();

        public void SetOnChangedEvent(Action action)
        {
            foreach (var op in spellings)
            {
                op.SetOnChangedEvent(action);
            }
        }
    }

    [System.Serializable]
    public class FillInTheBlanksDataStage
    {
        public FillInTheBlanksModel fillInTheBlanksLeft = new FillInTheBlanksModel();
        public FillInTheBlanksModel fillInTheBlanksCenter = new FillInTheBlanksModel();
        public FillInTheBlanksModel fillInTheBlanksRight = new FillInTheBlanksModel();
    }

    [System.Serializable]
    public class FillInTheBlanksDataStages
    {
        public FillInTheBlanksDataStage stage1 = new FillInTheBlanksDataStage();
        public FillInTheBlanksDataStage stage2 = new FillInTheBlanksDataStage();
        public FillInTheBlanksDataStage stage3 = new FillInTheBlanksDataStage();
    }
}
