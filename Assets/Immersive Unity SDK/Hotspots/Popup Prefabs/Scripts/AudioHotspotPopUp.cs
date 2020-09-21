using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Com.Immersive.Hotspots.AudioPopUpDataModel;

namespace Com.Immersive.Hotspots
{
    public class AudioHotspotPopUp : HotspotPopUp
    {
        public AudioSource audioSource;
        public Image imageThumbnail,imageCloseButton;
        public GameObject playButton, pauseButton, restartButton;

        AudioPopUpDataModel audioPopUpDataModel;

        public void Init(AudioPopUpDataModel popupDataModel)
        {
            this.audioPopUpDataModel = popupDataModel;

            SetCloseButtonImage(imageCloseButton, popupDataModel.popUpSetting); //Set Close Button

            audioSource.clip = popupDataModel.popUpSetting.audioClip;
            audioSource.Play();

            if (popupDataModel.popUpSetting.useThumbnail)
            {
                SetImageProperty(imageThumbnail, popupDataModel.popUpSetting.thumbnail, ImageEnum.None);//Set image property for thumbnail
            }
            else
            {
                imageThumbnail.gameObject.SetActive(false);
            }

            pauseButton.SetActive(popupDataModel.popUpSetting.controlPanelStyle != ControlPanelStyle.RestartAndClose && popupDataModel.popUpSetting.controlPanelStyle != ControlPanelStyle.OnlyClose);
            restartButton.SetActive(popupDataModel.popUpSetting.controlPanelStyle != ControlPanelStyle.PlayPauseAndClose && popupDataModel.popUpSetting.controlPanelStyle != ControlPanelStyle.OnlyClose);

            StartCoroutine(SetSize());

            audioSource.loop = popupDataModel.popUpSetting.loop;

            if (popupDataModel.popUpSetting.closeAfterPlay)
                audioSource.loop = false;
        }

        private IEnumerator SetSize()
        {
            yield return new WaitForEndOfFrame();
            SetContentSize(rectTransform.sizeDelta);
            //_spawningHotspot.PlacePopUp(rectTransform);
            PlacePopUp(rectTransform);
        }

        public void PlayButton()
        {
            playButton.SetActive(false);
            pauseButton.SetActive(true);
            audioSource.Play();
        }

        public void PauseButton()
        {
            playButton.SetActive(true);
            pauseButton.SetActive(false);
            audioSource.Pause();
        }

        public void RestartButton()
        {
            var playing = audioSource.isPlaying;
            audioSource.Stop();
            if (playing)
            {
                audioSource.Play();
            }
        }

        private void Update()
        {
            if(audioPopUpDataModel.popUpSetting.closeAfterPlay && pauseButton.activeSelf && !audioSource.isPlaying)
            {
                ClosePopUp();
            }
        }
    }
}