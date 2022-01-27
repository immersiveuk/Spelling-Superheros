/* Copyright (C) Immersive Interactive, Ltd - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Luke Bissell <luke@immersive.co.uk>, July 2019
 */

using UnityEngine;
using UnityEngine.UI;
using static Com.Immersive.Hotspots.ImagePopUpDataModel;

namespace Com.Immersive.Hotspots
{
    public class ImageHotspotPopUp : HotspotPopUp<ImagePopUpSetting>
    {
        [SerializeField] Image imageMain = null, imageBorder = null, mediaMask = null;

        protected override void SetupPopUpFromSettings(ImagePopUpSetting popUpSettings)
        {
            //Set image property for background
            SetImageProperty(imageMain, popUpSettings.background, ImageEnum.None);

            //Set image property for Border
            SetImageProperty(imageBorder, popUpSettings.border, ImageEnum.None, true);
            if (!popUpSettings.border.HasSprite)
                imageBorder.enabled = false;

            //Media Mask
            SetImageProperty(mediaMask, popUpSettings.mediaMask, ImageEnum.None);

            SetSize(popUpSettings);
        }

   

        void SetSize(PopUpSettings popUpSettings)
        {
            //Set size
            Vector2 size = new Vector2();

            switch (popUpSettings.sizeOption)
            {
                case SizeOption.FixedPopupSize:
                    size = popUpSettings.size;
                    break;

                case SizeOption.FixedPercentage:
                    size = GetSizeFromPercentage(popUpSettings.percentage);
                    break;
            }

            contentRect.GetComponent<VerticalLayoutGroup>().padding = popUpSettings.padding;

            Vector2 sizePlusPadding = size;
            sizePlusPadding.x += popUpSettings.padding.horizontal;
            sizePlusPadding.y += popUpSettings.padding.vertical;

            SetContentSizeAndPositionHotspot(sizePlusPadding);
        }

        protected override float DefaultAspectRatio => popUpSettings.background.sprite.rect.width / popUpSettings.background.sprite.rect.height;
    }
}