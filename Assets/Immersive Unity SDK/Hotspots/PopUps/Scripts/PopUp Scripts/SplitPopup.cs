using Immersive.Animation;
using Immersive.Utilities;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using static Com.Immersive.Hotspots.SplitPopUpDataModel;

namespace Com.Immersive.Hotspots
{ 
    public class SplitPopup : HotspotPopUp<SplitPopUpSetting>
    {
        [SerializeField] TextMeshProUGUI textTitleBody = null;
        [SerializeField] VideoPlayer videoPlayer = null; 
        [SerializeField] Image image = null, textBackground = null, mediaMask = null;
        [SerializeField] RectTransform textRect = null, maskRect = null;

        public Image playButton, pauseButton, restartButton;

        private const int referenceCanvasHeight = 1080;
        
        private RenderTexture renderTexture;

        FloatAnimator floatAnimator;
        private void FixedUpdate()
        {
            if (floatAnimator != null)
                FadeOut();
        }

        protected override void SetupPopUpFromSettings(SplitPopUpSetting popUpSettings)
        {
            textTitleBody.isRightToLeftText = popUpSettings.isRightToLeftText;
            
            textTitleBody.text = "";
            if (popUpSettings.includeTitle)
            {
                SetTitleText();
            }

            SetBodyText();

            //Text Background
            SetImageProperty(textBackground, popUpSettings.textBackground, ImageEnum.None);

            //Media Mask
            SetImageProperty(mediaMask, popUpSettings.mediaMask, ImageEnum.None);

            playButton.transform.parent.gameObject.SetActive(false);

            //Image
            if (popUpSettings.mediaType == SplitPopUpSetting.MediaType.Image)
            {
                ActivatePanel(false);

                SetImageProperty(image, popUpSettings.image, ImageEnum.None);
                SetSize();
            }
            //Video
            else
            {
                ActivatePanel(true);

                SetVideoProperty(videoPlayer, popUpSettings.video, popUpSettings.loopVideo, () =>
                {
                    SetSize();
                    playButton.transform.parent.gameObject.SetActive(popUpSettings.videoControl);
                    floatAnimator = new FloatAnimator(2.0f, 1, 0, EasingAnimations.Type.Linear);
                });
            }
        }

        /// <summary>
        /// Enable panel for Image or Video based on type of Media
        /// </summary>
        /// <param name="video"></param>
        void ActivatePanel(bool video)
        {
            image.gameObject.SetActive(!video);
            videoPlayer.GetComponent<RawImage>().enabled = video;
            videoPlayer.GetComponentInChildren<Image>().enabled = video;

            playButton.gameObject.SetActive(false);
            pauseButton.gameObject.SetActive(true);

            videoPlayer.GetComponentInChildren<CanvasGroup>().alpha = 1;
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

        /*
        private void SetVideoProperty(VideoPlayer videoPlayer, VideoProperty videoProperty) 
        {
            videoPlayer.source = videoProperty.videoSource;

            if (videoPlayer.source == VideoSource.VideoClip)
                videoPlayer.clip = videoProperty.videoClip;
            else
                videoPlayer.url = videoProperty.videoURL;

            videoPlayer.isLooping = popUpSettings.loopVideo;
                //Once video has loaded it will resize the player correctly
                videoPlayer.prepareCompleted += ResizeVideoPlayer;

        videoPlayer.Prepare();
        }

        private void ResizeVideoPlayer(VideoPlayer source)
        {
            renderTexture = new RenderTexture((int)source.width, (int)source.height, 16, RenderTextureFormat.ARGB32);
            renderTexture.Create();

            videoPlayer.GetComponent<RawImage>().texture = renderTexture;
            videoPlayer.targetTexture = renderTexture;

            controlPanelRect.localScale = Vector3.one;
            
            StartCoroutine(SetSize());

            videoPlayer.prepareCompleted -= ResizeVideoPlayer;

            floatAnimator = new FloatAnimator(2.0f, 1, 0, EasingAnimations.Type.Linear);
        }*/

        void FadeOut()
        {
            float value = floatAnimator.GetCurrentValue();
            videoPlayer.GetComponentInChildren<CanvasGroup>().alpha = value;

            if (floatAnimator.IsFinished)
                floatAnimator = null;
        }

        void SetSize()
        {
            //IMAGE POSITION and SEPERATION
            var leftAnchorMin = new Vector2(0, 0);
            var leftAnchorMax = new Vector2(0.5f, 1);

            var rightAnchorMin = new Vector2(0.5f, 0);
            var rightAnchorMax = new Vector2(1, 1);

            var rightOffsetMin = new Vector2(popUpSettings.separation / 2, 0);
            var leftOffsetMax = new Vector2(-popUpSettings.separation / 2, 0);


            RectTransform leftRectTransform = maskRect;
            RectTransform rightRectTransform = textBackground.rectTransform;

            switch (popUpSettings.mediaPosition)
            {
                case SplitPopUpSetting.MediaPosition.Left:
                    leftRectTransform = maskRect;
                    rightRectTransform = textBackground.rectTransform;
                    break;

                case SplitPopUpSetting.MediaPosition.Right:

                    leftRectTransform = textBackground.rectTransform;
                    rightRectTransform = maskRect;
                    break;
            }

            leftRectTransform.anchorMin = leftAnchorMin;
            leftRectTransform.anchorMax = leftAnchorMax;
            leftRectTransform.offsetMax = leftOffsetMax;

            rightRectTransform.anchorMin = rightAnchorMin;
            rightRectTransform.anchorMax = rightAnchorMax;
            rightRectTransform.offsetMin = rightOffsetMin;


            //MARGIN
            textRect.GetComponent<VerticalLayoutGroup>().padding = popUpSettings.padding;

            SetMediaSize();

            //FONT SIZE
            //txtBody.enableAutoSizing = true;

            //txtTitle.fontSize = popUpSettings.title.size;
            //yield return new WaitForFixedUpdate();

            //var titleSize = txtTitle.GetRenderedValues();
            //txtTitle.GetComponent<LayoutElement>().minHeight = titleSize.y;

            /*
            // Get correct parameters to change depending on whether it is an image or video.
            float mediaWidth, mediaHeight, aspect;
            RectTransform mediaRectTransform;

            if (popUpSettings.mediaType == SplitPopUpSetting.MediaType.Image)
            {
                var spriteRect = popUpSettings.image.sprite.rect;

                mediaWidth = spriteRect.width;
                mediaHeight = spriteRect.height;
                aspect = (float)mediaWidth / (float)mediaHeight;

                mediaRectTransform = image.rectTransform;

            }
            else
            {
                mediaWidth = videoPlayer.width;
                mediaHeight = videoPlayer.height;
                aspect = (float)mediaWidth / (float)mediaHeight;

                mediaRectTransform = videoPlayer.GetComponent<RectTransform>();
            }

            //IMAGE AND HOTSPOT SIZE
            //Fixed Popup Size - User defined width and height. Image matched vertical height. Offset can be set manually.
            //Fixed Percentage - Entire image shown. Size to percentage of vertical height.

            //Fixed Popup Size
            if (popUpSettings.sizeOption == SizeOption.FixedPopupSize)
            {
                var mediaSize = new Vector2(aspect * popUpSettings.size.y, popUpSettings.size.y);
                mediaRectTransform.sizeDelta = mediaSize;

                //Image offset
                mediaRectTransform.anchoredPosition = new Vector2(-popUpSettings.fixedPopupSizeImageOffset * (mediaSize.x - popUpSettings.size.x / 2 + popUpSettings.separation / 2), 0);
            }
            //Fixed Percentage
            else if (popUpSettings.sizeOption == SizeOption.FixedPercentage)
            {
                popUpSettings.size = GetSizeFromPercentage(popUpSettings.percentage);
                mediaRectTransform.sizeDelta = new Vector2(popUpSettings.size.y * DefaultAspectRatio, popUpSettings.size.y);
            }

            SetContentSizeAndPositionHotspot(popUpSettings.size);
            */
        }

        void SetMediaSize()
        {
            if (popUpSettings.sizeOption == SizeOption.FixedPercentage)
            {
                popUpSettings.size = GetSizeFromPercentage(popUpSettings.percentage);
            }

            RectTransform mediaRectTransform = popUpSettings.mediaType == SplitPopUpSetting.MediaType.Image ? image.rectTransform : videoPlayer.GetComponent<RectTransform>();
            mediaRectTransform.sizeDelta = new Vector2(popUpSettings.size.y * DefaultAspectRatio, popUpSettings.size.y);

            SetPopUpSize();
        }

        void SetPopUpSize()
        {
            SetContentSizeAndPositionHotspot(popUpSettings.size);
        }

        protected override float DefaultAspectRatio => GetDefaultAspectRatio();
        float GetDefaultAspectRatio()
        {
            float defaultAspectRatio;

            if (popUpSettings.mediaType == SplitPopUpSetting.MediaType.Image)
                defaultAspectRatio = popUpSettings.image.sprite.rect.width / popUpSettings.image.sprite.rect.height;
            else
                defaultAspectRatio = (float)popUpSettings.video.videoClip.width / (float)popUpSettings.video.videoClip.height;

            return defaultAspectRatio;
        }

        #region Button Methods
        public void PlayButton()
        {
            playButton.gameObject.SetActive(false);
            pauseButton.gameObject.SetActive(true);
            videoPlayer.Play();
        }

        public void PauseButton()
        {
            playButton.gameObject.SetActive(true);
            pauseButton.gameObject.SetActive(false);
            videoPlayer.Pause();
        }

        public void RestartButton()
        {
            Texture2D texture = new Texture2D(videoPlayer.texture.width, videoPlayer.texture.height, TextureFormat.ARGB32, false);
            Graphics.CopyTexture(videoPlayer.texture, texture);

            var playing = videoPlayer.isPlaying;
            videoPlayer.Stop();

            if (playing)
            {
                videoPlayer.Play();
            }
            else
            {
                //Generates black texture;
                texture = new Texture2D(1, 1);
                texture.SetPixel(0, 0, Color.black);
                texture.Apply();
            }
        }
        #endregion
    }
}
