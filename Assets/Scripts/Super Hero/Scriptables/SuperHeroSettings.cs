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
    }
}
