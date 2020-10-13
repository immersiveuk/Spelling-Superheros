using Immersive.SuperHero;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Immersive.SuperHero.SuperHeroCreator;

namespace Immersive.SuperHero
{
    public class HorizontalScroll : MonoBehaviour
    {
        //public SpriteRenderer prefabSprite;
        int partIndex = 0;
        int spriteIndex;

        List<Transform> parts = new List<Transform>();
        List<SuperHeroParts> superHeroParts;

        float gapValue;
        float transitionTime;

        //public WallType wallType;

        private void Awake()
        {
            transitionTime = 1.0f;
            gapValue = 0.35f;
            partIndex = 0;
        }

        public void SetScroll(List<SuperHeroParts> superHeroParts)
        {
            this.superHeroParts = superHeroParts;

            for (int i = 0; i < 2; i++)
            {
                SpriteRenderer objPart = Instantiate(SuperHeroManager.Instance.prefabSprite, this.transform, false);
                objPart.sprite = superHeroParts[i].creatorSprite;
                objPart.transform.localPosition = new Vector3(i * gapValue, 0, 0);

                parts.Add(objPart.transform);
            }
        }

        bool scrolling;

        public void MoveNext()
        {
            if (scrolling)
                return;

            partIndex++;
            spriteIndex++;

            Scroll(-1);
        }

        public void MovePrevious()
        {
            if (scrolling)
                return;

            partIndex--;
            spriteIndex--;

            Scroll(1);
        }

        void Scroll(int direction)
        {
            scrolling = true;

            if (spriteIndex >= superHeroParts.Count)
                spriteIndex = 0;

            if (spriteIndex < 0)
                spriteIndex = superHeroParts.Count - 1;

            parts[1].GetComponent<SpriteRenderer>().sprite = superHeroParts[spriteIndex].creatorSprite;
            parts[1].localPosition = new Vector3(partIndex * gapValue, 0, 0);

            iTween.MoveBy(this.gameObject, iTween.Hash("x", direction * gapValue, "y", 0, "z", 0, "islocal", false, "time", transitionTime,
                "easetype", iTween.EaseType.easeOutQuad, "oncomplete", (System.Action<object>)(newValue =>
                {
                    Transform temp = parts[0];
                    parts.Remove(temp);
                    parts.Add(temp);

                    scrolling = false;
                })));
        }

        public SuperHeroParts GetSelectedPart()
        {
            return superHeroParts[spriteIndex];
        }
    }
}