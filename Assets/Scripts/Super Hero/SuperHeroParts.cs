using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Immersive.SuperHero
{
    [CreateAssetMenu(fileName = "New Super Hero Part", menuName = "Super Hero/ Parts", order = 1)]
    public class SuperHeroParts : ScriptableObject
    {
        public Sprite creatorSprite;
        public Sprite gameSprite;
        //public bool front;
    }
}