using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static FillInTheBlanksManager;

public class Spelling : MonoBehaviour
{
    public TextMeshPro textSpelling, textGlow;
    public SpriteRenderer background;
    public Transform localPos;

    [HideInInspector]
    public FillInTheBlanksData spellingData;
    
    public void SetText(FillInTheBlanksData data)
    {
        this.spellingData = data;

        string spelling = data.spelling;
        char[] optionChar = data.option.ToCharArray();

        for (int i = 0; i < optionChar.Length; i++)
        {
            spelling = spelling.Replace(optionChar[i], '_');
        }

        textSpelling.text = spelling;
        textGlow.text = spelling;
    }

    public void OnCorrectAnswer()
    {
       string spelling = textSpelling.text.Replace("__", spellingData.option);
        textSpelling.text = spelling;
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
