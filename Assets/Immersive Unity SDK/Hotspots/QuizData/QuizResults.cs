using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Immersive.QuizData
{
    [CreateAssetMenu(menuName = "Immersive Interactive/Quiz/Results")]
    public class QuizResults : ScriptableObject
    {
        private Dictionary<string, (List<string>, bool)> processedQuizQuestions = new Dictionary<string, (List<string>, bool)>();
        public int Count => processedQuizQuestions.Count;

        public void SaveResult(string question, List<string> data, bool correct)
        {
            if (processedQuizQuestions.ContainsKey(question)) return;

            processedQuizQuestions.Add(question, (data, correct));
        }

        /// <summary>
        /// Returns the List of options and the overall result for a single question.
        /// If the question has not been answered yet it will return a default tuple of (null, false)
        /// </summary>
        /// <param name="question"></param>
        /// <returns></returns>
        public (List<string>, bool) GetQuestionData(string question) => 
            !processedQuizQuestions.ContainsKey(question)
                ? default
                : processedQuizQuestions[question];

        /// <summary>
        /// Returns the bool of the question provided
        /// </summary>
        /// <param name="question"></param>
        /// <returns></returns>
        public bool QuestionAnswered(string question) =>
            processedQuizQuestions.ContainsKey(question) && processedQuizQuestions[question].Item2;

        /// <summary>
        /// Returns a tuple list of the quiz question and its answer state, the options in this one are ignored.
        /// Expected use is for saving data or for displaying a list of questions like such:
        /// Question - Correct
        /// Question - Incorrect
        /// or
        /// Question (in green for correct)
        /// Question (in red for incorrect)
        /// </summary>
        /// <returns></returns>
        public List<(string, bool)> GetQuizState() => processedQuizQuestions.Select(processedQuizQuestion =>
            (processedQuizQuestion.Key, processedQuizQuestion.Value.Item2)).ToList();
    }
}