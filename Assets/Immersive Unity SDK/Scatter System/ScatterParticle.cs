/* Copyright (C) Immersive Interactive, Ltd - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Luke Bissell <luke@immersive.co.uk>, Dec 2019
 */

using Com.Immersive.Cameras;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Com.Immersive.Scatter
{
    /// <summary>
    /// Script which will apply a force which will move the object towards to screen.
    /// </summary>
    public class ScatterParticle : MonoBehaviour
    {
        public float padding = 0.2f;
        public float acceleration = 1f;

        private Camera floorCam;
        private Rigidbody rb;
        // Start is called before the first frame update
        void Start()
        {
            floorCam = AbstractImmersiveCamera.CurrentImmersiveCamera.floorCamera;
            rb = GetComponent<Rigidbody>();
        }

        // Update is called once per frame
        void FixedUpdate()
        {
            if (floorCam == null) return;
            Vector3 viewportPos = floorCam.WorldToViewportPoint(transform.position);

            var rightForce = Mathf.Clamp(padding - viewportPos.x, 0, padding) / padding;
            var leftForce = Mathf.Clamp(viewportPos.x + padding - 1, 0, padding) / padding;
            var upForce = Mathf.Clamp(padding - viewportPos.y, 0, padding) / padding;
            var downForce = Mathf.Clamp(viewportPos.y + padding - 1, 0, padding) / padding;

            rightForce *= acceleration;
            leftForce *= acceleration;
            upForce *= acceleration;
            downForce *= acceleration;

            rb.AddForce(new Vector3(rightForce - leftForce, 0, upForce - downForce));
        }
    }
}
