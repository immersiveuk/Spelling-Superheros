using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Com.Immersive.Hotspots
{
    public abstract class CustomPopUpSpawner: ScriptableObject 
    {
        public abstract void InstantiatePopUp(Camera cam, Canvas canvas, Vector3 worldPos, Action onCompleteAction, HotspotPopUp.PopupEventHandlerRetrieverDelegate popUpEventHandlerRetriever);
    }
   
    public abstract class CustomPopUpSpawner<TPopUpSettings, TPopUp> : CustomPopUpSpawner where TPopUpSettings : PopUpSettings where TPopUp : HotspotPopUp<TPopUpSettings>
    {
        [SerializeField] TPopUp popUpPrefab = null;
        [SerializeField] TPopUpSettings popUpSettings = null;

        public TPopUp PopUpPrefab => popUpPrefab;
        public TPopUpSettings PopUpSettings => popUpSettings;

        public override void InstantiatePopUp(Camera cam, Canvas canvas, Vector3 worldPos, Action onCompleteAction, HotspotPopUp.PopupEventHandlerRetrieverDelegate popUpEventHandlerRetriever)
        {
            TPopUp popUp = Instantiate(popUpPrefab, canvas.transform);
            PopUpPositioner positioner = popUpSettings.GetPopUpPositioner(cam, canvas, worldPos);
            popUp.Initialize(popUpSettings, positioner, onCompleteAction, popUpEventHandlerRetriever);
        }
    }
}