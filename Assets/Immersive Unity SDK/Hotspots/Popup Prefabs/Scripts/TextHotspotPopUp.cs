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

namespace Com.Immersive.Hotspots
{
    public class TextHotspotPopUp : HotspotPopUp
    {
        public TextMeshProUGUI txtBody;
        public TextMeshProUGUI txtTitle;
        public Image imageBackground, imageCloseButton;

        private void Start()
        {
            //This ensures it is off screen until it the size is calculated and it can be properly placed.
            rectTransform.anchoredPosition = new Vector2(0, -10000);
        }

        Vector2 size;
        public void Init(TextPopUpDataModel popupDataModel)
        {
            size = popupDataModel.popUpSetting.size;

            //Set size  
            StartCoroutine(SetSize(popupDataModel, popupDataModel.popUpSetting.sizeOption));

            SetTextProperty(txtTitle, popupDataModel.popUpSetting.title);//Set text property for Title
            SetTextProperty(txtBody, popupDataModel.popUpSetting.body); //Set text property for Message

            SetCloseButtonImage(imageCloseButton, popupDataModel.popUpSetting); //Set Close Button
            SetImageProperty(imageBackground, popupDataModel.popUpSetting.background, ImageEnum.None); //Set Image property for background

        }

        //
        /// <summary>
        /// Set the size of text popup based on editor setting
        /// </summary>
        /// <param name="sizeOption">Size option (FixedPopupSize/FixedFontSize)</param>
        /// <param name="popUpsize">Size of the popup</param>
        /// <param name="fontSizeTitle">Font size of Title</param>
        /// <param name="fontSizeBody">Font size of message</param>
        /// <param name="margin">Padding around text</param>
        /// <returns></returns>
        IEnumerator SetSize(TextPopUpDataModel textPopUpDataModel, SizeOption sizeOption)
        {
            contentRect.GetComponent<ContentSizeFitter>().enabled = false;
            //int margin = textPopUpDataModel.popUpSetting.margin;

            contentRect.GetComponent<VerticalLayoutGroup>().padding = textPopUpDataModel.popUpSetting.padding;//new RectOffset(margin, margin, margin, margin);

            switch (sizeOption)
            {
                case SizeOption.FixedPopupSize: //font size will be reset according to the popup size

                    SetContentSize(size);

                    txtTitle.fontSize = textPopUpDataModel.popUpSetting.title.size;
                    yield return null;
                    txtTitle.GetComponent<LayoutElement>().minHeight = txtTitle.textBounds.size.y;

                    txtBody.enableAutoSizing = true;

                    break;

                case SizeOption.FixedContentSize: //font size will be fixed but popup size will be changed accordingly

                    contentRect.GetComponent<ContentSizeFitter>().enabled = true;

                    yield return new WaitForEndOfFrame();
                    SetContentSize(contentRect.GetComponent<RectTransform>().sizeDelta);

                    break;

                case SizeOption.FixedPercentage: //popup size will be based on Percentage                   
                    var rect = transform.GetComponentInParent<Canvas>().GetComponent<RectTransform>().sizeDelta;

                    float height = rect.y * ((float)textPopUpDataModel.popUpSetting.percentage / 100.0f);
                    float width = (rect.x / rect.y) * height - 100;//100 referes control panel width

                    size = new Vector2(width, height);

                    StartCoroutine(SetSize(textPopUpDataModel, SizeOption.FixedPopupSize)); // call SetSize to fit text into new size of popup
                    break;
            }

            yield return new WaitForEndOfFrame();
            PlacePopUp(rectTransform);
        }
    }
}