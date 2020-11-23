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
        StopAllCoroutines();
        Play();
    }

    void Play()
    {
        StartCoroutine(PlayAnim());
    }

    private IEnumerator PlayAnim()
    {
        currentFrame = 0;

        //while (currentFrame < frames.Length)
        //{
        //    spriteRenderer.sprite = frames[currentFrame];
        //    currentFrame++;

        //    yield return new WaitForSeconds(2);
        //}
        spriteRenderer.sprite = frames[0];
        yield return new WaitForSeconds(Random.Range(1.5f, 2.5f));
        spriteRenderer.sprite = frames[1];

        if (loop)
        {
            yield return new WaitForSeconds(0.1f);
            StartCoroutine(PlayAnim());
        }
    }
}