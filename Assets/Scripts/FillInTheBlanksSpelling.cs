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

        [HideInInspector]
        public FillInTheBlanksData spellingData;

        /// <summary>
        /// It is to set "Spelling" text value to TextMesh pro and Highlighter Text after replacing "Missing Letters" with "_".
        /// </summary>
        /// <param name="data"></param>
        public void SetText(FillInTheBlanksData data)
        {
            this.spellingData = data;

            string spelling = "";

            List<SpellingParts> spellingParts = new List<SpellingParts>();

            for (int i = 0; i < data.missingPairs; i++)
            {
                if (i == 0 && data.indexs[i].x > 0)
                    spellingParts.Add(new SpellingParts("Spelling", data.spelling.Substring(0, data.indexs[i].x)));

                spellingParts.Add(new SpellingParts("Option", data.spelling.Substring(data.indexs[i].x, data.indexs[i].y - data.indexs[i].x + 1)));

                if (i < data.missingPairs - 1)
                    spellingParts.Add(new SpellingParts("Spelling", data.spelling.Substring(data.indexs[i].y + 1, data.indexs[i + 1].x - data.indexs[i].y - 1)));
            }

            spellingParts.Add(new SpellingParts("Spelling", data.spelling.Substring(data.indexs[data.indexs.Length - 1].y + 1, data.spelling.Length - data.indexs[data.indexs.Length - 1].y - 1)));

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

            /*
            int startIndex = -1;
            int endIndex = -1;

            for (int i = 0; i < data.missingPairs; i++)
            {
                int offset = 0;

                if (startIndex != data.indexs[i].x)
                {
                    startIndex = data.indexs[i].x;

                    if (spelling.Contains(">"))
                    {
                        offset = spelling.LastIndexOf('>') + data.indexs[i].x - endIndex;
                    }

                    spelling = spelling.Insert(offset , "<u><#00000000>");
                }

                if (endIndex != data.indexs[i].y)
                {
                    endIndex = data.indexs[i].y;
                    offset = spelling.LastIndexOf('>') + 1;
                    
                    int charLength = data.indexs[i].y - data.indexs[i].x + 1;
                    
                    spelling = spelling.Insert(offset + charLength, "</color></u>");
                }
            }
            */
            textSpelling.text = spelling;
        }

        /// <summary>
        /// It is to replace "_" with correct "Missing Letters" on correct answer
        /// </summary>
        public void OnCorrectAnswer()
        {
            textSpelling.text = spellingData.spelling;
            //char[] optionChar = spellingData.missingLetters.ToCharArray();

            //for (int i = 0; i < optionChar.Length; i++)
            //{             
            //    int index = textSpelling.text.IndexOf('_');
            //    textSpelling.text = textSpelling.text.Remove(index, 1);
            //    textSpelling.text = textSpelling.text.Insert(index, optionChar[i].ToString());
            //}
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