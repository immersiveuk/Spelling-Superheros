using Com.Immersive.Cameras;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Immersive.SuperHero
{
    public class SuperHeroCreatorManager : MonoBehaviour
    {
        public static SuperHeroCreatorManager Instance;

        [System.Serializable]
        public class SuperHeroSceneData
        {
            public SuperHeroCreatorStages Stage;
            public Sprite partSprite;
            public AudioClip introductionClip;            
            public AudioClip musicClip;
        }

        public Sprite continueSprite;
        public Sprite goSprite;
        public Sprite waitingSprite;

        [Header("SFX")]
        public AudioClip selectClip;
        public AudioClip switchClip;

        public List<SuperHeroSceneData> superHeroSceneDatas = new List<SuperHeroSceneData>();

        public AudioSource audioSource;

        public bool isTamplate;

        private void Awake()
        {
            Instance = this;
        }

        public void SetScene(SpriteRenderer spriteRenderer)
        {
            SuperHeroSceneData sceneData = superHeroSceneDatas.Find(obj => obj.Stage == SelectedSuperHeroData.Instance.currentStage);
            AbstractImmersiveCamera.PlayAudio(sceneData.introductionClip, 1);

            spriteRenderer.sprite = sceneData.partSprite;

            audioSource.clip = sceneData.musicClip;
            audioSource.Play();
        }

        public void ChangeButtonSprite(SpriteRenderer spriteRenderer, ButtonState buttonState)
        {
            switch (buttonState)
            {
                case ButtonState.Continue:
                    spriteRenderer.sprite = continueSprite;
                    break;

                case ButtonState.Waiting:
                    spriteRenderer.sprite = waitingSprite;
                    break;

                case ButtonState.Ready:
                    spriteRenderer.sprite = goSprite;
                    break;

            }
        }

        public void PlaySwitch()
        {
            AbstractImmersiveCamera.PlayAudio(switchClip);
        }

        public enum ButtonState { Continue, Waiting, Ready }
    }
}