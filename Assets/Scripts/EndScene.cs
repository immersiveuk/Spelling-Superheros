using Immersive.SuperHero;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndScene : MonoBehaviour
{
    public void SimpleModeButton()
    {
        PlayerPrefs.SetInt("GameMode", 0);
        SuperHeroManager.Instance.ResetManager();
        SuperHeroManager.Instance.LoadScene("Stage1");
    }

    public void AdvancedModeButton()
    {
        PlayerPrefs.SetInt("GameMode", 1);
        SuperHeroManager.Instance.ResetManager();
        SuperHeroManager.Instance.LoadScene("Stage1");
    }
}
