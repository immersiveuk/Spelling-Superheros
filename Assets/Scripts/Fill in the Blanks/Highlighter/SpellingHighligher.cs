using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Immersive.FillInTheBlank
{
    public class SpellingHighligher : FillInTheBlanksSpelling
    {
        public TextMeshPro textGlow;
        public SpriteRenderer background;


        protected override void Highlight()
        {
            textGlow.text = textSpelling.text;

            if (textGlow)
            {
                textGlow.enabled = true;
                textGlow.gameObject.AddComponent<PulseAnimation>();
            }

            if (background)
                background.enabled = true;
        }

        protected override void Unhighlight()
        {
            if (textGlow)
                textGlow.enabled = false;

            if (background)
                background.enabled = false;
        }
    }
}