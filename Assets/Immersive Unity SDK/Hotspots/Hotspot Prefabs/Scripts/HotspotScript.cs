/* Copyright (C) Immersive Interactive, Ltd - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Luke Bissell <luke@immersive.co.uk>, July 2019
 */

using UnityEngine;
using Com.Immersive.Cameras;
using UnityEngine.Video;
using UnityEditor;
using UnityEngine.SceneManagement;
using Com.Immersive.Cameras.PostProcessing;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Collections;

using System;

namespace Com.Immersive.Hotspots
{

    /// <summary>
    /// This component can be added to any GameObject to make it into a Hotspot.
    /// There are a variety of options for hotspot type and form.
    /// </summary>
    public class HotspotScript : MonoBehaviour, IHotspot
    {


        private Canvas canvas;
        private Camera cam;

        //-------SETTINGS--------

        public HotspotDataModel hotspotDataModel;

        //Image
        public ImagePopUpDataModel imagePopUpDataModel;

        //Image Sequence
        public ImageSequencePopUpDataModel imageSequencePopUpDataModel;

        //Video
        public VideoPopUpDataModel videoPopUpDataModel;

        //Text
        public TextPopUpDataModel textPopUpDataModel;

        //Q & A
        public QuizPopUpDataModel quizPopUpDataModel;

        //Audio popup
        public AudioPopUpDataModel audioPopUpDataModel;

        //SceneLink
        public SceneLinkDataModel sceneLinkDataModel;

        //Split Popup
        public SplitPopUpDataModel splitPopupDataModel;

        //Reveal And Hide Objects
        public GameObject[] objectsToHide;
        public GameObject[] objectsToReveal;


        //-------POPUP PREFABS--------
        public VideoHotspotPopUp videoPopUpPrefab;
        public ImageHotspotPopUp imagePopUpPrefab;
        public ImageSequenceHotspotPopup imageSequencePopUpPrefab;
        public TextHotspotPopUp textPopUpPrefab;
        public QuizHotspotPopUp qAndAPopUpPrefab;
        public AudioHotspotPopUp audioPopUpPrefab;
        public SplitPopup splitPopupPrefab;

        //-------PRIVATE VARIABLES--------

        //public bool isInteractable = true;
        private bool firstOpen = true;
        private bool restoreOutlineOnEnable = false;

        public bool IsInteractable { get; set; } = true;
        public bool IsPopupHotspot => (hotspotDataModel.actionType == ActionType.ImagePopup || hotspotDataModel.actionType == ActionType.VideoPopup || hotspotDataModel.actionType == ActionType.TextPopup || hotspotDataModel.actionType == ActionType.QuizPopup || hotspotDataModel.actionType == ActionType.ImageSequencePopup || hotspotDataModel.actionType == ActionType.AudioPopup || hotspotDataModel.actionType == ActionType.SplitPopup);

        private IHotspotActionCompleteHandler[] actionCompleteHandlers;


        private HotspotController controller;
        private MultiHotspot presentingMultiHotspot;
        private void FindPresentingHotspotControllerOrMultiHotspot()
        {
            var controllerTransform = transform.parent;
            if (controllerTransform.GetComponent<HotspotBatch>())
            {
                controllerTransform = controllerTransform.parent;
            }
            else if (controllerTransform.GetComponent<MultiHotspot>())
            {
                presentingMultiHotspot = controllerTransform.GetComponent<MultiHotspot>();
            }
            controller = controllerTransform.GetComponent<HotspotController>();
        }


        public void EnableInteractivity()
        {
            IsInteractable = true;
            if (restoreOutlineOnEnable)
            {
                AddOutline(highlightColour, highlightWidth);
                restoreOutlineOnEnable = false;
            }
        }

        public void DisableInteractivity()
        {
            IsInteractable = false;
            if (isOutlined)
            {
                restoreOutlineOnEnable = true;
                RemoveOutline();
            }
        }


        private bool isOutlined = false;
        private float angle = 0;
        private Color highlightColour = Color.yellow;
        private int highlightWidth = 0;

        public void AddOutline(Color highlightColour, int highlightWidth)
        {
            isOutlined = true;
            this.highlightColour = highlightColour;
            this.highlightWidth = highlightWidth;

            //Outline Sprite
            var spriteRenderer = GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                spriteRenderer.material.SetFloat("_OutlineEnabled", 1);
                spriteRenderer.material.SetColor("_GradientOutline1", highlightColour);
                spriteRenderer.material.SetFloat("_Thickness", highlightWidth);

                //Scale so that the image remains the same size
                var resolution = spriteRenderer.sprite.rect.size;
                var newResolution = new Vector2();
                newResolution.x = resolution.x + highlightWidth * 2;
                newResolution.y = resolution.y + highlightWidth * 2;
                var scale = transform.localScale;
                scale.x *= (newResolution.x / resolution.x);
                scale.y *= (newResolution.y / resolution.y);
                transform.localScale = scale;
            }
        }

        public void RemoveOutline()
        {
            isOutlined = false;

            //Outline Sprite
            var spriteRenderer = GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                spriteRenderer.material.SetFloat("_OutlineEnabled", 0);

                //Scale so that the image remains the same size
                var resolution = spriteRenderer.sprite.rect.size;
                var newResolution = new Vector2();
                newResolution.x = resolution.x - 16;
                newResolution.y = resolution.y - 16;
                var scale = transform.localScale;
                scale.x *= (newResolution.x / resolution.x);
                scale.y *= (newResolution.y / resolution.y);
                transform.localScale = scale;
            }
        }

        //==============================================================
        //SETUP
        //==============================================================



        public void Start()
        {
            (cam, canvas) = AbstractImmersiveCamera.CurrentImmersiveCamera.FindRenderingCameraAndCanvas(gameObject);

            LookAtCamera();

            FindPresentingHotspotControllerOrMultiHotspot();

            //Make sure the objects to be revealed are inactive
            if (hotspotDataModel.actionType == ActionType.ActivateAndHideObjects)
            {
                foreach (var obj in objectsToReveal)
                {
                    obj.SetActive(false);
                }
            }

            //Find any Action Complete Handlers
            actionCompleteHandlers = GetComponents<IHotspotActionCompleteHandler>();
        }

        /// <summary>
        /// Initialize a Hotspot from json
        /// </summary>
        /// <param name="hotspotDataModel"></param>
        /// <param name="actionType"></param>
        public void Init(HotspotDataModel hotspotDataModel)
        {
            this.hotspotDataModel.actionType = hotspotDataModel.actionType;
            this.hotspotDataModel.clickAction = hotspotDataModel.clickAction;

            Start();
        }

        private void LookAtCamera()
        {
            if (cam != null)
            {
                transform.rotation = cam.transform.rotation;
            }
        }

        public void OnPress() { }

        public void OnTouchEnter() { }

        public void OnTouchExit() { }


        private void Update()
        {
            if (transform.hasChanged)
            {
                (cam, canvas) = AbstractImmersiveCamera.CurrentImmersiveCamera.FindRenderingCameraAndCanvas(gameObject);
            }

            if (isOutlined)
            {
                angle += 1;
                GetComponent<Renderer>().material.SetFloat("_Angle", angle);
            }


            // If in place hotspot mode, Ensure that hotspots face correct camera.
#if UNITY_EDITOR
            if (EditorPrefs.HasKey("PlaceHotspotMode"))
            {
                if (EditorPrefs.GetBool("PlaceHotspotMode"))
                {
                    (cam, canvas) = AbstractImmersiveCamera.CurrentImmersiveCamera.FindRenderingCameraAndCanvas(gameObject);
                    LookAtCamera();
                }
            }
#endif
        }

        //==============================================================
        //POPUP
        //==============================================================

        //When touched spawn correct popup.
        public void OnRelease()
        {
            if (!canvas) return;
            if (!IsInteractable) return;

            //Pay Audio
            if (hotspotDataModel.playAudioOnAction && hotspotDataModel.clickAudio.audioClip != null)
            {
                AudioSource.PlayClipAtPoint(hotspotDataModel.clickAudio.audioClip, AbstractImmersiveCamera.CurrentImmersiveCamera.transform.position, hotspotDataModel.clickAudio.audioVolume);
            }

            //Inform Hotspot controller that popup has been opened
            if (controller != null) controller.PopUpOpened();
             
//#if UNITY_WEBGL && !UNITY_EDITOR
//            WebGLCommunication._instance.CallWeb("" + hotspotDataModel.actionType);
//#endif
            //Select and instantiate the correct pop up type
            switch (hotspotDataModel.actionType)
            {
                case ActionType.ImagePopup:
                    InstantiateImagePopUp();
                    break;
                case ActionType.ImageSequencePopup:
                    InstantiateImageSequencePopUp();
                    break;
                case ActionType.VideoPopup:
                    InstantiateVideoPopUp();
                    break;
                case ActionType.TextPopup:
                    InstantiateTextPopUp();
                    break;
                case ActionType.QuizPopup:
                    InstantiateQAndAPopUp();
                    break;
                case ActionType.SceneLink:
                    ChangeScene();
                    break;
                case ActionType.ActivateAndHideObjects:
                    HideOrRevealObjects();
                    break;
                case ActionType.AudioPopup:
                    InstantiateAudioPopUp();
                    break;
                case ActionType.SplitPopup:
                    InstantiateSplitPopup();
                    break;
            }

            if (presentingMultiHotspot != null) presentingMultiHotspot.ChildHotspotOpened(this, hotspotDataModel.clickAction);

            if (IsPopupHotspot) HideDeleteOrDisableOnClick();
        }


        /// <summary>
        /// Either Hides, Deletes or Disables the hotspot when clicked.
        /// Hide means that the hotspot disappears until the popup is closed.
        /// Delete permenantly deletes the hotspot.
        /// Disable leaves hotspots visible however is cannot be clicked until the popup is closed.
        /// </summary>
        private void HideDeleteOrDisableOnClick()
        {
            switch (hotspotDataModel.clickAction)
            {
                case OnClickAction.Disable:
                    IsInteractable = false;
                    break;
                case OnClickAction.Hide:
                case OnClickAction.Delete:
                    gameObject.SetActive(false);
                    break;
            }
        }

        public void ActionComplete()
        {
            //Let Hotspot Action Complete Handlers Know that action is complete.
            foreach (var actionCompleteHandler in actionCompleteHandlers)
            {
                actionCompleteHandler.HotspotActionComplete();
            }

            //Is Part Of Multi-Hotspot
            if (presentingMultiHotspot != null)
            {
                ActionCompleteMultiHotspot();
            }
            //Not Part of Multi-Hotspot
            else
            {
                ActionCompleteController();
            }

        }

        /// <summary>
        /// Popup has been close and hotspot has been presented by a multi hotspot.
        /// </summary>
        private void ActionCompleteMultiHotspot()
        {
            presentingMultiHotspot.ActionComplete();

            if (hotspotDataModel.clickAction == OnClickAction.Delete)
            {
                presentingMultiHotspot.DestroyChildHotspot(gameObject);
            }
        }

        /// <summary>
        /// Popup has been close and hotspot has not been presented by a multi hotspot.
        /// </summary>
        private void ActionCompleteController()
        {
//#if UNITY_WEBGL && !UNITY_EDITOR
//            WebGLCommunication._instance.CallWeb("Close " + hotspotDataModel.actionType);
//#endif
            controller.PopUpClosed();

            if (firstOpen)
            {
                controller.HotspotHasBeenViewed();
                firstOpen = false;
            }

            switch (hotspotDataModel.clickAction)
            {
                case OnClickAction.Disable:
                    IsInteractable = true;
                    break;
                case OnClickAction.Hide:
                    gameObject.SetActive(true);
                    break;
                case OnClickAction.Delete:
                    gameObject.SetActive(true);
                    transform.localScale = Vector3.zero;
                    StartCoroutine(DestroyOnEndOfFrame());
                    break;
            }
        }



        //-------QUESTION POPUP--------
        private void InstantiateQAndAPopUp()
        {
            var popUp = Instantiate(qAndAPopUpPrefab, canvas.transform) as QuizHotspotPopUp;
            popUp.SpawningHotspot = this.gameObject;

            popUp.ActionComplete = ActionComplete;
            popUp.PlacePopUp = PlacePopUp;

            popUp.Init(quizPopUpDataModel);
            var popUpRect = popUp.GetComponent<RectTransform>();

            // NOTE: The positioning of the pop up is called by the pop up itself. 
        }

        //-------TEXT POPUP--------
        public TextHotspotPopUp InstantiateTextPopUp()
        {
            var popUp = Instantiate(textPopUpPrefab, canvas.transform) as TextHotspotPopUp;
            popUp.SpawningHotspot = this.gameObject;
            //Set the text of the pop up.

            popUp.ActionComplete = ActionComplete;
            popUp.PlacePopUp = PlacePopUp;

            popUp.Init(textPopUpDataModel);
            var popUpRect = popUp.GetComponent<RectTransform>();

            // NOTE: The positioning of the pop up is called by the pop up itself.

            return popUp;
        }

        //-------IMAGE POPUP--------
        private void InstantiateImagePopUp()
        {
            var popUp = Instantiate(imagePopUpPrefab, canvas.transform) as ImageHotspotPopUp;
            popUp.SpawningHotspot = this.gameObject;
            //Set the image colour and scale of the pop up.

            popUp.ActionComplete = ActionComplete;
            popUp.PlacePopUp = PlacePopUp;

            popUp.Init(imagePopUpDataModel);

            var popUpRect = popUp.GetComponent<RectTransform>();

            //Position PopUp
            PlacePopUp(popUpRect);
        }

        //-------IMAGE SEQUENCE POPUP--------
        private void InstantiateImageSequencePopUp()
        {
            var popUp = Instantiate(imageSequencePopUpPrefab, canvas.transform) as ImageSequenceHotspotPopup;
            popUp.SpawningHotspot = this.gameObject;

            popUp.ActionComplete = ActionComplete;
            popUp.PlacePopUp = PlacePopUp;

            //Set the image colour and scale of the pop up.
            popUp.Init(imageSequencePopUpDataModel);

            var popUpRect = popUp.GetComponent<RectTransform>();

            //Position PopUp
            PlacePopUp(popUpRect);
        }

        //-------VIDEO POPUP--------
        private void InstantiateVideoPopUp()
        {
            var popUp = Instantiate(videoPopUpPrefab, canvas.transform) as VideoHotspotPopUp;
            popUp.SpawningHotspot = this.gameObject;

            popUp.ActionComplete = ActionComplete;
            popUp.PlacePopUp = PlacePopUp;

            popUp.Init(videoPopUpDataModel);

            // NOTE: The positioning of the pop up is called by the pop up itself. 
            // This is because the size is not known until the video is prepared. 
        }

        //-------AUDIO POPUP--------

        private void InstantiateAudioPopUp()
        {
            var popUp = Instantiate(audioPopUpPrefab, canvas.transform) as AudioHotspotPopUp;
            popUp.SpawningHotspot = this.gameObject;

            popUp.ActionComplete = ActionComplete;
            popUp.PlacePopUp = PlacePopUp;

            popUp.Init(audioPopUpDataModel);
        }

        //-------SPLIT POPUP--------
        private void InstantiateSplitPopup()
        {
            var popup = Instantiate(splitPopupPrefab, canvas.transform) as SplitPopup;
            popup.SpawningHotspot = gameObject;

            popup.ActionComplete = ActionComplete;
            popup.PlacePopUp = PlacePopUp;

            popup.Init(splitPopupDataModel);
        }

        //-------SCENE LINK--------

        private void ChangeScene()
        {
            if (sceneLinkDataModel.popUpSetting.fadeOut)
            {
                FadeInAndOut.CurrentFadeInAndOut.active = true;
                FadeInAndOut.CurrentFadeInAndOut.FadeOut(sceneLinkDataModel.popUpSetting.fadeOutDuration, sceneLinkDataModel.popUpSetting.fadeColor, sceneLinkDataModel.popUpSetting.fadeOutAudio, ChangeSceneCompletionHandler);
                FadeInAndOut.FadeInNextScene(sceneLinkDataModel.popUpSetting.fadeOutDuration, sceneLinkDataModel.popUpSetting.fadeColor, sceneLinkDataModel.popUpSetting.fadeOutAudio);
                AbstractImmersiveCamera.CurrentImmersiveCamera.interactionOn = false;
            }
            else
            {
                SceneManager.LoadScene(sceneLinkDataModel.popUpSetting.linkName);
            }
        }

        private void ChangeSceneCompletionHandler()
        {
            SceneManager.LoadScene(sceneLinkDataModel.popUpSetting.linkName);
        }

        //-------HIDE OR REVEAL--------

        private void HideOrRevealObjects()
        {
            foreach (var obj in objectsToHide)
            {
                obj.SetActive(false);
            }

            foreach (var obj in objectsToReveal)
            {
                obj.SetActive(true);
            }

            ActionComplete();
        }



        IEnumerator DestroyOnEndOfFrame()
        {
            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();
            Destroy(gameObject);
        }


        //-------PLACE POPUPS--------
        public void PlacePopUp(RectTransform popUpRect)
        {
            switch (GetPopUpSetting().popUpPosition)
            {
                case PopUpPosition.SameAsHotspot: PlacePopupOnCanvas(popUpRect); break;
                case PopUpPosition.SurfaceCenter: PlacePopupAtCanvasCenter(popUpRect); break;
                case PopUpPosition.SurfaceCenterTop: PlacePopupAtCanvasCenterTop(popUpRect); break;
                case PopUpPosition.Custom: PlacePopupCustomPosition(popUpRect); break;
            }

            var popUp = popUpRect.GetComponent<HotspotPopUp>();
            var canvasRect = canvas.GetComponent<RectTransform>().rect;

            if (popUpRect.anchoredPosition.x > canvas.pixelRect.width / 2) popUp.DisplayCloseButtonLeft();
            else popUp.DisplayCloseButtonRight();

            popUp.SetControlPanelYOffset(canvasRect.height / 2 - popUpRect.anchoredPosition.y);
        }

        private void PlacePopupCustomPosition(RectTransform popUpRect)
        {
            var canvasRect = canvas.GetComponent<RectTransform>().rect;

            var popUpPosition = cam.WorldToViewportPoint(transform.position);
            popUpPosition = new Vector3(popUpPosition.x * canvasRect.width, popUpPosition.y * canvasRect.height, popUpPosition.z);

            popUpPosition.x += GetPopUpSetting().popUpPositionOffset.x;
            popUpPosition.y += GetPopUpSetting().popUpPositionOffset.y;
            popUpRect.anchoredPosition = popUpPosition;
        }

        private void PlacePopupAtCanvasCenter(RectTransform popUpRect)
        {
            var canvasRect = canvas.GetComponent<RectTransform>().rect;
            popUpRect.anchoredPosition = new Vector2(canvasRect.width / 2, canvasRect.height / 2);
        }

        private void PlacePopupAtCanvasCenterTop(RectTransform popUpRect)
        {
            var canvasRect = canvas.GetComponent<RectTransform>().rect;
            var yPos = canvasRect.height - popUpRect.rect.height / 2 - 50;
            popUpRect.anchoredPosition = new Vector2(canvasRect.width / 2, yPos);
        }

        private void PlacePopupOnCanvas(RectTransform popUpRect)
        {
            var canvasRect = canvas.GetComponent<RectTransform>().rect;

            var popUpPosition = cam.WorldToViewportPoint(transform.position);
            popUpPosition = new Vector3(popUpPosition.x * canvasRect.width, popUpPosition.y * canvasRect.height, popUpPosition.z);

            //Stop going over edge.
            // X-Axis
            if (popUpPosition.x - popUpRect.rect.width / 2 < 0) popUpPosition.x = popUpRect.rect.width / 2;
            if (popUpPosition.x + popUpRect.rect.width / 2 > canvasRect.width) popUpPosition.x = canvasRect.width - popUpRect.rect.width / 2;

            //Y-Axis
            if (popUpPosition.y - popUpRect.rect.height / 2 < 0) popUpPosition.y = popUpRect.rect.height / 2;
            if (popUpPosition.y + popUpRect.rect.height / 2 > canvasRect.height) popUpPosition.y = canvasRect.height - popUpRect.rect.height / 2;

            popUpRect.anchoredPosition = new Vector2(popUpPosition.x, popUpPosition.y);
        }

        PopUpSetting GetPopUpSetting()
        {
            PopUpSetting popUpSetting = new PopUpSetting();

            switch (hotspotDataModel.actionType)
            {
                case ActionType.ImagePopup:
                    popUpSetting = imagePopUpDataModel.popUpSetting;
                    break;
                case ActionType.ImageSequencePopup:
                    popUpSetting = imageSequencePopUpDataModel.popUpSetting;
                    break;
                case ActionType.VideoPopup:
                    popUpSetting = videoPopUpDataModel.popUpSetting;
                    break;
                case ActionType.TextPopup:
                    popUpSetting = textPopUpDataModel.popUpSetting;
                    break;
                case ActionType.QuizPopup:
                    popUpSetting = quizPopUpDataModel.popUpSetting;
                    break;
                case ActionType.SceneLink:
                    popUpSetting = sceneLinkDataModel.popUpSetting;
                    break;
                case ActionType.ActivateAndHideObjects:

                    break;
                case ActionType.AudioPopup:
                    popUpSetting = audioPopUpDataModel.popUpSetting;
                    break;
                case ActionType.SplitPopup:
                    popUpSetting = splitPopupDataModel.popUpSetting;
                    break;
            }

            return popUpSetting;
        }
    }
}