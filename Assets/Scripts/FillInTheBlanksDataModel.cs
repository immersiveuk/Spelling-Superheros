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
        public int startIndex;
        public int endIndex;

        [NonSerialized]
        public string missingLetters;
    }
}