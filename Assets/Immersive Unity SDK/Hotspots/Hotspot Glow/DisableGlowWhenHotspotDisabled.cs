using Com.Immersive.Hotspots;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DisableGlowWhenHotspotDisabled : MonoBehaviour
{

    private IHotspot hotspot;
    public SpriteRenderer spriteRenderer;
    public TextMeshPro textMeshPro;


    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        textMeshPro = GetComponent<TextMeshPro>();
        if (hotspot == null)
            hotspot = transform.parent.GetComponent<IHotspot>();
    }

    public void Init(IHotspot hotspot)
    {
        this.hotspot = hotspot;
    }

    // Update is called once per frame
    void Update()
    {
        if (spriteRenderer != null)
            spriteRenderer.enabled = hotspot.IsInteractable;

        if (textMeshPro != null)
            textMeshPro.enabled = hotspot.IsInteractable;
    }
}