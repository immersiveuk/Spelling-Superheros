using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Immersive.SuperHero
{
    public class SuperHero : MonoBehaviour
    {
        [Header("Super Hero Sprites")]
        public SpriteRenderer head;
        public SpriteRenderer body;
        public SpriteRenderer leg;

        public void SetSuperHero(SelectedSuperHero superHero)
        {
            if (superHero.head)
                head.GetComponent<AnimationScript>().Init(0.12f, superHero.head.gameSprites);

            if (superHero.body)
                body.sprite = superHero.body?.gameSprites[0];

            if (superHero.leg)
                leg.sprite = superHero.leg?.gameSprites[0];

            FlyOut();
        }

        public void FlyOut()
        {
            iTween.MoveTo(this.gameObject, iTween.Hash("x", this.transform.localPosition.x, "y", 1, "z", 0, "islocal", true,
                     "time", 4.0f, "easetype", iTween.EaseType.easeInOutQuad, "delay", Random.Range(0.0f, 3.0f), "oncomplete", (System.Action<object>)(newValue =>
                      {
                          this.gameObject.SetActive(false);
                      })));
        }
    }
}
