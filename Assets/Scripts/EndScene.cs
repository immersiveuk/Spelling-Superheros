using Com.Immersive.Cameras;
using Immersive.SuperHero;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndScene : MonoBehaviour
{
    public AudioClip victoryClip;
    public AudioClip victoryMusicClip;

    private void Start()
    {
        AbstractImmersiveCamera.PlayAudio(victoryClip, 1);
        StartCoroutine(PlayVictoryMusic(victoryClip.length));
    }

    IEnumerator PlayVictoryMusic(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        AbstractImmersiveCamera.PlayAudio(victoryMusicClip, 1);
    }

    public void SimpleModeButton()
    {
        PlayerPrefs.SetInt("GameMode", 0);
        GameData.Instance.ResetManager();
        GameData.Instance.LoadScene("Stage1");
    }

    public void AdvancedModeButton()
    {
        PlayerPrefs.SetInt("GameMode", 1);
        GameData.Instance.ResetManager();
        GameData.Instance.LoadScene("Stage1");
    }
}
