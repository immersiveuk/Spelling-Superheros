using Com.Immersive.Cameras;
using System.Collections;
using System.Collections.Generic;
using TMPro;
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

        public GameObject superHeroObj;
        public TrailRenderer prefabTrail;
        public Transform laserStartPoint;

        public GameObject laserIconHotspot;

        public TextMeshPro textEnemy;

        protected virtual void OnEnemyDestory() { }
        SelectedSuperHero superHero;

        public void Start()
        {
            this.transform.localScale = new Vector3(1 / FindObjectOfType<Stage>().transform.localScale.x, 1, 1);
           
            SetHero();
        }

        void SetHero()
        {
            superHero = GameData.Instance.GetSuperHero(wallType);

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

        public void AddWallTouchEvent()
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

        public void RemoveWallTouchEvent()
        {
            switch (wallType)
            {
                case WallType.Left:
                    AbstractImmersiveCamera.LeftScreenTouched.RemoveListener(WallTouched);
                    break;

                case WallType.Center:
                    AbstractImmersiveCamera.CenterScreenTouched.RemoveListener(WallTouched);
                    break;

                case WallType.Right:
                    AbstractImmersiveCamera.RightScreenTouched.RemoveListener(WallTouched);
                    break;
            }
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
            if (laserIconHotspot)
                laserIconHotspot.SetActive(false);

            var ray = AbstractImmersiveCamera.CurrentImmersiveCamera.cameras[cameraIndex].ScreenPointToRay(screenPosition);
            RaycastHit rayInfo;

            if (Physics.Raycast(ray, out rayInfo))
            {
                if (rayInfo.collider.gameObject.name.Equals("SuperHero"))
                {
                    Debug.Log(rayInfo.collider.gameObject.name);
                    return;
                }
                    
            }

            AbstractImmersiveCamera.PlayAudio(SuperHeroGameManager.Instance.laserBlastClip, 1);

            Camera cam = AbstractImmersiveCamera.CurrentImmersiveCamera.cameras[cameraIndex];
            var p2 = cam.ScreenToWorldPoint(new Vector3(screenPosition.x, screenPosition.y, -0.1f));

            float direction = p2.x - body.transform.position.x;
            Vector2 laserStartPosition = CheckSize(superHero.body?.gameSprites[2], body.transform);

            if (direction > 0)
            {
                body.sprite = superHero.body?.gameSprites[2];               
                laserStartPoint.localPosition = new Vector3(laserStartPosition.x, laserStartPosition.y, -0.1f);
            }
            else
            {
                body.sprite = superHero.body?.gameSprites[1];               
                laserStartPoint.localPosition = new Vector3(-laserStartPosition.x, laserStartPosition.y, -0.1f);
            }

            Vector2 p1 = laserStartPoint.position;

            TrailRenderer trail = Instantiate(prefabTrail, this.transform, false);
            trail.transform.position = p1;

            Vector2 p3 = Utils.GetNewPoint(p1, p2);

            trail.GetComponentInChildren<LaserTrail>().MoveTrail(p1, p2, p3, OnEnemyDestoryCallback);

            StopAllCoroutines();
            StartCoroutine(SetIdlePose()) ;
        }

        IEnumerator SetIdlePose()
        {
            yield return new WaitForSeconds(1);
            body.sprite = superHero.body?.gameSprites[0];
        }

        void OnEnemyDestoryCallback()
        {
            AbstractImmersiveCamera.PlayAudio(SuperHeroGameManager.Instance.explosionClip, 1);
            OnEnemyDestory();
        }

        Vector2 CheckSize(Sprite sprite, Transform obj)
        {
            float minX, maxX;
            float minY, maxY;

            float xSize = sprite.rect.width;
            float ySize = sprite.rect.height;

            minX = maxX = xSize / 2;
            minY = maxY = ySize / 2;

            for (int x = 0; x < xSize; x++)
            {
                for (int y = 0; y < ySize; y++)
                {
                    Color col = sprite.texture.GetPixel(x, y);

                    if (col.a != 0)
                    {
                        if (x < minX)
                            minX = x;

                        if (x > maxX)
                            maxX = x;

                        if (y < minY)
                            minY = y;

                        if (y > maxY)
                            maxY = y;
                    }
                }
            }

            float xPos = CalculateWorldPosOfPixelCoordinate((int)maxX, sprite.bounds.size.x, 0.5f, obj.localPosition.x, obj.localScale.x);
            float yPos = CalculateWorldPosOfPixelCoordinate((int)maxY, sprite.bounds.size.y, 0.32f, obj.localPosition.y, obj.localScale.y);

            Vector2 startPosition = new Vector2(xPos, yPos) - (Vector2.one) * 0.03f;

            return startPosition;
        }

        float CalculateWorldPosOfPixelCoordinate(int coord, float boundsSize, float pivot, float position, float scale)
        {
            float PixelInWorldSpace = 1.0f / 1080;
            float startPos = position - (boundsSize * pivot * scale);

            return startPos + (PixelInWorldSpace * coord) * scale;
        }

        public void OnComplete()
        {
            superHeroObj.GetComponent<Animation>().enabled = false;
            textEnemy.transform.parent.gameObject.SetActive(false);

            iTween.MoveTo(superHeroObj.gameObject, iTween.Hash("x", superHeroObj.transform.localPosition.x, "y", 1, "z", 0, "islocal", true,
                     "time", 2.0f, "easetype", iTween.EaseType.easeInOutQuad, "delay", 0, "oncomplete", (System.Action<object>)(newValue =>
                     {
                         superHeroObj.SetActive(false);
                     })));
        }
    }
}
