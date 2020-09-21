/* Copyright (C) Immersive Interactive, Ltd - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Luke Bissell <luke@immersive.co.uk>, July 2019
 */

using Com.Immersive.Cameras;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Com.Immersive.Hotspots
{
    /// <summary>
    /// A script applied to image hotspots.
    /// Adds a Polygon Collider to the hotspot.
    /// </summary>
    public class ImageHotspot : MonoBehaviour, IInteractableObject
    {
        public AudioClip clickAudioClip;

        private IHotspot hotspot;
        private SpriteRenderer spriteRend;
        private Color initialColor;

        // Start is called before the first frame update
        void Start()
        {
            //Add box collider so touches are caught.
            if (GetComponent<BoxCollider>() == null)
            {
                BoxCollider collider = gameObject.AddComponent<BoxCollider>();
                collider.size = new Vector3(collider.size.x, collider.size.y, 0.01f);
            }


            //Get Sprite Info
            spriteRend = GetComponent<SpriteRenderer>();
            initialColor = spriteRend.color;

            //Get Hotspot Script
            hotspot = GetComponent<IHotspot>();

            //Create hotspot glow if it's enabled from HotspotController.
            HotspotController hotspotController = this.gameObject.GetComponentInParent<HotspotController>();
            if(hotspotController != null && hotspotController.hotspotEffects == HotspotController.HotspotEffects.Glow)
            {
                this.gameObject.AddComponent<CreateHotspotGlow>().SetValue(hotspotController.hotspotGlowSettings);
            }                
        }

        private bool firstHeldFrame = true;

        public void OnTouchEnter()
        {
            if (!hotspot.IsInteractable) return;

            if (firstHeldFrame)
            {
                spriteRend.color = new Color(initialColor.r * 0.6f, initialColor.g * 0.6f, initialColor.b * 0.6f);
                if (clickAudioClip != null) AudioSource.PlayClipAtPoint(clickAudioClip, transform.position, 0.4f);
                firstHeldFrame = false;
            }
        }

        public void OnTouchExit()
        {
            if (!hotspot.IsInteractable) return;

            if (spriteRend != null ) spriteRend.color = initialColor;
            if (clickAudioClip != null ) AudioSource.PlayClipAtPoint(clickAudioClip, transform.position, 0.4f);
            firstHeldFrame = true;
        }


        public void OnPress() {}
        
        //Reset color
        public void OnRelease()
        {
            spriteRend.color = initialColor;
            firstHeldFrame = true;
        }
    }
}