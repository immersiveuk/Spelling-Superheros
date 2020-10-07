using Com.Immersive.Cameras;
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
        [Header("Sounds")]
        public AudioClip positiveClip;
        public AudioClip negativeClip;

        public string sceneName;

        int totalQuestions = 0;
        int answerCount = 0;

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

            if (answerCount >= totalQuestions && !string.IsNullOrEmpty(sceneName))
            {
                Debug.Log("All questions answered");
                SceneManager.LoadScene(sceneName);
            }
        }
    }
}