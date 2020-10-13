using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Immersive.SuperHero
{
    public class SuperHeroManager : Singleton<SuperHeroManager>
    {
        public SpriteRenderer prefabSprite;

        public List<Sprite> explosionSprites;

        Dictionary<WallType, SuperHeroSettings> createdSuperHeros = new Dictionary<WallType, SuperHeroSettings>();

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
            if (createdSuperHeros.Count > 0)
                return createdSuperHeros[wallType];
            else
                return null;
        }
    }
}