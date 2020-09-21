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
    public class VideoHotspotPopUp : HotspotPopUp
    {
        public Material videoMaterial;

        private VideoPopUpDataModel videoPopUpDataModel;

        public VideoPlayer video;

        public GameObject playButton, pauseButton, restartButton;
        public Image imageCloseButton;

        private void Start()
        {
            //Close button shouldn't be visible until video is loaded
            controlPanelRect.localScale = Vector3.zero;
            rectTransform.anchoredPosition = new Vector2(0, -10000);
        }

        public void Init(VideoPopUpDataModel popupDataModel)
        {
            this.videoPopUpDataModel = popupDataModel;

            video.source = popupDataModel.popUpSetting.video.videoSource;

            if (video.source == VideoSource.VideoClip)
                video.clip = popupDataModel.popUpSetting.video.videoClip;
            else
                video.url = popupDataModel.popUpSetting.video.videoURL;

            SetCloseButtonImage(imageCloseButton, popupDataModel.popUpSetting); //Set Close Button

            pauseButton.SetActive(popupDataModel.popUpSetting.controlPanelStyle != ControlPanelStyle.RestartAndClose && popupDataModel.popUpSetting.controlPanelStyle != ControlPanelStyle.OnlyClose);
            restartButton.SetActive(popupDataModel.popUpSetting.controlPanelStyle != ControlPanelStyle.PlayPauseAndClose && popupDataModel.popUpSetting.controlPanelStyle != ControlPanelStyle.OnlyClose);

            if (popupDataModel.popUpSetting.closeAfterPlay)
                video.loopPointReached += VideoComplete;

            if (!popupDataModel.popUpSetting.closeAfterPlay && popupDataModel.popUpSetting.loop)
                video.isLooping = true;

            //Once video has loaded it will resize the player correctly
            video.prepareCompleted += ResizeVideoPlayer;            
        }

        void SetSize(VideoPopUpDataModel videoPopUpDataModel)
        {
            //Set size
            var size = videoPopUpDataModel.popUpSetting.size;

            switch (videoPopUpDataModel.popUpSetting.sizeOption)
            {
                case SizeOption.FixedContentSize:
                    size = new Vector2((int)video.width, (int)video.height);
                    break;

                case SizeOption.FixedPercentage:
                    var rect = transform.GetComponentInParent<Canvas>().GetComponent<RectTransform>().sizeDelta;

                    float height = rect.y * ((float)videoPopUpDataModel.popUpSetting.percentage / 100.0f);
                    float width = (rect.x / rect.y) * height - 100;//100 referes control panel width

                    size = new Vector2(width, height);

                    break;
            }

            //video.GetComponent<RawImage>().color = Color.black;
            SetContentSize(size);
            PlacePopUp(rectTransform);
        }

        public RenderTexture  renderTexture;
        private void ResizeVideoPlayer(VideoPlayer source)
        {
            renderTexture = new RenderTexture((int)source.width, (int)source.height, 16, RenderTextureFormat.ARGB32);
            renderTexture.Create();

            video.GetComponent<RawImage>().texture = renderTexture;
            video.targetTexture = renderTexture;

            controlPanelRect.localScale = Vector3.one;

            SetSize(videoPopUpDataModel);

            video.prepareCompleted -= ResizeVideoPlayer;
        }

        public void PlayButton()
        {
            playButton.SetActive(false);
            pauseButton.SetActive(true);
            video.Play();
        }

        public void PauseButton()
        {
            playButton.SetActive(true);
            pauseButton.SetActive(false);
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

        private void VideoComplete(VideoPlayer source) => ClosePopUp();
    }
}