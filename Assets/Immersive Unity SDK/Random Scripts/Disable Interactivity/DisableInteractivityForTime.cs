using Com.Immersive.Cameras;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableInteractivityForTime : MonoBehaviour
{
    public void DisableInteractivity(float duration)
    {
        IInteractableObject[] ios = transform.GetComponents<IInteractableObject>();
        foreach (var io in ios)
        {
            ((MonoBehaviour)io).enabled = false;
        }
        UpdateVisuals(false);
        StartCoroutine(EnableInteractivity(duration));
    }

    private IEnumerator EnableInteractivity(float delay) 
    {
        yield return new WaitForSeconds(delay);
        IInteractableObject[] ios = transform.GetComponents<IInteractableObject>();
        foreach (var io in ios)
        {
            ((MonoBehaviour)io).enabled = true;
        }
        UpdateVisuals(true);
    }

    protected virtual void UpdateVisuals(bool enabled) { }
}
