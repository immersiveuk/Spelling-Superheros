using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static Immersive.FillInTheBlank.FillInTheBlanksMissingLetter;

namespace Immersive.FillInTheBlank
{
    public class FillInTheBlanksSpelling : MonoBehaviour
    {
        public TextMeshPro textSpelling;
        public Transform missingLetterPosition;
       
        protected virtual void Highlight()
        {

        }

        protected virtual void Unhighlight()
        {

        }

        protected virtual void Solved() { }

        [HideInInspector]
        public SpellingSettings spellingData;

        /// <summary>
        /// It is to set "Spelling" text value to TextMesh pro and Highlighter Text after replacing "Missing Letters" with "_".
        /// </summary>
        /// <param name="data"></param>
        public void SetText(SpellingSettings data)
        {
            this.spellingData = data;

            string spelling = SplitSpelling(spellingData.spelling, spellingData.missingLettersPairs);

            if (FillInTheBlanksManager.Instance.gameMode == GameMode.Simple)
            {
                textSpelling.fontSize = FillInTheBlanksManager.Instance.fontSizeSimpleMode;
                textSpelling.text = spellingData.spelling + " - " + spelling;
            }
            else
            {
                textSpelling.fontSize = FillInTheBlanksManager.Instance.fotSizeAdvancedMode;
                textSpelling.text = spelling;
            }
        }

        string SplitSpelling(string spelling, List<MissingLettersPair> missingLettersPairs)
        {
            if (string.IsNullOrEmpty(spelling) || missingLettersPairs.Count == 0)
                return "";

            for (int i = missingLettersPairs.Count - 1; i >= 0; i--)
            {
                MissingLettersPair position = missingLettersPairs[i];

                spelling = spelling.Insert(position.endIndex + 1, "</color></u>");
                spelling = spelling.Insert(position.startIndex, "<u><#00000000>");
            }

            return spelling;
        }

        /// <summary>
        /// It is to replace "_" with correct "Missing Letters" on correct answer
        /// </summary>
        public void OnCorrectAnswer()
        {
            textSpelling.text = textSpelling.text.Replace("<u>", "").Replace("</u>", "");
        }

        public void OnSolved()
        {
            Solved();
        }

        /// <summary>
        /// Callback for Selected Spelling to Highlight the spelling.
        /// </summary>
        public void OnSelect()
        {
            Highlight();
        }

        /// <summary>
        /// Callback for Selected Spelling to remove Highlight
        /// </summary>
        public void OnDeselect()
        {
            Unhighlight();
        }
    }
}