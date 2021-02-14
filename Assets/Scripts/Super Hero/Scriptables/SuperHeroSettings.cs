using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Immersive.SuperHero
{
    [CreateAssetMenu(fileName = "New Super Hero Settings", menuName = "Super Hero/ Settings", order = 1)]
    public class SuperHeroSettings : ScriptableObject
    {
        public List<SuperHeroParts> heads = new List<SuperHeroParts>();
        public List<SuperHeroParts> bodies = new List<SuperHeroParts>();
        public List<SuperHeroParts> legs = new List<SuperHeroParts>();
    }

    public class SelectedSuperHero
    {
        public SuperHeroParts head;
        public SuperHeroParts body;
        public SuperHeroParts leg;

        public SelectedSuperHero() { }

        public SelectedSuperHero(SuperHeroSettings superHeroSetting)
        {
            this.head = superHeroSetting.heads[Random.Range(0, superHeroSetting.heads.Count)];
            this.body = superHeroSetting.bodies[Random.Range(0, superHeroSetting.bodies.Count)];
            this.leg = superHeroSetting.legs[Random.Range(0, superHeroSetting.legs.Count)];
        }        
    }
}
