using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Immersive.FillInTheBlank
{
    public class MissingLetterHighlighter : FillInTheBlanksMissingLetter
    {
        public TextMeshPro textGlow;
        bool solved = false;

        private void Start()
        {
            if (FillInTheBlanksManager.Instance.gameMode == GameMode.Simple)
                textGlow.fontSize = FillInTheBlanksManager.Instance.fontSizeSimpleMode;
            else
                textGlow.fontSize = FillInTheBlanksManager.Instance.fotSizeAdvancedMode;
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
            {
                textGlow.enabled = false;
            }
        }

        protected override void Solved()
        {
            solved = true;
            textOption.color = Color.black;
        }
    }
}