using Com.Immersive.Cameras;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Moves a sprite back and forward between two points.
/// The sprite will flip horizontally.
/// </summary>
public class MoveSpriteBackAndForth : MonoBehaviour
{
    //Variables
    public Transform target;
    public float movementDurationMax = 5;
    public float movementDurationMin = 5;
    public float staticDuractionMax = 0;
    public float staticDuractionMin = 0;
    public int wavesPerPeriod = 3;
    public float waveAmplitude = 0.1f;


    //Current State
    private float movementTimeRemaining = 0;
    private float currentMovementDuration;
    private float staticTimeRemaining = 0;
    private enum MoveDirection { Backwards, Forwards };
    private MoveDirection moveDirection = MoveDirection.Forwards;

    private Vector3 lastPos;

    //References
    private SpriteRenderer spriteRend;

    //Initial Info
    private bool xFlippedAtStart;
    private Vector3 startPos;
    private Vector3 targetPos;

    // Start is called before the first frame update
    void Start()
    {
        //Get positions
        startPos = transform.position;
        targetPos = target.position;

        //Calculate initial movement times
        currentMovementDuration = Random.Range(movementDurationMin, movementDurationMax);
        movementTimeRemaining = currentMovementDuration;
       
        //Get Sprite Info
        spriteRend = GetComponent<SpriteRenderer>();
        xFlippedAtStart = spriteRend.flipX;
    }


    // Update is called once per frame
    void Update()
    {
        //If not moving
        if (staticTimeRemaining > 0)
        {
            staticTimeRemaining -= Time.deltaTime;
            return;
        }

        movementTimeRemaining -= Time.deltaTime;

        //Movement complete
        if (movementTimeRemaining < 0)
        {
            //Reset durations
            staticTimeRemaining = Random.Range(staticDuractionMin, staticDuractionMax);
            currentMovementDuration = Random.Range(movementDurationMin, movementDurationMax);
            movementTimeRemaining = currentMovementDuration;

            //Movement direction
            if (moveDirection == MoveDirection.Forwards) moveDirection = MoveDirection.Backwards;
            else moveDirection = MoveDirection.Forwards;
            spriteRend.flipX = moveDirection == MoveDirection.Forwards ^ xFlippedAtStart ? false : true;
        }

        //Move
        float lerpValue = movementTimeRemaining / currentMovementDuration;
        if (moveDirection == MoveDirection.Forwards) lerpValue = 1 - lerpValue;

        var position = Vector3.Lerp(startPos, targetPos, lerpValue);

        var wave = CalculateWave(lerpValue);
        position += wave;
        transform.position = position;


        //Set Angle
        var dir = position - lastPos;

        var x = Quaternion.LookRotation(dir).eulerAngles.x;
        if (moveDirection == MoveDirection.Forwards)
        {
            x *= -1;
        }
        transform.eulerAngles = new Vector3(0, 0, x);
        lastPos = position;
    }

    private Vector3 CalculateWave(float lerpValue)
    {
        Vector3 directionOfTravel = targetPos - startPos;
        Vector3 directionOfCamera = AbstractImmersiveCamera.CurrentImmersiveCamera.transform.forward;

        Vector3 wave = Vector3.Cross(directionOfTravel, directionOfCamera).normalized;
        wave *= Mathf.Sin(lerpValue * wavesPerPeriod * Mathf.PI);
        var a = lerpValue * wavesPerPeriod * Mathf.PI * Mathf.Rad2Deg;

        wave *= waveAmplitude;
        return wave;
    }

}
