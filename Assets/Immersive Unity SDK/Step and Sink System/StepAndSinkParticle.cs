using Com.Immersive.Cameras;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StepAndSinkParticle : MonoBehaviour, IInteractableObject
{
    private StepAndSinkFloorManager manager;

    private bool melting = false;
    private bool inBounds = true;

    private SpriteRenderer spriteRenderer;

    public void Init(StepAndSinkFloorManager manager)
    {
        this.manager = manager;

        transform.parent = manager.transform;

        //Create Sprite Renderer
        if (!spriteRenderer) spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
        else spriteRenderer = GetComponent<SpriteRenderer>();

        //Set initial sprite
        spriteRenderer.sprite = manager.GetNewSprite();

        //Add box collider
        var collider = gameObject.AddComponent<BoxCollider>();
        collider.size = spriteRenderer.sprite.bounds.size;

        //Set initial scale
        var scale = manager.GetNewScale();
        transform.localScale = new Vector3(scale, scale, scale);

        //Set initial position
        transform.localPosition = manager.GetRandomPositionInView();

        transform.localRotation = Quaternion.identity;
    }


    public void OnRelease()
    {
        if (!melting)
        {
            StartCoroutine(MeltSelf());
        }
    }

    private IEnumerator MeltSelf()
    {
        melting = true;

        var duration = manager.SinkDuration;
        var startScale = transform.localScale;
        var targetScale = Vector3.zero;

        var startTime = Time.time;
        var endTime = Time.time + duration;

        while (Time.time < endTime)
        {
            var lerpValue = Mathf.InverseLerp(startTime, endTime, Time.time);
            var scale = Vector3.Lerp(startScale, targetScale, lerpValue);
            transform.localScale = scale;
            
            spriteRenderer.color = new Color(1,1,1, 1 - lerpValue);

            yield return new WaitForEndOfFrame();
        }

        ResetObject();
    }

    private void ResetObject()
    {
        melting = false;
        inBounds = false;
    
        //Update sprite        
        spriteRenderer.sprite = manager.GetNewSprite();

        //Update scale
        var scale = manager.GetNewScale();
        transform.localScale = new Vector3(scale, scale, scale);

        //Update position
        var position = manager.GetNewStartPosition(spriteRenderer.bounds.size);
        transform.localPosition = position;

        //Reset color
        spriteRenderer.color = Color.white;
    }


    // Update is called once per frame
    void Update()
    {
        //Move
        var velocity = manager.Velocity;
        transform.Translate(new Vector3(velocity.x * Time.deltaTime, velocity.y * Time.deltaTime));

        //Check if reapeared on screen.
        if (!inBounds && manager.CheckIfObjectInBounds(spriteRenderer.bounds.size, transform.localPosition))
        {
            inBounds = true;
        } 
        //Check if gone off screen.
        else if (!melting && !manager.CheckIfObjectInBounds(spriteRenderer.bounds.size, transform.localPosition))
        {
            ResetObject();
        }
    }

    public void OnPress(){}
    public void OnTouchEnter(){}
    public void OnTouchExit(){}
}
