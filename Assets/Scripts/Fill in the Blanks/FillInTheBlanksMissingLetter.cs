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

        protected virtual void Solved() { }

        public enum MissingLettersStats {NotPlace, Placing, Placed, CanPlace }
        MissingLettersStats letterStats = MissingLettersStats.NotPlace;

        public TextMeshPro textOption;

        FillInTheBlanksSpelling selectedSpelling;
        FillInTheBlanksData fillInTheBlanksController;

        Action<bool> resultAction;

        private void Awake()
        {
            
        }

        private void OnDestroy()
        {
           
        }

        /// <summary>
        /// It is to set "Missing Letter" text value to TextMesh pro and Highlighter Text.
        /// </summary>
        /// <param name="data"></param>
        public void SetText(SpellingSettings data, FillInTheBlanksData controller, Action<bool> resultAction)
        {
            fillInTheBlanksController = controller;
            controller.OnSpellingSelected += OnSpellingSelected;

            string option = SplitSpelling(data.spelling, data.missingLettersPairs);

            this.resultAction = resultAction;
            data.missingLetters = option;
            textOption.text = option;

            if (FillInTheBlanksManager.Instance.gameMode == GameMode.Simple)
                textOption.fontSize = FillInTheBlanksManager.Instance.fontSizeSimpleMode;
            else
                textOption.fontSize = FillInTheBlanksManager.Instance.fotSizeAdvancedMode;

            OnSelect();
        }

        string SplitSpelling(string spelling, List<MissingLettersPair> missingLettersPairs)
        {
            if (string.IsNullOrEmpty(spelling) || missingLettersPairs.Count == 0)
                return "";

            for (int i = missingLettersPairs.Count - 1; i >= 0; i--)
            {
                MissingLettersPair position = missingLettersPairs[i];

                spelling = spelling.Insert(position.endIndex + 1, "<#00000000>");
                spelling = spelling.Insert(position.startIndex, "</color>");
            }

            spelling = spelling.Insert(0, "<#00000000>");
            spelling = spelling.Insert(spelling.Length, "</color>");

            spelling = spelling.Remove(0, spelling.IndexOf("/") + 7);
            spelling = spelling.Remove(spelling.LastIndexOf("#") - 1);

            return spelling;
        }

        private void OnSpellingSelected(FillInTheBlanksSpelling spelling)
        {
            selectedSpelling = spelling;
        }

        public void OnPress()
        {
            if (fillInTheBlanksController.missingLettersStats != MissingLettersStats.CanPlace || letterStats != MissingLettersStats.NotPlace)
                return;

            FillInTheBlanksManager.Instance.PlaySelect();

            letterStats = MissingLettersStats.Placing;
            fillInTheBlanksController.missingLettersStats = MissingLettersStats.Placing;

            selectedSpelling.missingLetterPosition.localPosition = GetPosition(selectedSpelling.textSpelling);

            OnDeselect();

            StartCoroutine(MoveLetters());
        }

        IEnumerator MoveLetters()
        {            
            yield return new WaitForSeconds(0.3f);

            FillInTheBlanksManager.Instance.PlayMove();

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
            Solved();
        }

        void OnIncorrectAnswer(Vector2 targetPosition)
        {
            Vector2 startPos = this.transform.position;

            iTween.MoveTo(textOption.gameObject, iTween.Hash("x", targetPosition.x, "y", targetPosition.y, "z", -0.2f, "islocal", false,
                     "time", 0.7f, "easetype", iTween.EaseType.easeInOutQuad, "delay", 0, "oncomplete", (System.Action<object>)(newValue =>
                     {
                         StartCoroutine(OnIncorrectAnswerWait());
                         iTween.MoveTo(textOption.gameObject, iTween.Hash("x", startPos.x, "y", startPos.y, "z", -0.2f, "islocal", false,
                             "time", 0.5f, "easetype", iTween.EaseType.easeInOutQuad, "delay", 1.5f, "oncomplete", (System.Action<object>)(newNewValue =>
                             {                                
                                 letterStats = MissingLettersStats.NotPlace;                                 
                                 OnSelect();
                             })));

                     })));
        }

        IEnumerator OnIncorrectAnswerWait()
        {
            yield return new WaitForSeconds(1.0f);
            resultAction(false);
            yield return new WaitForSeconds(0.5f);

            FillInTheBlanksManager.Instance.PlayMoveBack();
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

        
        Vector2 GetPosition(TextMeshPro textMeshPro)
        {
            TMP_TextInfo textInfo = textMeshPro.textInfo;

            Vector2 centerPosition;

            Vector3 bottomLeft = GetPositionOfCharacter(textInfo, selectedSpelling.spellingData.missingLettersPairs[0].startIndex, true);
            Vector3 bottomRight = GetPositionOfCharacter(textInfo, selectedSpelling.spellingData.missingLettersPairs[selectedSpelling.spellingData.missingLettersPairs.Count - 1].endIndex, false);

            centerPosition = (bottomLeft + bottomRight) / 2;

            centerPosition = new Vector2(centerPosition.x, -(textInfo.lineCount - 1) * textMeshPro.textBounds.extents.y / 2);

            return centerPosition;
        }

   
        Vector2 GetPositionOfCharacter(TMP_TextInfo textInfo, int index, bool isLeft)
        {
            if (FillInTheBlanksManager.Instance.gameMode == GameMode.Simple)
                index += selectedSpelling.spellingData.spelling.Length + 3;

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