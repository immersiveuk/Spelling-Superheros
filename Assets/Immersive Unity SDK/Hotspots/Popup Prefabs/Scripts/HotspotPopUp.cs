/* Copyright (C) Immersive Interactive, Ltd - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Luke Bissell <luke@immersive.co.uk>, July 2019
 */

using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Com.Immersive.Hotspots
{
    public class HotspotPopUp : MonoBehaviour
    {
        protected GameObject _spawningHotspot;
        public GameObject SpawningHotspot
        {
            set
            {
                _spawningHotspot = value;
            }
        }

        //CONTROL PANEL POSITIONING
        private enum ControlPanelSide { Left, Right };
        private ControlPanelSide controlPanelSide = ControlPanelSide.Left;
        private float controlPanelYOffset = 0;
        private float controlPanelXOffest => controlPanelSide == ControlPanelSide.Left ? 0 : 1;
        private float ControlPanelWidth => controlPanelRect.sizeDelta.x;

        //Rect Transforms
        public RectTransform contentRect;
        public RectTransform controlPanelRect;

        protected RectTransform rectTransform;

        public Action ActionComplete;
        public Action<RectTransform> PlacePopUp;

        void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
        }


        //-------PUBLIC METHODS--------

        /// <summary>
        /// Sets the size of the whole pop up based on a given main content size.
        /// </summary>
        /// <param name="contentSize">The size of the main content as a Vector2.</param>
        public void SetContentSize(Vector2 contentSize)
        {
            //Set Total Hotspot Size
            rectTransform.sizeDelta = new Vector2(contentSize.x + ControlPanelWidth, contentSize.y);

            //Set Content Size
            contentRect.sizeDelta = contentSize;

            //Set the position of the close button
            if (controlPanelSide == ControlPanelSide.Right) DisplayCloseButtonRight();
            else DisplayCloseButtonLeft();
        }

        /// <summary>
        /// set image property from serializable class
        /// </summary>
        /// <param name="image"></param>
        /// <param name="imageProperty"></param>
        public void SetImageProperty(Image image, ImageProperty imageProperty, ImageEnum imageEnum)
        {
            if (imageProperty != null && imageProperty.sprite != null)
            {
                image.sprite = imageProperty.sprite;
                image.color = imageProperty.color;
                image.type = imageProperty.type;
            }
            else if(imageEnum == ImageEnum.CloseButton && _spawningHotspot.GetComponentInParent<HotspotController>().closeButton.sprite != null)
            {
                ImageProperty closeButton = _spawningHotspot.GetComponentInParent<HotspotController>().closeButton;
                image.sprite = closeButton.sprite;
                image.color = closeButton.color;
                image.type = closeButton.type;
            }
            else if(imageEnum != ImageEnum.None)
            {
                image.sprite = DefaultAssets._instance.GetImage(imageEnum);
                image.color = Color.white;
                image.type = Image.Type.Simple;
            }
            else
            {
                image.color = imageProperty.color;
            }
        }

        public void SetCloseButtonImage(Image closeButton, PopUpSetting popupSettings)
        {
            if (popupSettings.overrideDefaultCloseButton) SetImageProperty(closeButton, popupSettings.closeButton, ImageEnum.CloseButton);
            else SetImageProperty(closeButton, null, ImageEnum.CloseButton);
        }

        public void SetTextProperty(TextMeshProUGUI text, TextProperty textProperty)
        {
            text.text = textProperty.text.Equals("") ? "Default Text" : textProperty.text;
            text.fontSize = textProperty.size == 0 ? 50 : textProperty.size;
            text.color = textProperty.color;
            text.font = textProperty.font == null ? DefaultAssets._instance.GetFont(textProperty.fontName) : textProperty.font;            
        }

        public void ClosePopUp()
        {
            //_spawningHotspot.ActionComplete();
            ActionComplete();
            Destroy(gameObject);
        }

        //-------POSITION CONTROL PANEL--------

        /// <summary>
        /// Place the close button to the left of the content.
        /// </summary>
        public void DisplayCloseButtonLeft()
        {
            //Position Content
            //SetContentAnchors(new Vector2(1 - (contentRect.sizeDelta.x / rectTransform.sizeDelta.x) * 0.5f, 0.5f));
            SetContentAnchors(ControlPanelSide.Left);

            //Position Control Panel to Left of Content
            controlPanelSide = ControlPanelSide.Left;
            PositionControlPanel();
        }

        /// <summary>
        /// Place the close button to the right of the content.
        /// </summary>
        public void DisplayCloseButtonRight()
        {
            //Position Content
            //SetContentAnchors(new Vector2((contentRect.sizeDelta.x / rectTransform.sizeDelta.x) * 0.5f, 0.5f));
            SetContentAnchors(ControlPanelSide.Right);

            //Position Control Panel to Right of Content
            controlPanelSide = ControlPanelSide.Right;
            PositionControlPanel();

        }

        public void SetControlPanelYOffset(float yOffsetPixels)
        {
            //Convert yOffset to value between 1 and 0
            var yOffset = yOffsetPixels / (rectTransform.sizeDelta.y);

            //This is the maximum offset so that the control panel doesn't go above or below 
            var maxOffset = ((rectTransform.sizeDelta.y - controlPanelRect.sizeDelta.y) / rectTransform.sizeDelta.y) / 2;

            // Make sure yOffset doesn't exceed maximum
            if (yOffset < -maxOffset) yOffset = -maxOffset;
            if (yOffset > maxOffset) yOffset = maxOffset;

            //Set position of control panel
            controlPanelYOffset = yOffset;
            PositionControlPanel();
        }

        //-------PRIVATE METHODS--------
        private void SetContentAnchors(Vector2 position)
        {
            contentRect.anchorMin = position;
            contentRect.anchorMax = position;
        }

        //To resize content automatically on scale change in discovery editor
        private void SetContentAnchors(ControlPanelSide side)
        {
            contentRect.anchorMin = new Vector2(0, 0);
            contentRect.anchorMax = new Vector2(1, 1);

            if (side == ControlPanelSide.Left)
            {
                contentRect.offsetMin = new Vector2(100, 0);
                contentRect.offsetMax = new Vector2(0, 0);
            }
            else
            {
                contentRect.offsetMin = new Vector2(0, 0);
                contentRect.offsetMax = new Vector2(-100, 0);
            }
        }

        protected void PositionControlPanel()
        {
            var position = new Vector2(controlPanelXOffest, 0.5f + controlPanelYOffset);
            controlPanelRect.anchorMin = position;
            controlPanelRect.anchorMax = position;
            controlPanelRect.pivot = new Vector2(position.x, 0.5f);
        }
    }
}