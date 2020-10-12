using Immersive.SuperHero;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AppData : Singleton<AppData>
{
    public SuperHeroParts head, body, leg;

    public void SetSuperHeroParts(SuperHeroParts head, SuperHeroParts body, SuperHeroParts leg)
    {
        this.head = head;
        this.body = body;
        this.leg = leg;
    }
}
