using Com.Immersive.Cameras;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

//NEED TO ADD COLLIDER AND DO STUFF FROM IMAGE HOTSPOT IN SCRIPT

namespace Com.Immersive.Hotspots
{
    [ExecuteInEditMode]
    public class MultiHotspot : MonoBehaviour, IHotspot
    {
        public GameObject baseHotspotPrefab;
        public GameObject imageHotspotPrefab;
        public GameObject invisibleHotspotPrefab;

        private bool firstOpen = true;
        public bool IsInteractable { get; private set; } = true;

        [SerializeField] private OnClickAction _clickAction = OnClickAction.Hide;
        public OnClickAction ClickAction { get { return _clickAction; } set { _clickAction = value; } }

        //------------------------
        //SETUP
        //------------------------

        private void Start()
        {
#if UNITY_EDITOR
            if (EditorApplication.isPlaying)
            {
                //Disable Children
                DisableChildHotpsots();
                FindPresentingHotspotController();
            }
#else 
                //Disable Children
                DisableChildHotpsots();
                FindPresentingHotspotController();

#endif
        }

        private void Update()
        {
#if UNITY_EDITOR
            if (!EditorApplication.isPlaying)
            {
                if (CheckIfMultiHotspotOrChildHotspotSelected())
                {
                    EnableChildHotspots();
                }
                else
                {
                    DisableChildHotpsots();
                }
            }
#endif
        }

#if UNITY_EDITOR
        private bool CheckIfMultiHotspotOrChildHotspotSelected()
        {
            if (Selection.activeObject == gameObject) return true;
            for (int i = 0; i < transform.childCount; i++)
            {
                if (Selection.activeGameObject == transform.GetChild(i).gameObject)
                {
                    return true;
                }
            }
            return false;
        }
#endif

        private HotspotController controller;
        private void FindPresentingHotspotController()
        {
            var controllerTransform = transform.parent;
            if (controllerTransform.GetComponent<HotspotBatch>())
            {
                controllerTransform = controllerTransform.parent;
            }
            controller = controllerTransform.GetComponent<HotspotController>();
        }

        //------------------------
        // OPEN AND CLOSE
        //------------------------

        public void OnRelease()
        {
            if (!IsInteractable) return;

            //Inform Hotspot controller that popup has been opened
            if (controller != null) controller.PopUpOpened();

            //Enable Children
            EnableChildHotspots();
            DisableNonHotspotChildren();

            //Click Action
            switch (ClickAction)
            {
                case OnClickAction.Disable:
                    IsInteractable = false;
                    break;
                case OnClickAction.Hide:
                case OnClickAction.Delete:
                    HideHotspot();
                    break;
            }
        }

        public void ChildHotspotOpened(IHotspot childHotspot, OnClickAction childClickAction)
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                var hotspot = transform.GetChild(i).GetComponent<IHotspot>();
                if (hotspot == childHotspot)
                {
                    continue;
                }
                if (hotspot != null)
                {
                    switch (childClickAction)
                    {
                        case OnClickAction.Disable:
                            hotspot.DisableInteractivity();
                            break;
                        case OnClickAction.Delete:
                        case OnClickAction.Hide:
                            transform.GetChild(i).gameObject.SetActive(false);
                            break;
                    }   
                }
            }
        }

        public void ActionComplete()
        {
            controller.PopUpClosed();

            //First Open
            if (firstOpen)
            {
                controller.HotspotHasBeenViewed();
                firstOpen = false;
            }

            //Click Action
            switch (ClickAction)
            {
                case OnClickAction.Disable:
                    IsInteractable = true;
                    break;
                case OnClickAction.Hide:
                    RevealHotspot();
                    break;
                case OnClickAction.Delete:
                    Destroy(gameObject);
                    break;
            }

            //Disable Children
            DisableChildHotpsots();
            EnableNonHotspotChildren();

            for (int i = 0; i < transform.childCount; i++)
            {
                var hotspot = transform.GetChild(i).GetComponent<IHotspot>();
                if (hotspot != null)
                {
                    hotspot.EnableInteractivity();
                }
            }
        }

        public void DestroyChildHotspot(GameObject childObj)
        {
            var childHotspotCount = 0;
            for (int i = 0; i < transform.childCount; i++)
            {
                var hotspot = transform.GetChild(i).GetComponent<IHotspot>();
                if (hotspot != null)
                {
                    childHotspotCount++;
                }
            }

            StartCoroutine(DestroyOnEndOfFrame(childObj));

            //1 = empty as he hotspot that calls this will not yet be deleted
            if (childHotspotCount == 1)
            {
                Destroy(gameObject);
            }
        }

        IEnumerator DestroyOnEndOfFrame(GameObject obj)
        {
            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();
            Destroy(obj);
        }

        public void DisableInteractivity()
        {
            IsInteractable = false;
        }

        public void EnableInteractivity()
        {
            IsInteractable = true;
        }

        private void DisableChildHotpsots()
        {
            //Disable Children
            for (int i = 0; i < transform.childCount; i++)
            {
                var hotspot = transform.GetChild(i).GetComponent<HotspotScript>();
                if (hotspot != null) hotspot.gameObject.SetActive(false);
            }
        }

        private void DisableNonHotspotChildren()
        {
            //Disable Children
            for (int i = 0; i < transform.childCount; i++)
            {
                var hotspot = transform.GetChild(i).GetComponent<HotspotScript>();
                if (hotspot == null) transform.GetChild(i).gameObject.SetActive(false);
            }
        }

        private void EnableChildHotspots()
        {            
            //Disable Children
            for (int i = 0; i < transform.childCount; i++)
            {
                var hotspot = transform.GetChild(i).GetComponent<HotspotScript>();
                if (hotspot != null) hotspot.gameObject.SetActive(true);
            }
        }

        private void EnableNonHotspotChildren()
        {
            //Disable Children
            for (int i = 0; i < transform.childCount; i++)
            {
                var hotspot = transform.GetChild(i).GetComponent<HotspotScript>();
                if (hotspot == null) transform.GetChild(i).gameObject.SetActive(true);
            }
        }

        private void HideHotspot()
        {
            var spriteRenderer = GetComponent<SpriteRenderer>();
            if (spriteRenderer) spriteRenderer.enabled = false;
            var collider = GetComponent<Collider>();
            var collider2D = GetComponent<Collider2D>();
            if (collider != null) collider.enabled = false;
            if (collider2D != null) collider2D.enabled = false;
        }

        private void RevealHotspot()
        {
            var spriteRenderer = GetComponent<SpriteRenderer>();
            if (spriteRenderer) spriteRenderer.enabled = true;
            var collider = GetComponent<Collider>();
            var collider2D = GetComponent<Collider2D>();
            if (collider != null) collider.enabled = true;
            if (collider2D != null) collider2D.enabled = true;
        }





        //=============================================
        //HOTSPOT CREATION
        //=============================================

        public void AddBaseHotspot()
        {
#if UNITY_EDITOR
            var hotspot = PrefabUtility.InstantiatePrefab(baseHotspotPrefab) as GameObject;
            hotspot.name = "New Hotspot (Base)";
            hotspot.transform.SetParent(transform);

            //Position Hotspot
            hotspot.transform.localPosition = new Vector3(0.2f, 0);

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
            hotspot.transform.localPosition = new Vector3(0.2f, 0);

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
            hotspot.transform.localPosition = new Vector3(0.2f, 0);

            Selection.activeGameObject = hotspot;
#endif
        }

        public void OnPress() { }
        public void OnTouchEnter() { }
        public void OnTouchExit() { }
    }



    //==============================================================
    // CUSTOM EDITOR
    //==============================================================

#if UNITY_EDITOR

    [CustomEditor(typeof(MultiHotspot))]
    public class MultiHotspotEditor : Editor
    {

        private MultiHotspot multiHotspot;
        private int currentTab = 0;

        //Hotspot Prefabs
        private SerializedProperty baseHotspotPrefab;
        private SerializedProperty imageHotspotPrefab;
        private SerializedProperty invisibleHotspotPrefab;

        //General Settings
        private SerializedProperty onClickAction;


        private void OnEnable()
        {
            multiHotspot = (MultiHotspot)target;

            //Hotspot Prefabs
            baseHotspotPrefab = serializedObject.FindProperty("baseHotspotPrefab");
            imageHotspotPrefab = serializedObject.FindProperty("imageHotspotPrefab");
            invisibleHotspotPrefab = serializedObject.FindProperty("invisibleHotspotPrefab");

            //GeneralSettings
            onClickAction = serializedObject.FindProperty("_clickAction");

        }


        public override void OnInspectorGUI()
        {
            EditorGUILayout.Space();
            OnInspectorGUISettings();

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

            serializedObject.ApplyModifiedProperties();

        }

        private void OnInspectorGUISettings()
        {
            EditorGUILayout.LabelField("Settings");
            //General Settings
            EditorGUILayout.PropertyField(onClickAction, new GUIContent("When Selected", "What should be done to the when it is selected."));
        }

        private void OnInspectorGUICreate()
        {
            if (GUILayout.Button("New Basic Hotspot"))
            {
                multiHotspot.AddBaseHotspot();
            }

            if (GUILayout.Button("New Image Hotspot"))
            {
                multiHotspot.AddImageHotspot();
            }

            if (GUILayout.Button("New Invisible Hotspot"))
            {
                multiHotspot.AddInvisibleHotspot();
            }
        }

       

        private void OnInspectorGUIPrefab()
        {
            EditorGUILayout.PropertyField(baseHotspotPrefab, new GUIContent("Basic Hotspot"));
            EditorGUILayout.PropertyField(imageHotspotPrefab, new GUIContent("Image Hotspot"));
            EditorGUILayout.PropertyField(invisibleHotspotPrefab, new GUIContent("Invisible Hotspot"));

            serializedObject.ApplyModifiedProperties();

        }
    }
#endif

}
