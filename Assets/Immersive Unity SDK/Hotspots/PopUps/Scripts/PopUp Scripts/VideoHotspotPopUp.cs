/* Copyright (C) Immersive Interactive, Ltd - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Luke Bissell <luke@immersive.co.uk>, July 2019
 */

using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using static Com.Immersive.Hotspots.VideoPopUpDataModel;

namespace Com.Immersive.Hotspots
{
    public class VideoHotspotPopUp : HotspotPopUp<VideoPopUpSetting>
    {
        public Material videoMaterial;

        public VideoPlayer video;
        public Image imageBorder = null, mediaMask = null;
        public Image playButton, pauseButton, restartButton;

        protected override void SetupPopUpFromSettings(VideoPopUpSetting popUpSettings)
        {
            video.source = popUpSettings.video.videoSource;

            //Set image property for Border
            SetImageProperty(imageBorder, popUpSettings.border, ImageEnum.None);
            if (!popUpSettings.border.HasSprite)
                imageBorder.enabled = false;

            //Media Mask
            SetImageProperty(mediaMask, popUpSettings.mediaMask, ImageEnum.None);

            if (video.source == VideoSource.VideoClip)
                video.clip = popUpSettings.video.videoClip;
            else
                video.url = popUpSettings.video.videoURL;

            if (popUpSettings.useCustomButtons)
            {
                SetImageProperty(pauseButton, popUpSettings.pauseButtonImage, ImageEnum.PauseIcon);
                SetImageProperty(playButton, popUpSettings.playButtonImage, ImageEnum.PlayIcon);
                SetImageProperty(restartButton, popUpSettings.restartButtonImage, ImageEnum.ReplayIcon);
            }

            pauseButton.gameObject.SetActive(popUpSettings.controlPanelStyle != ControlPanelStyle.RestartAndClose && popUpSettings.controlPanelStyle != ControlPanelStyle.OnlyClose);
            restartButton.gameObject.SetActive(popUpSettings.controlPanelStyle != ControlPanelStyle.PlayPauseAndClose && popUpSettings.controlPanelStyle != ControlPanelStyle.OnlyClose);

            if (popUpSettings.closeAfterPlay)
                video.loopPointReached += VideoComplete;

            if (!popUpSettings.closeAfterPlay && popUpSettings.loop)
                video.isLooping = true;

            //Once video has loaded it will resize the player correctly
            video.prepareCompleted += ResizeVideoPlayer;
        }


        void SetSize(PopUpSettings popUpSettings)
        {
            //Set size
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

        protected override float DefaultAspectRatio => (float)video.width / (float)video.height;

        private RenderTexture renderTexture;
        private void ResizeVideoPlayer(VideoPlayer source)
        {
            renderTexture = new RenderTexture((int)source.width, (int)source.height, 16, RenderTextureFormat.ARGB32);
            renderTexture.Create();

            video.GetComponent<RawImage>().texture = renderTexture;
            video.targetTexture = renderTexture;

            controlPanelRect.localScale = Vector3.one;

            SetSize(popUpSettings);

            video.prepareCompleted -= ResizeVideoPlayer;
        }

        private void VideoComplete(VideoPlayer source) => ClosePopUp();

        private void OnDestroy()
        {
            if (renderTexture != null)
                Destroy(renderTexture);
        }

        #region Button Methods
        public void PlayButton()
        {
            playButton.gameObject.SetActive(false);
            pauseButton.gameObject.SetActive(true);
            video.Play();
        }

        public void PauseButton()
        {
            playButton.gameObject.SetActive(true);
            pauseButton.gameObject.SetActive(false);
            video.Pause();
        }

        public void RestartButton()
        {
            Texture2D texture = new Texture2D(video.texture.width, video.texture.height, TextureFormat.ARGB32, false);
            Graphics.CopyTexture(video.texture, texture);

            var playing = video.isPlaying;
            video.Stop();

            if (playing)
            {
                video.Play();
            }
            else
            {
                //Generates black texture;
                texture = new Texture2D(1, 1);
                texture.SetPixel(0, 0, Color.black);
                texture.Apply();
            }
            videoMaterial.SetTexture("_MainTex", texture);

        }
        #endregion


    }
}