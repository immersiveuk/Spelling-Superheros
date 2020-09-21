using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IQuestionAnsweredHandler
{
    void QuestionAnswered(bool isAnswerCorrect); 
}
