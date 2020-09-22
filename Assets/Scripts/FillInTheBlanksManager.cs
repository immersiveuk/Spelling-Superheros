using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FillInTheBlanksManager : MonoBehaviour
{
    [System.Serializable]
    public class FillInTheBlanksData
    {
        public string spelling;
        public string option;
    }

    public RectTransform SpellingPanel, OptionPanel;
    public Spelling spellingPrefab;
    public Option optionPrefab;

    public List<FillInTheBlanksData> fillInTheBlanksData = new List<FillInTheBlanksData>();

    List<Spelling> spellings;

    Spelling selectedWord;
    private int questionNo = 0;

    void Start()
    {
        spellings = new List<Spelling>();
        RandomiseOptions();
    }

    void RandomiseOptions()
    {
        List<string> options = new List<string>();

        for (int i = 0; i < fillInTheBlanksData.Count; i++)
        {
            options.Add(fillInTheBlanksData[i].option);
        }

        IListExtensions.Shuffle(options);

        CreateGamePanel(options);
    }

    void CreateGamePanel(List<string> options)
    {
        for (int i = 0; i < fillInTheBlanksData.Count; i++)
        {
            Spelling textSpelling = Instantiate(spellingPrefab, SpellingPanel);
            Option textOption = Instantiate(optionPrefab, OptionPanel);

            textSpelling.SetText(fillInTheBlanksData[i]);
            textOption.SetText(options[i]);

            spellings.Add(textSpelling);
        }

        DisableLayout();
        SelectSpeling();
    }

    void DisableLayout()
    {
        LayoutRebuilder.ForceRebuildLayoutImmediate(SpellingPanel);
        LayoutRebuilder.ForceRebuildLayoutImmediate(OptionPanel);

        SpellingPanel.GetComponent<SpriteRenderer>().size = SpellingPanel.sizeDelta + Vector2.one * 0.05f;
        OptionPanel.GetComponent<SpriteRenderer>().size = OptionPanel.sizeDelta + Vector2.one * 0.05f;

        SpellingPanel.GetComponent<ContentSizeFitter>().enabled = false;
        SpellingPanel.GetComponent<VerticalLayoutGroup>().enabled = false;

        OptionPanel.GetComponent<ContentSizeFitter>().enabled = false;
        OptionPanel.GetComponent<VerticalLayoutGroup>().enabled = false;
    }

    public void SelectSpeling()
    {
        foreach (var obj in spellings)
        {
            obj.OnDeselect();
        }

        if (questionNo >= spellings.Count)
            return;

        selectedWord = spellings[questionNo];
        selectedWord.OnSelect();

        questionNo++;
    }

    public Spelling OnOptionSelect()
    {
        Vector2 firstPos = GetPos(selectedWord.textSpelling.textInfo.characterInfo.ToList().FindIndex(obj => obj.character == '_'));
        Vector2 secondPos = GetPos(selectedWord.textSpelling.textInfo.characterInfo.ToList().FindLastIndex(obj => obj.character == '_'));

        selectedWord.localPos.localPosition = new Vector3((firstPos.x + secondPos.x)/2, selectedWord.localPos.localPosition.y);

        return selectedWord;
    }

    Vector2 GetPos(int index)
    {    
        int materialIndex = selectedWord.textSpelling.textInfo.characterInfo[index].materialReferenceIndex;

        // Get the index of the first vertex of the selected character.
        int vertexIndex = selectedWord.textSpelling.textInfo.characterInfo[index].vertexIndex;

        // Get a reference to the vertices array.
        Vector3[] vertices = selectedWord.textSpelling.textInfo.meshInfo[materialIndex].vertices;

        // Determine the center point of the character.
        Vector2 charMidBasline = (vertices[vertexIndex + 0] + vertices[vertexIndex + 2]) / 2;

        return charMidBasline;
    }
}

public static class IListExtensions
{
    /// <summary>
    /// Shuffles the element order of the specified list.
    /// </summary>
    public static void Shuffle<T>(this IList<T> ts)
    {
        var count = ts.Count;
        var last = count - 1;
        for (var i = 0; i < last; ++i)
        {
            var r = UnityEngine.Random.Range(i, count);
            var tmp = ts[i];
            ts[i] = ts[r];
            ts[r] = tmp;
        }
    }
}