using Immersive.SuperHero;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleScene : MonoBehaviour
{

    [Header("Settings")]
    public SuperHeroSettings superHeroSettings;
    public List<SuperHero> superHeros;

    void Start()
    {
        for (int i=0; i<superHeros.Count; i++)
        {
            SelectedSuperHero selected = new SelectedSuperHero(superHeroSettings);
            superHeros[i].SetSuperHero(selected);
        }
    }
}
