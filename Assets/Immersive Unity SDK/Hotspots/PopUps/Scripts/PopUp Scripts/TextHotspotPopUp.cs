/* Copyright (C) Immersive Interactive, Ltd - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Luke Bissell <luke@immersive.co.uk>, July 2019
 */

using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using static Com.Immersive.Hotspots.TextPopUpDataModel;
using Immersive.Utilities;

namespace Com.Immersive.Hotspots
{
    public class TextHotspotPopUp : HotspotPopUp<TextPopUpSetting>
    {
        [SerializeField] TextMeshProUGUI textTitleBody = null;
        [SerializeField] Image imageBackground = null;
        Vector2 size;

        protected override void SetupPopUpFromSettings(TextPopUpSetting popUpSettings)
        {
            size = popUpSettings.size;

            textTitleBody.text = "";
            if (popUpSettings.includeTitle)
            {
                SetTitleText();
            }

            SetBodyText();

            textTitleBody.isRightToLeftText = popUpSettings.isRightToLeftText;

            //Set size
            SetSize(popUpSettings, popUpSettings.sizeOption);
            SetImageProperty(imageBackground, popUpSettings.background, ImageEnum.None); //Set Image property for background
        }

        /// <summary>
        /// Set Title text with Tags
        /// </summary>
        void SetTitleText()
        {
            string titleText = popUpSettings.title.GenerateTMPStyledText();
            titleText = TMProTagExtensions.AddNewLine(titleText);
            if (popUpSettings.space > 0)
                titleText = TMProTagExtensions.AddSpace(titleText, popUpSettings.space);

            textTitleBody.text = titleText;
        }

        /// <summary>
        /// Set Body text with tags
        /// </summary>
        void SetBodyText()
        {
            textTitleBody.text += popUpSettings.body.GenerateTMPStyledText();
        }

        /// <summary>
        /// Set the size of text popup based on editor setting
        /// </summary>
        /// <param name="popUpSettings">popup settings defined into editor</param>
        /// <param name="sizeOption">Size option (FixedPopupSize/FixedFontSize)</param>
        /// <returns></returns>
        void SetSize(TextPopUpSetting popUpSettings, SizeOption sizeOption)
        {
            contentRect.GetComponent<ContentSizeFitter>().enabled = false;
            contentRect.GetComponent<VerticalLayoutGroup>().padding = popUpSettings.padding;

            switch (sizeOption)
            {
                case SizeOption.FixedPopupSize: //font size will be reset according to the popup size
                    SetContentSize(size);
                    break;

                case SizeOption.FixedPercentage: //popup size will be based on Percentage
                    var rect = transform.GetComponentInParent<Canvas>().GetComponent<RectTransform>().sizeDelta;

                    float height = rect.y * ((float)popUpSettings.percentage / 100.0f);
                    float width = (rect.x / rect.y) * height - 100;//100 referes control panel width

                    size = new Vector2(width, height);

                    SetSize(popUpSettings, SizeOption.FixedPopupSize);// call SetSize to fit text into new size of popup
                    break;
            }

            PositionHotspot();
        }

    }
}
