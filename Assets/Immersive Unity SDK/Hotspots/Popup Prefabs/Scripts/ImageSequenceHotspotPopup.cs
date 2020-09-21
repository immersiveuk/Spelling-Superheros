/* Copyright (C) Immersive Interactive, Ltd - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Luke Bissell <luke@immersive.co.uk>, July 2019
 */

using UnityEngine;
using UnityEngine.UI;
using static Com.Immersive.Hotspots.ImageSequencePopUpDataModel;

namespace Com.Immersive.Hotspots
{
    public class ImageSequenceHotspotPopup : HotspotPopUp
    {
        public Image imageMain, imageBorder;

        public Image imageCloseButton, imageNextButton, imagePreviousButton;
        private ImageSequencePopUpDataModel imageSequencePopUpDataModel;

        private int index = 0;

        private IImageSequenceIndexChangeHandler[] indexChangeHandlers;

        private void Start()
        {
            indexChangeHandlers = _spawningHotspot.GetComponents<IImageSequenceIndexChangeHandler>();
            //Index Change Handlers
            foreach (var handler in indexChangeHandlers)
            {
                handler.IndexChanged(index);
            }

            PositionControlPanel();
            EnableAndDisableButtons();
        }

        public void Init(ImageSequencePopUpDataModel popupDataModel)
        {
            this.imageSequencePopUpDataModel = popupDataModel;

            SetCloseButtonImage(imageCloseButton, popupDataModel.popUpSetting); //Set Close Button

            SetImageProperty(imageMain, popupDataModel.popUpSetting.backgroundSprites[0], ImageEnum.None);
            //Set image property for Border
            SetImageProperty(imageBorder, popupDataModel.popUpSetting.border, ImageEnum.None);

            if (popupDataModel.popUpSetting.customButtons)
            {
                SetImageProperty(imageNextButton, popupDataModel.popUpSetting.nextButton, ImageEnum.NextButton);
                SetImageProperty(imagePreviousButton, popupDataModel.popUpSetting.previousButton, ImageEnum.PreviousButton);                    
            }

            imageCloseButton.gameObject.SetActive(popupDataModel.popUpSetting.controlPanelStyle == ControlPanelStyle.Full || popupDataModel.popUpSetting.controlPanelStyle == ControlPanelStyle.ForwardAndClose);
            imagePreviousButton.gameObject.SetActive(popupDataModel.popUpSetting.controlPanelStyle == ControlPanelStyle.Full || popupDataModel.popUpSetting.controlPanelStyle == ControlPanelStyle.ForwardAndBack);

            SetSize(popupDataModel);
        }

        void SetSize(ImageSequencePopUpDataModel imageSequencePopUpDataModel)
        {
            var size = imageSequencePopUpDataModel.popUpSetting.size;

            switch (imageSequencePopUpDataModel.popUpSetting.sizeOption)
            {
                case SizeOption.FixedContentSize:
                    size = imageSequencePopUpDataModel.popUpSetting.backgroundSprites[0].sprite.rect.size;
                    break;

                case SizeOption.FixedPercentage:
                    var rect = transform.GetComponentInParent<Canvas>().GetComponent<RectTransform>().sizeDelta;

                    float height = rect.y * ((float)imageSequencePopUpDataModel.popUpSetting.percentage / 100.0f);
                    float width = (rect.x / rect.y) * height - 100;//100 referes control panel width

                    size = new Vector2(width, height);

                    break;
            }

            contentRect.GetComponent<VerticalLayoutGroup>().padding = imageSequencePopUpDataModel.popUpSetting.padding;
            SetContentSize(size);

            PlacePopUp(rectTransform);
        }

        private void Update()
        {
            if (changeImageCooldownTimeRemaining > 0)
            {
                changeImageCooldownTimeRemaining -= Time.deltaTime;
            }
        }

        public new void ClosePopUp()
        {
            if (changeImageCooldownTimeRemaining > 0) return;
            changeImageCooldownTimeRemaining = changeImageCooldownDuration;

            //_spawningHotspot.ActionComplete();
            ActionComplete();
            Destroy(gameObject);
        }

        private float changeImageCooldownDuration = 0.5f;
        private float changeImageCooldownTimeRemaining;

        public void NextImage()
        {
            //Button Cooldown
            if (changeImageCooldownTimeRemaining > 0) return;
            changeImageCooldownTimeRemaining = changeImageCooldownDuration;

            index++;            
            SetImageProperty(imageMain, imageSequencePopUpDataModel.popUpSetting.backgroundSprites[index], ImageEnum.None);
            EnableAndDisableButtons();

            //Display close button when at end of images in forward only style
            if ((imageSequencePopUpDataModel.popUpSetting.controlPanelStyle == ControlPanelStyle.ForwardOnly || imageSequencePopUpDataModel.popUpSetting.controlPanelStyle == ControlPanelStyle.ForwardAndBack) &&
                index == imageSequencePopUpDataModel.popUpSetting.backgroundSprites.Count - 1)
            {
                imageNextButton.gameObject.SetActive(false);
                imageCloseButton.gameObject.SetActive(true);
            }

            //Index Change Handlers
            foreach (var handler in indexChangeHandlers)
            {
                handler.IndexChanged(index);
            }
        }

        public void PreviousImage()
        {
            //Button Cooldown
            if (changeImageCooldownTimeRemaining > 0) return;
            changeImageCooldownTimeRemaining = changeImageCooldownDuration;

            index--;            
            SetImageProperty(imageMain, imageSequencePopUpDataModel.popUpSetting.backgroundSprites[index], ImageEnum.None);
            EnableAndDisableButtons();

            //Display close button when at end of images in forward only style
            if ((imageSequencePopUpDataModel.popUpSetting.controlPanelStyle == ControlPanelStyle.ForwardOnly || imageSequencePopUpDataModel.popUpSetting.controlPanelStyle == ControlPanelStyle.ForwardAndBack) &&
                index == imageSequencePopUpDataModel.popUpSetting.backgroundSprites.Count - 2)
            {
                imageNextButton.gameObject.SetActive(true);
                imageCloseButton.gameObject.SetActive(false);
            }

            //Index Change Handlers
            foreach (var handler in indexChangeHandlers)
            {
                handler.IndexChanged(index);
            }
        }

        private void EnableAndDisableButtons()
        {
            var nextButtonActive = index < imageSequencePopUpDataModel.popUpSetting.backgroundSprites.Count - 1;
            var prevButtonActive = index > 0;

            imageNextButton.raycastTarget = nextButtonActive;
            ChangeImageOpacity(imageNextButton, nextButtonActive ? 1 : 0.5f);

            if (imageSequencePopUpDataModel.popUpSetting.controlPanelStyle == ControlPanelStyle.Full || imageSequencePopUpDataModel.popUpSetting.controlPanelStyle == ControlPanelStyle.ForwardAndBack)
            {
                imagePreviousButton.raycastTarget = prevButtonActive;
                ChangeImageOpacity(imagePreviousButton, prevButtonActive ? 1 : 0.5f);
            }
        }

        private void ChangeImageOpacity(Image image, float alpha)
        {
            var tmpColor = image.color;
            tmpColor.a = alpha;
            image.color = tmpColor;
        }
    }
}