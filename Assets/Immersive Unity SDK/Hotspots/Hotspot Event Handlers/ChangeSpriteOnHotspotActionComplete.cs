using Com.Immersive.Hotspots;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeSpriteOnHotspotActionComplete : MonoBehaviour,IHotspotActionCompleteHandler
{
    public Sprite newSprite;
    private SpriteRenderer spriteRenderer;

    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null) Debug.LogError("No SpriteRenderer attached to object " + gameObject.name + ".");
    }

    public void HotspotActionComplete()
    {
        spriteRenderer.sprite = newSprite;
    }
}
