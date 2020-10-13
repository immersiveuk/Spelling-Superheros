using Com.Immersive.Cameras;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Immersive.SuperHero
{
    public class SuperHeroGame : MonoBehaviour
    {
        public WallType wallType;

        public SpriteRenderer head, body, leg;

        private LineRenderer laser;

        void Start()
        {
            laser = GetComponentInChildren<LineRenderer>();

            SetWallTouchEvent();
            SetHero();
        }

        void SetHero()
        {
            SuperHeroSettings superHero = SuperHeroManager.Instance.GetSuperHero(wallType);
            head.sprite = superHero.heads[0].gameSprite;
            body.sprite = superHero.bodies[0].gameSprite;
            leg.sprite = superHero.legs[0].gameSprite;
        }

        void SetWallTouchEvent()
        {
            switch (wallType)
            {
                case WallType.Left:
                    AbstractImmersiveCamera.LeftScreenTouched.AddListener(WallTouched);
                    break;

                case WallType.Center:
                    AbstractImmersiveCamera.CenterScreenTouched.AddListener(WallTouched);
                    break;

                case WallType.Right:
                    AbstractImmersiveCamera.RightScreenTouched.AddListener(WallTouched);
                    break;
            }
        }

        private void WallTouched(Vector2 screenPosition, int cameraIndex, TouchPhase phase, int index)
        {
            switch (phase)
            {
                case TouchPhase.Began:
                    Camera cam = AbstractImmersiveCamera.CurrentImmersiveCamera.cameras[cameraIndex];

                    var touchPosition = cam.ScreenToWorldPoint(new Vector3(screenPosition.x, screenPosition.y, 0));

                    GameObject objTarget = new GameObject();
                    objTarget.transform.SetParent(laser.transform);
                    objTarget.transform.position = touchPosition;

                    laser.SetPosition(1, new Vector3(objTarget.transform.localPosition.x*10, objTarget.transform.localPosition.y*10, -0.1f));
                    
                    break;

                case TouchPhase.Moved:
                    
                    break;

                case TouchPhase.Ended:
                   
                    break;
            }
        }
    }
}
