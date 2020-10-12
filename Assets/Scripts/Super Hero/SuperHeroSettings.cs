using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Immersive.SuperHero
{
    [CreateAssetMenu(fileName = "New Super Hero Settings", menuName = "Super Hero/ Settings", order = 1)]
    public class SuperHeroSettings : ScriptableObject
    {
        public List<SuperHeroParts> superHeroHeads;
        public List<SuperHeroParts> superHeroBodies;
        public List<SuperHeroParts> superHeroLegs;
    }
}
