using Com.Immersive.Cameras;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Com.Immersive.Hotspots
{
    public interface IHotspot : IInteractableObject
    {
        bool IsInteractable { get; }

        //OnClickAction ClickAction { get; set; }

        void ActionComplete();

        void DisableInteractivity();

        void EnableInteractivity();
    }
}

