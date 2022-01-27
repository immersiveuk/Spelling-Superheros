using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeepQuizSceneInfoUpdated : MonoBehaviour, IQuestionsAnsweredHandler
{
    [CreatableScriptableObject]
    public QuizSceneInfo quizSceneInfo;

    public void QuestionAnswered(int index, bool isAnswerCorrect)
    {
        quizSceneInfo.QuestionAnswered(index, isAnswerCorrect);
    }

    public void QuizCompleted() { }
}
