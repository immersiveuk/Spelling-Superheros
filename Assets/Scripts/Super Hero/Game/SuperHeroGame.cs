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

        [Header("SFX")]
        public AudioClip laserBlastClip;
        public AudioClip explosionClip;

        protected virtual void OnEnemyDestory() { }

        public void Start()
        {
            this.transform.localScale = new Vector3(1 / FindObjectOfType<Stage>().transform.localScale.x, 1, 1);
           
            SetWallTouchEvent();
            SetHero();
        }

        void SetHero()
        {
            SelectedSuperHero superHero = SuperHeroManager.Instance.GetSuperHero(wallType);

            if (superHero != null)
            {
                if (superHero.head)
                    head.GetComponent<AnimationScript>().Init(0.12f, superHero.head.gameSprites);

                if (superHero.body)
                    body.sprite = superHero.body?.gameSprites[0];

                if (superHero.leg)
                    leg.sprite = superHero.leg?.gameSprites[0];
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

            this.transform.position = new Vector3(AbstractImmersiveCamera.CurrentImmersiveCamera.cameras[(int)wallType].transform.position.x, 0, 0);
        }

        private void WallTouched(Vector2 screenPosition, int cameraIndex, TouchPhase phase, int index)
        {
            switch (phase)
            {
                case TouchPhase.Began:
                    CreateLaser(screenPosition, cameraIndex);
                    break;

                case TouchPhase.Moved:
                    break;

                case TouchPhase.Ended:
                    break;
            }
        }

        void CreateLaser(Vector2 screenPosition, int cameraIndex)
        {
            AbstractImmersiveCamera.PlayAudio(laserBlastClip, 1);

            Vector2 p1 = laserStartPoint.position;

            Camera cam = AbstractImmersiveCamera.CurrentImmersiveCamera.cameras[cameraIndex];
            var p2 = cam.ScreenToWorldPoint(new Vector3(screenPosition.x, screenPosition.y, -0.1f));

            TrailRenderer trail = Instantiate(prefabTrail, this.transform, false);
            trail.transform.position = p1;

            Vector2 p3 = Utils.GetNewPoint(p1, p2);

            trail.GetComponentInChildren<LaserTrail>().MoveTrail(p1, p2, p3, OnEnemyDestoryCallback);
        }

        void OnEnemyDestoryCallback()
        {
            AbstractImmersiveCamera.PlayAudio(explosionClip, 1);
            OnEnemyDestory();
        }
    }
}
