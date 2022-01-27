using Com.Immersive.Cameras;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Com.Immersive.Hotspots
{
    public static class EditorHotspotCreator
    {
        private const string imageHotspotName = "Image Hotspot Prefab";
        private const string invisibleHotspotName = "Invisible Hotspot Prefab";
        private const string textHotspotName = "Text Hotspot Prefab";
        private const string regionHotspotName = "Region Hotspot Prefab";

        private const string multiHotspotName = "Multi-Hotspot (Image)";

        public static void CreateImageHotspot(Transform parent) => CreateHotspot(imageHotspotName, "New Hotspot (Image)", parent);
        public static void CreateInvisibleHotspot(Transform parent) => CreateHotspot(invisibleHotspotName, "New Hotspot (Invisible)", parent);
        public static void CreateTextHotspot(Transform parent) => CreateHotspot(textHotspotName, "New Hotspot (Text)", parent);
        public static void CreateRegionHotspot(Transform parent) => CreateHotspot(regionHotspotName, "New Hotspot (Region)", parent, SetupRegionHotspot);

        public static void CreateMultiHotspot(Transform parent) => CreateHotspot(multiHotspotName, "New Multi Hotspot", parent);
        
        public static void CreateBatch(Transform parent)
        {
            HotspotBatch batch = new GameObject("Batch").AddComponent<HotspotBatch>();
            batch.transform.SetParent(parent);
            Undo.RegisterCreatedObjectUndo(batch.gameObject, "Undo Create Batch");
            Selection.activeTransform = batch.transform;
        }

        public static void CreateHotspotButtonsGUI(Transform parent)
        {
            EditorGUILayout.BeginVertical(GUI.skin.box);
            if (GUILayout.Button("New Image Hotspot"))
                CreateImageHotspot(parent);
            if (GUILayout.Button("New Invisible Hotspot"))
                CreateInvisibleHotspot(parent);
            if (GUILayout.Button("New Text Hotspot"))
                CreateTextHotspot(parent);
            if (GUILayout.Button("New Region Hotspot"))
                CreateRegionHotspot(parent);
            EditorGUILayout.EndVertical();
        }

        public static void CreateHotspotGroupButtonsGUI(Transform parent)
        {
            EditorGUILayout.BeginVertical(GUI.skin.box);
            if (GUILayout.Button("New Batch"))
                CreateBatch(parent);
            if (GUILayout.Button("New Multi-Hotspot"))
                CreateMultiHotspot(parent);
            EditorGUILayout.EndVertical();
        }

        private static void CreateHotspot(string hotspotPrefabFileName, string newObjectName, Transform parent, Action<GameObject> setupAction = null)
        {
            GameObject hotspotPrefab = GetPrefab(hotspotPrefabFileName);
            var hotspot = PrefabUtility.InstantiatePrefab(hotspotPrefab) as GameObject;
            hotspot.name = newObjectName;
            hotspot.transform.SetParent(parent);
            setupAction?.Invoke(hotspot);

            hotspot.transform.position = AbstractImmersiveCamera.CurrentImmersiveCamera.mainCamera.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 1));
            Undo.RegisterCreatedObjectUndo(hotspot, "Undo Create " + newObjectName);
            Selection.activeTransform = hotspot.transform;
        }

        private static void SetupRegionHotspot(GameObject hotspotObject) => hotspotObject.GetComponent<RegionHotspot>().Init();

        private static GameObject GetPrefab(string fileName)
        {
            string path = GetAssetPath(fileName);
            if (path == null)
            {
                Debug.LogError($"Prefab {fileName} cannot be found");
                return null;
            }
            return AssetDatabase.LoadAssetAtPath<GameObject>(path);
        }

        private static string GetAssetPath(string fileName)
        {
            var guids = AssetDatabase.FindAssets(fileName);
            if (guids.Length == 1)
                return AssetDatabase.GUIDToAssetPath(guids[0]);
            return null;
        }
    }

}