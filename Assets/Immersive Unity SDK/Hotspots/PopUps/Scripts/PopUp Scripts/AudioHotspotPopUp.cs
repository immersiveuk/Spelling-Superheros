using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Com.Immersive.Hotspots.AudioPopUpDataModel;

namespace Com.Immersive.Hotspots
{
    public class AudioHotspotPopUp : HotspotPopUp<AudioPopUpDataModel.AudioPopUpSetting>
    {
        [SerializeField] AudioSource audioSource = null;
        [SerializeField] Image imageThumbnail = null;
        [SerializeField] GameObject playButton = null, pauseButton = null, restartButton = null;
        protected override void SetupPopUpFromSettings(AudioPopUpSetting popUpSettings)
        {
            audioSource.clip = popUpSettings.audioClip;
            audioSource.Play();
            audioSource.loop = popUpSettings.loop;
            if (popUpSettings.closeAfterPlay)
                audioSource.loop = false;

            if (popUpSettings.useThumbnail)
                SetImageProperty(imageThumbnail, popUpSettings.thumbnail, ImageEnum.None); //Set image property for thumbnail
            else
                imageThumbnail.gameObject.SetActive(false);

            pauseButton.SetActive(popUpSettings.controlPanelStyle != ControlPanelStyle.RestartAndClose && popUpSettings.controlPanelStyle != ControlPanelStyle.OnlyClose);
            restartButton.SetActive(popUpSettings.controlPanelStyle != ControlPanelStyle.PlayPauseAndClose && popUpSettings.controlPanelStyle != ControlPanelStyle.OnlyClose);

            StartCoroutine(SetSize());
        }

        private IEnumerator SetSize()
        {
            yield return new WaitForEndOfFrame();
            SetContentSizeAndPositionHotspot(rectTransform.sizeDelta);
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
            if(popUpSettings.closeAfterPlay && pauseButton.activeSelf && !audioSource.isPlaying)
                ClosePopUp();
        }

    }
}