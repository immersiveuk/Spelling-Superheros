using Com.Immersive.Cameras;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Immersive.SuperHero
{
    public class SuperHeroGame : MonoBehaviour
    {
        [Header("Wall Type")]
        public WallType wallType;

        [Header("Super Hero Sprites")]
        public SpriteRenderer head;
        public SpriteRenderer body;
        public SpriteRenderer leg;

        public TrailRenderer prefabTrail;
        public Transform laserStartPoint;

        void Start()
        {
            SetWallTouchEvent();
            SetHero();
        }

        void SetHero()
        {
            SuperHeroSettings superHero = SuperHeroManager.Instance.GetSuperHero(wallType);

            if (superHero != null)
            {
                head.sprite = superHero.heads[0].gameSprite;
                body.sprite = superHero.bodies[0].gameSprite;
                leg.sprite = superHero.legs[0].gameSprite;
            }
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

        /*
        private void WallTouched(Vector2 screenPosition, int cameraIndex, TouchPhase phase, int index)
        {
            switch (phase)
            {
                case TouchPhase.Began:

                    Camera cam = AbstractImmersiveCamera.CurrentImmersiveCamera.cameras[cameraIndex];
                    var touchPosition = cam.ScreenToWorldPoint(new Vector3(screenPosition.x, screenPosition.y, 0));

                    Vector3 endPosition = touchPosition;

                    TrailRenderer trail = Instantiate(prefabTrail, null, false);
                    trail.transform.position = laserStartPoint.position;

                    float distance = Vector3.Distance(trail.transform.position, endPosition);

                    float dx = (endPosition.x - laserStartPoint.position.x)/distance;
                    float dy = (endPosition.y - laserStartPoint.position.y)/distance;

                    Vector2 newPos = new Vector2(laserStartPoint.position.x + 5 * dx, laserStartPoint.position.y + 5 * dy);

                    float transitionTime = distance*0.2f;

                    iTween.MoveTo(trail.gameObject, iTween.Hash("x", newPos.x, "y", newPos.y, "z", 0, "islocal", false,
                        "time", transitionTime, "easetype", iTween.EaseType.linear, "oncomplete", (System.Action<object>)(newValue =>
                            {
                            })));

                    break;

                case TouchPhase.Moved:
                    
                    break;

                case TouchPhase.Ended:
                    isClicked = true;
                    break;
            }
        }
        */


        
        private void WallTouched(Vector2 screenPosition, int cameraIndex, TouchPhase phase, int index)
        {
            switch (phase)
            {
                case TouchPhase.Began:

                    Vector2 p1 = laserStartPoint.position;

                    Camera cam = AbstractImmersiveCamera.CurrentImmersiveCamera.cameras[cameraIndex];
                    var p2 = cam.ScreenToWorldPoint(new Vector3(screenPosition.x, screenPosition.y, 0));

                    TrailRenderer trail = Instantiate(prefabTrail, this.transform, false);
                    trail.transform.position = p1;

                    Vector2 p3 = GetNewPoint(p1, p2);

                    float transitionTime = Vector3.Distance(p1, p3);

                    trail.GetComponentInChildren<LaserTrail>().cube.transform.localEulerAngles = new Vector3(0, 0, AngleInDeg(p1, p2)-90);

                    iTween.MoveTo(trail.gameObject, iTween.Hash("x", p3.x, "y", p3.y, "z", 0, "islocal", false,
                        "time", transitionTime, "easetype", iTween.EaseType.linear, "oncomplete", (System.Action<object>)(newValue =>
                        {
                            Destroy(trail.gameObject);
                        })));

                    
                    break;

                case TouchPhase.Moved:
                    break;

                case TouchPhase.Ended:
                    break;
            }
        }

        Vector2 GetNewPoint(Vector2 p1, Vector2 p2)
        {
            float newPointDistance = 5;

            float distance = Vector3.Distance(p1, p2);

            float dx = (p2.x - p1.x) / distance;
            float dy = (p2.y - p1.y) / distance;

            Vector2 p3 = new Vector2(p1.x + newPointDistance * dx, p1.y + newPointDistance * dy);

            return p3;
        }

        public static float AngleInRad(Vector3 vec1, Vector3 vec2)
        {
            return Mathf.Atan2(vec2.y - vec1.y, vec2.x - vec1.x);
        }

        //This returns the angle in degrees
        public static float AngleInDeg(Vector3 vec1, Vector3 vec2)
        {
            return AngleInRad(vec1, vec2) * 180 / Mathf.PI;
        }
    }
}
