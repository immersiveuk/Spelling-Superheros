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

        int totalQuestions = 0;
        int answerCount = 0;

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
        }

        private void OnDestroy()
        {
            foreach (var obj in FindObjectsOfType<FillInTheBlanksData>())
            {
                obj.OnResultAction -= OnResultAction;
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
            {
                SuperHeroManager.Instance.currentStage = stage;
                SuperHeroManager.Instance.LoadScene("Super Hero Creator");
            }
        }

        private void OnApplicationQuit()
        {
            PlayerPrefs.SetInt("GameMode", -1);
        }
    }
}