using Com.Immersive.Hotspots;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class AllQuizResultScreen : MonoBehaviour
{
    public TextMeshPro textTotalScore;

    void Awake()
    {
        QuizSceneInfo.OnQuestionAnsweredEvent += QuestionAnswered;
    }

    public void QuestionAnswered(int correct, int totalQuestion)
    {        
        textTotalScore.text = "Score: " + correct;
    }
}
