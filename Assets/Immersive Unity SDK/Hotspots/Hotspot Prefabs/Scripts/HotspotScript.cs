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
using UnityEngine.Events;
using UnityEngine.Serialization;

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
        //Quiz V2
        public QuizPopUpSetting_V2 quizPopUpDataModel_V2; //To-DO
        //TextMulti
        public TextSequencePopUpSetting textSequencePopUpDataModel;

        //Reveal And Hide Objects
        public GameObject[] objectsToHide;
        public GameObject[] objectsToReveal;

        //Event
        public UnityEvent hotspotEvent;

        //Custom PopUp
        public CustomPopUpSpawner customPopUpSpawner = null;

        //-------POPUP PREFABS--------
        public VideoHotspotPopUp videoPopUpPrefab;
        public ImageHotspotPopUp imagePopUpPrefab;
        public ImageSequenceHotspotPopup imageSequencePopUpPrefab;
        public TextHotspotPopUp textPopUpPrefab;
        public QuizHotspotPopUp qAndAPopUpPrefab;
        public AudioHotspotPopUp audioPopUpPrefab;
        public SplitPopup splitPopupPrefab;
        public QuizHotspotPopUp_V2 quizPopUpPrefab_V2;
        public TextSequenceHotspotPopUp textSequencePopUpPrefab;

        //-------PRIVATE VARIABLES--------

        private bool firstOpen = true;

        public bool IsInteractable { get; set; } = true;
        public bool IsPopupHotspot => (hotspotDataModel.actionType == ActionType.ImagePopup || hotspotDataModel.actionType == ActionType.VideoPopup || hotspotDataModel.actionType == ActionType.TextPopup || hotspotDataModel.actionType == ActionType.QuizPopup || hotspotDataModel.actionType == ActionType.ImageSequencePopup || hotspotDataModel.actionType == ActionType.AudioPopup || hotspotDataModel.actionType == ActionType.SplitPopup || hotspotDataModel.actionType == ActionType.QuizPopup_V2 || hotspotDataModel.actionType == ActionType.TextSequencePopup);

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
        }

        public void DisableInteractivity()
        {
            IsInteractable = false;
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
                    InstantiatePopUp(imagePopUpPrefab, imagePopUpDataModel.popUpSetting);
                    break;
                case ActionType.ImageSequencePopup:
                    InstantiatePopUp(imageSequencePopUpPrefab, imageSequencePopUpDataModel.popUpSetting);
                    break;
                case ActionType.VideoPopup: 
                    InstantiatePopUp(videoPopUpPrefab, videoPopUpDataModel.popUpSetting);
                    break;
                case ActionType.TextPopup:
                    InstantiatePopUp(textPopUpPrefab, textPopUpDataModel.popUpSetting);
                    break;
                case ActionType.QuizPopup:
                    InstantiatePopUp(qAndAPopUpPrefab, quizPopUpDataModel.popUpSetting);
                    break;
                case ActionType.AudioPopup:
                    InstantiatePopUp(audioPopUpPrefab, audioPopUpDataModel.popUpSetting);
                    break;
                case ActionType.SplitPopup:
                    InstantiatePopUp(splitPopupPrefab, splitPopupDataModel.popUpSetting);
                    break;
                case ActionType.QuizPopup_V2:
                    InstantiatePopUp(quizPopUpPrefab_V2, quizPopUpDataModel_V2);
                    break;
                case ActionType.TextSequencePopup:
                    InstantiatePopUp(textSequencePopUpPrefab, textSequencePopUpDataModel);
                    break;
                case ActionType.SceneLink:
                    ChangeScene();
                    break;
                case ActionType.ActivateAndHideObjects:
                    HideOrRevealObjects();
                    break;
                case ActionType.Event:
                    CallHotspotEvent();
                    break;

                case ActionType.CustomPopUp:
                    customPopUpSpawner?.InstantiatePopUp(cam, canvas, transform.position, ActionComplete, GetComponents);
                    break;
            }

            if (presentingMultiHotspot != null) presentingMultiHotspot.ChildHotspotOpened(this, hotspotDataModel.clickAction);

            if (IsPopupHotspot) StartCoroutine(HideDeleteOrDisableOnClick());
        }


        /// <summary>
        /// Either Hides, Deletes or Disables the hotspot when clicked.
        /// Hide means that the hotspot disappears until the popup is closed.
        /// Delete permenantly deletes the hotspot.
        /// Disable leaves hotspots visible however is cannot be clicked until the popup is closed.
        /// </summary>
        private IEnumerator HideDeleteOrDisableOnClick()
        {
            yield return new WaitForEndOfFrame();
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
                    IsInteractable = false;
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

        //private PopUpPositioner GetPopUpPositioner(PopUpSettings popUpSettings)
        //{
        //    switch (popUpSettings.popUpPosition)
        //    {
        //        case PopUpPosition.SurfaceCenterTop:
        //            return new PopUpPositionerCentredOnCanvasTop(canvas);
        //        case PopUpPosition.SurfaceCenter:
        //            return new PopUpPositionerCentredOnCanvas(canvas);
        //        case PopUpPosition.SameAsHotspot:
        //            return new PopUpPositionerCentredOnHotspot(transform.position, cam, canvas);
        //        case PopUpPosition.Custom:
        //            return new PopUpPositionerOffsetFromHotspot(transform.position, cam, popUpSettings.popUpPositionOffset, canvas);
        //        default:
        //            Debug.LogError("Could create IPopUpPositioner for "+popUpSettings.popUpPosition);
        //            return null;
        //    }
        //}

        private void InstantiatePopUp<TPopUp, TPopUpSettings>(TPopUp prefab, TPopUpSettings popUpSettings) where TPopUp : HotspotPopUp<TPopUpSettings> where TPopUpSettings : PopUpSettings
        {
            TPopUp popUp = Instantiate(prefab, canvas.transform);
            PopUpPositioner positioner = popUpSettings.GetPopUpPositioner(cam, canvas, transform.position);//GetPopUpPositioner(popUpSettings);

            //NOTE: This allows the Hotspot Controller to define the close button. This is a bit of a hack.
            if (popUpSettings.overrideDefaultCloseButton == false && controller != null && controller.closeButton != null)
            {
                popUpSettings.overrideDefaultCloseButton = true;
                popUpSettings.closeButton = controller.closeButton;
            }

            popUp.Initialize(popUpSettings, positioner, ActionComplete, GetComponents);
        }

        //-------SCENE LINK--------

        private void ChangeScene()
        {
            if (sceneLinkDataModel.sceneLinkSettings.fadeOut)
            {
                FadeInAndOut.CurrentFadeInAndOut.active = true;
                FadeInAndOut.CurrentFadeInAndOut.FadeOut(sceneLinkDataModel.sceneLinkSettings.fadeOutDuration, sceneLinkDataModel.sceneLinkSettings.fadeColor, sceneLinkDataModel.sceneLinkSettings.fadeOutAudio, ChangeSceneCompletionHandler);
                FadeInAndOut.FadeInNextScene(sceneLinkDataModel.sceneLinkSettings.fadeOutDuration, sceneLinkDataModel.sceneLinkSettings.fadeColor, sceneLinkDataModel.sceneLinkSettings.fadeOutAudio);
                AbstractImmersiveCamera.CurrentImmersiveCamera.interactionOn = false;
            }
            else
            {
                SceneManager.LoadScene(sceneLinkDataModel.sceneLinkSettings.linkName);
            }
        }

        private void ChangeSceneCompletionHandler()
        {
            SceneManager.LoadScene(sceneLinkDataModel.sceneLinkSettings.linkName);
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

        //-------HOTSPOT EVENT--------

        private void CallHotspotEvent()
        {
            hotspotEvent.Invoke();
            ActionComplete();
        }


        IEnumerator DestroyOnEndOfFrame()
        {
            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();
            Destroy(gameObject);
        }
    }
}