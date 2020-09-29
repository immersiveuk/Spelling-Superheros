using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Immersive.FillInTgeBlank.FillInTheBlanksMissingLetters;

namespace Immersive.FillInTgeBlank
{
    public class MissingLettersPanel : MonoBehaviour
    {
        public static MissingLettersStats missingLettersStats = MissingLettersStats.CanPlace;

        System.Action<bool> resultAction;

        /// <summary>
        /// Assign Missing Letter to th Child object Textmesh pro
        /// </summary>
        /// <param name="fillInTheBlanksData"></param>
        /// <param name="resultAction"></param>
        public void SetPanel(List<FillInTheBlanksData> fillInTheBlanksData, System.Action<bool> resultAction)
        {
            this.resultAction = resultAction;
            var options = GetRandomisedOptions(fillInTheBlanksData);

            FillInTheBlanksMissingLetters[] textMissingLetters = GetComponentsInChildren<FillInTheBlanksMissingLetters>();

            for (int i = 0; i < textMissingLetters.Length; i++)
            {
                textMissingLetters[i].SetText(options[i], OnResultAction);
            }

            Highlight();
        }

        /// <summary>
        /// Callback of Missing Letter click action with result
        /// </summary>
        /// <param name="resultValue"></param>
        void OnResultAction(bool resultValue)
        {
            missingLettersStats = MissingLettersStats.CanPlace;
            resultAction(resultValue);
            Highlight();
        }

        /// <summary>
        /// To Highlight all the available missing letters
        /// </summary>
        void Highlight()
        {
            foreach (var letter in GetComponentsInChildren<FillInTheBlanksMissingLetters>())
            {
                letter.OnSelect();
            }
        }

        /// <summary>
        /// It Randomise all missing letters given into <FillInTheBlanksData>
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        List<string> GetRandomisedOptions(List<FillInTheBlanksData> data)
        {
            List<string> options = new List<string>();

            for (int i = 0; i < data.Count; i++)
            {
                data[i].missingLetters = data[i].spelling.Substring(data[i].startIndex, data[i].endIndex - data[i].startIndex + 1);
                options.Add(data[i].missingLetters);
            }

            IListExtensions.Shuffle(options);
            return options;
        }
    }
}
