using Immersive.SuperHero;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Immersive.SuperHero.SuperHeroCreator;

namespace Immersive.SuperHero
{
    public class HorizontalScroll : MonoBehaviour
    {
        int partIndex = 0;
        int spriteIndex = -1;

        List<Transform> parts = new List<Transform>();
        List<SuperHeroParts> superHeroParts = new List<SuperHeroParts>();
        SuperHeroParts selectedPart;
         
        float gapValue;
        float transitionTime;

        public SuperHeroPart part;
        //public GameObject buttonsParent;

        public SpriteRenderer selected_Sprite;

        Action onScroll;

        private void Awake()
        {
            //buttonsParent.SetActive(false);

            transitionTime = 1.0f;
            gapValue = 0.35f;
            partIndex = 0;
        }

        public void SetScroll(List<SuperHeroParts> superHeroPart, Action action)
        {
            onScroll = action;
            //buttonsParent.SetActive(true);
            selected_Sprite.gameObject.SetActive(false);

            spriteIndex = 0;
            this.superHeroParts.AddRange(superHeroPart);
            IListExtensions.Shuffle(this.superHeroParts);

            for (int i = 0; i < 2; i++)
            {
                SpriteRenderer objPart = Utils.GetSpriteRenderer(this.transform);
                objPart.maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
                objPart.transform.localScale = Vector3.one * 0.8f;
                objPart.transform.localPosition = new Vector3(i * gapValue, 0, 0);

                objPart.gameObject.AddComponent<AnimationScript>();

                if (part == SuperHeroPart.Head)
                    objPart.GetComponent<AnimationScript>().Init(0.5f, superHeroParts[i].gameSprites);
                else
                    objPart.sprite = superHeroParts[i].creatorSprite;

                parts.Add(objPart.transform);
            }
        }

        public void SetSelectedSprite(SuperHeroParts selectedSprite)
        {
            selectedPart = selectedSprite;

            if (selectedSprite != null)
            {
                spriteIndex = 0;
                selected_Sprite.enabled = true;
                selected_Sprite.sprite = selectedSprite.creatorSprite;
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
            onScroll();

            scrolling = true;

            if (spriteIndex >= superHeroParts.Count)
                spriteIndex = 0;

            if (spriteIndex < 0)
                spriteIndex = superHeroParts.Count - 1;

            if (part == SuperHeroPart.Head)
                parts[1].GetComponent<AnimationScript>().Init(0.12f, superHeroParts[spriteIndex].gameSprites);
            else
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