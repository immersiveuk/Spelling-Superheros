using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Immersive.SuperHero
{
    public class LaserTrail : MonoBehaviour
    {
        public TrailRenderer trail;
        public GameObject cube;
        public List<Sprite> explosionSprites;

        bool isSet = false;

        System.Action onAnemyDestroy;

        public void MoveTrail(Vector3 p1, Vector2 p2, Vector2 p3, System.Action action)
        {
            onAnemyDestroy = action;
            float transitionTime = Vector3.Distance(p1, p3);

            cube.transform.localEulerAngles = new Vector3(0, 0, Utils.AngleInDeg(p1, p2) - 90);

            iTween.MoveTo(trail.gameObject, iTween.Hash("x", p3.x, "y", p3.y, "z", p1.z, "islocal", false,
                "time", transitionTime, "easetype", iTween.EaseType.linear, "oncomplete", (System.Action<object>)(newValue =>
                {
                    Destroy(trail.gameObject);
                })));
        }

        void Update()
        {
            int count = trail.positionCount;

            if (!isSet && count > 1)
            {
                isSet = true;
                Vector3 start = trail.GetPosition(0);
                Vector3 end = trail.GetPosition(count - 1);

                cube.transform.localScale = new Vector3(0.01f, Vector3.Distance(end, start)/2f, 0.01f);
                cube.transform.position = (start + end) / 2;
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.GetComponent<Enemy>() && other.transform.localPosition.y < 0.5f)
            {
                //onAnemyDestroy();
                other.GetComponentInParent<SuperHeroGame>().OnEnemyDestoryCallback();
                CreateExplosion(other);
            }
        }

        void CreateExplosion(Collider other)
        {
            SpriteRenderer objExplosion = Utils.GetSpriteRenderer(null); 

            objExplosion.sprite = explosionSprites[Random.Range(0, explosionSprites.Count)];
            objExplosion.transform.position = new Vector3(other.transform.position.x, other.transform.position.y, -0.2f);
            
            objExplosion.sortingOrder = 2;

            objExplosion.transform.localScale = Vector3.zero;
            iTween.ScaleTo(objExplosion.gameObject, Vector3.one * 0.3f, 0.5f);

            Destroy(other.gameObject);
            Destroy(objExplosion.gameObject, 2);
        }
    }
}