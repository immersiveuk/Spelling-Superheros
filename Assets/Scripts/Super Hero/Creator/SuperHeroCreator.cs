using Com.Immersive.Cameras;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Immersive.SuperHero
{
    public enum WallType { Left, Center, Right }

    public class SuperHeroCreator : MonoBehaviour
    {
        public WallType wallType;

        [Header("Settings")]
        public SuperHeroSettings superHero;

        [Header("Body Parts Scroll")]
        public HorizontalScroll headsPanel;
        public HorizontalScroll bodiesPanel;
        public HorizontalScroll legsPanel;

        public SpriteRenderer continueButton;
        public Sprite goSprite;
        public GameObject startButton;

        SelectedSuperHero selectedSuperHero;

        void Start()
        {
            this.transform.localScale = new Vector3(1 / FindObjectOfType<Stage>().transform.localScale.x, 1, 1);

            selectedSuperHero = SuperHeroManager.Instance.GetSuperHero(wallType);

            SetSuperHero();
            SetPosition();
        }

        void SetSuperHero()
        {
            if (SuperHeroManager.Instance.currentStage != FillInTheBlankStages.None)
                startButton.SetActive(false);

            switch (SuperHeroManager.Instance.currentStage)
            {
                case FillInTheBlankStages.Stage1:
                    headsPanel.SetScroll(superHero.heads, OnScroll);
                    break;

                case FillInTheBlankStages.Stage2:
                    headsPanel.SetSelectedSprite(selectedSuperHero.head);
                    bodiesPanel.SetScroll(superHero.bodies, OnScroll);                    
                    break;

                case FillInTheBlankStages.Stage3:
                    headsPanel.SetSelectedSprite(selectedSuperHero.head);
                    bodiesPanel.SetSelectedSprite(selectedSuperHero.body);
                    legsPanel.SetScroll(superHero.legs, OnScroll);

                    continueButton.sprite = goSprite;
                    break;
            }

            SuperHeroManager.Instance.ResetWallSelected();
        }

        void OnScroll()
        {
            SuperHeroManager.Instance.selectedWalls[wallType] = false;
            continueButton.color = Color.white;
            continueButton.gameObject.SetActive(true);
        }

        void SetPosition() 
        {
            this.transform.position = new Vector3(AbstractImmersiveCamera.CurrentImmersiveCamera.cameras[(int)wallType].transform.position.x, 0, 0);
        }

        public void ContinueButton()
        {
            continueButton.color = Color.black;

            switch (SuperHeroManager.Instance.currentStage)
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

            SuperHeroManager.Instance.SetSuperHeroData(wallType, selectedSuperHero);
        }

        public void LoadFillInTheBlank()
        {            
            SuperHeroManager.Instance.LoadScene("Stage1");
        }
    }
}