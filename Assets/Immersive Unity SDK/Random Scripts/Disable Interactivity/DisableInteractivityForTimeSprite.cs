using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class DisableInteractivityForTimeSprite : DisableInteractivityForTime
{
    [SerializeField] Color disabledColour = new Color(0.3f, 0.3f, 0.3f);

    private SpriteRenderer spriteRenderer;
    private Color enabledColour;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        enabledColour = spriteRenderer.color;
    }

    protected override void UpdateVisuals(bool enabled)
    {
        StartCoroutine(UpdateVisualsAfterAFrame(enabled));
    }

    private IEnumerator UpdateVisualsAfterAFrame(bool enabled)
    {
        yield return null;
        spriteRenderer.color = enabled ? enabledColour : disabledColour;
        yield return null;
        spriteRenderer.color = enabled ? enabledColour : disabledColour;
    }
}
