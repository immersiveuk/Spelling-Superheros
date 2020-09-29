using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Immersive.FillInTgeBlank
{
    public class FillInTheBlanksSpelling : Highlighter
    {
        public TextMeshPro textSpelling;
        public Transform missingLetterPosition;
         
        [HideInInspector]
        public FillInTheBlanksData spellingData;

        /// <summary>
        /// It is to set "Spelling" text value to TextMesh pro and Highlighter Text after replacing "Missing Letters" with "_".
        /// </summary>
        /// <param name="data"></param>
        public void SetText(FillInTheBlanksData data)
        {
            this.spellingData = data;

            string spelling = data.spelling;
            char[] optionChar = data.missingLetters.ToCharArray();

            for (int i = 0; i < optionChar.Length; i++)
            {
                spelling = spelling.Replace(optionChar[i], '_');
            }

            textSpelling.text = spelling;

            SetText(spelling);
        }

        /// <summary>
        /// It is to replace "_" with correct "Missing Letters" on correct answer
        /// </summary>
        public void OnCorrectAnswer()
        {
            char[] optionChar = spellingData.missingLetters.ToCharArray();

            for (int i = 0; i < optionChar.Length; i++)
            {             
                int index = textSpelling.text.IndexOf('_');
                textSpelling.text = textSpelling.text.Remove(index, 1);
                textSpelling.text = textSpelling.text.Insert(index, optionChar[i].ToString());
            }
        }

        /// <summary>
        /// Callback for Selected Spelling to Highlight the spelling.
        /// </summary>
        public new void OnSelect()
        {
            base.OnSelect();
        }

        /// <summary>
        /// Callback for Selected Spelling to remove Highlight
        /// </summary>
        public new void OnDeselect()
        {
            base.OnDeselect();
        }
    }
}