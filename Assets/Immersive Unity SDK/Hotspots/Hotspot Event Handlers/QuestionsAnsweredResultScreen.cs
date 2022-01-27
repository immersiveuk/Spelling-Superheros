using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class QuestionsAnsweredResultScreen : MonoBehaviour, IQuestionsAnsweredHandler
{
    Dictionary<int, bool> questionsAnsweredList = new Dictionary<int, bool>();
    public TextMeshProUGUI textResult;

    public void QuestionAnswered(int index, bool isAnswerCorrect)
    {
        if (questionsAnsweredList.ContainsKey(index))
            questionsAnsweredList[index] = isAnswerCorrect;
        else
            questionsAnsweredList.Add(index, isAnswerCorrect);

        UpdateText();
    }

    void UpdateText()
    {
        int correctAnswered = 0;

        foreach (var question in questionsAnsweredList)
        {
            if (question.Value)
                correctAnswered++;
        }

        textResult.text = "Questions answered correctly: " + correctAnswered + "/" + questionsAnsweredList.Count;
    }

    public void QuizCompleted()
    {
        
    }
}
