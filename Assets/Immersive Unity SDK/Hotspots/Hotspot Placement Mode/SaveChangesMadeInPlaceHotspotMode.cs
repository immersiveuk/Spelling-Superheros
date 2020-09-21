using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

/// <summary>
/// In order to save changes made to hotspot place while in play mode the hotspot controller saved as a prefab, and passes to this class. Once the editor is no longer in a play mode, the old hotspot controller is replaced by an instantiation of the prefab.
/// </summary>
[ExecuteInEditMode]
public class SaveChangesMadeInPlaceHotspotMode : MonoBehaviour
{
    public static GameObject hotspots = null;
   
    // Update is called once per frame
    void Update()
    {
#if UNITY_EDITOR

        if (hotspots != null && !Application.isPlaying)
        {
            //var newHotspotController = PrefabUtility.InstantiatePrefab(hotspots, transform.parent) as GameObject;
            var newHotspotController = Instantiate(hotspots, transform.parent);
            newHotspotController.name = "Hotspot Controller";
            var index = transform.GetSiblingIndex();
            newHotspotController.transform.SetSiblingIndex(index);
            Selection.activeGameObject = newHotspotController;
            hotspots = null;
            DestroyImmediate(hotspots, true);
            DestroyImmediate(gameObject);
        }
#endif

    }

    /// <summary>
    /// Save changes made in hotspot mode so they persist
    /// </summary>
    public static void SaveChanges(GameObject hotspotsController)
    {
#if UNITY_EDITOR
        string path = "Assets/Immersive Unity SDK/Hotspots/Hotspot Placement Mode/SavedHotspot.prefab";
        var newPrefab = PrefabUtility.SaveAsPrefabAsset(hotspotsController, path);
        global::SaveChangesMadeInPlaceHotspotMode.hotspots = newPrefab;
#endif
    }
}
