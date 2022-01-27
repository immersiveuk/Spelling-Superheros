using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class QuestionsAnsweredBadge : MonoBehaviour, IQuestionsAnsweredHandler
{
    Dictionary<int, bool> questionsAnsweredList = new Dictionary<int, bool>();
    public TextMeshPro textResult;

    public void QuestionAnswered(int index, bool isAnswerCorrect)
    {
        if (questionsAnsweredList.ContainsKey(index))
            questionsAnsweredList[index] = isAnswerCorrect;
        else
            questionsAnsweredList.Add(index, isAnswerCorrect);
    }

    public void QuizCompleted()
    {
        int correctAnswered = 0;

        foreach (var question in questionsAnsweredList)
        {
            if (question.Value)
                correctAnswered++;
        }

        textResult.text = "" + correctAnswered + "/" + questionsAnsweredList.Count;
    }
}
