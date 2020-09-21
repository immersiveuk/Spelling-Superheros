using System.Collections;
using System.Collections.Generic;
using Com.Immersive.Hotspots;
using UnityEngine;

[System.Serializable]
public class HotspotGlowSettings
{
    public float pulseDuration = 3;

    public Color colour = Color.white;

    [Range(0, 1.0f)]
    public float minAlpha = 0f;

    [Range(0, 1.0f)]
    public float maxAlpha = 1f;

    public bool maintainGlobalSync = true;
}

public class CreateHotspotGlow : MonoBehaviour
{
    private Material blurMat;
    private Material whiteMat;

    public HotspotGlowSettings hotspotGlowSettings = new HotspotGlowSettings();
    private readonly float blurSize = 0.3f;

    void Start()
    {
        blurMat = new Material(Shader.Find("Custom/BlurShader"));
        whiteMat = new Material(Shader.Find("Unlit/MakeWhiteShader"));

        blurMat.SetFloat("_BlurSize", blurSize);
        blurMat.SetFloat("_StandardDeviation", 0.03f);
        blurMat.SetFloat("_Gauss", 1f);
        blurMat.SetFloat("_Samples", 2f);

        var sprite = GetComponent<SpriteRenderer>().sprite;
        var blurredSprite = GenerateBlurredWhiteSprite(sprite);

        GameObject obj = new GameObject("Glow");
        obj.transform.SetParent(transform, false);
        obj.transform.localPosition = new Vector3(0, 0, 0.0001f);
        obj.AddComponent<SpriteRenderer>().sprite = blurredSprite;


        obj.AddComponent<PulseAnimation>().InitSettings(hotspotGlowSettings);
        obj.AddComponent<DisableGlowWhenHotspotDisabled>().Init(this.GetComponentInParent<IHotspot>());

        Destroy(blurMat);
        Destroy(whiteMat);
    }

    public void SetValue(HotspotGlowSettings _hotspotGlowSettings)
    {
        hotspotGlowSettings.pulseDuration = _hotspotGlowSettings.pulseDuration;
        hotspotGlowSettings.minAlpha = _hotspotGlowSettings.minAlpha;
        hotspotGlowSettings.maxAlpha = _hotspotGlowSettings.maxAlpha;
        hotspotGlowSettings.maintainGlobalSync = _hotspotGlowSettings.maintainGlobalSync;
        hotspotGlowSettings.colour = _hotspotGlowSettings.colour;
    }

    private Sprite GenerateBlurredWhiteSprite(Sprite sprite)
    {
        var baseTexture = sprite.texture;

        //Add Padding
        var maxDimension = Mathf.Max(baseTexture.width, baseTexture.height);
//        print("Padding = " + maxDimension / 5);
        Texture2D paddedTexture = AddPadding(baseTexture, maxDimension / 5);

        //Create Blurred White Texture
        var whiteTexture = GetWhiteTexture(paddedTexture);
        var blurredTexture = GetBlurredTexture(whiteTexture);


        var texture2D = ConvertRenderTextureToTexture2D(blurredTexture);

        RenderTexture.ReleaseTemporary(whiteTexture);
        RenderTexture.ReleaseTemporary(blurredTexture);

        return Sprite.Create(texture2D, new Rect(0, 0, texture2D.width, texture2D.height), new Vector2(0.5f, 0.5f), sprite.pixelsPerUnit);
    }

    private Texture2D AddPadding(Texture2D source, int padSize)
    {
        //make a new empty texture a bit bigger than your first
        var paddedTexture = new Texture2D(source.width + (padSize * 2), source.height + padSize * 2);

        //make a loop to paint our new image clear

        var i = paddedTexture.height;
        while (i > 0)
        {
            i--;
            var j = paddedTexture.width;
            while (j > 0)
            {
                j--;
                paddedTexture.SetPixel(j, i, Color.clear);
            }
        }
        paddedTexture.Apply();


        i = source.height;
        while (i > 0)
        {
            i--;
            var j = source.width;
            while (j > 0)
            {
                j--;
                var color = source.GetPixel(j, i);
                paddedTexture.SetPixel(j + padSize, i + padSize, color);
            }
        }
        paddedTexture.Apply();

        return paddedTexture;
    }

    private RenderTexture GetBlurredTexture(Texture source)
    {
        var verticallyBlurredTexture = RenderTexture.GetTemporary(source.width, source.height);
        var fullyBlurredTexture = RenderTexture.GetTemporary(source.width, source.height);
        Graphics.Blit(source, verticallyBlurredTexture, blurMat, 0);
        Graphics.Blit(verticallyBlurredTexture, fullyBlurredTexture, blurMat, 1);

        RenderTexture.ReleaseTemporary(verticallyBlurredTexture);

        return fullyBlurredTexture;
    }

    private RenderTexture GetWhiteTexture(Texture source)
    {
        var whiteTexture = RenderTexture.GetTemporary(source.width, source.height);
        Graphics.Blit(source, whiteTexture, whiteMat);

        return whiteTexture;
    }

    private Texture2D ConvertRenderTextureToTexture2D(RenderTexture source)
    {
        RenderTexture.active = source;
        var texture = new Texture2D(source.width, source.height);
        texture.ReadPixels(new Rect(0, 0, source.width, source.height), 0, 0);
        texture.Apply();

        return texture;
    }
}