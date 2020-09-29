using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Immersive.FillInTgeBlank
{
    public class Highlighter : MonoBehaviour
    {
        public TextMeshPro textGlow;
        public SpriteRenderer background;

        public void SetText(string textValue)
        {
            textGlow.text = textValue;
        }

        public void OnSelect()
        {
            if (textGlow)
            {
                textGlow.enabled = true;
                textGlow.gameObject.AddComponent<PulseAnimation>();
            }

            if (background)
                background.enabled = true;
        }

        public void OnDeselect()
        {
            if (textGlow)
                textGlow.enabled = false;

            if (background)
                background.enabled = false;
        }
    }
}