using Com.Immersive.Cameras;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Immersive.SuperHero
{
    public class SuperHeroCreator : MonoBehaviour
    {
        public SuperHeroSettings superHero;

        public GameObject headsPanel, bodiesPanel, legsPanel;
        public SpriteRenderer prefabSprite;

        float gapValue;
        float transitionTime;

        void Start()
        {
            transitionTime = 1.0f;
            gapValue = 0.3f;

            this.transform.localScale = new Vector3(1 / FindObjectOfType<Stage>().transform.localScale.x, 1, 1);
            CreateSuperHeroParts(superHero.superHeroHeads, headsPanel.transform);
            CreateSuperHeroParts(superHero.superHeroBodies, bodiesPanel.transform);
            CreateSuperHeroParts(superHero.superHeroLegs, legsPanel.transform);
        }

        void CreateSuperHeroParts(List<SuperHeroParts> superHeroParts, Transform parent)
        {
            for (int i=0; i< superHeroParts.Count; i++)
            {
                SpriteRenderer objPart = Instantiate(prefabSprite, parent, false);
                objPart.sprite = superHeroParts[i].creatorSprite;

                objPart.transform.localPosition = new Vector3(i * gapValue, 0, superHeroParts[i].front == true ? -0.01f : 0);
            }
        }

        public void NextHeadButton()
        {
            if (headsPanel.transform.localPosition.x > (superHero.superHeroHeads.Count - 1) * -gapValue + 0.1f)
                iTween.MoveBy(headsPanel, new Vector3(-gapValue, 0, 0), transitionTime);
        }

        public void PreviouHeadButton()
        {
            if (headsPanel.transform.localPosition.x < -gapValue/2)
                iTween.MoveBy(headsPanel, new Vector3(gapValue, 0, 0), transitionTime);
        }

        public void NextBodyButton()
        {
            if (bodiesPanel.transform.localPosition.x > (superHero.superHeroBodies.Count - 1) * -gapValue + 0.1f)
                iTween.MoveBy(bodiesPanel, new Vector3(-gapValue, 0, 0), transitionTime);
        }

        public void PreviouBodyButton()
        {
            if (bodiesPanel.transform.localPosition.x < -gapValue/2)
                iTween.MoveBy(bodiesPanel, new Vector3(gapValue, 0, 0), transitionTime);
        }

        public void NextLegButton()
        {
            if (legsPanel.transform.localPosition.x > (superHero.superHeroLegs.Count - 1) * -gapValue + 0.1f)
                iTween.MoveBy(legsPanel, new Vector3(-gapValue, 0, 0), transitionTime);
        }

        public void PreviouLegButton()
        {
            if (legsPanel.transform.localPosition.x < -gapValue/2)
                iTween.MoveBy(legsPanel, new Vector3(gapValue, 0, 0), transitionTime);
        }
    }
}