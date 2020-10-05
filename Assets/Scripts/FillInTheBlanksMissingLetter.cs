using Com.Immersive.Cameras;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

namespace Immersive.FillInTheBlank
{
    public class FillInTheBlanksMissingLetter : MonoBehaviour, IInteractableObject
    {
        [System.Serializable]
        public class SpellingParts
        {
            public string type;
            public string value;

            public SpellingParts(string type, string value)
            {
                this.type = type;
                this.value = value;
            }
        }

        protected virtual void Highlight() {}

        protected virtual void Unhighlight() {}

        public enum MissingLettersStats {NotPlace, Placing, Placed, CanPlace }
        MissingLettersStats letterStats = MissingLettersStats.NotPlace;

        public TextMeshPro textOption;

        FillInTheBlanksSpelling selectedSpelling;
        

        Action<bool> resultAction;

        private void Awake()
        {
            FillInTheBlanksManager.OnSpellingSelected += OnSpellingSelected;
        }

        private void OnDestroy()
        {
            FillInTheBlanksManager.OnSpellingSelected -= OnSpellingSelected;
        }

        /// <summary>
        /// It is to set "Missing Letter" text value to TextMesh pro and Highlighter Text.
        /// </summary>
        /// <param name="data"></param>
        public void SetText(FillInTheBlanksData data, Action<bool> resultAction)
        {
            string option = "";

            List<SpellingParts> spellingParts = new List<SpellingParts>();

            for (int i = 0; i < data.missingPairs; i++)
            {
                if (i == 0 && data.indexs[i].x > 0)
                    spellingParts.Add(new SpellingParts("Spelling", data.spelling.Substring(0, data.indexs[i].x)));

                spellingParts.Add(new SpellingParts("Option", data.spelling.Substring(data.indexs[i].x, data.indexs[i].y - data.indexs[i].x + 1)));

                if (i < data.missingPairs - 1)
                    spellingParts.Add(new SpellingParts("Spelling", data.spelling.Substring(data.indexs[i].y + 1, data.indexs[i + 1].x - data.indexs[i].y - 1)));
            }

            spellingParts.Add(new SpellingParts("Spelling", data.spelling.Substring(data.indexs[data.indexs.Length - 1].y + 1, data.spelling.Length - data.indexs[data.indexs.Length - 1].y-1)));

            foreach (var obj in spellingParts)
            {
                if (!string.IsNullOrEmpty(obj.value))
                {
                    if (obj.type.Contains("Spelling"))
                    {
                        option += "<#00000000>" + obj.value + "</color>";
                    }
                    else
                        option += obj.value;
                }
            }

            data.missingLetters = option;
            textOption.text = option;
            this.resultAction = resultAction;

            OnSelect();
        }

        private void OnSpellingSelected(FillInTheBlanksSpelling spelling)
        {
            selectedSpelling = spelling;
        }

        public void OnPress()
        {
            if (FillInTheBlanksManager.missingLettersStats != MissingLettersStats.CanPlace || letterStats != MissingLettersStats.NotPlace)
                return;

            letterStats = MissingLettersStats.Placing;
            FillInTheBlanksManager.missingLettersStats = MissingLettersStats.Placing;

            //selectedSpelling.missingLetterPosition.localPosition = GetPosition(selectedSpelling.textSpelling.textInfo);

            OnDeselect();

            if (selectedSpelling.spellingData.missingLetters.Equals(textOption.text))
            {
                OnCorrectAnswer(selectedSpelling.missingLetterPosition.position);
            }
            else
            {
                OnIncorrectAnswer(selectedSpelling.missingLetterPosition.position);  
            }
        }

        void OnCorrectAnswer(Vector2 targetPosition)
        {
            iTween.MoveTo(textOption.gameObject, iTween.Hash("x", targetPosition.x, "y", targetPosition.y, "z", -0.2f, "islocal", false,
                     "time", 0.7f, "easetype", iTween.EaseType.easeInOutQuad, "delay", 0, "oncomplete", (System.Action<object>)(newValue =>
                     {
                         letterStats = MissingLettersStats.Placed;
                         StartCoroutine(OnCorrectAnswerWait());
                     })));
        }

        IEnumerator OnCorrectAnswerWait()
        {
            yield return new WaitForSeconds(1);
            selectedSpelling.OnCorrectAnswer();
            resultAction(true);
        }

        void OnIncorrectAnswer(Vector2 targetPosition)
        {
            Vector2 startPos = this.transform.position;

            iTween.MoveTo(textOption.gameObject, iTween.Hash("x", targetPosition.x, "y", targetPosition.y, "z", -0.2f, "islocal", false,
                     "time", 0.7f, "easetype", iTween.EaseType.easeInOutQuad, "delay", 0, "oncomplete", (System.Action<object>)(newValue =>
                     {
                         iTween.MoveTo(textOption.gameObject, iTween.Hash("x", startPos.x, "y", startPos.y, "z", -0.2f, "islocal", false,
                             "time", 0.5f, "easetype", iTween.EaseType.easeInOutQuad, "delay", 1, "oncomplete", (System.Action<object>)(newNewValue =>
                             {
                                 letterStats = MissingLettersStats.NotPlace;
                                 resultAction(false);
                             })));

                     })));
        }

        public void OnRelease()
        {

        }

        public void OnTouchEnter()
        {

        }

        public void OnTouchExit()
        {

        }

        Vector2 GetPosition(TMP_TextInfo textInfo)
        {
            Vector2 centerPosition;

            Vector3 bottomLeft = GetPositionOfCharacter(textInfo, selectedSpelling.spellingData.indexs[0].x, true);
            Vector3 bottomRight = GetPositionOfCharacter(textInfo, selectedSpelling.spellingData.indexs[selectedSpelling.spellingData.indexs.Length-1].y, false);

            centerPosition = (bottomLeft + bottomRight) / 2;

            centerPosition = new Vector2(selectedSpelling.textSpelling.transform.position.x, 0);

            Debug.Log(centerPosition);

            return centerPosition;
        }

   
        Vector2 GetPositionOfCharacter(TMP_TextInfo textInfo, int index, bool isLeft)
        {
            int materialIndex = textInfo.characterInfo[index].materialReferenceIndex;

            // Get the index of the first vertex of the selected character.
            int vertexIndex = textInfo.characterInfo[index].vertexIndex;

            // Get a reference to the vertices array.
            Vector3[] vertices = textInfo.meshInfo[materialIndex].vertices;

            // Determine the center point of the character.
            Vector2 charMidBasline;

            if (isLeft)
            {
                charMidBasline = vertices[vertexIndex + 0];
                //Debug.Log("Left "+vertices[vertexIndex + 0]);
            }
            else
            {
                charMidBasline = vertices[vertexIndex + 2];
                //Debug.Log("Right    "+vertices[vertexIndex + 2]);
            }

            return charMidBasline;
        }

        /// <summary>
        /// Callback for Missing Letter to Highlight it.
        /// </summary>
        public void OnSelect()
        {
            if (letterStats == MissingLettersStats.NotPlace)
            {
                Highlight();
            }
        }

        /// <summary>
        /// Callback for Missing Letter to remove the Highlight.
        /// </summary>
        public void OnDeselect()
        {
            Unhighlight();
        }
    }
}