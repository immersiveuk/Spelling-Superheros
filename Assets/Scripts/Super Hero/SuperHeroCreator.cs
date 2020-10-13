using Com.Immersive.Cameras;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Immersive.SuperHero
{
    public enum WallType { Left, Center, Right }

    public class SuperHeroCreator : MonoBehaviour
    {
        public WallType wallType;

        [Header("Settings")]
        public SuperHeroSettings superHero;

        [Header("Body Parts Scroll")]
        public HorizontalScroll headsPanel, bodiesPanel, legsPanel;

        void Start()
        {
            this.transform.localScale = new Vector3(1 / FindObjectOfType<Stage>().transform.localScale.x, 1, 1);

            headsPanel.SetScroll(superHero.heads);
            bodiesPanel.SetScroll(superHero.bodies);
            legsPanel.SetScroll(superHero.legs);
        }

        public (WallType, SuperHeroSettings) GetSelectedSuperHero()
        {
            SuperHeroSettings selectedSuperHero = new SuperHeroSettings();
            selectedSuperHero.heads.Add(headsPanel.GetSelectedPart());
            selectedSuperHero.bodies.Add(bodiesPanel.GetSelectedPart());
            selectedSuperHero.legs.Add(legsPanel.GetSelectedPart());

            return (wallType, selectedSuperHero);
        }
    }
}