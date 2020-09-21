/* Copyright (C) Immersive Interactive, Ltd - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Luke Bissell <luke@immersive.co.uk>, July 2019
 */

using Com.Immersive.Cameras;
using UnityEditor;
using UnityEngine;

namespace Com.Immersive.Hotspots
{

    /// <summary>
    /// Contains a batch of hotspots.
    /// When hotspots are displayed in ordered mode.
    /// </summary>
    [ExecuteInEditMode]
    public class HotspotBatch : MonoBehaviour
    {
        public GameObject baseHotspotPrefab;
        public GameObject imageHotspotPrefab;
        public GameObject invisibleHotspotPrefab;
        public GameObject multiHotspotPrefab;

        public void AddBaseHotspot()
        {
#if UNITY_EDITOR
            var hotspot = PrefabUtility.InstantiatePrefab(baseHotspotPrefab) as GameObject;
            hotspot.name = "New Hotspot (Base)";
            hotspot.transform.SetParent(transform);

            //Position Hotspot
            if (AbstractImmersiveCamera.CurrentImmersiveCamera is ImmersiveCamera3D)
            {
                hotspot.transform.localPosition = new Vector3(0, 1, 1);
            }
            else if (AbstractImmersiveCamera.CurrentImmersiveCamera is ImmersiveCamera2D)
            {
                hotspot.transform.localPosition = new Vector3(0, 0, -1);
            }

            Selection.activeGameObject = hotspot;
#endif
        }

        public void AddImageHotspot()
        {
#if UNITY_EDITOR
            var hotspot = PrefabUtility.InstantiatePrefab(imageHotspotPrefab) as GameObject;
            hotspot.name = "New Hotspot (Image)";
            hotspot.transform.SetParent(transform);

            //Position Hotspot
            if (AbstractImmersiveCamera.CurrentImmersiveCamera is ImmersiveCamera3D)
            {
                hotspot.transform.localPosition = new Vector3(0, 1, 1);
            }
            else if (AbstractImmersiveCamera.CurrentImmersiveCamera is ImmersiveCamera2D)
            {
                hotspot.transform.localPosition = new Vector3(0, 0, -1);
            }

            Selection.activeGameObject = hotspot;
#endif
        }

        public void AddInvisibleHotspot()
        {
#if UNITY_EDITOR
            var hotspot = PrefabUtility.InstantiatePrefab(invisibleHotspotPrefab) as GameObject;
            hotspot.name = "New Hotspot (Invisible)";
            hotspot.transform.SetParent(transform);

            //Position Hotspot
            if (AbstractImmersiveCamera.CurrentImmersiveCamera is ImmersiveCamera3D)
            {
                hotspot.transform.localPosition = new Vector3(0, 1, 1);
            }
            else if (AbstractImmersiveCamera.CurrentImmersiveCamera is ImmersiveCamera2D)
            {
                hotspot.transform.localPosition = new Vector3(0, 0, -1);
            }

            Selection.activeGameObject = hotspot;
#endif
        }

        public void AddMultiHotspot()
        {
#if UNITY_EDITOR
            var hotspot = PrefabUtility.InstantiatePrefab(multiHotspotPrefab) as GameObject;
            hotspot.name = "New Multi-Hotspot (Image)";
            hotspot.transform.SetParent(transform);

            //Position Hotspot
            if (AbstractImmersiveCamera.CurrentImmersiveCamera is ImmersiveCamera3D)
            {
                hotspot.transform.localPosition = new Vector3(0, 1, 1);
            }
            else if (AbstractImmersiveCamera.CurrentImmersiveCamera is ImmersiveCamera2D)
            {
                hotspot.transform.localPosition = new Vector3(0, 0, -1);
            }

            Selection.activeGameObject = hotspot;
#endif
        }

    }

    //==============================================================
    // CUSTOM EDITOR
    //==============================================================

#if UNITY_EDITOR

    [CustomEditor(typeof(HotspotBatch))]
    public class HotspotBatchEditor : Editor
    {

        private HotspotBatch batch;
        private int currentTab = 0;

        //Hotspot Prefabs
        private SerializedProperty baseHotspotPrefab;
        private SerializedProperty imageHotspotPrefab;
        private SerializedProperty invisibleHotspotPrefab;
        private SerializedProperty multiHotspotPrefab;

        private void OnEnable()
        {
            batch = (HotspotBatch)target;

            baseHotspotPrefab = serializedObject.FindProperty("baseHotspotPrefab");
            imageHotspotPrefab = serializedObject.FindProperty("imageHotspotPrefab");
            invisibleHotspotPrefab = serializedObject.FindProperty("invisibleHotspotPrefab");
            multiHotspotPrefab = serializedObject.FindProperty("multiHotspotPrefab");
        }


        public override void OnInspectorGUI()
        {
            EditorGUILayout.Space();
           
            //Navigation UI.
            //OnInspectorGUINaviagtion();


            EditorGUILayout.Space();
            currentTab = GUILayout.Toolbar(currentTab, new string[] { "Create", "Hotspot Prefabs" });
            EditorGUILayout.Space();

            switch (currentTab)
            {
                case 0:
                    OnInspectorGUICreate();
                    break;
                case 1:
                    OnInspectorGUIPrefab();
                    break;
            }
        }

        private void OnInspectorGUICreate()
        {
            if (GUILayout.Button("New Basic Hotspot"))
            {
                batch.AddBaseHotspot();
            }

            if (GUILayout.Button("New Image Hotspot"))
            {
                batch.AddImageHotspot();
            }

            if (GUILayout.Button("New Invisible Hotspot"))
            {
                batch.AddInvisibleHotspot();
            }

            if (GUILayout.Button("New Multi-Hotspot"))
            {
                batch.AddMultiHotspot();
            }
        }

        private void OnInspectorGUIPrefab()
        {
            EditorGUILayout.PropertyField(baseHotspotPrefab, new GUIContent("Basic Hotspot"));
            EditorGUILayout.PropertyField(imageHotspotPrefab, new GUIContent("Image Hotspot"));
            EditorGUILayout.PropertyField(invisibleHotspotPrefab, new GUIContent("Invisible Hotspot"));
            EditorGUILayout.PropertyField(multiHotspotPrefab, new GUIContent("Multi-Hotspot"));

            serializedObject.ApplyModifiedProperties();

        }
    }
#endif

}
