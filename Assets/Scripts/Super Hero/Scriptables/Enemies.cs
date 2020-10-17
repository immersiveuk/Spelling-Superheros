using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Enemies", menuName = "Super Hero/ Enemies", order = 1)]
public class Enemies : ScriptableObject
{
    public List<Sprite> EnemyList = new List<Sprite>();
}
