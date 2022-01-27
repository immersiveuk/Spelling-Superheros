/* Copyright (C) Immersive Interactive, Ltd - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Luke Bissell <luke@immersive.co.uk>, July 2019
 */

using Immersive.Properties;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

namespace Com.Immersive.Hotspots
{
    public abstract class HotspotPopUp: MonoBehaviour
    {
        public delegate Component[] PopupEventHandlerRetrieverDelegate(Type type);
    }

    [RequireComponent(typeof(RectTransform))]
    public abstract class HotspotPopUp<TPopUpSettings> : HotspotPopUp where TPopUpSettings : PopUpSettings
    {

        [SerializeField] protected ButtonWithGlow closeButton = null;

        protected PopUpPositioner popUpPositioner;
        protected TPopUpSettings popUpSettings;

        //Rect Transforms
        public RectTransform contentRect;
        public RectTransform controlPanelRect;

        protected RectTransform rectTransform;

        private Action onCloseAction;

        void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
            transform.localScale = Vector3.zero;
        }

        public void Initialize(TPopUpSettings popUpSettings, PopUpPositioner popUpPositioner, Action onCompleteAction, PopupEventHandlerRetrieverDelegate popUpEventHandlerRetriever)
        {
            this.onCloseAction = onCompleteAction;
            this.popUpSettings = popUpSettings;
            this.popUpPositioner = popUpPositioner;
            this.popUpEventHandlerRetriever = popUpEventHandlerRetriever;

            if (closeButton != null)
                SetupCloseButton();
            SetupPopUpFromSettings(popUpSettings);

            RetrievePopUpEventHandlers();
        }

        protected virtual void RetrievePopUpEventHandlers() { }

        protected abstract void SetupPopUpFromSettings(TPopUpSettings popUpSettings);

        /// <summary>
        /// Should be Width/height;
        /// </summary>
        protected virtual float DefaultAspectRatio { get; } = 1;

        private PopupEventHandlerRetrieverDelegate popUpEventHandlerRetriever;

        protected T[] GetPopUpEventHandlers<T>() where T: IPopUpEventHandler
        {
            if (popUpEventHandlerRetriever != null)
            {
                Component[] components = popUpEventHandlerRetriever(typeof(T));
                if (components.Length > 0)
                    return components[0].GetComponents<T>();
                else
                    return new T[0];
            }
            return new T[0];
        }

        public virtual void ClosePopUp()
        {
            onCloseAction();
            Destroy(gameObject);
        }

        protected void SetupCloseButton()
        {
            SetCloseButtonImage(closeButton.ButtonImage, popUpSettings);
            closeButton.ToggleGlow(popUpSettings.addGlowToButtons);
            closeButton.SetGlowColour(popUpSettings.glowColor);
        }


        #region PopUp Positioning

        private PopUpPositioner.ControlPanelSide GetControlPanelSide()
        {
            if (popUpSettings.controlPanelSide == ControlPanelPositionOption.Default)
                return popUpPositioner.DefaultControlPanelSide;
            else
            {
                return popUpSettings.controlPanelSide == ControlPanelPositionOption.Left ? PopUpPositioner.ControlPanelSide.Left : PopUpPositioner.ControlPanelSide.Right;
            }
        }

        protected void PositionHotspot()
        {
            if (popUpPositioner != null)
            {
                popUpPositioner.PositionPopUp(rectTransform);
                PositionControlPanel(GetControlPanelSide(), popUpPositioner.GetYOffsetFromCentre(rectTransform.sizeDelta));
                transform.localScale = Vector3.one;
            }
            else
                Debug.LogError("PopUp Has no PopUpPositioner");
        }

        private void PositionControlPanel(PopUpPositioner.ControlPanelSide side, float yOffsetPixels)
        {
            SetButtonSize();

            //Convert yOffset to value between 1 and 0
            var yOffset = yOffsetPixels / (rectTransform.sizeDelta.y);
            //This is the maximum offset so that the control panel doesn't go above or below 

            var maxOffset = ((rectTransform.sizeDelta.y - controlPanelRect.sizeDelta.y) / rectTransform.sizeDelta.y) / 2;
            yOffset = Mathf.Clamp(yOffset, -maxOffset, maxOffset);

            var position = new Vector2(GetControlPanelXOffset(side), 0.5f + yOffset);
            controlPanelRect.anchorMin = position;
            controlPanelRect.anchorMax = position;
            controlPanelRect.pivot = new Vector2(1 - position.x, 0.5f);
        }

        private void SetButtonSize()
        {
            foreach (var button in controlPanelRect.GetComponentsInChildren<LayoutElement>(true))
            {
                button.minWidth = popUpSettings.controlPanelWidth;
                button.minHeight = popUpSettings.controlPanelWidth;
            }
            LayoutRebuilder.ForceRebuildLayoutImmediate(controlPanelRect);
        }
        private float GetControlPanelXOffset(PopUpPositioner.ControlPanelSide controlPanelSide) => controlPanelSide == PopUpPositioner.ControlPanelSide.Left ? 0 : 1;
        #endregion


        //-------PUBLIC METHODS--------

        #region PopUp Size
        public void SetContentSizeAndPositionHotspot(Vector2 contentSize)
        {
            SetContentSize(contentSize);
            PositionHotspot();
        }

        /// <summary>
        /// Sets the size of the whole pop up based on a given main content size.
        /// </summary>
        /// <param name="contentSize">The size of the main content as a Vector2.</param>
        public void SetContentSize(Vector2 contentSize)
        {
            rectTransform.sizeDelta = new Vector2(contentSize.x, contentSize.y);

            //Set Content Size
            contentRect.anchorMin = new Vector2(0, 0);
            contentRect.anchorMax = new Vector2(1, 1);

            contentRect.offsetMin = new Vector2(0, 0);
            contentRect.offsetMax = new Vector2(0, 0);
        }
        protected Vector2 GetSizeFromPercentage(float percentage)
        {
            Vector2 canvasSize = popUpPositioner.CanvasSize;
            float height = canvasSize.y * ((float)percentage / 100.0f);
            float width = DefaultAspectRatio * height;
            return new Vector2(width, height);
        }

        #endregion
        
        #region Setting Image and Text
        /// <summary>
        /// set image property from serializable class
        /// </summary>
        /// <param name="image"></param>
        /// <param name="imageProperty"></param>
        protected void SetImageProperty(Image image, ImageProperty imageProperty, ImageEnum imageEnum, bool disabledIfNone = false)
        {

            if (imageProperty != null && imageProperty.sprite != null)
            {
                image.sprite = imageProperty.sprite;
                image.color = imageProperty.color;
                image.type = imageProperty.type;
            }
            // TODO: Reimplement this.
            //else if(imageEnum == ImageEnum.CloseButton && _spawningHotspot.GetComponentInParent<HotspotController>().closeButton.sprite != null)
            //{
            //    ImageProperty closeButton = _spawningHotspot.GetComponentInParent<HotspotController>().closeButton;
            //    image.sprite = closeButton.sprite;
            //    image.color = closeButton.color;
            //    image.type = closeButton.type;
            //}
            else if (imageEnum != ImageEnum.None)
            {
                image.sprite = DefaultAssets._instance.GetImage(imageEnum);
                image.color = Color.white;
                image.type = Image.Type.Simple;
            }
            else
            {
                if(disabledIfNone)
                    image.enabled = false;

                image.color = imageProperty.color;
            }
        }

        protected void SetCloseButtonImage(Image closeButton, PopUpSettings popupSettings)
        {
            if (popupSettings.overrideDefaultCloseButton) SetImageProperty(closeButton, popupSettings.closeButton, ImageEnum.CloseButton);
            else SetImageProperty(closeButton, null, ImageEnum.CloseButton);
        }

        protected void SetTextProperty(TextMeshProUGUI text, TextProperty textProperty)
        {
            text.text = textProperty.Text.Equals("") ? "Default Text" : textProperty.Text;
            text.fontSize = textProperty.FontSize == 0 ? 50 : textProperty.FontSize;
            text.color = textProperty.Color;
            text.font = textProperty.Font == null ? DefaultAssets._instance.GetFont(textProperty.FontName) : textProperty.Font;
        }

        protected void SetVideoProperty(VideoPlayer videoPlayer, VideoProperty videoProperty, bool loopVideo, Action OnVideoPrepared)
        {
            videoPlayer.source = videoProperty.videoSource;

            if (videoPlayer.source == VideoSource.VideoClip)
                videoPlayer.clip = videoProperty.videoClip;
            else
                videoPlayer.url = videoProperty.videoURL;

            videoPlayer.isLooping = loopVideo;

            //Once video has loaded it will resize the player correctly
            //videoPlayer.prepareCompleted += PrepareCompleted;
            videoPlayer.prepareCompleted += delegate { PrepareCompleted(videoPlayer, OnVideoPrepared); };
            videoPlayer.Prepare();
        }

        private void PrepareCompleted(VideoPlayer videoPlayer, Action OnVideoPrepared)
        {
            RenderTexture renderTexture = new RenderTexture((int)videoPlayer.width, (int)videoPlayer.height, 16, RenderTextureFormat.ARGB32);
            renderTexture.Create();

            videoPlayer.GetComponent<RawImage>().texture = renderTexture;
            videoPlayer.targetTexture = renderTexture;

            controlPanelRect.localScale = Vector3.one;

            //videoPlayer.prepareCompleted -= PrepareCompleted;
            videoPlayer.prepareCompleted -= delegate { PrepareCompleted(videoPlayer, OnVideoPrepared); };
            videoPlayer.Play();

            OnVideoPrepared();
        }

        #endregion

    }
}