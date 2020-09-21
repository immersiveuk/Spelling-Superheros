using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;

/// <summary>
/// This component allows you to define a a speechbox which will change text at the required timestamp.
/// </summary>
public class SpeechBox : MonoBehaviour
{
    public SpeechBoxOption[] speechBoxes = new SpeechBoxOption[2];
    
    [NonSerialized]
    public TextMeshPro textMesh;

    private int currentIndex = 0;
    private float currentTime;

    // Start is called before the first frame update
    void OnEnable ()
    {
        textMesh = GetComponent<TextMeshPro>();   
    }

    // Update is called once per frame
    void Update()
    {
        currentTime += Time.deltaTime;
        
        if (currentIndex < speechBoxes.Length)
        {
            if (currentTime > speechBoxes[currentIndex].timeStamp)
            {
                textMesh.text = speechBoxes[currentIndex].text;
                textMesh.faceColor = speechBoxes[currentIndex].textColour;
                currentIndex++;
            }
        }

    }

    public void ResetSpeechBox()
    {
        currentIndex = 0;
        currentTime = 0;
    }


}


[Serializable]
public class SpeechBoxOption
{
    [TextArea]
    public string text = "";
    public Color textColour = Color.white;
    public float timeStamp;
}
