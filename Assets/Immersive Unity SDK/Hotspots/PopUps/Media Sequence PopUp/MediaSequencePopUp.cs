using Com.Immersive.Hotspots;
using Immersive.Animation;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using static Com.Immersive.Hotspots.MediaSequencePopUpSetting.MediaPopUp;

namespace Com.Immersive.Hotspots
{
    public class MediaSequencePopUp : HotspotPopUpSequence<MediaSequencePopUpSetting>
    {
        public List<RectTransform> mediaPanels = new List<RectTransform>();
        public VideoPlayer[] videoPlayers;
        public Image[] images;
        public VideoPlayer videoPlayer;
        public Image imageBorder = null, mediaMask = null;
        public Image playButton, pauseButton, restartButton;
        public Image image;
        int mediaIndex = 0;

        protected override void UpdateVisualsForIndex(int newIndex, bool startIndex) => SetPopup(newIndex, startIndex);

        protected override void SetupPopUpFromSettings(MediaSequencePopUpSetting popUpSettings)
        {
            base.SetupPopUpFromSettings(popUpSettings);
        }

        private void Start()
        {
            mediaPanels[1].anchoredPosition = new Vector3(popUpSettings.size.x, 0, 0);
        }

        Vector3Animator vectorAnimator;
        FloatAnimator floatAnimator;

        enum MoveDirection { Left,Right}

        MoveDirection moveDirection;

        private void FixedUpdate()
        {
            if (vectorAnimator != null)
            {
                OnVectorTransition();
            }

            if (floatAnimator != null)
                FadeOutVideoScreen();
        }

        void OnVectorTransition()
        {
            Vector2 newPos = vectorAnimator.GetCurrentValue();

            if (moveDirection == MoveDirection.Right)
            {
                mediaPanels[0].anchoredPosition = new Vector2(-newPos.x, 0);
                mediaPanels[1].anchoredPosition = new Vector2(popUpSettings.size.x - newPos.x, 0);
            }
            else
            {
                mediaPanels[0].anchoredPosition = new Vector2(newPos.x, 0);
                mediaPanels[1].anchoredPosition = new Vector2(-popUpSettings.size.x + newPos.x, 0);
            }

            if (vectorAnimator.IsFinished)
            {
                vectorAnimator = null;

                RectTransform temp = mediaPanels[0];
                mediaPanels.Remove(temp);
                mediaPanels.Add(temp);

                OnTransitionCompleted();
            }
        }

        void FadeOutVideoScreen()
        {
            float value = floatAnimator.GetCurrentValue();
            videoPlayer.GetComponentInChildren<CanvasGroup>().alpha = value;

            if (floatAnimator.IsFinished)
                floatAnimator = null;
        }

        private void SetPopup(int index, bool startIndex)
        {
            if(startIndex)
            {
                if (popUpSettings.sizeOption == SizeOption.FixedPercentage)
                    popUpSettings.size = GetSizeFromPercentage(popUpSettings.percentage);
            }

            if (mediaIndex != index)
            {
                moveDirection = index > mediaIndex ? MoveDirection.Right : MoveDirection.Left;
                vectorAnimator = new Vector3Animator(0.5f, Vector2.zero, new Vector2(popUpSettings.size.x, 0), EasingAnimations.Type.Linear);            
            }

            this.mediaIndex = index;

            videoPlayer.Stop();

            videoPlayer = videoPlayers[index % 2];
            image = images[index % 2];

            floatAnimator = null;            

            //Media Mask
            SetImageProperty(mediaMask, popUpSettings.mediaMask, ImageEnum.None);

            //Border
            SetImageProperty(imageBorder, popUpSettings.border, ImageEnum.None, true);

            //Image
            if (popUpSettings.mediaPopups[mediaIndex].mediaType == MediaType.Image)
            {
                ActivatePanel(false);
                SetImageProperty(image, popUpSettings.mediaPopups[mediaIndex].image, ImageEnum.None);
            }
            else
            {
                ActivatePanel(true);

                if (startIndex)
                    OnTransitionCompleted();
            }

            SetMediaSize();
        }

        void OnTransitionCompleted()
        {
            if (popUpSettings.mediaPopups[mediaIndex].mediaType == MediaType.Video)
            {
                SetVideoProperty(videoPlayer, popUpSettings.mediaPopups[mediaIndex].video, popUpSettings.mediaPopups[mediaIndex].loopVideo, () =>
                {
                    playButton.transform.parent.gameObject.SetActive(true);
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
            playButton.transform.parent.gameObject.SetActive(false);

            image.gameObject.SetActive(!video);
            videoPlayer.GetComponent<RawImage>().enabled = video;//.gameObject.SetActive(video);
            videoPlayer.GetComponentInChildren<Image>().enabled = video;

            playButton.gameObject.SetActive(false);
            pauseButton.gameObject.SetActive(true);

            videoPlayer.GetComponentInChildren<CanvasGroup>().alpha = 1;
        }

        bool contentSizeSet = false;
        /// <summary>
        /// Set size of Video Player
        /// </summary>
        /// <param name="popUpSettings"></param>
        void SetMediaSize()
        {
            RectTransform mediaRectTransform = popUpSettings.mediaPopups[mediaIndex].mediaType == MediaType.Image? image.rectTransform: videoPlayer.GetComponent<RectTransform>();
            mediaRectTransform.sizeDelta = new Vector2(popUpSettings.size.y * DefaultAspectRatio, popUpSettings.size.y);

            if (!contentSizeSet)
                SetPopUpSize();           
        }

        void SetPopUpSize()
        {
            contentSizeSet = true;

            contentRect.GetComponent<VerticalLayoutGroup>().padding = popUpSettings.padding;

            Vector2 sizePlusPadding = popUpSettings.size;
            sizePlusPadding.x += popUpSettings.padding.horizontal;
            sizePlusPadding.y += popUpSettings.padding.vertical;

            SetContentSizeAndPositionHotspot(sizePlusPadding);
        }

        protected override float DefaultAspectRatio => GetDefaultAspectRatio();
        float GetDefaultAspectRatio()
        {
            float defaultAspectRatio;

            MediaSequencePopUpSetting.MediaPopUp media = popUpSettings.mediaPopups[mediaIndex];

            if (media.mediaType == MediaType.Image)
                defaultAspectRatio = media.image.sprite.rect.width / media.image.sprite.rect.height;
            else
                defaultAspectRatio = (float)media.video.videoClip.width / (float)media.video.videoClip.height;

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