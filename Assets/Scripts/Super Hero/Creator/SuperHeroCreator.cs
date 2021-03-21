using Com.Immersive.Cameras;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public enum WallType { Left, Center, Right }

namespace Immersive.SuperHero
{    
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

        //public Animator animator;
        public SpriteRenderer silhouetteParticle;

        SelectedSuperHero selectedSuperHero;
        SuperHeroCreatorManager superHeroCreatorManager;

        private void OnDestroy()
        {
            SelectedSuperHeroData.OnSuperHeroPartSelectedEvent -= OnSuperHeroPartSelected;
        }

        void Start()
        {
            superHeroCreatorManager = FindObjectOfType<SuperHeroCreatorManager>();

            if (superHeroCreatorManager.customizationType != SuperHeroCreatorManager.CustomizationType.None)
            {
                SelectedSuperHeroData.OnSuperHeroPartSelectedEvent += OnSuperHeroPartSelected;
            }

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
            superHeroCreatorManager.SetScene(choosePart);
            selectedSuperHero = SelectedSuperHeroData.Instance.GetSuperHero(wallType);

            SetSuperHero();
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

                    superHeroCreatorManager.ChangeButtonSprite(continueButton, SuperHeroCreatorManager.ButtonState.Continue);

                    superHeroCreatorManager.ChangeSilhouetteParticleSprite(silhouetteParticle, SuperHeroCreatorStages.Stage1);

                    //animator.SetInteger("Stats",1);
                    break;

                case SuperHeroCreatorStages.Stage2:
                    headsPanel.SetSelectedSprite(selectedSuperHero.head);
                    bodiesPanel.SetScroll(superHero.bodies, OnScroll);

                    superHeroCreatorManager.ChangeButtonSprite(continueButton, SuperHeroCreatorManager.ButtonState.Continue);
                    superHeroCreatorManager.ChangeSilhouetteParticleSprite(silhouetteParticle, SuperHeroCreatorStages.Stage2);

                    //animator.SetInteger("Stats", 2);
                    break;

                case SuperHeroCreatorStages.Stage3:
                    headsPanel.SetSelectedSprite(selectedSuperHero.head);
                    bodiesPanel.SetSelectedSprite(selectedSuperHero.body);
                    legsPanel.SetScroll(superHero.legs, OnScroll);

                    superHeroCreatorManager.ChangeButtonSprite(continueButton, SuperHeroCreatorManager.ButtonState.Ready);

                    silhouetteParticle.gameObject.SetActive(false);
                    break;

                case SuperHeroCreatorStages.Full:
                    headsPanel.SetScroll(superHero.heads, OnScroll);
                    bodiesPanel.SetScroll(superHero.bodies, OnScroll);
                    legsPanel.SetScroll(superHero.legs, OnScroll);

                    superHeroCreatorManager.ChangeButtonSprite(continueButton, SuperHeroCreatorManager.ButtonState.Ready);

                    silhouetteParticle.gameObject.SetActive(false);
                    break;
            }

            SelectedSuperHeroData.Instance.ResetWallSelected();
        }

        void OnScroll()
        {
            superHeroCreatorManager.PlaySwitch();
            SelectedSuperHeroData.Instance.selectedWalls[wallType] = false;
            continueButton.gameObject.SetActive(true);
        }

        public void ContinueButton()
        {
            panelBodyButtons.SetActive(false);
            panelHeadButtons.SetActive(false);
            panelLegButtons.SetActive(false);

            superHeroCreatorManager.ChangeButtonSprite(continueButton, SuperHeroCreatorManager.ButtonState.Waiting);

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

            AbstractImmersiveCamera.PlayAudio(superHeroCreatorManager.selectClip);
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