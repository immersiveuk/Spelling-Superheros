using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Immersive.FillInTgeBlank
{
    public class SpellingPanel : MonoBehaviour
    {
        public FillInTheBlankSpelling[] SetPanel(List<FillInTheBlanksData> fillInTheBlanksData)
        {
            FillInTheBlankSpelling[] textSpelling = GetComponentsInChildren<FillInTheBlankSpelling>();

            for (int i = 0; i < textSpelling.Length; i++)
            {
                textSpelling[i].SetText(fillInTheBlanksData[i]);
            }

            return textSpelling;
        }
    }
}
