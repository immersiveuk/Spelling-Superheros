using Com.Immersive.Cameras;
using Immersive.SuperHero;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntroScene : MonoBehaviour
{
    public SpriteRenderer introSprite;
    public GameObject buttonConitnue;
    public AudioClip introClip;

    IEnumerator Start()
    {
        yield return new WaitForSeconds(1);

        iTween.RotateBy(introSprite.gameObject, Vector3.forward * 5, 2.0f);
        iTween.ScaleTo(introSprite.gameObject, Vector3.one * 0.8f, 2);

        AbstractImmersiveCamera.PlayAudio(introClip, 1);

        yield return new WaitForSeconds(2);
        buttonConitnue.SetActive(true);
    }

    public void ContinueButton()
    {
        PlayerPrefs.SetInt("GameMode", 0);
        GameData.Instance.currentStage = FillInTheBlankStages.None;
        GameData.Instance.LoadScene("Super Hero Creator");
    }
}
