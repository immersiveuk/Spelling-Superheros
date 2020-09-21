/* Copyright (C) Immersive Interactive, Ltd - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Luke Bissell <luke@immersive.co.uk>, July 2019
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

#if UNITY_EDITOR

[ExecuteInEditMode]
public class EditorNavigation : MonoBehaviour
{
    private int childCount = 0;

    private void Update()
    {

        if (transform.childCount > childCount)
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                EditorNavigation nav = transform.GetChild(i).GetComponent<EditorNavigation>();
                if (nav == null)
                {
                    nav = transform.GetChild(i).gameObject.AddComponent<EditorNavigation>();
                    while(UnityEditorInternal.ComponentUtility.MoveComponentUp(nav)) { }
                }
            }
        }

        if (transform.childCount != childCount)
        {
            childCount = transform.childCount;
            return;
        }

    }
}


[CustomEditor(typeof(EditorNavigation))]
public class NavigationEditor: Editor
{

    private Transform transform;
    private void OnEnable()
    {
        transform = ((EditorNavigation)target).transform;
    }

    public override void OnInspectorGUI()
    {

        EditorGUILayout.Space();

        //Go to parent button
        GUI.enabled = transform.parent != null;
        if (GUILayout.Button("Go To Parent"))
        {
            Selection.activeGameObject = transform.parent.gameObject;
        }

        //Go to child button.
        GUI.enabled = transform.childCount > 0;
        if (GUILayout.Button("Go To Child"))
        {
            Selection.activeGameObject = transform.GetChild(0).gameObject;
        }


        var siblingIndex = transform.GetSiblingIndex();
        var siblingCount = transform.parent != null ? transform.parent.childCount : 0;

        //Go to sibling above
        GUI.enabled = siblingIndex > 0;
        if (GUILayout.Button("Go Up"))
        {
            Selection.activeGameObject = transform.parent.GetChild(siblingIndex - 1).gameObject;
        }

        // Go to sibling below
        GUI.enabled = siblingIndex < siblingCount - 1;
        if (GUILayout.Button("Go Down"))
        {
            Selection.activeGameObject = transform.parent.GetChild(siblingIndex + 1).gameObject;
        }

        GUI.enabled = true;
    }
}

#endif