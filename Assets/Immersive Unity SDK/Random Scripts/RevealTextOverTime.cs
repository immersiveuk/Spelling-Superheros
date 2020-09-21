using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RevealTextOverTime : MonoBehaviour
{
    [Min(1)]
    [SerializeField] float revealDuration = 10;

    private TextMeshPro textMesh;
    private TextMeshProUGUI textMeshUGUI;

    private string text;
    private float startTime, endTime;

    // Start is called before the first frame update
    void Start()
    {
        textMesh = GetComponent<TextMeshPro>();
        if (textMesh)
        {
            text = textMesh.text;
            textMesh.text = "";
        }
        else
        {
            textMeshUGUI = GetComponent<TextMeshProUGUI>();
            text = textMeshUGUI.text;
            textMeshUGUI.text = "";
        }

        startTime = Time.time;
        endTime = Time.time + revealDuration;
    }

    // Update is called once per frame
    void Update()
    {
        float lerpValue = Mathf.InverseLerp(startTime, endTime, Time.time);
        int endIndex = Mathf.CeilToInt(Mathf.Lerp(0, text.Length, lerpValue));

        string displayString = text.Substring(0, endIndex);

        if (textMesh) textMesh.text = displayString;
        else if (textMeshUGUI) textMeshUGUI.text = displayString; 
    }
}
