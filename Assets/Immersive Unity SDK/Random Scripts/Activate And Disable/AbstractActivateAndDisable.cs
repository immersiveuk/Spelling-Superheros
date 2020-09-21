using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbstractActivateAndDisable : MonoBehaviour
{
    public GameObject[] objectsToActivate;
    public GameObject[] objectsToDisable;

    [Tooltip("If true all objects in the array Objects To Activate will be disabled at the start of the scene.")]
    public bool disableActivateObjectsOnStart = false;

    [Tooltip("How long to wait to activate and disable objects")]
    public float delay = 0;

    private bool inDelayPhase = false;

    protected void DisableActivateObjectsOnStart()
    {
        if (disableActivateObjectsOnStart)
        {
            foreach (var obj in objectsToActivate)
            {
                obj.SetActive(false);
            }
        }
    }

    protected void ActivateAndDisable()
    {
        if (!inDelayPhase)
        {
            if (delay > 0)
                StartCoroutine(ActivateAndDisableWithDelay());
            else
                ActivateAndDisableWithoutDelay();
        }
    }

    private IEnumerator ActivateAndDisableWithDelay()
    {
        inDelayPhase = true;
        yield return new WaitForSeconds(delay);
        foreach (var obj in objectsToActivate)
        {
            obj.SetActive(true);
        }

        foreach (var obj in objectsToDisable)
        {
            if (obj == gameObject)
            {
                gameObject.SetActive(true);
                StartCoroutine(DisableSelf());
                continue;
            }
            obj.SetActive(false);
        }

        inDelayPhase = false;
    }

    private void ActivateAndDisableWithoutDelay()
    {
        foreach (var obj in objectsToActivate)
        {
            obj.SetActive(true);
        }

        foreach (var obj in objectsToDisable)
        {
            if (obj == gameObject)
            {
                gameObject.SetActive(true);
                StartCoroutine(DisableSelf());
                continue;
            }
            obj.SetActive(false);
        }
    }

    private IEnumerator DisableSelf()
    {
        yield return new WaitForEndOfFrame();
        gameObject.SetActive(false);
    }
}
