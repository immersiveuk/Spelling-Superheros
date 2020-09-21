using Com.Immersive.Cameras;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockBackAndForthOnTouch : MonoBehaviour, IInteractableObject
{

    [Range(-30, 30)]
    [SerializeField] float startAngle = 10;
    [Range(0,1)][SerializeField] float gravity = 0.1f;
    [Range(0,1)][SerializeField] float damping = 0.1f;

    private float angularVelocity = 0;

    public void OnRelease()
    {
        //Set angle to start angle
        var eulers = transform.localEulerAngles;
        eulers.z = startAngle;
        transform.localEulerAngles = eulers;
    }

    // Update is called once per frame
    void Update()
    {
        var eulers = transform.localEulerAngles;
        var angle = eulers.z;
        if (angle > 180) angle =  angle - 360;

        //Calculate new angula
        angularVelocity += -angle * gravity;
        angularVelocity *= 1 - damping;

        angle += angularVelocity;
        eulers.z = angle;
        transform.localEulerAngles = eulers;
    }

    public void OnPress() { }
    public void OnTouchEnter() { }
    public void OnTouchExit() { }
}
