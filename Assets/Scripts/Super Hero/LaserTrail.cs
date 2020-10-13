using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Immersive.SuperHero
{
    public class LaserTrail : MonoBehaviour
    {
        public TrailRenderer trail;
        public GameObject cube;

        bool isSet = false;

        void Update()
        {
            int count = trail.positionCount;

            if (!isSet && count > 1)
            {
                isSet = true;
                Vector3 start = trail.GetPosition(0);
                Vector3 end = trail.GetPosition(count - 1);

                cube.transform.localScale = new Vector3(0.01f, Vector3.Distance(end, start), 0.01f);
                cube.transform.position = (start + end) / 2;
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            CreateExplosion(other);
        }

        void CreateExplosion(Collider other)
        {
            SpriteRenderer objExplosion = Instantiate(SuperHeroManager.Instance.prefabSprite, null, false);
            objExplosion.sprite = SuperHeroManager.Instance.explosionSprites[Random.Range(0, SuperHeroManager.Instance.explosionSprites.Count)];
            objExplosion.transform.position = new Vector3(other.transform.position.x, other.transform.position.y, -0.2f);
            objExplosion.maskInteraction = SpriteMaskInteraction.None;
            objExplosion.sortingOrder = 2;

            objExplosion.transform.localScale = Vector3.zero;
            iTween.ScaleTo(objExplosion.gameObject, Vector3.one * 0.5f, 0.5f);

            Destroy(other.gameObject);
            Destroy(objExplosion.gameObject, 3);
        }
    }
}