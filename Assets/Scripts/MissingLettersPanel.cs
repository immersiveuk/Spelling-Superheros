using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Immersive.FillInTgeBlank
{
    public class MissingLettersPanel : MonoBehaviour
    {
        public void SetPanel(List<FillInTheBlanksData> fillInTheBlanksData)
        {
            var options = GetRandomisedOptions(fillInTheBlanksData);

            FillInTheBlankMissingLetters[] textMissingLetters = GetComponentsInChildren<FillInTheBlankMissingLetters>();

            for (int i = 0; i < textMissingLetters.Length; i++)
            {
                textMissingLetters[i].SetText(options[i]);
            }
        }

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
