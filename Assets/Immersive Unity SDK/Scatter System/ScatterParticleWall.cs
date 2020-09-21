using Com.Immersive.Cameras;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Com.Immersive.Scatter
{
    public class ScatterParticleWall : MonoBehaviour
    {
        public float padding = 0f;
        public float acceleration = 1f;

        private Camera lastCam;
        private Rigidbody rb;

        // Start is called before the first frame update
        void Start()
        {
            rb = GetComponent<Rigidbody>();
        }

        // Update is called once per frame
        void FixedUpdate()
        {
            var (cam, canvas) = AbstractImmersiveCamera.CurrentImmersiveCamera.FindRenderingCameraAndCanvas(gameObject);

            if (cam != null) lastCam = cam;

            if (lastCam == null) return;
            Vector3 viewportPos = lastCam.WorldToViewportPoint(transform.position);

            var rightForce = Mathf.Clamp(padding - viewportPos.x, 0, padding) / padding;
            var leftForce = Mathf.Clamp(viewportPos.x + padding - 1, 0, padding) / padding;
            var upForce = Mathf.Clamp(padding - viewportPos.y, 0, padding) / padding;
            var downForce = Mathf.Clamp(viewportPos.y + padding - 1, 0, padding) / padding;

            rightForce *= acceleration;
            leftForce *= acceleration;
            upForce *= acceleration;
            downForce *= acceleration;

            rb.AddForce(new Vector3(rightForce - leftForce, upForce - downForce, 0));
        }
    }
}
