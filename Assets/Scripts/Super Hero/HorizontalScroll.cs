using Immersive.SuperHero;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HorizontalScroll : MonoBehaviour
{
    public SpriteRenderer prefabSprite;
    int partIndex = 0;
    int spriteIndex;

    List<Transform> parts = new List<Transform>();
    List<SuperHeroParts> superHeroParts;

    public static float gapValue;
    float transitionTime;

    private void Start()
    {
        transitionTime = 1.0f;
        gapValue = 0.35f;
        partIndex = 0;
    }

    public void SetScroll(List<SuperHeroParts> superHeroParts)
    {
        this.superHeroParts = superHeroParts;

        for (int i = 0; i < 2; i++)
        {
            SpriteRenderer objPart = Instantiate(prefabSprite, this.transform, false);
            objPart.sprite = superHeroParts[i].creatorSprite;
            objPart.transform.localPosition = new Vector3(i * gapValue, 0, 0);

            parts.Add(objPart.transform);
        }
    }

    public void MoveNext()
    {
        partIndex++;
        spriteIndex++;

        Scroll(-1);
    }

    public void MovePrevious()
    {
        partIndex--;
        spriteIndex--;

        Scroll(1);
    }

    void Scroll(int direction)
    {
        if (spriteIndex >= superHeroParts.Count)
            spriteIndex = 0;

        if (spriteIndex < 0)
            spriteIndex = superHeroParts.Count - 1;

        parts[1].GetComponent<SpriteRenderer>().sprite = superHeroParts[spriteIndex].creatorSprite;
        parts[1].localPosition = new Vector3(partIndex * gapValue, 0, 0);

        iTween.MoveBy(this.gameObject, iTween.Hash("x", direction * gapValue, "y", 0, "z", 0, "islocal", false, "time", transitionTime,
            "easetype", iTween.EaseType.easeInOutQuad, "oncomplete", (System.Action<object>)(newValue =>
            {
                Transform temp = parts[0];
                parts.Remove(temp);
                parts.Add(temp);
            })));
    }

    public SuperHeroParts GetSelectedPart()
    {
        return superHeroParts[spriteIndex];
    }
}
