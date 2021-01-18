using Com.Immersive.Cameras;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

namespace Immersive.SuperHero
{
    public enum WallType { Left, Center, Right }
    public enum SuperHeroPart { Head, Body, Leg }

    public class SuperHeroCreator : MonoBehaviour
    {
        public WallType wallType;

        [Header("Settings")]
        public SuperHeroSettings superHero;

        [Header("Body Parts Scroll")]
        public HorizontalScroll headsPanel;
        public HorizontalScroll bodiesPanel;
        public HorizontalScroll legsPanel;

        [Header("Buttons")]
        public SpriteRenderer continueButton;
        public Sprite goSprite;
        public Sprite waitingSprite;
        public GameObject startButton;
        public GameObject nextButton;
        public GameObject previousButton;
        
        [Header("Audio Clip")]
        public AudioClip chooseHeadClip;
        public AudioClip chooseBodyClip;
        public AudioClip chooseLegClip;

        [Header("Video")]
        public VideoPlayer videoPlayer;
        public VideoClip clip1, clip2;

        public Animator animator;

        SelectedSuperHero selectedSuperHero;

        void Start()
        {
            this.transform.localScale = new Vector3(1 / FindObjectOfType<Stage>().transform.localScale.x, 1, 1);

            selectedSuperHero = GameData.Instance.GetSuperHero(wallType);

            SetSuperHero();
            SetPosition();

            if (wallType == WallType.Center)
            {
                GameData.Instance.PlaySuperHeroLabMusic();
            }
        }

        void SetSuperHero()
        {
            if (GameData.Instance.currentStage != FillInTheBlankStages.None)
            {
                nextButton.SetActive(true);
                previousButton.SetActive(true);
                startButton.SetActive(false);
            }
            else
            {
                animator.SetInteger("Stats", 0);

                nextButton.SetActive(false);
                previousButton.SetActive(false);
                startButton.SetActive(true);
            }

            switch (GameData.Instance.currentStage)
            {
                case FillInTheBlankStages.Stage1:
                    headsPanel.SetScroll(superHero.heads, OnScroll);

                    //PlayVideo(clip1);                   
                    animator.SetInteger("Stats",1);
                    if (chooseHeadClip)
                        AbstractImmersiveCamera.PlayAudio(chooseHeadClip, 1);
                    break;

                case FillInTheBlankStages.Stage2:
                    headsPanel.SetSelectedSprite(selectedSuperHero.head);
                    bodiesPanel.SetScroll(superHero.bodies, OnScroll);

                    //PlayVideo(clip2);
                    animator.SetInteger("Stats", 2);

                    if (chooseBodyClip)
                        AbstractImmersiveCamera.PlayAudio(chooseBodyClip, 1);
                    break;

                case FillInTheBlankStages.Stage3:
                    headsPanel.SetSelectedSprite(selectedSuperHero.head);
                    bodiesPanel.SetSelectedSprite(selectedSuperHero.body);
                    legsPanel.SetScroll(superHero.legs, OnScroll);

                    continueButton.sprite = goSprite;

                    //PlayVideo(null);
                    animator.gameObject.SetActive(false);

                    if (chooseLegClip)
                        AbstractImmersiveCamera.PlayAudio(chooseLegClip, 1);
                    break;
            }

            GameData.Instance.ResetWallSelected();
        }

        void OnScroll()
        {
            GameData.Instance.selectedWalls[wallType] = false;
            continueButton.gameObject.SetActive(true);
        }

        void SetPosition() 
        {
            this.transform.position = new Vector3(AbstractImmersiveCamera.CurrentImmersiveCamera.cameras[(int)wallType].transform.position.x, 0, 0);
        }

        public void ContinueButton()
        {
            continueButton.sprite = waitingSprite;

            switch (GameData.Instance.currentStage)
            {
                case FillInTheBlankStages.Stage1:
                    selectedSuperHero.head = headsPanel.GetSelectedPart();
                    break;

                case FillInTheBlankStages.Stage2:
                    selectedSuperHero.body = bodiesPanel.GetSelectedPart();
                    break;

                case FillInTheBlankStages.Stage3:
                    selectedSuperHero.leg = legsPanel.GetSelectedPart();
                    break;
            }

            GameData.Instance.PlaySelect();
            GameData.Instance.SetSuperHeroData(wallType, selectedSuperHero);
        }

        public void LoadFillInTheBlank()
        {
            GameData.Instance.PlaySelect();
            GameData.Instance.currentStage = FillInTheBlankStages.Stage1;
            GameData.Instance.LoadScene("Stage1");
        }

        void PlayVideo(VideoClip clip)
        {
            videoPlayer.Stop();
            videoPlayer.gameObject.SetActive(false);

            if (clip == null)
                return;

            videoPlayer.gameObject.SetActive(true);
            videoPlayer.clip = clip;
            videoPlayer.Play();
        }

        public void NextButton()
        {
            Move(true);
        }

        public void PreviousButton()
        {
            Move(false);
        }

        void Move(bool next)
        {
            HorizontalScroll panel = headsPanel;

            switch (GameData.Instance.currentStage)
            {
                case FillInTheBlankStages.Stage1:
                    panel = headsPanel;
                    break;

                case FillInTheBlankStages.Stage2:
                    panel = bodiesPanel;
                    break;

                case FillInTheBlankStages.Stage3:
                    panel = legsPanel;
                    break;
            }

            if (next)
                panel.MoveNext();
            else
                panel.MovePrevious();
        }
    }
}