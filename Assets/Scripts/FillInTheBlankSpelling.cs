using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Immersive.FillInTgeBlank
{
    public class FillInTheBlankSpelling : MonoBehaviour
    {
        public TextMeshPro textSpelling, textGlow;
        public SpriteRenderer background;
        public Transform missingLetterPosition;
         
        [HideInInspector]
        public FillInTheBlanksData spellingData;

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
            textGlow.text = spelling;
        }

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

        public void OnSelect()
        {
            textGlow.enabled = true;
            textGlow.gameObject.AddComponent<PulseAnimation>();
            background.enabled = true;
        }

        public void OnDeselect()
        {
            textGlow.enabled = false;
            background.enabled = false;
        }
    }
}