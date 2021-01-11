using Com.Immersive.Cameras;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Immersive.SuperHero
{
    public class SuperHeroGameManager : MonoBehaviour
    {
        public static SuperHeroGameManager Instance;

        [Header("Popup Instructions")]
        public GameObject[] PopupInstructions;
        public GameObject levelUpButton;

        [Header("SFX")]
        public AudioClip laserBlastClip;
        public AudioClip explosionClip;
        public AudioClip levelUpClicp;

        int wallCompleted = 0;

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            wallCompleted = 0;
            PlayAudio();
        }

        void PlayAudio()
        {
            AbstractImmersiveCamera.PlayAudio(GameData.Instance.superheroReadyClip, 1);
            StartCoroutine(DisableIntroductionPopUp());
        }

        IEnumerator DisableIntroductionPopUp()
        {
            yield return new WaitForSeconds(GameData.Instance.superheroReadyClip.length - 1);

            foreach (var obj in PopupInstructions)
            {
                obj.SetActive(false);
            }

            AddEvent();
        }

        public void OnAllEnemiesDestroyedOfWall()
        {
            wallCompleted++;

            if (wallCompleted > 2)
            {
                levelUpButton.SetActive(true);
                //GameData.Instance.LoadScene("End Scene");

                RemoveEvent();
            }
        }

        void AddEvent()
        {
            foreach (var wall in FindObjectsOfType<EnemiesController>())
            {
                wall.AddWallTouchEvent();
                wall.SetWall();
            }
        }

        void RemoveEvent()
        {
            foreach (var wall in FindObjectsOfType<EnemiesController>())
            {
                wall.RemoveWallTouchEvent();
            }
        }

        public void ButtonLevelUp()
        {
            foreach (var wall in FindObjectsOfType<EnemiesController>())
            {
                wall.SetWall();
            }

            StartCoroutine(SetAdvancedMode());
        }

        IEnumerator SetAdvancedMode()
        {
            AbstractImmersiveCamera.PlayAudio(levelUpClicp, 1);
            yield return new WaitForSeconds(levelUpClicp.length);

            PlayerPrefs.SetInt("GameMode", 1);
        }
    }
}
