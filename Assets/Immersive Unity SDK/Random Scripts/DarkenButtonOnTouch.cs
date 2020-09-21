using Com.Immersive.Cameras;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DarkenButtonOnTouch : MonoBehaviour, IInteractableObject
{
    public bool isInteractive = true;

    private SpriteRenderer spriteRend;
    private Color initialColor;

    private bool isFirstHeldFrame = true;

    private void Start()
    {
        //Get Sprite Info
        spriteRend = GetComponent<SpriteRenderer>();
        initialColor = spriteRend.color;
    }

    public void OnPress() { }

    public void OnRelease()
    {
        if (!isInteractive) return;
        if (spriteRend != null) spriteRend.color = initialColor;
        isFirstHeldFrame = true;
    }

    public void OnTouchEnter()
    {
        if (!isInteractive) return;

        if (isFirstHeldFrame)
        {
            spriteRend.color = new Color(initialColor.r * 0.6f, initialColor.g * 0.6f, initialColor.b * 0.6f);
            isFirstHeldFrame = false;
        }
    }

    public void OnTouchExit()
    {
        if (!isInteractive) return;

        if (spriteRend != null) spriteRend.color = initialColor;
        isFirstHeldFrame = true;
    }
}
