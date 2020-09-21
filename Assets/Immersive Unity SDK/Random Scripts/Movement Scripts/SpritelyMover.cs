using Com.Immersive.Cameras;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class SpritelyMover : MonoBehaviour
{

    public Vector3[] positions = new Vector3[2];

    //Variables
    public float movementDurationMax = 5;
    public float movementDurationMin = 5;
    public float staticDuractionMax = 0;
    public float staticDuractionMin = 0;
    public int wavesPerPeriod = 3;
    public float waveAmplitude = 0.1f;


    //Current State
    private float totalTimeRemaining = 0;
    private float currentMovementDuration;
    //private float staticTimeRemaining = 0;
    private enum MoveDirection { Backwards, Forwards };
    //private MoveDirection moveDirection = MoveDirection.Forwards;

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
        if (moveOnStart) StartMovement();
    }




    // Update is called once per frame
    void Update()
    {
        if (totalTimeRemaining > 0) totalTimeRemaining -= Time.deltaTime; 

        if (segmentTimeRemaining <= 0) return;

        segmentTimeRemaining -= Time.deltaTime;
        
        if (segmentTimeRemaining <= 0)
        {
            currentTargetIndex++;
            if (currentTargetIndex > positions.Length)
            {
                currentTargetIndex = 0;
                if (loop)
                {
                    StartMovement();
                }
            }
            else StartSegment();
        }
        else
        {
            MoveThroughtSegment();
            if (faceDirectionOfTravel) FaceDirectionOfTravel();
        }

        
    }

    public bool moveOnStart = true;
    public bool loop = true;
    public bool faceDirectionOfTravel = true;
    public bool waveyMovement = false;

    private int currentTargetIndex = 0;
    private float totalDistance;
    private float segmentDuration;
    private float segmentTimeRemaining;
    private Vector3 segmentStart;
    private Vector3 segmentTarget;

    private void StartMovement()
    {
        startPos = transform.position;
        totalDistance = CalculateTotalDistance();
        currentMovementDuration = Random.Range(movementDurationMin, movementDurationMax);
        totalTimeRemaining = currentMovementDuration;
        StartSegment();
    }

    private void StartSegment()
    {

        if (currentTargetIndex >= positions.Length) segmentTarget = startPos;
        else segmentTarget = positions[currentTargetIndex];

        if (currentTargetIndex == 0) segmentStart = startPos;
        else segmentStart = positions[currentTargetIndex - 1];

        var segmentLength = Vector3.Distance(segmentStart, segmentTarget);
        segmentDuration = currentMovementDuration * (segmentLength / totalDistance);
        segmentTimeRemaining = segmentDuration;
        print(segmentTimeRemaining);
    }

 

    private void MoveThroughtSegment()
    {
        var lerpValue = 1 - (segmentTimeRemaining / segmentDuration);
        var position = Vector3.Lerp(segmentStart, segmentTarget, lerpValue);

        if (waveyMovement)
        {
            var wave = CalculateWave(totalTimeRemaining / currentMovementDuration, lerpValue);
            position += wave;
        }
        transform.position = position;
    }

    private Vector3 lastFramePosition;
    private void FaceDirectionOfTravel()
    {
        var direction = transform.position - lastFramePosition;
        if (direction != Vector3.zero)
        {
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }

        lastFramePosition = transform.position;
    }



    private float CalculateTotalDistance()
    {
        float distance = 0;

        distance += Vector3.Distance(startPos, positions[0]);
        for (int i = 0; i < positions.Length - 1; i++)
        {
            distance += Vector3.Distance(positions[i], positions[i + 1]);
        }
        distance += Vector3.Distance(positions[positions.Length - 1], startPos);

        return distance;
    }


    private Vector3 CalculateWave(float lerpValue, float segmentLerpValue)
    {
        var nextSegmentIndex = currentTargetIndex + 1;
        Vector3 nextSegmentTarget;
        Vector3 nextSegmentStart;

        if (nextSegmentIndex >= positions.Length) nextSegmentTarget = startPos;
        else nextSegmentTarget = positions[nextSegmentIndex];

        if (nextSegmentIndex == 0) nextSegmentStart = startPos;
        else nextSegmentStart = positions[nextSegmentIndex - 1];



        print("Lerp: "+lerpValue);

        Vector3 directionOfTravelCurrent = segmentTarget - segmentStart;
        Vector3 directionOfTravelNextSegment = nextSegmentTarget - nextSegmentStart;

        Vector3 directionOfTravel = segmentTarget - segmentStart;

        if (segmentLerpValue > 0.9f)
        {
            var dirLerp = (segmentLerpValue - 0.9f)/0.1f;
            directionOfTravel = Vector3.Lerp(directionOfTravelCurrent, directionOfTravelNextSegment, dirLerp);
        }
        //else if (segmentLerpValue < 0.1f)
        //{
        //    var dirLerp = 
        //}
        //else
        //{
        //    directionOfTravel
        //}
        Vector3 directionOfCamera = AbstractImmersiveCamera.CurrentImmersiveCamera.transform.forward;

        Vector3 wave = Vector3.Cross(directionOfTravel, directionOfCamera).normalized;
        wave *= Mathf.Sin(lerpValue * wavesPerPeriod * Mathf.PI);
        var a = lerpValue * wavesPerPeriod * Mathf.PI * Mathf.Rad2Deg;

        wave *= waveAmplitude;
        return wave;
    }



    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;

        Gizmos.DrawLine(transform.position, positions[0]);
        for (int i = 0; i < positions.Length - 1; i++)
        {
            Gizmos.DrawLine(positions[i], positions[i + 1]);
        }
        Gizmos.DrawLine(positions[positions.Length - 1], transform.position);
    }

}

//==============================================================
// CUSTOM EDITOR
//==============================================================

#if UNITY_EDITOR

[CustomEditor(typeof(SpritelyMover))]
public class SpritelyMoverEditor : Editor
{

    private SpritelyMover spritelyMover;

    private void OnEnable()
    {
        spritelyMover = (SpritelyMover)target;
    }

    private void OnSceneGUI()
    {
        for (int i = 0; i < spritelyMover.positions.Length; i++)
        {
            EditorGUI.BeginChangeCheck();
            Vector3 newPosition = Handles.PositionHandle(spritelyMover.positions[i], Quaternion.identity);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(spritelyMover, "Change Target Position");
                spritelyMover.positions[i] = newPosition;
            }
        }
    }


}

#endif
