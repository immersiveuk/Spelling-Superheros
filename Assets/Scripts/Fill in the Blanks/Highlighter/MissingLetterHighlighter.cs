using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Immersive.FillInTheBlank
{
    public class MissingLetterHighlighter : FillInTheBlanksMissingLetter
    {
        public TextMeshPro textGlow;

        private void Start()
        {
            if (FillInTheBlanksManager.Instance.gameMode == FillInTheBlanksManager.GameMode.Simple)
                textGlow.fontSize = 15;
            else
                textGlow.fontSize = 25;
        }

        protected override void Highlight()
        {
            textGlow.text = textOption.text;

            if (textGlow)
            {
                textGlow.enabled = true;
                textGlow.gameObject.AddComponent<PulseAnimation>();
            }
        }

        protected override void Unhighlight()
        {
            if (textGlow)
                textGlow.enabled = false;
        }
    }
}