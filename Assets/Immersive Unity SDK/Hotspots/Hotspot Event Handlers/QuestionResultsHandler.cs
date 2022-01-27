using System.Collections.Generic;
using Com.Immersive.Hotspots;
using Immersive.Properties;
using Immersive.QuizData;
using UnityEngine;

public class QuestionResultsHandler : MonoBehaviour, IQuestionAnsweredHandler
{
    [SerializeField] private QuizResults quizResultsVariable;

    private string question;
    private List<string> options;

    private static bool eventAssigned;

    public void ForceInitializeQuestionHandler() => Initialize();

    private void Initialize()
    {
        HotspotScript hps = GetComponent<HotspotScript>();
        
        QuizPopUpDataModel qpm = hps.quizPopUpDataModel;
        if (qpm != null)
        {
            SetVariables(qpm.popUpSetting.question, qpm.popUpSetting.options);
            bool answered = CheckIfAnswered();

            if (answered)
            {
                IQuestionAnsweredHandler[] handlers = GetComponents<IQuestionAnsweredHandler>();
                bool correct = quizResultsVariable.GetQuestionData(question).Item2;

                foreach (var handler in handlers)
                {
                    handler.QuestionAnswered(correct);
                }

                /*HotspotPopUp hotspotPopup = GetComponentInParent<HotspotPopUp>();
                if (hotspotPopup == null) return;
                hotspotPopup.ClosePopUp();*/
            }
        }
    }
    
    private void SetVariables(TextProperty q, OptionsProperty o)
    {
        question = q.Text;
        options = o.options;
    }

    private bool CheckIfAnswered() => quizResultsVariable.QuestionAnswered(question);

    public void QuestionAnswered(bool isAnswerCorrect)
    {
        if (quizResultsVariable == null) return;
        
        quizResultsVariable.SaveResult(question, options, isAnswerCorrect);
    }
}
