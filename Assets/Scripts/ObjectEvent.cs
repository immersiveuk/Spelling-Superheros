using Com.Immersive.Cameras;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ObjectEvent : MonoBehaviour, IInteractableObject
{
    public UnityEvent OnPressEvent;
    public UnityEvent OnReleaseEvent;

    public void OnPress()
    {
        OnPressEvent.Invoke();
        if (transform.childCount > 0 && transform.GetChild(0).transform.GetComponent<SpriteRenderer>())
        {
            transform.GetChild(0).gameObject.SetActive(false);
        }
    }

    public void OnRelease()
    {
        OnReleaseEvent.Invoke();
    }

    public void OnTouchEnter()
    {
        
    }

    public void OnTouchExit()
    {
       
    }
}
