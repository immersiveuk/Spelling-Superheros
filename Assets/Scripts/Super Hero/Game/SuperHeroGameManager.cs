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
        public AudioClip superheroReadyClip;
        public AudioClip laserBlastClip;
        public AudioClip explosionClip;
        public AudioClip levelUpClicp;
        public AudioClip flyClip;
        public AudioClip victoryClip;

        public AudioSource audioSource;

        int wallCompleted = 0;

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            this.transform.localScale = new Vector3(1 / FindObjectOfType<Stage>().transform.localScale.x, 1, 1);

            wallCompleted = 0;
            GameData.Instance.labAmbienceAudioSource.Stop();

            PlayAudio();
        }

        void PlayAudio()
        {
            AbstractImmersiveCamera.PlayAudio(superheroReadyClip, 1);
            StartCoroutine(DisableIntroductionPopUp());
        }

        IEnumerator DisableIntroductionPopUp()
        {
            yield return new WaitForSeconds(superheroReadyClip.length - 1);

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
                audioSource.Stop();
                audioSource.clip = victoryClip;
                audioSource.Play();
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
            AbstractImmersiveCamera.PlayAudio(flyClip, 1);

            foreach (var wall in FindObjectsOfType<EnemiesController>())
            {
                wall.RemoveWallTouchEvent();
                wall.OnComplete();
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
            audioSource.Stop();

            AbstractImmersiveCamera.PlayAudio(levelUpClicp, 1);
            yield return new WaitForSeconds(levelUpClicp.length);

            PlayerPrefs.SetInt("GameMode", 1);
            GameData.Instance.ResetManager();
            GameData.Instance.LoadScene("Stage1");
        }
    }
}
