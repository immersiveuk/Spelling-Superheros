using Com.Immersive.Cameras;
using Immersive.SuperHero;
using Mono.Options;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Immersive.FillInTheBlank
{
    public enum GameMode { Simple, Advanced }

    public class FillInTheBlanksManager : MonoBehaviour
    {
        public static FillInTheBlanksManager Instance;
       
        public GameMode gameMode;

        public SuperHeroCreatorStages stage;

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

        [Header("Popup Monitor")]
        public GameObject[] monitors;

        [Header("Font Size")]
        public int fontSizeSimpleMode = 12;
        public int fotSizeAdvancedMode = 12;

        public FillInTheBlanksData leftWallWords;
        public FillInTheBlanksData centerWallWords;
        public FillInTheBlanksData rightWallWords;

        //public TextMeshPro textLevel;

        private void Awake()
        {
            Instance = this;
            gameMode = (GameMode)PlayerPrefs.GetInt("GameMode");

            fontSizeSimpleMode = 12;
            fotSizeAdvancedMode = 12;

            SetWords();
        }

        void SetWords()
        {
            FillInTheBlanksDataStage words = GameData.Instance.GetWords();

            if (GameData.Instance && words != null)
            {                
                leftWallWords.fillInTheBlanksList = words.fillInTheBlanksLeft;
                centerWallWords.fillInTheBlanksList = words.fillInTheBlanksCenter;
                rightWallWords.fillInTheBlanksList = words.fillInTheBlanksRight;
            }

            //leftWallWords.letterCase = centerWallWords.letterCase = centerWallWords.letterCase = GameData.Instance.letterCase;
        }

        private void Start()
        {
            //textLevel.text = "Level: " + ((int)stage + 1);

            foreach (var obj in FindObjectsOfType<FillInTheBlanksData>())
            {
                totalQuestions += obj.fillInTheBlanksList.Count;
                obj.OnResultAction += OnResultAction;
            }

            if (GameData.Instance.currentStage == SuperHeroCreatorStages.Stage1)
            {
                AbstractImmersiveCamera.PlayAudio(introClip, 1);
                StartCoroutine(DisableIntroductionPopUp());

                for (int i = 0; i < instructionsPopup.Length; i++)
                {
                    iTween.ScaleFrom(instructionsPopup[i], Vector3.zero, 1);
                }
            }
            else
            {
                for (int i = 0; i < monitors.Length; i++)
                {
                    iTween.ScaleFrom(monitors[i], Vector3.zero, 1);
                }
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
            yield return new WaitForSeconds(introClip.length + 1);

            for (int i = 0; i < instructionsPopup.Length; i++)
            {
                instructionsPopup[i].SetActive(false);
                monitors[i].SetActive(true);
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

            //if (answerCount >= 1) //for quick testing in Unity Editor it's set to one spelling
            if (answerCount >= totalQuestions)
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
    }
}