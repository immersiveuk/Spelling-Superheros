using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Immersive.FillInTheBlank
{
    [Serializable]
    public class FillInTheBlanksData
    {
        public string spelling;
        public Vector2Int[] indexs;

        [NonSerialized]
        public string missingLetters;

        [Range(1, 5)]
        public int missingPairs = 1;
    }
}