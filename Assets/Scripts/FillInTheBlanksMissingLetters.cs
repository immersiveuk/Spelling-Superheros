using Com.Immersive.Cameras;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

namespace Immersive.FillInTgeBlank
{
    
    public class FillInTheBlanksMissingLetters : Highlighter, IInteractableObject
    {
        public enum MissingLettersStats {NotPlace, Placing, Placed, CanPlace }

        public TextMeshPro textOption;

        FillInTheBlanksSpelling selectedSpelling;
        MissingLettersStats letterStats = MissingLettersStats.NotPlace;

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
        public void SetText(string option, Action<bool> resultAction)
        {
            textOption.text = option;
            this.resultAction = resultAction;

            SetText(option);
        }

        private void OnSpellingSelected(FillInTheBlanksSpelling spelling)
        {
            selectedSpelling = spelling;
        }

        public void OnPress()
        {
            if (MissingLettersPanel.missingLettersStats != MissingLettersStats.CanPlace || letterStats != MissingLettersStats.NotPlace)
                return;

            letterStats = MissingLettersStats.Placing;
            MissingLettersPanel.missingLettersStats = MissingLettersStats.Placing;

            selectedSpelling.missingLetterPosition.localPosition = GetCenterPositionOfCharacters(selectedSpelling.textSpelling.textInfo);

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

        /// <summary>
        /// It gives center local position of given characters info
        /// </summary>
        /// <param name="textInfo"></param>
        /// <returns></returns>
        Vector2 GetCenterPositionOfCharacters(TMP_TextInfo textInfo)
        {
            List<TMP_CharacterInfo> characterInfos = textInfo.characterInfo.ToList();
            List<Vector2> characterPosition = new List<Vector2>();
            Vector2 centerPosition = Vector2.zero;

            for (int i=0; i<characterInfos.Count; i++)
            {
                if (characterInfos[i].character == '_')
                    characterPosition.Add(GetPositionOfCharacter(textInfo, i));
            }

            for (int i=0; i<characterPosition.Count; i++)
            {
                centerPosition += characterPosition[i];
            }

            centerPosition = new Vector2(centerPosition.x / characterPosition.Count, 0);

            return centerPosition;
        }

        /// <summary>
        /// It calculates the center position of Missing Letters
        /// </summary>
        /// <param name="textInfo"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        Vector2 GetPositionOfCharacter(TMP_TextInfo textInfo, int index)
        {
            int materialIndex = textInfo.characterInfo[index].materialReferenceIndex;

            // Get the index of the first vertex of the selected character.
            int vertexIndex = textInfo.characterInfo[index].vertexIndex;

            // Get a reference to the vertices array.
            Vector3[] vertices = textInfo.meshInfo[materialIndex].vertices;

            // Determine the center point of the character.
            Vector2 charMidBasline = (vertices[vertexIndex + 0] + vertices[vertexIndex + 2]) / 2;

            return charMidBasline;
        }

        /// <summary>
        /// Callback for Missing Letter to Highlight it.
        /// </summary>
        public new void OnSelect()
        {
            if (letterStats == MissingLettersStats.NotPlace)
            {
                base.OnSelect();
            }
        }

        /// <summary>
        /// Callback for Missing Letter to remove the Highlight.
        /// </summary>
        public new void OnDeselect()
        {
            base.OnDeselect();
        }
    }
}