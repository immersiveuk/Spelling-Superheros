using Com.Immersive.Cameras;
using Immersive.SuperHero;
using Mono.Options;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Immersive.FillInTheBlank
{
    public class FillInTheBlanksManager : MonoBehaviour
    {
        public static FillInTheBlanksManager Instance;

        public enum GameMode { Simple,Advanced}
        public GameMode gameMode;

        public FillInTheBlankStages stage;

        [Header("Sounds")]
        public AudioClip positiveClip;
        public AudioClip negativeClip;
        public AudioClip selectClip;
        public AudioClip moveClip;
        public AudioClip moveBackClip;

        [Header("Music")]
        public AudioClip introClip;

        int totalQuestions = 0;
        int answerCount = 0;

        [Header("Popup Instructions")]
        public GameObject[] instructionsPopup;

        [Header("Popup Complete")]
        public GameObject completePopupCenter;
        public GameObject completePopupLeft;
        public GameObject completePopupRight;

        [Header("Font Size")]
        public int fontSizeSimpleMode = 12;
        public int fotSizeAdvancedMode = 20;

        
        private void Awake()
        {
            Instance = this;
            if (PlayerPrefs.GetInt("GameMode", -1) != -1)
            {
                Debug.Log("game Mode");
                gameMode = (GameMode)PlayerPrefs.GetInt("GameMode");
            }
        }

        private void Start()
        {

            foreach (var obj in FindObjectsOfType<FillInTheBlanksData>())
            {
                totalQuestions += obj.fillInTheBlanksList.Count;
                obj.OnResultAction += OnResultAction;
            }

            if (GameData.Instance.currentStage == FillInTheBlankStages.Stage1)
            {
                AbstractImmersiveCamera.PlayAudio(introClip, 1);
                StartCoroutine(DisableIntroductionPopUp());
            }                
        }

        private void OnDestroy()
        {
            foreach (var obj in FindObjectsOfType<FillInTheBlanksData>())
            {
                obj.OnResultAction -= OnResultAction;
            }
        }

        IEnumerator DisableIntroductionPopUp()
        {
            yield return new WaitForSeconds(introClip.length-1);

            foreach (var obj in instructionsPopup)
            {
                obj.SetActive(false);
            }
        }

        /// <summary>
        /// Callback after click on Missing Letter with result
        /// </summary>
        /// <param name="result"></param>
        void OnResultAction(bool result)
        {
            if (result)
            {
                answerCount++;
                AbstractImmersiveCamera.PlayAudio(positiveClip);
            }
            else
                AbstractImmersiveCamera.PlayAudio(negativeClip);

            if (answerCount >= 1)
            //if (answerCount >= totalQuestions)
            {
                GameData.Instance.currentStage = stage;
                GameData.Instance.LoadScene("Super Hero Creator");
            }
        }

        public void PlaySelect()
        {
            AbstractImmersiveCamera.PlayAudio(selectClip);
        }

        public void PlayMove()
        {
            AbstractImmersiveCamera.PlayAudio(moveClip);
        }

        public void PlayMoveBack()
        {
            AbstractImmersiveCamera.PlayAudio(moveBackClip);
        }

        private void OnApplicationQuit()
        {
            PlayerPrefs.SetInt("GameMode", -1);
        }
    }
}