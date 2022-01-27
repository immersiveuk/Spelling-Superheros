/* Copyright (C) Immersive Interactive, Ltd - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Luke Bissell <luke@immersive.co.uk>, July 2019
 */

using UnityEngine;
using UnityEngine.UI;
using static Com.Immersive.Hotspots.ImageSequencePopUpDataModel;
using static Com.Immersive.Hotspots.PopUpSequenceSettings ;

namespace Com.Immersive.Hotspots
{
    public class ImageSequenceHotspotPopup : HotspotPopUpSequence<ImageSequencePopUpSetting>
    {
        [SerializeField] Image imageMain = null, imageBorder = null, mediaMask = null;

        protected override void SetupPopUpFromSettings(ImageSequencePopUpSetting popUpSettings)
        {
            base.SetupPopUpFromSettings(popUpSettings);

            //Border
            SetImageProperty(imageBorder, popUpSettings.border, ImageEnum.None);
            imageBorder.enabled = popUpSettings.border.HasSprite;

            //Media Mask
            SetImageProperty(mediaMask, popUpSettings.mediaMask, ImageEnum.None);

            SetSize(popUpSettings);
        }

        protected override float DefaultAspectRatio => popUpSettings.backgroundSprites[0].sprite.rect.width / popUpSettings.backgroundSprites[0].sprite.rect.height;

        void SetSize(PopUpSettings popUpSettings)
        {
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

        protected override void UpdateVisualsForIndex(int newIndex, bool startIndex)
        {
            SetImageProperty(imageMain, popUpSettings.backgroundSprites[newIndex], ImageEnum.None);
        }
    }
}