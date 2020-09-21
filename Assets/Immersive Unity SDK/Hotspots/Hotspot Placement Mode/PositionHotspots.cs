/* Copyright (C) Immersive Interactive, Ltd - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Luke Bissell <luke@immersive.co.uk>, Aug 2019
 */

using Com.Immersive.Cameras;
using Com.Immersive.Hotspots;
using UnityEditor;
using UnityEngine;

/// <summary>
/// When is position hotspot mode is true, this script will allow you to control and save changes to the hotspot.
/// </summary>
public class PositionHotspots : MonoBehaviour
{

    public static PositionHotspots CurrentPositionHotspot;

    public GameObject hotspotPositionCanvasPrefab;

    [System.NonSerialized]
    public float distanceFromCamera = 1f;

    //The currently selected 
    private Transform selectedHotspot;

    // Start is called before the first frame update
    void Start()
    {
        CurrentPositionHotspot = this;
        AbstractImmersiveCamera.PlaceHotspotsWallTouchEvent.AddListener(ControlHotspot);

#if UNITY_EDITOR
        if (EditorPrefs.HasKey("PlaceHotspotMode") && EditorPrefs.GetBool("PlaceHotspotMode") && hotspotPositionCanvasPrefab != null)
        {
            Instantiate(hotspotPositionCanvasPrefab);
        }
#endif
    }

    private void ControlHotspot(Vector2 position, int cameraIndex, TouchPhase phase, int touchIndex)
    {
        var cam = AbstractImmersiveCamera.CurrentImmersiveCamera.cameras[cameraIndex];

        //Create or select hotspot
        if (phase == TouchPhase.Began)
        {
            Ray ray = cam.ScreenPointToRay(position);
            RaycastHit hit;

            //Check if there is a hotspot where the user clicks. If so create new hotspot.
            if (Physics.Raycast(ray, out hit))
            {
                var hotspot = hit.collider.GetComponent<IHotspot>();
                if (hotspot != null)
                {
                    selectedHotspot = hit.collider.transform;
                }
            }
            else
            {
                selectedHotspot = null;
            }
        }

        //Move Hotspot
        if (phase == TouchPhase.Moved)
        {
            if (selectedHotspot)
            {
                Ray ray = cam.ScreenPointToRay(position);
                selectedHotspot.position = ray.GetPoint(distanceFromCamera);

                selectedHotspot.transform.position = cam.ScreenToWorldPoint(new Vector3(position.x, position.y, distanceFromCamera));

            }
        }

        if (phase == TouchPhase.Ended || phase == TouchPhase.Canceled)
        {
            selectedHotspot = null;
        }
    }

    public void SaveChanges()
    {
#if UNITY_EDITOR
        SaveChangesMadeInPlaceHotspotMode.SaveChanges(gameObject);
        EditorApplication.isPlaying = false;
#endif
    }

    public void DiscardChanges()
    {
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#endif
    }
}


#if UNITY_EDITOR

//==============================================================
// CUSTOM EDITOR
//==============================================================

[CustomEditor(typeof(PositionHotspots))]
public class PositionHotspotsEditor : Editor
{
    PositionHotspots positionHotspots;


    private void OnEnable()
    {
        positionHotspots = (PositionHotspots)target;
    }

    public override void OnInspectorGUI()
    {

        EditorGUILayout.Space();

        // Hotspot positioning mode
        if (!EditorApplication.isPlaying)
        {
            if (GUILayout.Button("Position Hotspots"))
            {
                EditorPrefs.SetBool("PlaceHotspotMode", true);
                UnityEditor.EditorApplication.isPlaying = true;
            }
        }

        if (EditorApplication.isPlaying)
        {
            if (EditorPrefs.HasKey("PlaceHotspotMode") && EditorPrefs.GetBool("PlaceHotspotMode"))
            {
                if (GUILayout.Button("Save Changes"))
                {
                    positionHotspots.SaveChanges();
                }
                if (GUILayout.Button("Discard Changes"))
                {
                    positionHotspots.DiscardChanges();
                }
            }
        }

    }
}

#endif