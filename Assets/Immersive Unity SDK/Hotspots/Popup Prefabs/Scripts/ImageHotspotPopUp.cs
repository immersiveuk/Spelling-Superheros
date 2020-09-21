/* Copyright (C) Immersive Interactive, Ltd - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Luke Bissell <luke@immersive.co.uk>, July 2019
 */

using Com.Immersive.Cameras;
using UnityEngine;
using UnityEngine.UI;

namespace Com.Immersive.Hotspots
{
    public class ImageHotspotPopUp : HotspotPopUp
    {
        public Image imageMain,imageBorder;
        public Image imageCloseButton;

        public void Init(ImagePopUpDataModel popupDataModel)
        {
            //Set image property for CloseButton
            SetCloseButtonImage(imageCloseButton, popupDataModel.popUpSetting); //Set Close Button

            //Set image property for background
            SetImageProperty(imageMain, popupDataModel.popUpSetting.background, ImageEnum.None);

            //Set image property for Border
            SetImageProperty(imageBorder, popupDataModel.popUpSetting.border, ImageEnum.None);

            SetSize(popupDataModel); 
        }

        void SetSize(ImagePopUpDataModel imagePopUpDataModel)
        {
            //Set size
            var size = imagePopUpDataModel.popUpSetting.size;

            switch (imagePopUpDataModel.popUpSetting.sizeOption)
            {
                case SizeOption.FixedContentSize:
                    size = imagePopUpDataModel.popUpSetting.background.sprite.rect.size;
                    break;

                case SizeOption.FixedPercentage:
                    var rect = transform.GetComponentInParent<Canvas>().GetComponent<RectTransform>().sizeDelta;

                    float height = rect.y * ((float)imagePopUpDataModel.popUpSetting.percentage / 100.0f);
                    float width = (rect.x / rect.y) * height - 100;//100 referes control panel width

                    size = new Vector2(width, height);

                    break;
            }

            contentRect.GetComponent<VerticalLayoutGroup>().padding = imagePopUpDataModel.popUpSetting.padding;
            SetContentSize(size);

            PlacePopUp(rectTransform);
        }
    }
}