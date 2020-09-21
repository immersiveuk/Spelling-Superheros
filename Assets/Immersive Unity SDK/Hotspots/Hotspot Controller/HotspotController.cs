/* Copyright (C) Immersive Interactive, Ltd - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Luke Bissell <luke@immersive.co.uk>, July 2019
 */

using Com.Immersive.Cameras;
using Com.Immersive.Cameras.PostProcessing;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Com.Immersive.Hotspots
{

    public class HotspotController : MonoBehaviour
    {
        private Canvas canvas;
        private Camera cam;

        //The HotspotController for the current scene.
        //public static HotspotController CurrentController;
        public static List<HotspotController> CurrentControllers;

        public ImageProperty closeButton;
        public HotspotEffects hotspotEffects;
        public HotspotGlowSettings hotspotGlowSettings;

        public HotspotRevealType revealType = HotspotRevealType.AllAtOnce;

        [Tooltip("Only allow one hotspot to be opened at a time.")]
        public bool singlePopUpOpenAtOnce = false;

        //Highlight settings
        public Color highlightColour = Color.yellow;
        public int highlightWidth = 8;

        //Blur and Desaturate settings
        public bool blurAndDesaturate = false;
        public float blurInDuration = 0;      //Seconds
        public float blurOutDuration = 0;     //Seconds

        //Handling reveals or hotspot and batches.
        private int currentIndex = 0;
        private bool isBatch = false;
        private int numHotspotsInBatch = 0;
        private GameObject[] hotspotsAndBatches;

        //Prefabs
        public GameObject batchPrefab;
        public GameObject baseHotspotPrefab;
        public GameObject imageHotspotPrefab;
        public GameObject invisibleHotspotPrefab;
        public GameObject multiHotspotPrefab;
        public GameObject textHotspotPrefab;
        public GameObject regionHotspotPrefab;

        private void OnEnable()
        {
            //CurrentControllers = this;
            if (CurrentControllers == null)
            {
                CurrentControllers = new List<HotspotController>();
            }
            CurrentControllers.Add(this);
        }

        private void OnDisable()
        {
            CurrentControllers.Remove(this);
        }

        private bool isFirstFrame = true;
        private void Update()
        {
            //If not in hotspot placement mode
            var inHotspotPlacementMode = false;
#if UNITY_EDITOR
            if (EditorPrefs.HasKey("PlaceHotspotMode") && EditorPrefs.GetBool("PlaceHotspotMode"))
            {
                inHotspotPlacementMode = true;
            }
#endif

            if (!inHotspotPlacementMode)
            {
                if (isFirstFrame)
                {
                    isFirstFrame = false;
                    if (revealType == HotspotRevealType.Ordered || revealType == HotspotRevealType.Highlight || revealType == HotspotRevealType.SequenceActivate)
                    {
                        hotspotsAndBatches = new GameObject[transform.childCount];
                        //Disable All Child Hotspots
                        for (int i = 0; i < transform.childCount; i++)
                        {
                            if (revealType == HotspotRevealType.Ordered)
                                transform.GetChild(i).gameObject.SetActive(false);
                            else if (revealType == HotspotRevealType.Highlight)
                                DisableHotspots();
                            else if (revealType == HotspotRevealType.SequenceActivate)
                                DeActivateHotspot();

                            hotspotsAndBatches[i] = transform.GetChild(i).gameObject;
                        }

                        //Reveal first hotspot.
                        HotspotHasBeenViewed();
                    }

                }
                if (blurAndDesaturate)
                {
                    BlurAndDesaturateUpdate();
                }
            }
        }


        /// <summary>
        /// Called after a hotspot has been interacted with for the first time.
        /// Displays or highlights the next hotspot or batch.
        /// </summary>
        public void HotspotHasBeenViewed()
        {
            if (revealType == HotspotRevealType.AllAtOnce) return;

            //Is the currently revealed set of hotspots a batch?
            if (isBatch)
            {
                NextHotspotInBatch();
                //Is still batch?
                if (isBatch) return;
            }

            //Are there still hotspots to reveal or highlight?
            if (currentIndex < hotspotsAndBatches.Length)
            {
                if (revealType == HotspotRevealType.Ordered)
                    RevealHotspotOrBatch();
                else if (revealType == HotspotRevealType.Highlight)
                    HightlightAndEnableHotspotOrBatch();
                else if (revealType == HotspotRevealType.SequenceActivate)
                    ActivateHotspotOrBatch();
            }
        }

        //--------------------------------------------------------------
        // Activate HOTSPOTS OR BATCH
        //--------------------------------------------------------------

        /// <summary>
        /// Activate the next batch or hotspot.
        /// </summary>
        private void ActivateHotspotOrBatch()
        {
            var hotspotOrBatch = hotspotsAndBatches[currentIndex];

            //Batch
            if (hotspotOrBatch.GetComponent<HotspotScript>() == null)
            {
                numHotspotsInBatch = hotspotOrBatch.transform.childCount;
                isBatch = true;

                for (int i = 0; i < numHotspotsInBatch; i++)
                {
                    var hotspot = hotspotOrBatch.transform.GetChild(i).GetComponent<HotspotScript>();
                    if (hotspot != null)
                    {
                        Debug.Log("..");
                        hotspot.EnableInteractivity();
                    }
                }

                if (blurAndDesaturate) focusPhase = FocusPhase.Defocussing;
            }

            // Hotspot
            var hotspotScript = hotspotOrBatch.GetComponent<HotspotScript>();
            if (hotspotScript != null)
            {
                hotspotScript.EnableInteractivity();

                if (blurAndDesaturate) BlurAndDesaturateHotspot(hotspotOrBatch);
            }
            currentIndex++;
        }

        //--------------------------------------------------------------
        // REVEAL HOTSPOTS OR BATCH
        //--------------------------------------------------------------

        /// <summary>
        /// Reveals the next batch or hotspot.
        /// </summary>
        private void RevealHotspotOrBatch()
        {
            var hotspotOrBatch = hotspotsAndBatches[currentIndex];
            //Batch
            if (hotspotOrBatch.GetComponent<HotspotBatch>() != null)
            {
                isBatch = true;
                numHotspotsInBatch = hotspotOrBatch.transform.childCount;
                if (blurAndDesaturate) focusPhase = FocusPhase.Defocussing;
            }
            //Multi-Hotspot
            else if (hotspotOrBatch.GetComponent<MultiHotspot>())
            {
                if (blurAndDesaturate) focusPhase = FocusPhase.Defocussing;
            }
            //Hotspot
            else if (hotspotOrBatch.GetComponent<HotspotScript>())
            {
                if (blurAndDesaturate) BlurAndDesaturateHotspot(hotspotOrBatch);
            }

            hotspotOrBatch.SetActive(true);
            currentIndex++;
        }

        /// <summary>
        /// Handles what to do when a hotspot in a batch is opened and closed for the first time.
        /// </summary>
        private void NextHotspotInBatch()
        {
            //All hotspots in batch have been opened.
            if (numHotspotsInBatch == 1)
            {
                numHotspotsInBatch = 0;
                isBatch = false;
            }
            //Hotspots in batch still to be revealed.
            else
            {
                numHotspotsInBatch--;
            }
        }

        //--------------------------------------------------------------
        // BLUR AND DESATURATE
        //--------------------------------------------------------------

        //Focus Phase
        private enum FocusPhase { Focussing, Defocussing, Fixed };
        private FocusPhase focusPhase = FocusPhase.Fixed;

        //Blur Intensity
        private readonly float maxBlurIntensity = 3;
        private float currentBlurIntensity = 0;

        //Focus Info
        private (Vector2, int) focusPoint;
        private float focalRadius = 0;

        /// <summary>
        /// Starts the blur and desaturate process.
        /// If currently focussed on an object will first defocus.
        /// </summary>
        /// <param name="obj">The object to focus on.</param>
        private void BlurAndDesaturateHotspot(GameObject obj)
        {
            var immersiveCam = AbstractImmersiveCamera.CurrentImmersiveCamera;
            var blurAndDesaturate = BlurAndDesaturate.CurrentBlurAndDesaturate;

            //Get hotspots position on screen;
            Camera cam = immersiveCam.FindCameraLookingAtPosition(obj.transform.position);
            var camIndex = immersiveCam.GetIndexOfCamera(cam);
            var pos = cam.WorldToViewportPoint(obj.transform.position);
            pos.x *= cam.pixelWidth;
            pos.y *= cam.pixelHeight;


            //print("Focus Point: " + pos + ", "+immersiveCam +", "+blurAndDesaturate);
            //Set blur and desature info.
            blurAndDesaturate.active = true;
            focusPoint = (pos, camIndex);
            print("Focus Point: " + focusPoint);

            //Calculate the size of the hotspot to determine the focal radius
            var rend = obj.GetComponent<Renderer>();
            if (rend)
            {
                var radius = rend.bounds.extents.magnitude;
                focalRadius = radius;
            }
            else
            {
                focalRadius = 0.3f;
            }

            //Set focussing phase to start defocus.
            focusPhase = FocusPhase.Defocussing;
        }

        //Called once a frame to update the blur and desature filter
        private void BlurAndDesaturateUpdate()
        {
            //Focus
            if (focusPhase == FocusPhase.Focussing)
            {
                float increment = maxBlurIntensity;

                if (blurInDuration != 0)
                {
                    increment = (maxBlurIntensity * Time.fixedDeltaTime) / blurInDuration;
                }

                currentBlurIntensity += increment;

                if (currentBlurIntensity > maxBlurIntensity)
                {
                    currentBlurIntensity = maxBlurIntensity;
                    focusPhase = FocusPhase.Fixed;
                }

                BlurAndDesaturate.CurrentBlurAndDesaturate.SetIntensity(currentBlurIntensity);
            }

            //Defocus
            if (focusPhase == FocusPhase.Defocussing)
            {
                float increment = maxBlurIntensity;

                if (blurOutDuration != 0)
                {
                    increment = (maxBlurIntensity * Time.fixedDeltaTime) / blurOutDuration;
                }

                currentBlurIntensity -= increment;

                if (currentBlurIntensity < 0)
                {
                    currentBlurIntensity = 0;

                    //If its a batch no blur or desature applied
                    if (isBatch)
                    {
                        focusPhase = FocusPhase.Fixed;
                    }
                    else
                    {
                        focusPhase = FocusPhase.Focussing;
                        BlurAndDesaturate.CurrentBlurAndDesaturate.SetFocalPoint(focusPoint.Item1, focusPoint.Item2);
                        BlurAndDesaturate.CurrentBlurAndDesaturate.SetFocalRadius(focalRadius);

                    }
                }
                BlurAndDesaturate.CurrentBlurAndDesaturate.SetIntensity(currentBlurIntensity);
            }
        }

        //--------------------------------------------------------------
        // HIGHLIGHT
        //--------------------------------------------------------------


        /// <summary>
        /// Highlights and enables the next hotspot or batch of hotspot.
        /// </summary>
        private void HightlightAndEnableHotspotOrBatch()
        {
            var hotspotOrBatch = hotspotsAndBatches[currentIndex];

            //Batch
            if (hotspotOrBatch.GetComponent<HotspotScript>() == null)
            {
                numHotspotsInBatch = hotspotOrBatch.transform.childCount;
                isBatch = true;

                for (int i = 0; i < numHotspotsInBatch; i++)
                {
                    var hotspot = hotspotOrBatch.transform.GetChild(i).GetComponent<HotspotScript>();
                    if (hotspot != null)
                    {
                        hotspot.AddOutline(highlightColour, highlightWidth);
                        hotspot.EnableInteractivity();
                    }
                }

                if (blurAndDesaturate) focusPhase = FocusPhase.Defocussing;
            }

            // Hotspot
            var hotspotScript = hotspotOrBatch.GetComponent<HotspotScript>();
            if (hotspotScript != null)
            {
                hotspotScript.AddOutline(highlightColour, highlightWidth);
                hotspotScript.EnableInteractivity();

                if (blurAndDesaturate) BlurAndDesaturateHotspot(hotspotOrBatch);
            }
            currentIndex++;
        }

        //--------------------------------------------------------------
        // OTHER
        //--------------------------------------------------------------


        /// <summary>
        /// Call this when a PopUp is opened
        /// </summary>
        public void PopUpOpened()
        {
            if (singlePopUpOpenAtOnce) StartCoroutine(DisableHotspots());
        }

        /// <summary>
        /// Call this when a PopUp is closed
        /// </summary>
        public void PopUpClosed()
        {
            if (singlePopUpOpenAtOnce) StartCoroutine(EnableHotspots());
        }


        /// <summary>
        /// DeActivate all hotspot in scene
        /// </summary>
        /// <returns></returns>
        public void DeActivateHotspot()
        {
            //yield return new WaitForFixedUpdate();

            for (int i = 0; i < transform.childCount; i++)
            {
                var hotspotOrBatch = transform.GetChild(i);
                var hotspot = hotspotOrBatch.GetComponent<HotspotScript>();
                var batch = hotspotOrBatch.GetComponent<HotspotBatch>();


                //Hotspot
                if (hotspot != null)
                {

                    hotspot.DisableInteractivity();
                }

                //Batch 
                else if (batch != null)
                {
                    for (int j = 0; j < hotspotOrBatch.transform.childCount; j++)
                    {
                        hotspot = hotspotOrBatch.transform.GetChild(j).GetComponent<HotspotScript>();
                        //Hotspot
                        if (hotspot != null)
                        {
                            hotspot.DisableInteractivity();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Disables all hotspots in the scene.
        /// </summary>
        public IEnumerator DisableHotspots()
        {
            yield return new WaitForFixedUpdate();
            for (int i = 0; i < transform.childCount; i++)
            {
                var hotspotOrBatch = transform.GetChild(i);
                var hotspot = hotspotOrBatch.GetComponent<IHotspot>();
                var batch = hotspotOrBatch.GetComponent<HotspotBatch>();


                //Hotspot
                if (hotspot != null)
                {
                    hotspot.DisableInteractivity();
                }

                //Batch 
                else if (batch != null)
                {
                    for (int j = 0; j < hotspotOrBatch.transform.childCount; j++)
                    {
                        hotspot = hotspotOrBatch.transform.GetChild(j).GetComponent<IHotspot>();
                        //Hotspot
                        if (hotspot != null)
                        {
                            hotspot.DisableInteractivity();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Disables hotspots for all hotspot controllers in current game.
        /// </summary>
        public static void DisableHotspotForAllControllers()
        {
            if (HotspotController.CurrentControllers == null) return;
            foreach (var controller in CurrentControllers)
            {

                controller.StartCoroutine(controller.DisableHotspots());
            }
        }

        /// <summary>
        /// Enables all hotspots in the scene.
        /// </summary>
        public IEnumerator EnableHotspots()
        {
            yield return new WaitForFixedUpdate();
            // If in highlight mode only enable the hotspots which have been activated up to this point.
            if (revealType == HotspotRevealType.Highlight)
            {
                for (int i = 0; i < currentIndex; i++)
                {
                    //Hotspot deleted
                    if (hotspotsAndBatches[i] == null) continue;

                    var hotspot = hotspotsAndBatches[i].GetComponent<IHotspot>();

                    //Hotspot
                    if (hotspot != null)
                    {
                        hotspot.EnableInteractivity();
                    }
                    //Batch
                    else
                    {
                        for (int j = 0; j < hotspotsAndBatches[i].transform.childCount; j++)
                        {
                            hotspot = hotspotsAndBatches[i].transform.GetChild(j).GetComponent<IHotspot>();
                            //Hotspot
                            if (hotspot != null)
                            {
                                hotspot.EnableInteractivity();
                            }
                        }
                    }
                }
            }

            //Activate all hotspots
            else
            {
                for (int i = 0; i < transform.childCount; i++)
                {
                    var hotspotOrBatch = transform.GetChild(i);
                    var hotspot = hotspotOrBatch.GetComponent<IHotspot>();

                    //Hotspot
                    if (hotspot != null)
                    {
                        hotspot.EnableInteractivity();
                    }

                    //Batch 
                    else
                    {
                        for (int j = 0; j < hotspotOrBatch.transform.childCount; j++)
                        {
                            hotspot = hotspotOrBatch.transform.GetChild(j).GetComponent<IHotspot>();
                            //Hotspot
                            if (hotspot != null)
                            {
                                hotspot.EnableInteractivity();
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Enables Hotspots for all hotspot controllers in current scene. 
        /// </summary>
        public static void EnableHotspotsForAllControllers()
        {
            if (HotspotController.CurrentControllers == null) return;

            foreach (var controller in CurrentControllers)
            {
                controller.StartCoroutine(controller.EnableHotspots());
            }
        }


        //==============================================================
        // ADD NEW HOTSPOTS
        //==============================================================
        public HotspotScript AddHotspot(HotspotDataModel hotspotDataModel, string imageUrl)
        {
            GameObject hotspot = null;

            switch (hotspotDataModel.hotspotType)
            {
                case HotspotType.Image:
                    hotspot = Instantiate(imageHotspotPrefab, transform) as GameObject;
                    hotspot.transform.localScale = new Vector3(hotspotDataModel.scale.x, hotspotDataModel.scale.y, 1);

                    if (!string.IsNullOrEmpty(((ImageHotspotDataModel)hotspotDataModel).imageHotspotSprite.imageUrl))
                        imageUrl = ((ImageHotspotDataModel)hotspotDataModel).imageHotspotSprite.imageUrl;
                    else
                        ((ImageHotspotDataModel)hotspotDataModel).imageHotspotSprite.imageUrl = imageUrl;

                    RuntimeLoading.Instance.LoadImage(imageUrl, (Texture2D texture, Sprite sprite) =>
                    {
                        if (texture)
                            hotspot.GetComponent<SpriteRenderer>().sprite = RuntimeLoading.ScaleTexture(texture, 125, 125);
                    });

                    break;

                case HotspotType.Region:
                    hotspot = Instantiate(regionHotspotPrefab, transform) as GameObject;
                    hotspot.GetComponent<BoxCollider>().size = new Vector3(hotspotDataModel.scale.x, hotspotDataModel.scale.y, 1);

                    RegionHotspot regionHotspot = hotspot.GetComponent<RegionHotspot>();
                    regionHotspot.lineColor = ((RegionHotspotDataModel)hotspotDataModel).regionHotspotColor;
                    regionHotspot.lineType = ((RegionHotspotDataModel)hotspotDataModel).lineType;
                    regionHotspot.lineThikness = ((RegionHotspotDataModel)hotspotDataModel).lineThikness;

                    regionHotspot.DrawLine();
                    break;

                case HotspotType.Text:
                    hotspot = Instantiate(textHotspotPrefab, transform) as GameObject;
                    hotspot.transform.localScale = new Vector3(hotspotDataModel.scale.x, hotspotDataModel.scale.y, 1);

                    TextHotspot textHotspot = hotspot.GetComponent<TextHotspot>();

                    ((TextHotspotDataModel)hotspotDataModel).text.size = 1;

                    textHotspot.textProperty = ((TextHotspotDataModel)hotspotDataModel).text;
                    textHotspot.imageProperty = ((TextHotspotDataModel)hotspotDataModel).image;

                    textHotspot.Init();
                    break;

                case HotspotType.Invisible:
                    hotspot = Instantiate(invisibleHotspotPrefab, transform) as GameObject;
                    hotspot.transform.localScale = new Vector3(hotspotDataModel.scale.x, hotspotDataModel.scale.y, 0.1f) * 0.1f;
                    break;
            }

            hotspot.transform.localPosition = hotspotDataModel.position;

            (cam, canvas) = AbstractImmersiveCamera.CurrentImmersiveCamera.FindRenderingCameraAndCanvas(hotspot.gameObject);
            if (cam)
                hotspot.transform.localEulerAngles = cam.transform.localEulerAngles;

            HotspotScript hotspotScript = hotspot.GetComponent<HotspotScript>();

            hotspotScript.hotspotDataModel = hotspotDataModel;
            hotspotScript.hotspotDataModel.actionType = hotspotDataModel.actionType;
            hotspotScript.hotspotDataModel.clickAction = hotspotDataModel.clickAction;

            return hotspotScript;
        }

        public void AddBaseHotspot()
        {

#if UNITY_EDITOR
            var hotspot = PrefabUtility.InstantiatePrefab(baseHotspotPrefab) as GameObject;
            hotspot.name = "New Hotspot (Base)";
            hotspot.transform.SetParent(transform);

            var ray = AbstractImmersiveCamera.CurrentImmersiveCamera.mainCamera.ViewportPointToRay(new Vector2(0.5f, 0.5f));
            hotspot.transform.localPosition = ray.GetPoint(1);

            Selection.activeGameObject = hotspot;
#endif
        }

        public void AddImageHotspot()
        {
#if UNITY_EDITOR
            var hotspot = PrefabUtility.InstantiatePrefab(imageHotspotPrefab) as GameObject;                    
            hotspot.name = "New Hotspot (Image)";
            hotspot.transform.SetParent(transform);

            var ray = AbstractImmersiveCamera.CurrentImmersiveCamera.mainCamera.ViewportPointToRay(new Vector2(0.5f, 0.5f));
            hotspot.transform.localPosition = ray.GetPoint(1);

            Selection.activeGameObject = hotspot;
#endif
        }

        public void AddInvisibleHotspot()
        {
#if UNITY_EDITOR
            var hotspot = PrefabUtility.InstantiatePrefab(invisibleHotspotPrefab) as GameObject;
            hotspot.name = "New Hotspot (Invisible)";
            hotspot.transform.SetParent(transform);

            var ray = AbstractImmersiveCamera.CurrentImmersiveCamera.mainCamera.ViewportPointToRay(new Vector2(0.5f, 0.5f));
            hotspot.transform.localPosition = ray.GetPoint(1);

            Selection.activeGameObject = hotspot;
#endif
        }

        public void AddMultiHotspot()
        {
#if UNITY_EDITOR
            var hotspot = PrefabUtility.InstantiatePrefab(multiHotspotPrefab) as GameObject;
            hotspot.name = "New Multi-Hotspot (Image)";
            hotspot.transform.SetParent(transform);

            var ray = AbstractImmersiveCamera.CurrentImmersiveCamera.mainCamera.ViewportPointToRay(new Vector2(0.5f, 0.5f));
            hotspot.transform.localPosition = ray.GetPoint(1);

            Selection.activeGameObject = hotspot;
#endif
        }

        public void AddTextHotspot()
        {
#if UNITY_EDITOR
            var hotspot = PrefabUtility.InstantiatePrefab(textHotspotPrefab) as GameObject;
            hotspot.name = "New Text Hotspot";
            hotspot.transform.SetParent(transform);

            var ray = AbstractImmersiveCamera.CurrentImmersiveCamera.mainCamera.ViewportPointToRay(new Vector2(0.5f, 0.5f));
            hotspot.transform.localPosition = ray.GetPoint(1);

            Selection.activeGameObject = hotspot;
#endif
        }

        public void AddRegionHotspot()
        {
#if UNITY_EDITOR
            var hotspot = PrefabUtility.InstantiatePrefab(regionHotspotPrefab) as GameObject;
            hotspot.name = "New Region Hotspot";
            hotspot.transform.SetParent(transform);
            hotspot.GetComponent<RegionHotspot>().Init();

            var ray = AbstractImmersiveCamera.CurrentImmersiveCamera.mainCamera.ViewportPointToRay(new Vector2(0.5f, 0.5f));
            hotspot.transform.localPosition = ray.GetPoint(1);

            Selection.activeGameObject = hotspot;
#endif
        }


        public void AddBatch()
        {
#if UNITY_EDITOR
            var batch = PrefabUtility.InstantiatePrefab(batchPrefab) as GameObject;
            batch.name = "Batch";
            batch.transform.SetParent(transform);

            Selection.activeGameObject = batch;
#endif
        }


        /// <summary>
        /// On Editor closing, place hotspots mode must be turned off.
        /// </summary>
        private void OnApplicationQuit()
        {
#if UNITY_EDITOR
            EditorPrefs.SetBool("PlaceHotspotMode", false);
#endif
        }

        //==============================================================
        // DATA STRUCTURES
        //==============================================================

        public enum HotspotRevealType
        {
            AllAtOnce,
            Ordered,
            Highlight,
            SequenceActivate
        }

        public enum HotspotEffects
        {
            None,
            Glow
        }
    }


#if UNITY_EDITOR

    //==============================================================
    // CUSTOM EDITOR
    //==============================================================

    [CustomEditor(typeof(HotspotController))]
    public class HotspotCreatorEditor : Editor
    {

        private HotspotController controller;
        private int currentTab = 0;

        //Settings
        private SerializedProperty revealType;
        private SerializedProperty singlePopUpOpenAtOnce;

        //Close Button
        private SerializedProperty closeButton;

        //Hotspot glow settings
        private SerializedProperty hotspotEffects;
        private SerializedProperty hotspotGlowSettings;

        //Hightlight Settings
        private SerializedProperty highlightColour;
        private SerializedProperty highlightWidth;

        //Blur and Desature Settings
        private SerializedProperty blurAndDesaturate;
        private SerializedProperty blurInDuration;
        private SerializedProperty blurOutDuration;

        //Hotspot Prefabs
        private SerializedProperty baseHotspotPrefab;
        private SerializedProperty imageHotspotPrefab;
        private SerializedProperty invisibleHotspotPrefab;
        private SerializedProperty batchPrefab;
        private SerializedProperty multiHotspotPrefab;
        private SerializedProperty textHotspotPrefab;
        private SerializedProperty regionHotspotPrefab;

        private void OnEnable()
        {
            controller = (HotspotController)target;

            //Settings
            revealType = serializedObject.FindProperty("revealType");
            singlePopUpOpenAtOnce = serializedObject.FindProperty("singlePopUpOpenAtOnce");

            hotspotEffects = serializedObject.FindProperty("hotspotEffects");
            hotspotGlowSettings = serializedObject.FindProperty("hotspotGlowSettings");

            //Close Button
            closeButton = serializedObject.FindProperty("closeButton");

            //Highlight Settings
            highlightColour = serializedObject.FindProperty("highlightColour");
            highlightWidth = serializedObject.FindProperty("highlightWidth");

            //Blur and Desature Settings
            blurAndDesaturate = serializedObject.FindProperty("blurAndDesaturate");
            blurInDuration = serializedObject.FindProperty("blurInDuration");
            blurOutDuration = serializedObject.FindProperty("blurOutDuration");

            //Hotspot Prefabs
            baseHotspotPrefab = serializedObject.FindProperty("baseHotspotPrefab");
            imageHotspotPrefab = serializedObject.FindProperty("imageHotspotPrefab");
            invisibleHotspotPrefab = serializedObject.FindProperty("invisibleHotspotPrefab");
            multiHotspotPrefab = serializedObject.FindProperty("multiHotspotPrefab");
            textHotspotPrefab = serializedObject.FindProperty("textHotspotPrefab");
            regionHotspotPrefab = serializedObject.FindProperty("regionHotspotPrefab");
            batchPrefab = serializedObject.FindProperty("batchPrefab");
        }


        public override void OnInspectorGUI()
        {
            //Settings
            EditorGUILayout.Space();

            //Navigation UI
            //OnInspectorGUINaviagtion();

            EditorGUILayout.LabelField("Settings", EditorStyles.boldLabel);
            OnInspectorGUISettings();

            //Create
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

            if (EditorApplication.isPlaying)
            {
                if (GUILayout.Button("Generate JSON"))
                {
                    HotspotsSerializer.GenerateControllerJSON(controller);
                }
            }
        }


        private void OnInspectorGUISettings()
        {
            EditorGUILayout.PropertyField(closeButton, new GUIContent("Close Button"), true);

            EditorGUILayout.PropertyField(hotspotEffects);
            if (controller.hotspotEffects == HotspotController.HotspotEffects.Glow)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(hotspotGlowSettings, new GUIContent("Hotspot Glow Settings"), true);
                EditorGUI.indentLevel--;
            }            

            EditorGUILayout.PropertyField(revealType);

            //Settings specific to the reveal type
            EditorGUI.indentLevel++;
            //Highlight
            if (controller.revealType == HotspotController.HotspotRevealType.Highlight)
            {
                EditorGUILayout.PropertyField(highlightColour);
                EditorGUILayout.PropertyField(highlightWidth);
            }
            EditorGUI.indentLevel--;

            //Blur and Desaturate
            if (controller.revealType != HotspotController.HotspotRevealType.AllAtOnce)
            {
                EditorGUILayout.PropertyField(blurAndDesaturate);
                if (controller.blurAndDesaturate)
                {
                    EditorGUI.indentLevel++;
                    EditorGUILayout.LabelField("Batches will not work with blur and desaturate.");
                    EditorGUILayout.PropertyField(blurInDuration);
                    EditorGUILayout.PropertyField(blurOutDuration);
                    EditorGUI.indentLevel--;
                }
            }

            EditorGUILayout.PropertyField(singlePopUpOpenAtOnce);
        }

        private void OnInspectorGUICreate()
        {

            EditorGUILayout.LabelField("Create Hotspots");

            if (GUILayout.Button("New Batch"))
            {
                controller.AddBatch();
            }

            if (GUILayout.Button("New Basic Hotspot"))
            {
                controller.AddBaseHotspot();
            }

            if (GUILayout.Button("New Image Hotspot"))
            {
                controller.AddImageHotspot();
            }

            if (GUILayout.Button("New Invisible Hotspot"))
            {
                controller.AddInvisibleHotspot();
            }

            if (GUILayout.Button("New Multi-Hotspot"))
            {
                controller.AddMultiHotspot();
            }

            if (GUILayout.Button("New Text Hotspot"))
            {
                controller.AddTextHotspot();
            }

            if (GUILayout.Button("New Region Hotspot"))
            {
                controller.AddRegionHotspot();
            }
        }

        private void OnInspectorGUIPrefab()
        {
            EditorGUILayout.PropertyField(batchPrefab, new GUIContent("Batch"));
            EditorGUILayout.PropertyField(baseHotspotPrefab, new GUIContent("Basic Hotspot"));
            EditorGUILayout.PropertyField(imageHotspotPrefab, new GUIContent("Image Hotspot"));
            EditorGUILayout.PropertyField(invisibleHotspotPrefab, new GUIContent("Invisible Hotspot"));
            EditorGUILayout.PropertyField(multiHotspotPrefab, new GUIContent("Multi-Hotspot"));
            EditorGUILayout.PropertyField(textHotspotPrefab, new GUIContent("Text Hotspot"));
            EditorGUILayout.PropertyField(regionHotspotPrefab, new GUIContent("Region Hotspot"));
            serializedObject.ApplyModifiedProperties();
        }
    }

#endif

}