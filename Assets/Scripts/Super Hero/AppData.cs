using Immersive.SuperHero;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AppData : Singleton<AppData>
{
    public SuperHeroSettings superHero_CenterWall;

    public void SetSuperHeroParts(SuperHeroParts head, SuperHeroParts body, SuperHeroParts leg)
    {
        //superHero_CenterWall.superHeroHeads.Add(head);
        //superHero_CenterWall.superHeroBodies.Add(body);
        //superHero_CenterWall.superHeroLegs.Add(leg);
    }
}
