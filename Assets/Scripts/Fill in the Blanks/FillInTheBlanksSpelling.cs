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
        public FillInTheBlanksModel spellingData;

        /// <summary>
        /// It is to set "Spelling" text value to TextMesh pro and Highlighter Text after replacing "Missing Letters" with "_".
        /// </summary>
        /// <param name="data"></param>
        public void SetText(FillInTheBlanksModel data)
        {
            this.spellingData = data;

            string spelling = SplitSpelling(spellingData.spelling, spellingData.missingLettersPosition);

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

        string SplitSpelling(string spelling, Vector2Int[] missingLettersPosition)
        {
            List<SpellingParts> spellingParts = new List<SpellingParts>();

            for (int i = 0; i < missingLettersPosition.Length; i++)
            {
                Vector2Int position = missingLettersPosition[i];

                if (i == 0 && position.x > 0)
                    spellingParts.Add(new SpellingParts("Spelling", spelling.Substring(0, position.x)));

                spellingParts.Add(new SpellingParts("Option", spelling.Substring(position.x, position.y - position.x + 1)));

                if (i < missingLettersPosition.Length - 1)
                    spellingParts.Add(new SpellingParts("Spelling", spelling.Substring(position.y + 1, missingLettersPosition[i + 1].x - position.y - 1)));
            }

            spellingParts.Add(new SpellingParts("Spelling", spelling.Substring(missingLettersPosition[missingLettersPosition.Length - 1].y + 1, spelling.Length - missingLettersPosition[missingLettersPosition.Length - 1].y - 1)));

            return ApplyTransperancy(spellingParts);
        }

        string ApplyTransperancy(List<SpellingParts> spellingParts)
        {
            string spelling = "";

            foreach (var obj in spellingParts)
            {
                if (!string.IsNullOrEmpty(obj.value))
                {
                    if (obj.type.Contains("Option"))
                    {
                        spelling += "<u><#00000000>" + obj.value + "</color></u>";
                    }
                    else
                        spelling += obj.value;
                }
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