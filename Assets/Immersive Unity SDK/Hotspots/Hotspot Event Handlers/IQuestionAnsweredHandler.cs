using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IQuestionAnsweredHandler: IPopUpEventHandler
{
    void QuestionAnswered(bool isAnswerCorrect); 
}
