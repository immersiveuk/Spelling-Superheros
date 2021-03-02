using Com.Immersive.Cameras;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Com.FireSafety
{
    public class ClickArea : MonoBehaviour, IInteractableObject
    {
        public enum ClickAreaType
        {
            None, Positive, Negative, Introduction, Good, Bad
        }
        [Space]
        public ClickAreaType clickAreaType;

        public Sprite onClickSprite;
        public AudioClip onClickAudio;


        [Serializable]
        public class ClickAreaEvent : UnityEvent<ClickArea> { }
        [Space]
        public ClickAreaEvent onClickAction;

        public bool IsInteractable { get; set; } = true;



        void Start()
        {
            IsInteractable = true;
        }

        public void OnPress()
        {

        }

        public void OnRelease()
        {
            switch (clickAreaType)
            {
                //case ClickAreaType.None:
                //    break;

                //case ClickAreaType.Positive:                    
                //    break;

                //case ClickAreaType.Negative:
                //    break;

                //case ClickAreaType.Introduction:
                //    break;

                case ClickAreaType.Good:
                    clickAreaType = ClickAreaType.Bad;
                    break;

                case ClickAreaType.Bad:
                    clickAreaType = ClickAreaType.Good;
                    break;
            }

            if (IsInteractable)
                onClickAction.Invoke(GetComponent<ClickArea>());

            if(clickAreaType == ClickAreaType.Positive)
                IsInteractable = false;
        }

        public void OnTouchEnter()
        {

        }

        public void OnTouchExit()
        {

        }


    }
}
