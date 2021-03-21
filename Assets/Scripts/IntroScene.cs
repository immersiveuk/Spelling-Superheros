using Com.Immersive.Cameras;
using Immersive.SuperHero;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntroScene : MonoBehaviour
{
    public SpriteRenderer introSprite;
    public GameObject introDoor_Top;
    public GameObject introDoor_Bottom;

    [Header("SFX")]
    public AudioClip selectClip;
    public AudioClip doorOpenClip;
    public AudioClip newspaperClip;

    public List<Animator> animators;
    public List<GameObject> startButtons;

    void Start()
    {
        foreach (var obj in startButtons)
        {
            obj.SetActive(false);
        }

        /*
        foreach (var anim in animators)
        {
            anim.SetInteger("Stats", 0);
        }
        */

        Invoke("OpenDoor", 23);
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
        yield return new WaitForSeconds(3);
        AbstractImmersiveCamera.PlayAudio(newspaperClip, 0.5f);
        iTween.RotateBy(introSprite.gameObject, Vector3.forward * 5, 3);
        iTween.ScaleTo(introSprite.gameObject, Vector3.one * 0.8f, 3);

        yield return new WaitForSeconds(2);

        foreach (var obj in startButtons)
        {
            obj.SetActive(true);
        }
    }

    IEnumerator PlayLabAmbience()
    {
        yield return new WaitForSeconds(1);
        GameData.Instance.labAmbienceAudioSource.Play();
    }

    public void LoadFillInTheBlank()
    {
        AbstractImmersiveCamera.PlayAudio(selectClip);
        GameData.Instance.currentStage = SuperHeroCreatorStages.Stage1;
        GameData.Instance.LoadScene("Stage1");
    }
}
