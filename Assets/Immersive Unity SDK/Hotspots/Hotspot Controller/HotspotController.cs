/* Copyright (C) Immersive Interactive, Ltd - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Luke Bissell <luke@immersive.co.uk>, July 2019
 */

using Com.Immersive.Cameras;
using Immersive.Properties;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Com.Immersive.Hotspots
{
    public class HotspotController : MonoBehaviour
    {
        private Canvas canvas;
        private Camera cam;

        //The HotspotController for the current scene.
        public static List<HotspotController> CurrentControllers;

        public ImageProperty closeButton;
        public HotspotEffects hotspotEffects;
        public HotspotGlowSettings hotspotGlowSettings;

        public HotspotRevealType revealType = HotspotRevealType.AllAtOnce;

        [Tooltip("Only allow one hotspot to be opened at a time.")]
        public bool singlePopUpOpenAtOnce = false;

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

        private IHotspotVisibilityHandler[] visibilityHandlers;
        private List<GameObject> hotspotsHidden;

        private void OnEnable()
        {
            //CurrentControllers = this;
            if (CurrentControllers == null)
            {
                CurrentControllers = new List<HotspotController>();
            }
            CurrentControllers.Add(this);

            visibilityHandlers = GetGenericEventHandlers<IHotspotVisibilityHandler>();

            foreach (var hotspotVisibilityHandler in visibilityHandlers)
            {
                hotspotVisibilityHandler.parentController = this;
            }

            hotspotsHidden = new List<GameObject>();
        }
        
        public T[] GetGenericEventHandlers<T>()
        {
            Component[] components = GetComponents(typeof(T));
            T[] eventHandlers = new T[components.Length];
            for (int i = 0; i < components.Length; i++)
                eventHandlers[i] = components[i].GetComponent<T>();
            return eventHandlers;
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
                    if (revealType == HotspotRevealType.Ordered || revealType == HotspotRevealType.SequenceActivate)
                    {
                        hotspotsAndBatches = new GameObject[transform.childCount];
                        //Disable All Child Hotspots
                        for (int i = 0; i < transform.childCount; i++)
                        {
                            if (revealType == HotspotRevealType.Ordered)
                                transform.GetChild(i).gameObject.SetActive(false);
                            else if (revealType == HotspotRevealType.SequenceActivate)
                                DeActivateHotspot();

                            hotspotsAndBatches[i] = transform.GetChild(i).gameObject;
                        }

                        //Reveal first hotspot.
                        HotspotHasBeenViewed();
                    }

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
            }

            // Hotspot
            var hotspotScript = hotspotOrBatch.GetComponent<HotspotScript>();
            if (hotspotScript != null)
            {
                hotspotScript.EnableInteractivity();
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
                        hotspot.EnableInteractivity();
                    }
                }
            }

            // Hotspot
            var hotspotScript = hotspotOrBatch.GetComponent<HotspotScript>();
            if (hotspotScript != null)
                hotspotScript.EnableInteractivity();
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
            foreach (var hotspotVisibilityHandler in visibilityHandlers)
            {
                hotspotVisibilityHandler.HotspotsVisible();
            }
            if (singlePopUpOpenAtOnce) StartCoroutine(DisableHotspots());
        }

        /// <summary>
        /// Call this when a PopUp is closed
        /// </summary>
        public void PopUpClosed()
        {
            foreach (var hotspotVisibilityHandler in visibilityHandlers)
            {
                hotspotVisibilityHandler.HotspotsHidden();
            }
            
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

        public IEnumerator HideHotspots()
        {
            yield return new WaitForFixedUpdate();

            foreach (Transform t in transform)
            {
                if (t.gameObject.activeSelf || t.gameObject.activeInHierarchy)
                {
                    t.gameObject.SetActive(false);
                    hotspotsHidden.Add(t.gameObject);
                }
            }
        }

        public IEnumerator ShowHotspots()
        {
            yield return new WaitForFixedUpdate();
            
            foreach (GameObject go in hotspotsHidden)
            {
                go.SetActive(true);
            }

            hotspotsHidden.Clear();
            hotspotsHidden = new List<GameObject>();
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

                    ((TextHotspotDataModel)hotspotDataModel).text.FontSize = 1;

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
            AllAtOnce = 0,
            Ordered = 1,
            //Highlight = 2,
            SequenceActivate = 3
        }

        public enum HotspotEffects
        {
            None,
            Glow
        }
    }
}