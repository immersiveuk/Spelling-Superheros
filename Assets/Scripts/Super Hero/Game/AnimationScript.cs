using UnityEngine;
using System.Collections;

public class AnimationScript : MonoBehaviour
{
    public Sprite[] frames;
    
    private bool loop = true;
    private float animationSpeed = 0.13f;
    private SpriteRenderer spriteRenderer;
    private int currentFrame;

    public void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void Init(float speed, Sprite[] sprites)
    {
        animationSpeed = speed;
        frames = sprites;

        Play();
    }

    void Play()
    {
        StartCoroutine(PlayAnim());
    }

    private IEnumerator PlayAnim()
    {
        currentFrame = 0;

        while (currentFrame < frames.Length)
        {
            spriteRenderer.sprite = frames[currentFrame];
            currentFrame++;

            yield return new WaitForSeconds(animationSpeed);
        }

        if (loop)
        {
            yield return new WaitForSeconds(animationSpeed);
            StartCoroutine(PlayAnim());
        }
    }
}