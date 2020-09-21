using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeMultiHotspotSpriteOnQandACorrectlyAnswered : MonoBehaviour, IQuestionAnsweredHandler
{
    public Sprite newSprite;

    private SpriteRenderer spriteRenderer;

    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = transform.parent.GetComponent<SpriteRenderer>();
        if (spriteRenderer == null) Debug.LogError("No SpriteRenderer attached to object "+gameObject.name+".");
    }

    public void QuestionAnswered(bool isAnswerCorrect)
    {
        if (isAnswerCorrect)
        {
            spriteRenderer.sprite = newSprite;
        }
    }

}
