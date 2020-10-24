﻿using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Immersive.FillInTheBlank
{
    public class SpellingHighligher : FillInTheBlanksSpelling
    {
        public TextMeshPro textGlow;
        public SpriteRenderer background;

        private void Start()
        {
            if (FillInTheBlanksManager.Instance.gameMode == FillInTheBlanksManager.GameMode.Simple)
                textGlow.fontSize = FillInTheBlanksManager.Instance.fontSizeSimpleMode;
            else
                textGlow.fontSize = FillInTheBlanksManager.Instance.fotSizeAdvancedMode;
        }
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