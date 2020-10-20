using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Immersive.SuperHero
{
    public class Enemy : MonoBehaviour
    {
        Vector3 moveAround;
        bool hover;

        public float xSpread;
        public float zSpread;
        public float yOffset;
        public Transform centerPoint;

        public float rotSpeed;
        bool rotateClockwise;

        float timer = 0;

        private void Awake()
        {
            rotateClockwise = Random.Range(0, 100) > 50 ? true : false;
        }

        public void Init(Sprite enemySprite, Vector3 endPosition)
        {
            this.GetComponent<SpriteRenderer>().sprite = enemySprite;

            float transitionTime = Vector2.Distance(this.transform.localPosition, endPosition) * 5;

            moveAround = endPosition;
            Vector3 pos = GetPosition() + endPosition;

            iTween.MoveTo(this.gameObject, iTween.Hash("x", pos.x, "y", pos.y, "z", 0, "islocal", true,
              "time", transitionTime, "easetype", iTween.EaseType.linear, "oncomplete", (System.Action<object>)(newValue =>
              {
                  timer = 0;
                  hover = true;
              })));
        }

        private void Update()
        {
            if (hover)
            {
                timer += Time.deltaTime * rotSpeed;
                Rotate();
            }
        }

        void Rotate()
        {
            if (rotateClockwise)
            {
                float x = -Mathf.Cos(timer) * xSpread;
                float z = Mathf.Sin(timer) * zSpread;
                Vector3 pos = new Vector3(x, z, 0);
                transform.localPosition = pos + moveAround;
            }
            else
            {
                float x = Mathf.Cos(timer) * xSpread;
                float z = Mathf.Sin(timer) * zSpread;
                Vector3 pos = new Vector3(x, z, 0);
                transform.localPosition = pos + moveAround;
            }
        }

        Vector3 GetPosition()
        {
            Vector3 pos;

            if (rotateClockwise)
            {
                float x = -Mathf.Cos(0) * xSpread;
                float z = Mathf.Sin(0) * zSpread;
                pos = new Vector3(x, z, 0);                
            }
            else
            {
                float x = Mathf.Cos(0) * xSpread;
                float z = Mathf.Sin(0) * zSpread;
                pos = new Vector3(x, z, 0);                
            }

            return pos;
        }
    }
}
