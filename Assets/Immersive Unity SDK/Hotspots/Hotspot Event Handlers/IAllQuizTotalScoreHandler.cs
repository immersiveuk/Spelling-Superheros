using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAllQuizTotalScoreHandler: IPopUpEventHandler
{
    void QuestionAnswered(int index, bool isAnswerCorrect);
}
