using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Immersive.SuperHero
{
    public class SuperHeroManager : Singleton<SuperHeroManager>
    {
        Dictionary<WallType, SuperHeroSettings> createdSuperHeros;

        public void GameButton()
        {
            createdSuperHeros = new Dictionary<WallType, SuperHeroSettings>();

            WallType wallType;
            SuperHeroSettings superHeroSettings;

            foreach (var obj in FindObjectsOfType<SuperHeroCreator>())
            {
                (wallType, superHeroSettings) = obj.GetSelectedSuperHero();
                createdSuperHeros.Add(wallType, superHeroSettings);
            }

            SceneManager.LoadScene("Super Hero Game");
        }

        public SuperHeroSettings GetSuperHero(WallType wallType)
        {
            return createdSuperHeros[wallType];
        }
    }
}