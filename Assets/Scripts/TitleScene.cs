using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleScene : MonoBehaviour
{
    public List<Animator> animators;

    void Start()
    {
        foreach (var anim in animators)
        {
            //anim.SetTrigger("Full");
            anim.SetInteger("Stats", 0);
        }
    }
}
