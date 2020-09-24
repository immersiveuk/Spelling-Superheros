using Com.Immersive.Cameras;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Option : MonoBehaviour, IInteractableObject
{
    public TextMeshPro textOption;
    private Spelling selectedSpelling;

    Vector2 startPos;

    public void OnPress()
    {
        startPos = this.transform.position;

        selectedSpelling = FindObjectOfType<FillInTheBlanksManager>().OnOptionSelect();

        if (selectedSpelling.spellingData.option.Equals(textOption.text))
        {
            iTween.MoveTo(this.gameObject, iTween.Hash("x", selectedSpelling.localPos.position.x, "y", selectedSpelling.localPos.position.y, "z", -0.2f, "islocal", false,
                 "time", 0.7f, "easetype", iTween.EaseType.easeInOutQuad, "delay", 0, "oncomplete", (System.Action<object>)(newValue =>
                  {
                      StartCoroutine(OnCorrectAnswer());
                  })));
        }
        else
        {
            iTween.MoveTo(this.gameObject, iTween.Hash("x", selectedSpelling.localPos.position.x, "y", selectedSpelling.localPos.position.y, "z", -0.2f, "islocal", false,
                  "time", 0.7f, "easetype", iTween.EaseType.easeInOutQuad, "delay", 0, "oncomplete", (System.Action<object>)(newValue =>
                  {
                      iTween.MoveTo(this.gameObject, iTween.Hash("x", startPos.x, "y", startPos.y, "z", -0.2f, "islocal", false,
                          "time", 0.7f, "easetype", iTween.EaseType.easeInOutQuad, "delay", 1, "oncomplete", (System.Action<object>)(newNewValue =>
                          {

                          })));

                  })));
        }
    }

    public void OnRelease()
    {
        
    }

    public void OnTouchEnter()
    {
        
    }

    public void OnTouchExit()
    {
        
    }

    public void SetText(string option)
    {
        textOption.text = option;
        StartCoroutine(SetCollider());
    }

    IEnumerator SetCollider()
    {
        yield return new WaitForEndOfFrame();
        this.GetComponent<BoxCollider>().size = textOption.textBounds.size;
    }

    IEnumerator OnCorrectAnswer()
    {
        yield return new WaitForSeconds(1);
        FindObjectOfType<FillInTheBlanksManager>().SelectNextSpelling();
        selectedSpelling.OnCorrectAnswer();
    }
}
