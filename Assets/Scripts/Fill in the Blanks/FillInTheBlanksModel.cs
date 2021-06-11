using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Immersive.FillInTheBlank
{
    [Serializable]
    public class FillInTheBlanksModel
    {
        public string spelling;
        public Vector2Int[] missingLettersPosition;

        [NonSerialized]
        public string missingLetters;

        [Range(1, 5)]
        public int missingLettersPairs = 1;

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
    }

    [System.Serializable]
    public class FillInTheBlanksDataStage
    {
        public List<FillInTheBlanksModel> fillInTheBlanksLeft = new List<FillInTheBlanksModel>();
        public List<FillInTheBlanksModel> fillInTheBlanksCenter = new List<FillInTheBlanksModel>();
        public List<FillInTheBlanksModel> fillInTheBlanksRight = new List<FillInTheBlanksModel>();
    }

    [System.Serializable]
    public class FillInTheBlanksDataStages
    {
        public FillInTheBlanksDataStage stage1 = new FillInTheBlanksDataStage();
        public FillInTheBlanksDataStage stage2 = new FillInTheBlanksDataStage();
        public FillInTheBlanksDataStage stage3 = new FillInTheBlanksDataStage();
    }
}
