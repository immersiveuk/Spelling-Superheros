using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PulseAnimation : MonoBehaviour
{
    float timePeriod = 3;
    float minAlpha = 0f;
    float maxAlpha = 1f;

    bool maintainGlobalSync = true;

    private float time = 0;

    private SpriteRenderer spriteRenderer;
    private Image image;
    private TextMeshPro textMeshPro;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        image = GetComponent<Image>();
        textMeshPro = GetComponent<TextMeshPro>();
    }

    public void InitSettings(HotspotGlowSettings hotspotGlowSettings)
    {
        this.timePeriod = hotspotGlowSettings.pulseDuration;
        this.minAlpha = hotspotGlowSettings.minAlpha;
        this.maxAlpha = hotspotGlowSettings.maxAlpha;
        this.maintainGlobalSync = hotspotGlowSettings.maintainGlobalSync;

        if (spriteRenderer)
            spriteRenderer.color = hotspotGlowSettings.colour;
        else if (image)
            image.color = hotspotGlowSettings.colour;
        else if (textMeshPro)
            textMeshPro.color = hotspotGlowSettings.colour;
    }

    void Update()
    {
        CalculateTime();

        var lerpValue = time / (timePeriod / 2);

        if (lerpValue > 1)
            lerpValue = 2 - lerpValue;

        var alpha = Mathf.Lerp(minAlpha, maxAlpha, lerpValue);

        var colour = Color.white;
        colour.a = alpha;

        if (spriteRenderer)
        {
            colour = spriteRenderer.color;
            colour.a = alpha;
            spriteRenderer.color = colour;
        }
        else if (image)
        {
            colour = image.color;
            colour.a = alpha;
            image.color = colour;
        }
        else if (textMeshPro)
        {
            colour = textMeshPro.color;
            colour.a = alpha;
            textMeshPro.color = colour;
        }
    }

    private void CalculateTime()
    {
        if (maintainGlobalSync)
        {
            time = Time.time % timePeriod;
        }
        else
        {
            time += Time.deltaTime;
            if (time > timePeriod)
                time -= timePeriod;
        }
    }
}
