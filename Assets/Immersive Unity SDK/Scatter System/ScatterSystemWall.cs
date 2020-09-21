using Com.Immersive.Cameras;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Com.Immersive.Scatter
{
    public class ScatterSystemWall : MonoBehaviour
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

            var cameras = AbstractImmersiveCamera.CurrentImmersiveCamera.wallCameras;
            if (cameras == null || cameras.Count == 0) return;

            for (int i = 0; i < numberOfParticles; i++)
            {
                var scatterParticle = CreateNewScatterParticle();
                var camera = GetRandonCamera(cameras);
                scatterParticle.transform.parent = transform;

                scatterParticle.transform.position = camera.ViewportToWorldPoint(new Vector3(Random.value * (1 - padding), Random.value * (1 - padding), camera.nearClipPlane + 1));
                scatterParticle.transform.localEulerAngles = new Vector3(0, 0, 0);
            }
        }

        private Camera GetRandonCamera(List<Camera> cameras)
        {
            var index = Random.Range(0, cameras.Count);
            return cameras[index];
        }

        private GameObject CreateNewScatterParticle()
        {
            var obj = new GameObject("Scatter Particle");

            var spriteRenderer = obj.AddComponent<SpriteRenderer>();
            spriteRenderer.sprite = sprites[Random.Range(0, sprites.Length)];

            var rb = obj.AddComponent<Rigidbody>();
            rb.mass = mass;
            rb.drag = drag;
            rb.useGravity = false;
            rb.angularDrag = angularDrag;
            rb.constraints = RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY;

            obj.AddComponent<BoxCollider>();

            var scatterParticle = obj.AddComponent<ScatterParticleWall>();
            scatterParticle.padding = padding;
            scatterParticle.acceleration = acceleration;

            return obj;
        }
    }
}
