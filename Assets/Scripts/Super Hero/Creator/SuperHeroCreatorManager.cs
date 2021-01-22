using Com.Immersive.Cameras;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Immersive.SuperHero
{
    public class SuperHeroCreatorManager : MonoBehaviour
    {
        public static SuperHeroCreatorManager Instance;

        [Header("Audio Clip")]
        public AudioClip chooseHeadClip;
        public AudioClip chooseBodyClip;
        public AudioClip chooseLegClip;

        public Sprite chooseHeadSprite;
        public Sprite chooseBodySprite;
        public Sprite chooseLegSprite;

        private void Awake()
        {
            Instance = this;
        }

        public void PlayIntroductionClip()
        {
            switch (GameData.Instance.currentStage)
            {
                case FillInTheBlankStages.Stage1:
                    AbstractImmersiveCamera.PlayAudio(chooseHeadClip, 1);
                    break;

                case FillInTheBlankStages.Stage2:
                    AbstractImmersiveCamera.PlayAudio(chooseBodyClip, 1);
                    break;

                case FillInTheBlankStages.Stage3:
                    AbstractImmersiveCamera.PlayAudio(chooseLegClip, 1);
                    break;
            }
        }

        public void SetMonitor(SpriteRenderer spriteRenderer)
        {
            switch (GameData.Instance.currentStage)
            {
                case FillInTheBlankStages.Stage1:
                    spriteRenderer.sprite = chooseHeadSprite;
                    break;

                case FillInTheBlankStages.Stage2:
                    spriteRenderer.sprite = chooseBodySprite;
                    break;

                case FillInTheBlankStages.Stage3:
                    spriteRenderer.sprite = chooseLegSprite;
                    break;
            }
        }
    }
}