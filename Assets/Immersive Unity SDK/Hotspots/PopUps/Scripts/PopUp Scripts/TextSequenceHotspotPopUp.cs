using Immersive.Utilities;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Com.Immersive.Hotspots.PopUpSequenceSettings ;

namespace Com.Immersive.Hotspots
{
    public class TextSequenceHotspotPopUp : HotspotPopUpSequence<TextSequencePopUpSetting>
    {
        public TextMeshProUGUI textTitleBody, textPageNumber;
        public Image imageBackground;
        Vector2 size;

        protected override void UpdateVisualsForIndex(int newIndex, bool startIndex) => SetText(newIndex, startIndex);

        protected override void SetupPopUpFromSettings(TextSequencePopUpSetting popUpSettings)
        {
            base.SetupPopUpFromSettings(popUpSettings);

            size = popUpSettings.size;

            SetImageProperty(imageBackground, popUpSettings.background, ImageEnum.None); //Set Image property for background

            //Set size  
            SetSize(popUpSettings, popUpSettings.sizeOption);
        }

        /// <summary>
        /// Set the size of text popup based on editor setting
        /// </summary>
        /// <param name="sizeOption">Size option (FixedPopupSize/FixedFontSize)</param>
        /// <param name="popUpsize">Size of the popup</param>
        /// <param name="fontSizeTitle">Font size of Title</param>
        /// <param name="fontSizeBody">Font size of message</param>
        /// <param name="margin">Padding around text</param>
        /// <returns></returns>
        void SetSize(TextSequencePopUpSetting popUpSettings, SizeOption sizeOption)
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
                    SetSize(popUpSettings, SizeOption.FixedPopupSize); // call SetSize to fit text into new size of popup
                    break;
            }

            PositionHotspot();
        }

        private void SetText(int index, bool startIndex)
        {
            textTitleBody.text = "";
            SetPageCountText(index);

            if (popUpSettings.textPopups[index].includeTitle)
            {
                SetTitleText(index);
            }

            SetBodyText(index);            
        }

        /// <summary>
        /// Set Title text with Tags
        /// </summary>
        private void SetTitleText(int index)
        {
            textTitleBody.text = TMProTagExtensions.AddFont(popUpSettings.textPopups[index].title.Text, popUpSettings.textPopups[index].title.Font);
            textTitleBody.text = TMProTagExtensions.AddSize(textTitleBody.text, popUpSettings.textPopups[index].title.FontSize);
            textTitleBody.text = TMProTagExtensions.AddColor(textTitleBody.text, popUpSettings.textPopups[index].title.Color);
            textTitleBody.text = TMProTagExtensions.AddNewLine(textTitleBody.text);

            if (popUpSettings.textPopups[index].space > 0)
                textTitleBody.text = TMProTagExtensions.AddSpace(textTitleBody.text, popUpSettings.textPopups[index].space);

            textTitleBody.text = TMProTagExtensions.SetAlignment(textTitleBody.text, popUpSettings.textPopups[index].title.Alignment);
        }

        /// <summary>
        /// Set Body text with tags
        /// </summary>
        private void SetBodyText(int index)
        {
            string text = TMProTagExtensions.AddFont(popUpSettings.textPopups[index].body.Text, popUpSettings.textPopups[index].body.Font);
            text = TMProTagExtensions.AddColor(text, popUpSettings.textPopups[index].body.Color);
            text = TMProTagExtensions.AddSize(text, popUpSettings.textPopups[index].body.FontSize);
            text = TMProTagExtensions.SetAlignment(text, popUpSettings.textPopups[index].body.Alignment);
            textTitleBody.text += text;
        }

        private void SetPageCountText(int index)
        {
            textPageNumber.text = "";

            if (!popUpSettings.includePageCount)
                return;

            textPageNumber.text = "" + (index + 1) + "/" + popUpSettings.textPopups.Count;

            textPageNumber.text = TMProTagExtensions.AddSize(textPageNumber.text, popUpSettings.pageCountSettings.size);
            textPageNumber.text = TMProTagExtensions.AddColor(textPageNumber.text, popUpSettings.pageCountSettings.color);
            textPageNumber.text = TMProTagExtensions.SetAlignment(textPageNumber.text, popUpSettings.pageCountSettings.alignment);
        }
    }
}