using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Quiz Scene", menuName = "Quiz Scene", order = 1)]
public class QuizSceneInfo : ScriptableObject
{
    public delegate void OnQuestionAnswered(int total, int correct);
    public static event OnQuestionAnswered OnQuestionAnsweredEvent;

    //public List<int> correctAnswered = new List<int>();
    //public List<int> totalQuestions = new List<int>();

    private int Score;
    private int Total;

    private void OnEnable()
    {
        Total = 0;
        Score = 0;
    }

    public void QuestionAnswered(int index, bool isAnswerCorrect)
    {
        Total++;

        if (isAnswerCorrect)
            Score++;

        Debug.Log("QuestionAnswered:    " + Score);

        if (OnQuestionAnsweredEvent != null)
            OnQuestionAnsweredEvent(Score, Total);
    }
}
