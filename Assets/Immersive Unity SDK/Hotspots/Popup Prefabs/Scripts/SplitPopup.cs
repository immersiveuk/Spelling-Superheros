using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

namespace Com.Immersive.Hotspots
{ 
    public class SplitPopup : HotspotPopUp
    {
        public TextMeshProUGUI txtBody;
        public TextMeshProUGUI txtTitle;
        public VideoPlayer videoPlayer; 
        public Image image, textBackground, imageCloseButton, mediaMask;
        public RectTransform textRect, maskRect;

        private SplitPopUpDataModel dataModel;
        private const int referenceCanvasHeight = 1080;

        private void Start()
        {
            //This ensures it is off screen until it the size is calculated and it can be properly placed.
            rectTransform.anchoredPosition = new Vector2(0, -10000);
        }

        public void Init(SplitPopUpDataModel popupDataModel)
        {
            this.dataModel = popupDataModel;
            SetTextProperty(txtTitle, popupDataModel.popUpSetting.title);
            SetTextProperty(txtBody, popupDataModel.popUpSetting.body);

            //Text Background
            SetImageProperty(textBackground, popupDataModel.popUpSetting.textBackground, ImageEnum.None);

            //Close Button
            SetCloseButtonImage(imageCloseButton, popupDataModel.popUpSetting); //Set Close Button

            //Media Mask
            SetImageProperty(mediaMask, popupDataModel.popUpSetting.mediaMask, ImageEnum.None);


            //Image
            if (popupDataModel.popUpSetting.mediaType == SplitPopUpDataModel.SplitPopUpSetting.MediaType.Image)
            {
                image.gameObject.SetActive(true);
                videoPlayer.gameObject.SetActive(false);

                SetImageProperty(image, popupDataModel.popUpSetting.image, ImageEnum.None);
                StartCoroutine(SetSize());

            }
            //Video
            else
            {
                image.gameObject.SetActive(false);
                videoPlayer.gameObject.SetActive(true);

                SetVideoProperty(videoPlayer, popupDataModel.popUpSetting.video);
                videoPlayer.isLooping = popupDataModel.popUpSetting.loopVideo;
                //Once video has loaded it will resize the player correctly
                videoPlayer.prepareCompleted += ResizeVideoPlayer;
            }
        }

        private void SetVideoProperty(VideoPlayer videoPlayer, VideoProperty videoProperty) 
        {
            videoPlayer.source = videoProperty.videoSource;

            if (videoPlayer.source == VideoSource.VideoClip)
                videoPlayer.clip = videoProperty.videoClip;
            else
                videoPlayer.url = videoProperty.videoURL;

        }

        public RenderTexture renderTexture;
        private void ResizeVideoPlayer(VideoPlayer source)
        {
            renderTexture = new RenderTexture((int)source.width, (int)source.height, 16, RenderTextureFormat.ARGB32);
            renderTexture.Create();

            videoPlayer.GetComponent<RawImage>().texture = renderTexture;
            videoPlayer.targetTexture = renderTexture;

            controlPanelRect.localScale = Vector3.one;
            
            StartCoroutine(SetSize());

            videoPlayer.prepareCompleted -= ResizeVideoPlayer;
        }

        IEnumerator SetSize()
        {
            //IMAGE POSITION and SEPERATION
            var leftAnchorMin = new Vector2(0, 0);
            var leftAnchorMax = new Vector2(0.5f, 1);

            var rightAnchorMin = new Vector2(0.5f, 0);
            var rightAnchorMax = new Vector2(1, 1);

            var rightOffsetMin = new Vector2(dataModel.popUpSetting.separation / 2, 0);
            var leftOffsetMax = new Vector2(-dataModel.popUpSetting.separation / 2, 0);


            RectTransform leftRectTransform = maskRect;
            RectTransform rightRectTransform = textBackground.rectTransform;

            switch (dataModel.popUpSetting.mediaPosition)
            {
                case SplitPopUpDataModel.SplitPopUpSetting.MediaPosition.Left:
                    leftRectTransform = maskRect;
                    rightRectTransform = textBackground.rectTransform;
                    break;

                case SplitPopUpDataModel.SplitPopUpSetting.MediaPosition.Right:

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
            textRect.GetComponent<VerticalLayoutGroup>().padding = dataModel.popUpSetting.padding;

            //FONT SIZE
            txtBody.enableAutoSizing = true;

            txtTitle.fontSize = dataModel.popUpSetting.title.size;
            yield return new WaitForFixedUpdate();

            var titleSize = txtTitle.GetRenderedValues();
            txtTitle.GetComponent<LayoutElement>().minHeight = titleSize.y;


            // Get correct parameters to change depending on whether it is an image or video.
            float mediaWidth, mediaHeight, aspect;
            RectTransform mediaRectTransform;
            if (dataModel.popUpSetting.mediaType == SplitPopUpDataModel.SplitPopUpSetting.MediaType.Image)
            {
                var spriteRect = dataModel.popUpSetting.image.sprite.rect;

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
            //Fixed Content Size - Entire image shown. Size set to match pixel value of image.
            //Fixed Percentage - Entire image shown. Size to percentage of vertical height.

            //Fixed Popup Size
            if (dataModel.popUpSetting.sizeOption == SizeOption.FixedPopupSize)
            {
                var mediaSize = new Vector2(aspect * dataModel.popUpSetting.size.y, dataModel.popUpSetting.size.y);
                mediaRectTransform.sizeDelta = mediaSize;

                //Image offset
                mediaRectTransform.anchoredPosition = new Vector2(-dataModel.popUpSetting.fixedPopupSizeImageOffset * (mediaSize.x - dataModel.popUpSetting.size.x / 2 + dataModel.popUpSetting.separation / 2), 0);
            }
            //Fixed Content Size
            //
            else if (dataModel.popUpSetting.sizeOption == SizeOption.FixedContentSize)
            {
                dataModel.popUpSetting.size = new Vector2(mediaWidth * 2 + dataModel.popUpSetting.separation, mediaHeight);
                mediaRectTransform.sizeDelta = new Vector2(mediaWidth, mediaHeight);
            }
            //Fixed Percentage
            else if (dataModel.popUpSetting.sizeOption == SizeOption.FixedPercentage)
            {
                var popupHeight = referenceCanvasHeight * dataModel.popUpSetting.percentage * 0.01f;

                dataModel.popUpSetting.size = new Vector2(aspect * 2 * popupHeight + dataModel.popUpSetting.separation, popupHeight);

                mediaRectTransform.sizeDelta = new Vector2(popupHeight * aspect, popupHeight);
            }

            SetContentSize(dataModel.popUpSetting.size);
            PlacePopUp(rectTransform);
        }
    }
}
