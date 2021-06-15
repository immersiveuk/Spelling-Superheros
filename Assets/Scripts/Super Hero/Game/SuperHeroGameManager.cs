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
        public GameObject advanceModePanel;
        public GameObject normalAdvanceModePanel;

        [Header("SFX")]
        public AudioClip superheroReadyClip;
        public AudioClip laserBlastClip;
        public AudioClip explosionClip;
        public AudioClip advanceModeLevelUpClip;
        public AudioClip normalModeLevelUpClip;
        public AudioClip flyClip;
        public AudioClip victoryClip;

        public AudioSource audioSource;

        //int wallCompleted = 0;

        public int totalEnemies;

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            this.transform.localScale = new Vector3(1 / FindObjectOfType<Stage>().transform.localScale.x, 1, 1);

            //wallCompleted = 0;
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
            //wallCompleted++;
            totalEnemies--;

            if (totalEnemies <= 0)
            {
                audioSource.Stop();
                audioSource.clip = victoryClip;
                audioSource.Play();
                RemoveEvent();

                if (PlayerPrefs.GetInt("GameMode") == 0)
                {
                    advanceModePanel.SetActive(true);
                }
                else
                {
                    audioSource.volume = 0.5f;
                    AbstractImmersiveCamera.PlayAudio(normalModeLevelUpClip, 1);
                    normalAdvanceModePanel.SetActive(true);
                }     

            }
        }

        void AddEvent()
        {
            totalEnemies = 0;
            foreach (var wall in FindObjectsOfType<EnemiesController>())
            {
                wall.AddWallTouchEvent();
                wall.SetWall();
                totalEnemies += wall.GetTotalEnemies();
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

        public void AdvanceModeButton()
        {
            PlayerPrefs.SetInt("GameMode", 1);

            StartCoroutine(SetAdvanceMode(advanceModeLevelUpClip));
        }

        IEnumerator SetAdvanceMode(AudioClip clip)
        {
            audioSource.Stop();

            foreach (var wall in FindObjectsOfType<EnemiesController>())
            {
                wall.SetWall();
            }

            AbstractImmersiveCamera.PlayAudio(clip, 1);
            yield return new WaitForSeconds(clip.length);

            SetGameMode();
        }

        public void Level1Button()
        {
            PlayerPrefs.SetInt("GameMode", 0);
            SetGameMode();
        }

        public void Level2Button()
        {
            PlayerPrefs.SetInt("GameMode", 1);
            SetGameMode();
        }

        void SetGameMode()
        {
            GameData.Instance.ResetManager();
            GameData.Instance.LoadScene("Stage1");
            GameData.Instance.SetLevel();
        }        
    }
}
