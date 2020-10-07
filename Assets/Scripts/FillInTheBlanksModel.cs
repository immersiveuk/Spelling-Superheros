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
    }
}
