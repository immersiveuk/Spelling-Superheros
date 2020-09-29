using Com.Immersive.Cameras;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

namespace Immersive.FillInTgeBlank
{
    public class FillInTheBlankMissingLetters : MonoBehaviour, IInteractableObject
    {
        enum MissingLettersStats{ NotPlace, Placing, Placed }

        public TextMeshPro textOption, textGlow;
        private FillInTheBlankSpelling selectedSpelling;

        MissingLettersStats LetterStats = MissingLettersStats.NotPlace;

        private void Awake()
        {
            FillInTheBlanksManager.OnSpellingSelected += OnSpellingSelected;
        }

        private void OnDestroy()
        {
            FillInTheBlanksManager.OnSpellingSelected -= OnSpellingSelected;
        }

        public void SetText(string option)
        {
            textOption.text = option;
        }

        private void OnSpellingSelected(FillInTheBlankSpelling spelling)
        {
            selectedSpelling = spelling;
        }

        public void OnPress()
        {
            if (LetterStats != MissingLettersStats.NotPlace)
                return;

            LetterStats = MissingLettersStats.Placing;

            selectedSpelling.missingLetterPosition.localPosition = GetMissingLetterPosition(selectedSpelling.textSpelling.textInfo);

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
                         LetterStats = MissingLettersStats.Placed;
                         StartCoroutine(OnCorrectAnswerWait());
                     })));
        }

        IEnumerator OnCorrectAnswerWait()
        {
            yield return new WaitForSeconds(1);
            selectedSpelling.OnCorrectAnswer();
            FindObjectOfType<FillInTheBlanksManager>().SelectNextSpelling();
        }

        void OnIncorrectAnswer(Vector2 targetPosition)
        {
            Vector2 startPos = this.transform.position;

            iTween.MoveTo(textOption.gameObject, iTween.Hash("x", targetPosition.x, "y", targetPosition.y, "z", -0.2f, "islocal", false,
                     "time", 0.7f, "easetype", iTween.EaseType.easeInOutQuad, "delay", 0, "oncomplete", (System.Action<object>)(newValue =>
                     {
                         iTween.MoveTo(textOption.gameObject, iTween.Hash("x", startPos.x, "y", startPos.y, "z", -0.2f, "islocal", false,
                             "time", 0.7f, "easetype", iTween.EaseType.easeInOutQuad, "delay", 1, "oncomplete", (System.Action<object>)(newNewValue =>
                             {
                                 LetterStats = MissingLettersStats.NotPlace;
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

        Vector3 GetMissingLetterPosition(TMP_TextInfo textInfo)
        {
            Vector2 firstPos = GetPos(textInfo, textInfo.characterInfo.ToList().FindIndex(obj => obj.character == '_'));
            Vector2 secondPos = GetPos(textInfo, textInfo.characterInfo.ToList().FindLastIndex(obj => obj.character == '_'));

            Vector3 centerPosition = new Vector3((firstPos.x + secondPos.x) / 2, 0);
            return centerPosition;
        }

        Vector2 GetPos(TMP_TextInfo textInfo, int index)
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

        public void OnSelect()
        {
            textGlow.enabled = true;
            textGlow.gameObject.AddComponent<PulseAnimation>();        
        }

        public void OnDeselect()
        {
            textGlow.enabled = false;
        }
    }
}