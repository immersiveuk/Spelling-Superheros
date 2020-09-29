using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Immersive.FillInTgeBlank
{
    public class SpellingPanel : MonoBehaviour
    {
        /// <summary>
        /// Assign Spelling to the child object TextMesh pro
        /// </summary>
        /// <param name="fillInTheBlanksData"></param>
        /// <returns></returns>
        public FillInTheBlanksSpelling[] SetPanel(List<FillInTheBlanksData> fillInTheBlanksData)
        {
            FillInTheBlanksSpelling[] textSpelling = GetComponentsInChildren<FillInTheBlanksSpelling>();

            for (int i = 0; i < textSpelling.Length; i++)
            {
                textSpelling[i].SetText(fillInTheBlanksData[i]);
            }

            return textSpelling;
        }
    }
}
