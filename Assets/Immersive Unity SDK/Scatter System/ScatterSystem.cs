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
    /// This is a system which will add a series of particles which can be pushed around on the floor of an Immersive Room.
    /// </summary>
    public class ScatterSystem : MonoBehaviour
    {
        public float mass = 1;
        public float drag = 1.5f;
        public float angularDrag = 0.5f;

        [Range(0.05f, 0.5f)]
        public float padding = 0.2f;
        [Range(0.1f, 3f)]
        public float acceleration = 1;

        [Range(5, 100)]
        public int numberOfParticles = 10;

        public Sprite[] sprites = new Sprite[1];

        private void Start()
        {

            var floorCam = AbstractImmersiveCamera.CurrentImmersiveCamera.floorCamera;
            if (floorCam == null) return;
            for (int i = 0; i < numberOfParticles; i++)
            {
                var scatterParticle = CreateNewScatterParticle();
                scatterParticle.transform.parent = transform;
                scatterParticle.transform.position = floorCam.ViewportToWorldPoint(new Vector3(Random.value * (1 - padding), Random.value * (1 - padding), floorCam.nearClipPlane + 1));
                scatterParticle.transform.localEulerAngles = new Vector3(0, 0, 0);
            }
        }

        private GameObject CreateNewScatterParticle()
        {
            var obj = new GameObject("Scatter Particle");

            var spriteRenderer = obj.AddComponent<SpriteRenderer>();
            spriteRenderer.sprite = sprites[Random.Range(0, sprites.Length)];

            var rb = obj.AddComponent<Rigidbody>();
            rb.mass = mass;
            rb.drag = drag;
            rb.angularDrag = angularDrag;
            rb.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY;

            obj.AddComponent<BoxCollider>();

            var scatterParticle = obj.AddComponent<ScatterParticle>();
            scatterParticle.padding = padding;
            scatterParticle.acceleration = acceleration;

            return obj;
        }
    }
}
