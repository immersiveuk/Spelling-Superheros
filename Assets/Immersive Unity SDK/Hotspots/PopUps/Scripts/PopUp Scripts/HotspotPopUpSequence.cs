using Com.Immersive.Cameras;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Com.Immersive.Hotspots.PopUpSequenceSettings ;

namespace Com.Immersive.Hotspots
{
    public abstract class HotspotPopUpSequence<T> : HotspotPopUp<T> where T : PopUpSequenceSettings 
    {
        [SerializeField] public Image imagePreviousButton = null;
        [SerializeField] public ButtonWithGlow nextButton = null;

        private const float changeImageCooldownDuration = 0.5f;
        private float changeImageCooldownTimeRemaining;
        private int index = 0;

        private ISequencePopUpIndexChangeHandler[] indexChangeHandlers;

        #region Public Methods
        public void NextButton() => ChangeIndex(() => index++);
        public void PreviousButton() => ChangeIndex(() => index--);
        #endregion

        protected abstract void UpdateVisualsForIndex(int newIndex, bool startIndex);

        protected override void SetupPopUpFromSettings(T popUpSettings)
        {
            UpdateVisualsForIndex(0, true);

            EnableAndDisableButtons();

            //GLOW
            nextButton.ToggleGlow(popUpSettings.ShouldGlowToNextButton);
            nextButton.SetGlowColour(popUpSettings.glowColor);

            closeButton.gameObject.SetActive(popUpSettings.controlPanelStyle == ControlPanelStyle.Full || popUpSettings.controlPanelStyle == ControlPanelStyle.ForwardAndClose);
            imagePreviousButton.gameObject.SetActive(popUpSettings.controlPanelStyle == ControlPanelStyle.Full || popUpSettings.controlPanelStyle == ControlPanelStyle.ForwardAndBack);

            if (popUpSettings.useCustomButtons)
            {
                SetImageProperty(nextButton.ButtonImage, popUpSettings.nextButton, ImageEnum.NextButton);
                SetImageProperty(imagePreviousButton, popUpSettings.previousButton, ImageEnum.PreviousButton);
            }
        }
        
        private void ChangeIndex(System.Action changeIndex)
        {
            //Button Cooldown
            if (changeImageCooldownTimeRemaining > 0)
                return;

            changeImageCooldownTimeRemaining = changeImageCooldownDuration;

            changeIndex?.Invoke();

            //NextButtonEventHandlers();
            UpdateVisualsForIndex(index, false);
            EnableAndDisableButtons();

            AbstractImmersiveCamera.PlayAudio(popUpSettings.indexChangedAudioClip.audioClip, popUpSettings.indexChangedAudioClip.audioVolume);

            UpdateIndexChangedHandlers();
        }


        private void UpdateIndexChangedHandlers()
        {
            foreach (var handler in indexChangeHandlers)
                handler.IndexChanged(index);
        }

        protected override void RetrievePopUpEventHandlers()
        {
            indexChangeHandlers = GetPopUpEventHandlers<ISequencePopUpIndexChangeHandler>();

            foreach (var handler in indexChangeHandlers)
            {
                handler.IndexChanged(index);
            }
        }

        private void EnableAndDisableButtons()
        {
            var nextButtonActive = index < popUpSettings.Count - 1;
            var prevButtonActive = index > 0;

            nextButton.ButtonImage.raycastTarget = nextButtonActive;
            ChangeImageOpacity(nextButton.ButtonImage, nextButtonActive ? 1 : 0.5f);

            if (popUpSettings.controlPanelStyle == ControlPanelStyle.Full || popUpSettings.controlPanelStyle == ControlPanelStyle.ForwardAndBack)
            {
                imagePreviousButton.raycastTarget = prevButtonActive;
                ChangeImageOpacity(imagePreviousButton, prevButtonActive ? 1 : 0.5f);
            }

            //Display close button when at end of images in forward only style
            if ((popUpSettings.controlPanelStyle == ControlPanelStyle.ForwardOnly || popUpSettings.controlPanelStyle == ControlPanelStyle.ForwardAndBack) &&
                index == popUpSettings.Count - 1)
            {
                nextButton.gameObject.SetActive(false);
                closeButton.gameObject.SetActive(true);
            }
        }

        private void ChangeImageOpacity(Image image, float alpha)
        {
            var tmpColor = image.color;
            tmpColor.a = alpha;
            image.color = tmpColor;
        }

        protected virtual void Update()
        {
            if (changeImageCooldownTimeRemaining > 0)
            {
                changeImageCooldownTimeRemaining -= Time.deltaTime;
            }
        }

        protected new void ClosePopUp()
        {
            if (changeImageCooldownTimeRemaining > 0)
                return;

            changeImageCooldownTimeRemaining = changeImageCooldownDuration;

            base.ClosePopUp();
        }
    }
}