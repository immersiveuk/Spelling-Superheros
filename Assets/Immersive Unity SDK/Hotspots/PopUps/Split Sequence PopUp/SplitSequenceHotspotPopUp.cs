using Immersive.Animation;
using Immersive.Utilities;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using static Com.Immersive.Hotspots.SplitSequencePopUpSetting;
using static Com.Immersive.Hotspots.SplitSequencePopUpSetting.SplitPopUp;

namespace Com.Immersive.Hotspots
{
    public class SplitSequenceHotspotPopUp : HotspotPopUpSequence<SplitSequencePopUpSetting>
    {
        [SerializeField] TextMeshProUGUI textTitleBody = null;
        [SerializeField] VideoPlayer videoPlayer = null;
        [SerializeField] Image image = null, textBackground = null, mediaMask = null;
        [SerializeField] RectTransform textRect = null, maskRect = null;

        public Image playButton, pauseButton, restartButton;

        private RenderTexture renderTexture;
        int mediaIndex = 0;

        FloatAnimator floatAnimator;
        private void FixedUpdate()
        {
            if (floatAnimator != null)
                FadeOut();
        }

        protected override void UpdateVisualsForIndex(int newIndex, bool startIndex) => SetPopup(newIndex, startIndex);

        protected override void SetupPopUpFromSettings(SplitSequencePopUpSetting popUpSettings)
        {
            base.SetupPopUpFromSettings(popUpSettings);
        }

        private void SetPopup(int index, bool startIndex)
        {
            if (startIndex)
            {
                if (popUpSettings.sizeOption == SizeOption.FixedPercentage)
                    popUpSettings.size = GetSizeFromPercentage(popUpSettings.percentage);
            }

            this.mediaIndex = index;
            textTitleBody.text = "";

            if (popUpSettings.splitPopups[index].includeTitle)
            {
                SetTitleText(popUpSettings.splitPopups[index]);
            }

            SetBodyText(popUpSettings.splitPopups[index]);

            //Text Background
            SetImageProperty(textBackground, popUpSettings.textBackground, ImageEnum.None);

            //Media Mask
            SetImageProperty(mediaMask, popUpSettings.mediaMask, ImageEnum.None);

            if (popUpSettings.keepSameMedia && index > 0)
                return;

            videoPlayer.Stop();

            //Image
            if (popUpSettings.splitPopups[mediaIndex].mediaType == MediaType.Image)
            {
                ActivatePanel(false);
                SetImageProperty(image, popUpSettings.splitPopups[mediaIndex].image, ImageEnum.None);                
            }
            //Video
            else
            {
                ActivatePanel(true);
                
                SetVideoProperty(videoPlayer, popUpSettings.splitPopups[mediaIndex].video, popUpSettings.splitPopups[mediaIndex].loopVideo, () =>
                {
                    playButton.transform.parent.gameObject.SetActive(popUpSettings.splitPopups[mediaIndex].videoControl);
                    floatAnimator = new FloatAnimator(2.0f, 1, 0, EasingAnimations.Type.Linear);
                });
            }

            SetSize();
        }

        /// <summary>
        /// Enable panel for Image or Video based on type of Media
        /// </summary>
        /// <param name="video"></param>
        void ActivatePanel(bool video)
        {
            playButton.transform.parent.gameObject.SetActive(false);

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
        void SetTitleText(SplitPopUp splitPopUp)
        {
            string titleText = splitPopUp.title.GenerateTMPStyledText();
            titleText = TMProTagExtensions.AddNewLine(titleText);

            if (splitPopUp.space > 0)
                titleText = TMProTagExtensions.AddSpace(titleText, splitPopUp.space);

            textTitleBody.text = titleText;
        }

        /// <summary>
        /// Set Body text with tags
        /// </summary>
        void SetBodyText(SplitPopUp splitPopUp)
        {
            textTitleBody.text += splitPopUp.body.GenerateTMPStyledText();
        }

        /*
        private void SetVideoProperty(VideoPlayer videoPlayer, VideoProperty videoProperty)
        {
            playButton.transform.parent.gameObject.SetActive(true);

            videoPlayer.source = videoProperty.videoSource;

            if (videoPlayer.source == VideoSource.VideoClip)
                videoPlayer.clip = videoProperty.videoClip;
            else
                videoPlayer.url = videoProperty.videoURL;

            videoPlayer.isLooping = popUpSettings.splitPopups[mediaIndex].loopVideo;
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

            videoPlayer.prepareCompleted -= ResizeVideoPlayer;

            videoPlayer.Play();

            floatAnimator = new FloatAnimator(2.0f, 1, 0, EasingAnimations.Type.Linear);
        }
        */

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
                case MediaPosition.Left:
                    leftRectTransform = maskRect;
                    rightRectTransform = textBackground.rectTransform;
                    break;

                case MediaPosition.Right:

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

            /*
            // Get correct parameters to change depending on whether it is an image or video.
            float mediaWidth, mediaHeight, aspect;
            RectTransform mediaRectTransform;

            if (splitPopUp.mediaType == MediaType.Image)
            {
                var spriteRect = splitPopUp.image.sprite.rect;

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

        bool contentSizeSet = false;
        void SetMediaSize()
        {
            RectTransform mediaRectTransform = popUpSettings.splitPopups[mediaIndex].mediaType == MediaType.Image ? image.rectTransform : videoPlayer.GetComponent<RectTransform>();
            mediaRectTransform.sizeDelta = new Vector2(popUpSettings.size.y * DefaultAspectRatio, popUpSettings.size.y);

            if (!contentSizeSet)
                SetPopUpSize();
        }

        void SetPopUpSize()
        {
            contentSizeSet = true;
            SetContentSizeAndPositionHotspot(popUpSettings.size);
        }


        protected override float DefaultAspectRatio => GetDefaultAspectRatio();
        float GetDefaultAspectRatio()
        {
            float defaultAspectRatio;

            SplitPopUp media = popUpSettings.splitPopups[mediaIndex];

            if (media.mediaType == MediaType.Image)
                defaultAspectRatio = media.image.sprite.rect.width / media.image.sprite.rect.height;
            else
                defaultAspectRatio = (float)media.video.videoClip.width / (float)media.video.videoClip.height;

            return defaultAspectRatio;
        }
    }
}