using Com.Immersive.Cameras;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Immersive.SuperHero
{
    public class SuperHeroGame : MonoBehaviour
    {
        public SpriteRenderer head, body, leg;
        public LineRenderer lineRenderer;
        public Transform objTemp;

        void Start()
        {
            AbstractImmersiveCamera.CenterScreenTouched.AddListener(WallTouched);

            head.sprite = AppData.Instance.head.gameSprite;
            body.sprite = AppData.Instance.body.gameSprite;
            leg.sprite = AppData.Instance.leg.gameSprite;
        }

        private void WallTouched(Vector2 screenPosition, int cameraIndex, TouchPhase phase, int index)
        {
            switch (phase)
            {
                case TouchPhase.Began:
                    Camera cam = AbstractImmersiveCamera.CurrentImmersiveCamera.cameras[cameraIndex];

                    var touchPosition = cam.ScreenToWorldPoint(new Vector3(screenPosition.x, screenPosition.y, 0));
                    objTemp.position = touchPosition;
                    lineRenderer.SetPosition(1, new Vector3(objTemp.localPosition.x*10, objTemp.localPosition.y*10, -0.1f));
                    
                    break;

                case TouchPhase.Moved:
                    
                    break;

                case TouchPhase.Ended:
                   
                    break;
            }
        }
    }
}
