using Com.Immersive.Cameras;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Immersive.SuperHero
{
    public class SuperHeroCreator : MonoBehaviour
    {
        public SuperHeroSettings superHero;
        public HorizontalScroll headsPanel, bodiesPanel, legsPanel;

        void Start()
        {
            this.transform.localScale = new Vector3(1 / FindObjectOfType<Stage>().transform.localScale.x, 1, 1);

            headsPanel.SetScroll(superHero.superHeroHeads);
            bodiesPanel.SetScroll(superHero.superHeroBodies);
            legsPanel.SetScroll(superHero.superHeroLegs);
        }

        public void ButtonLoadGame()
        {
            AppData.Instance.SetSuperHeroParts(headsPanel.GetSelectedPart(), bodiesPanel.GetSelectedPart(), legsPanel.GetSelectedPart());
            SceneManager.LoadScene("Super Hero Game");
        }
    }
}