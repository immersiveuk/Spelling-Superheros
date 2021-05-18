using Com.Immersive.Cameras;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Immersive.SuperHero
{
    public class SuperHeroCreatorManager : MonoBehaviour
    {

        public enum CustomizationType { None, TamplateParts, TamplateFullBody }

        [System.Serializable]
        public class SuperHeroSceneData
        {
            public SuperHeroCreatorStages Stage;
            public Sprite partSprite;
            public Sprite silhouetteParticle;
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

        public CustomizationType customizationType;
        //public TextMeshPro textLevel;

        private void Awake()
        {
            if (customizationType == CustomizationType.TamplateFullBody)
                SelectedSuperHeroData.Instance.currentStage = SuperHeroCreatorStages.Full;
        }

        private void Start()
        {
            //textLevel.text = "Level: " + ((int)SelectedSuperHeroData.Instance.currentStage + 1);
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

        public void ChangeSilhouetteParticleSprite(SpriteRenderer sprite, SuperHeroCreatorStages stage)
        {
            sprite.sprite = superHeroSceneDatas.Find(obj => obj.Stage == stage).silhouetteParticle;
        }

        public void PlaySwitch()
        {
            AbstractImmersiveCamera.PlayAudio(switchClip);
        }

        public enum ButtonState { Continue, Waiting, Ready }
    }
}