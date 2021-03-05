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
        public SpriteRenderer choosePart;

        public GameObject panelBodyButtons, panelHeadButtons, panelLegButtons;

        [Header("Video")]
        public VideoPlayer videoPlayer;
        public VideoClip clip1, clip2;

        public Animator animator;

        SelectedSuperHero selectedSuperHero;

        private void OnDestroy()
        {
            SelectedSuperHeroData.OnSuperHeroPartSelectedEvent -= OnSuperHeroPartSelected;
        }

        void Start()
        {
            if (SuperHeroCreatorManager.Instance.customizationType != SuperHeroCreatorManager.CustomizationType.None)
            {
                SelectedSuperHeroData.OnSuperHeroPartSelectedEvent += OnSuperHeroPartSelected;
            }

            this.transform.localScale = new Vector3(1 / FindObjectOfType<Stage>().transform.localScale.x, 1, 1);

            SetEvent();
            Inisialize();
        }

        void OnSuperHeroPartSelected(SuperHeroCreatorStages stage, bool completed)
        {
            if (!completed)
                Inisialize();
        }

        void Inisialize()
        {
            SuperHeroCreatorManager.Instance.SetScene(choosePart);
            selectedSuperHero = SelectedSuperHeroData.Instance.GetSuperHero(wallType);

            SetSuperHero();
            SetPosition();
        }

        void SetPosition()
        {
            this.transform.position = new Vector3(AbstractImmersiveCamera.CurrentImmersiveCamera.cameras[(int)wallType].transform.position.x, 0, 0);
        }

        void SetSuperHero()
        {
            continueButton.gameObject.SetActive(false);
            choosePart.gameObject.SetActive(true);

            panelBodyButtons.SetActive(true);

            if (SelectedSuperHeroData.Instance.currentStage == SuperHeroCreatorStages.Full)
            {
                panelHeadButtons.SetActive(true);
                panelLegButtons.SetActive(true);
            }
            else
            {
                panelHeadButtons.SetActive(false);
                panelLegButtons.SetActive(false);
            }

            switch (SelectedSuperHeroData.Instance.currentStage)
            {
                case SuperHeroCreatorStages.Stage1:
                    headsPanel.SetScroll(superHero.heads, OnScroll);

                    SuperHeroCreatorManager.Instance.ChangeButtonSprite(continueButton, SuperHeroCreatorManager.ButtonState.Continue);

                    animator.SetInteger("Stats",1);
                    break;

                case SuperHeroCreatorStages.Stage2:
                    headsPanel.SetSelectedSprite(selectedSuperHero.head);
                    bodiesPanel.SetScroll(superHero.bodies, OnScroll);

                    SuperHeroCreatorManager.Instance.ChangeButtonSprite(continueButton, SuperHeroCreatorManager.ButtonState.Continue);

                    animator.SetInteger("Stats", 2);
                    break;

                case SuperHeroCreatorStages.Stage3:
                    headsPanel.SetSelectedSprite(selectedSuperHero.head);
                    bodiesPanel.SetSelectedSprite(selectedSuperHero.body);
                    legsPanel.SetScroll(superHero.legs, OnScroll);

                    SuperHeroCreatorManager.Instance.ChangeButtonSprite(continueButton, SuperHeroCreatorManager.ButtonState.Ready);

                    animator.gameObject.SetActive(false);
                    break;

                case SuperHeroCreatorStages.Full:
                    headsPanel.SetScroll(superHero.heads, OnScroll);
                    bodiesPanel.SetScroll(superHero.bodies, OnScroll);
                    legsPanel.SetScroll(superHero.legs, OnScroll);

                    SuperHeroCreatorManager.Instance.ChangeButtonSprite(continueButton, SuperHeroCreatorManager.ButtonState.Ready);

                    animator.gameObject.SetActive(false);
                    break;
            }

            SelectedSuperHeroData.Instance.ResetWallSelected();
        }

        void OnScroll()
        {
            SelectedSuperHeroData.Instance.selectedWalls[wallType] = false;
            continueButton.gameObject.SetActive(true);
        }

        public void ContinueButton()
        {
            panelBodyButtons.SetActive(false);
            panelHeadButtons.SetActive(false);
            panelLegButtons.SetActive(false);

            SuperHeroCreatorManager.Instance.ChangeButtonSprite(continueButton, SuperHeroCreatorManager.ButtonState.Waiting);

            switch (SelectedSuperHeroData.Instance.currentStage)
            {
                case SuperHeroCreatorStages.Stage1:
                    selectedSuperHero.head = headsPanel.GetSelectedPart();
                    break;

                case SuperHeroCreatorStages.Stage2:
                    selectedSuperHero.body = bodiesPanel.GetSelectedPart();
                    break;

                case SuperHeroCreatorStages.Stage3:
                    selectedSuperHero.leg = legsPanel.GetSelectedPart();
                    break;

                case SuperHeroCreatorStages.Full:
                    selectedSuperHero.head = headsPanel.GetSelectedPart();
                    selectedSuperHero.body = bodiesPanel.GetSelectedPart();
                    selectedSuperHero.leg = legsPanel.GetSelectedPart();
                    break;
            }
            
            SelectedSuperHeroData.Instance.SetSuperHeroData(wallType, selectedSuperHero);
        }

        public void SetEvent()
        {
            switch (SelectedSuperHeroData.Instance.currentStage)
            {
                case SuperHeroCreatorStages.Stage1:
                    AddEvent(panelBodyButtons, headsPanel);
                    break;

                case SuperHeroCreatorStages.Stage2:
                    AddEvent(panelBodyButtons, bodiesPanel);
                    break;

                case SuperHeroCreatorStages.Stage3:
                    AddEvent(panelBodyButtons, legsPanel);
                    break;

                case SuperHeroCreatorStages.Full:
                    AddEvent(panelHeadButtons, headsPanel);
                    AddEvent(panelBodyButtons, bodiesPanel);
                    AddEvent(panelLegButtons, legsPanel);
                    break;
            }
        }

        void AddEvent(GameObject buttonPanle, HorizontalScroll scroll)
        {
            buttonPanle.transform.Find("Next Button").GetComponent<ObjectEvent>().OnPressEvent.AddListener(delegate { scroll.MoveNext(); });
            buttonPanle.transform.Find("Previous Button").GetComponent<ObjectEvent>().OnPressEvent.AddListener(delegate { scroll.MovePrevious(); });

            buttonPanle.transform.Find("Next Button").GetComponent<ObjectEvent>().OnPressEvent.AddListener(delegate { OnNextPrevious(); });
            buttonPanle.transform.Find("Previous Button").GetComponent<ObjectEvent>().OnPressEvent.AddListener(delegate { OnNextPrevious(); });
        }

        void OnNextPrevious()
        {
            choosePart.gameObject.SetActive(false);
        }
    }
}