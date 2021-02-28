using Com.Immersive.Cameras;
using Immersive.SuperHero;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntroScene : MonoBehaviour
{
    public SpriteRenderer introSprite;
    public GameObject buttonConitnue;
    public GameObject introDoor_Top;
    public GameObject introDoor_Bottom;

    public AudioClip doorOpenClip;

    public List<Animator> animators;

    void Start()
    {
        foreach (var anim in animators)
        {
            anim.SetInteger("Stats", 0);
        }

        Invoke("OpenDoor", 20);
        StartCoroutine(DisplayNewsPaper());
    }

    void OpenDoor()
    {
        AbstractImmersiveCamera.PlayAudio(doorOpenClip, 1);

        iTween.MoveTo(introDoor_Top, iTween.Hash("x", 0, "y", 1, "z", -0.2f, "islocal", true,
         "time", 4.0f, "easetype", iTween.EaseType.easeInOutQuad, "delay", 0, "oncomplete", (System.Action<object>)(newValue =>
         {

         })));

        iTween.MoveTo(introDoor_Bottom, iTween.Hash("x", 0, "y", -1, "z", -0.2f, "islocal", true,
         "time", 4.0f, "easetype", iTween.EaseType.easeInOutQuad, "delay", 0, "oncomplete", (System.Action<object>)(newValue =>
         {

         })));

        introSprite.gameObject.SetActive(false);

        StartCoroutine(PlayLabAmbience());
    }

    IEnumerator DisplayNewsPaper()
    {
        yield return new WaitForSeconds(2);
        iTween.RotateBy(introSprite.gameObject, Vector3.forward * 5, 2.0f);
        iTween.ScaleTo(introSprite.gameObject, Vector3.one * 0.8f, 2);

        yield return new WaitForSeconds(2);
        buttonConitnue.SetActive(true);
    }

    IEnumerator PlayLabAmbience()
    {
        yield return new WaitForSeconds(1);
        GameData.Instance.labAmbienceAudioSource.Play();
    }

    public void ContinueButton()
    {
        PlayerPrefs.SetInt("GameMode", 0);
        GameData.Instance.currentStage = FillInTheBlankStages.None;
        GameData.Instance.LoadScene("Super Hero Creator");
    }
}
