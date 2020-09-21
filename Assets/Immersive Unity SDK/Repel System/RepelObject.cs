using Com.Immersive.Cameras;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Controls an object as part of the repel system.
/// </summary>
public class RepelObject : MonoBehaviour
{
    protected enum MovementPhase { Random, Repelling }
    protected MovementPhase phase;

    private float distanceFromCamera = 1;
    private float repelDistance = 1;
    private float repelSpeed = 1;
    private float defaultSpeed = 1;
    private RepelManager.WallOrFloor wallOrFloor;

    private Vector3 closestTouchPosition;
    private float distanceToClosestTouch = -1;
    private float fixedAxisPosition;    //This is used to constrain to either the y or z axis
    private bool HasTouch { get => distanceToClosestTouch > 0; }

    private Vector3 moveDirection;

    private Transform camTransform;

    public void Init(float repelSpeed, float defaultSpeed, float repelDistance, float distanceFromCamera, RepelManager.WallOrFloor wallOrFloor)
    {
        SetVariables(repelSpeed, defaultSpeed, repelDistance);
        this.distanceFromCamera = distanceFromCamera;
        this.wallOrFloor = wallOrFloor;

        if (wallOrFloor == RepelManager.WallOrFloor.Floor)
            fixedAxisPosition = transform.position.y;
        else
            fixedAxisPosition = transform.position.z;
    }

    public void SetVariables(float repelSpeed, float defaultSpeed, float repelDistance)
    {
        this.repelSpeed = repelSpeed;
        this.defaultSpeed = defaultSpeed;
        this.repelDistance = repelDistance;
    }

    // Start is called before the first frame update
    void Start()
    {
        camTransform = AbstractImmersiveCamera.CurrentImmersiveCamera.transform;
        AbstractImmersiveCamera.AnyWallTouched.AddListener(OnSurfaceTouched);
        NewMoveDirection();
    }


    private void OnSurfaceTouched(Vector2 screenPosition, int cameraIndex, TouchPhase touchPhase, int touchIndex)
    {
        //Floor and camera is not floor.
        if (wallOrFloor == RepelManager.WallOrFloor.Floor && AbstractImmersiveCamera.CurrentImmersiveCamera.GetSurfacePositionFromIndex(cameraIndex) != SurfacePosition.Floor)
            return;

        //Wall and camera is floor
        if (wallOrFloor == RepelManager.WallOrFloor.Wall && !SurfacePosition.AllWalls.HasFlag(AbstractImmersiveCamera.CurrentImmersiveCamera.GetSurfacePositionFromIndex(cameraIndex)))
            return;

        if (touchPhase == TouchPhase.Began || touchPhase == TouchPhase.Moved)
        {
            Camera cam = AbstractImmersiveCamera.CurrentImmersiveCamera.cameras[cameraIndex];
            var touchPosition = cam.ScreenToWorldPoint(new Vector3(screenPosition.x, screenPosition.y, distanceFromCamera));

            var distanceFromTouch = Vector3.Distance(touchPosition, transform.position);

            //First Touch
            if (distanceToClosestTouch == -1)
            {
                distanceToClosestTouch = distanceFromTouch;
                closestTouchPosition = touchPosition;
            }
            //Check if closest touch
            else if (distanceFromTouch < distanceToClosestTouch)
            {
                distanceToClosestTouch = distanceFromTouch;
                closestTouchPosition = touchPosition;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {    
        CalculatePhase();

        if (phase == MovementPhase.Repelling)
        {
            MoveObjectRepel();
        }
        else
        {
            MoveObjectRandom();
        }

        var pos = transform.position;
        if (wallOrFloor == RepelManager.WallOrFloor.Floor)
            pos.y = fixedAxisPosition;
        else
            pos.z = fixedAxisPosition;
           
        transform.position = pos;

        distanceToClosestTouch = -1;
    }

    /// <summary>
    /// Calculates whether the object is close enough to a touch to repel.
    /// </summary>
    private void CalculatePhase()
    {
        if (distanceToClosestTouch > 0 && distanceToClosestTouch < repelDistance)
            phase = MovementPhase.Repelling;
        else
            phase = MovementPhase.Random;
    }

    /// <summary>
    /// Moves an object when in Repel MovementPhase
    /// </summary>
    private void MoveObjectRepel()
    {
        moveDirection = (transform.position - closestTouchPosition).normalized;
        if (wallOrFloor == RepelManager.WallOrFloor.Floor)
            moveDirection.y = 0;

        var prevPosition = transform.position;
        transform.Translate(moveDirection * repelSpeed * Time.deltaTime, Space.World);

        var cam = AbstractImmersiveCamera.CurrentImmersiveCamera.FindCameraLookingAtPosition(transform.position);
        if (cam == null)
        {
            transform.position = prevPosition;
            return;
        }

        SetRotation(cam);
    }

    /// <summary>
    /// Moves an object when in Random MovementPhase.
    /// </summary>
    private void MoveObjectRandom()
    {
        var prevPosition = transform.position;

        transform.Translate(moveDirection * defaultSpeed * Time.deltaTime, Space.World);

        var cam = AbstractImmersiveCamera.CurrentImmersiveCamera.FindCameraLookingAtPosition(transform.position);

        //If going off screen change direction.
        if (cam == null)
        {
            transform.position = prevPosition;
            moveDirection *= -1;
            transform.Translate(moveDirection * defaultSpeed * Time.deltaTime, Space.World);
            NewMoveDirection();
            return;
        }

        SetRotation(cam);
    }

    /// <summary>
    /// Sets the rotation to face direction of travel.
    /// </summary>
    /// <param name="cam"></param>
    private void SetRotation(Camera cam)
    {
        var targetForward = moveDirection.normalized;
        var forward = Vector3.Lerp(transform.forward, targetForward, 0.3f);
        var up = wallOrFloor == RepelManager.WallOrFloor.Floor ? camTransform.up : camTransform.forward;

        transform.rotation = Quaternion.LookRotation(forward, up);
    }

    private void NewMoveDirection()
    {
        switch (wallOrFloor)
        {
            case RepelManager.WallOrFloor.Wall:
                moveDirection = Random.insideUnitCircle.normalized;
                break;

            case RepelManager.WallOrFloor.Floor:
                var dir = Random.insideUnitCircle.normalized;
                moveDirection = new Vector3(dir.x, 0, dir.y);
                break;
        }
    }
}
