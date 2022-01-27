using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IQuestionsAnsweredHandler: IPopUpEventHandler
{
    void QuestionAnswered(int index, bool isAnswerCorrect);

    void QuizCompleted();
}
