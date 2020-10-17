using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Immersive.SuperHero
{
    public class Enemy : MonoBehaviour
    {
        Vector3 moveAround;
        bool hover;

        //public Transform center;
        //public Vector3 axis = Vector3.forward;
        //public Vector3 desiredPosition;
        //public float radius = 2.0f;
        //public float radiusSpeed = 0.5f;
        //public float rotationSpeed = 80.0f;

        public float xSpread;
        public float zSpread;
        public float yOffset;
        public Transform centerPoint;

        public float rotSpeed;
        bool rotateClockwise;

        float timer = 0;

        private void Start()
        {
            rotateClockwise = Random.Range(0, 100) > 50 ? true : false;
        }

        public void Init(Sprite enemySprite, Vector3 endPosition)
        {
            this.GetComponent<SpriteRenderer>().sprite = enemySprite;

            float transitionTime = Vector2.Distance(this.transform.localPosition, endPosition);

            iTween.MoveTo(this.gameObject, iTween.Hash("x", endPosition.x, "y", endPosition.y, "z", 0, "islocal", true,
              "time", transitionTime * 5, "easetype", iTween.EaseType.linear, "oncomplete", (System.Action<object>)(newValue =>
              {
                  Debug.Log(transform.position);
                  moveAround = this.transform.position + new Vector3(0.1f, 0);
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
                transform.position = pos + moveAround;
            }
            else
            {
                float x = Mathf.Cos(timer) * xSpread;
                float z = Mathf.Sin(timer) * zSpread;
                Vector3 pos = new Vector3(x, z, 0);
                transform.position = pos + moveAround;
            }
        }
    }
}
