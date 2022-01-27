/* Copyright (C) Immersive Interactive, Ltd - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Luke Bissell <luke@immersive.co.uk>, Dec 2019
 */

using Com.Immersive.Cameras;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Script which moves an object from its start position to a new position.
/// </summary>
public class MoveToPosition : MonoBehaviour, IInteractableObject
{

    //PUBLIC VARIABLES
    public enum MoveTrigger { MoveAtStart, MoveOnTouched, APIOnly };
    public MoveTrigger moveTrigger = MoveTrigger.APIOnly;
    private Vector3 startPosition;
    public Vector3 targetPosition = new Vector3(1, 0, 0);
    public float moveDuration = 3;

    [SerializeField] bool loop = false;

    //PRIVATE VARIABLES
    private bool canBeMoved = true;
    private float timeRemaining = 0;

    //------------------------------
    //PUBLIC API
    //------------------------------

    /// <summary>
    /// Moves the position of the object from current position to target.
    /// </summary>
    public void Move()
    {
        if (!canBeMoved) return;
        startPosition = transform.position;
        timeRemaining = moveDuration;
        canBeMoved = false;
    }

    /// <summary>
    /// Change the target position.
    /// </summary>
    public void ChangeTarget(Vector3 newTarget)
    {
        targetPosition = newTarget;
        canBeMoved = true;
    }

    //------------------------------
    // Moving object
    //------------------------------

    // Update is called once per frame
    void Update()
    {
        if (timeRemaining > 0)
        {
            timeRemaining -= Time.deltaTime;

            if (timeRemaining <= 0)
            {
                transform.position = targetPosition;
                
                if (loop)
                {
                    timeRemaining = moveDuration;
                    canBeMoved = false;
                }
                return;
            }

            else
            {
                var lerpValue = 1 - (timeRemaining / moveDuration);
                transform.position = Vector3.Lerp(startPosition, targetPosition, lerpValue);
            }
        }
    }

    //------------------------------
    // Triggering Moves
    //------------------------------


    // Start is called before the first frame update
    void Start()
    {
        if (moveTrigger == MoveTrigger.MoveAtStart) Move();
    }

    public void OnRelease()
    {
        if (moveTrigger == MoveTrigger.MoveOnTouched)
        {
            Move();
        }
    }

    public void OnPress() { }
    public void OnTouchEnter() { }
    public void OnTouchExit() { }
}



//==============================================================
// CUSTOM EDITOR
//==============================================================

#if UNITY_EDITOR

[CustomEditor(typeof(MoveToPosition))]
public class MoveToPositionEditor : Editor
{

    private MoveToPosition moveToPosition;

    private void OnEnable()
    {
        moveToPosition = (MoveToPosition)target;
    }

    private void OnSceneGUI()
    {
        EditorGUI.BeginChangeCheck();
        Vector3 newTargetPosition = Handles.PositionHandle(moveToPosition.targetPosition, Quaternion.identity);
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(moveToPosition, "Change Target Position");
            moveToPosition.targetPosition = newTargetPosition;
        }
    }
}

#endif
