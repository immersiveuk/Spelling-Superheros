using System.Collections;
using System.Collections.Generic;
using Com.Immersive.Hotspots;
using UnityEditor;
using UnityEngine;
using static Com.Immersive.WipeToReveal.WipeSettings;

namespace Com.Immersive.WipeToReveal
{
    /// <summary>
    /// Wipe manager is a component which will apply a Wipe to Reveal a GameObject.
    /// </summary>
    [ExecuteInEditMode]
    public class WipeManager : MonoBehaviour
    {
        public Sprite spriteBackground;
        public Sprite spriteForeground;
        public WipeSettings wipeSettings;

        private Vector2 backgroundSize, foregroundSize;
        private Sprite whiteSprite;

        [HideInInspector]
        public GameObject objForeground, objBackground;

        private void Awake()
        {
            if (!Application.isPlaying && wipeSettings == null)
                wipeSettings = new WipeSettings();
        }

        void Start()
        {
            if (spriteBackground)
                backgroundSize = new Vector2(spriteBackground.texture.width, spriteBackground.texture.height);

            if (spriteForeground)
                foregroundSize = new Vector2(spriteForeground.texture.width, spriteForeground.texture.height);

            if (Application.isPlaying)
            {
                whiteSprite = Sprite.Create(new Texture2D((int)foregroundSize.x, (int)foregroundSize.y), new Rect(0, 0, (int)foregroundSize.x, (int)foregroundSize.y), new Vector2(0.5f, 0.5f), spriteForeground.pixelsPerUnit);
                var pixles = spriteForeground.texture.GetPixels();
                whiteSprite.texture.SetPixels(pixles);
                whiteSprite.texture.Apply();
                objForeground.GetComponent<SpriteRenderer>().sprite = whiteSprite;
                objForeground.AddComponent<WipeHandler>().Init(wipeSettings, this.transform);

                //Add box collider
                var boxCollider = objForeground.AddComponent<BoxCollider>();
                var size = boxCollider.size;
                size.z = 0.01f;
                boxCollider.size = size;
            }
            else if (objBackground == null || objForeground == null)
            {
                if (objBackground)
                    DestroyImmediate(objBackground);

                if (objForeground)
                    DestroyImmediate(objForeground);

                if (wipeSettings.wipeOption == WipeOption.BackgroundForeground)
                {
                    //create background
                    objBackground = new GameObject("Background");
                    objBackground.transform.SetParent(this.transform, false);
                    objBackground.AddComponent<SpriteRenderer>();
                    objBackground.transform.localPosition = new Vector3(0, 0, 0);
                }

                //create foreground
                objForeground = new GameObject("Foreground");
                objForeground.transform.SetParent(this.transform, false);
                objForeground.AddComponent<SpriteRenderer>();
                objForeground.GetComponent<SpriteRenderer>().sortingOrder = 2;
            }
        }

        private void Update()
        {
            if (Application.isPlaying)
                return;

            if (wipeSettings.wipeOption == WipeOption.Foreground)
            {
                objBackground.SetActive(false);
            }
            else
            {
                objBackground.SetActive(true);
            }

            if (objForeground == null || objBackground == null && wipeSettings.wipeOption == WipeOption.BackgroundForeground)
            {
                Start();
            }

            if (spriteForeground != null && spriteForeground != objForeground.GetComponent<SpriteRenderer>().sprite)
            {
                objForeground.GetComponent<SpriteRenderer>().sprite = spriteForeground; //assign foteground sprite
            }

            if (wipeSettings.wipeOption == WipeOption.BackgroundForeground && spriteBackground != null && spriteBackground != objBackground.GetComponent<SpriteRenderer>().sprite)
            {
                objBackground.GetComponent<SpriteRenderer>().sprite = spriteBackground; //assign background sprite
            }

            // Scale background base Crop or Fill
            if (wipeSettings.wipeOption == WipeOption.BackgroundForeground && spriteBackground != null && spriteForeground != null)
            {
                backgroundSize = new Vector2(spriteBackground.texture.width, spriteBackground.texture.height);
                foregroundSize = new Vector2(spriteForeground.texture.width, spriteForeground.texture.height);

                if (wipeSettings.backgroundScaling == WipeSettings.SizeOption.Crop)
                {
                    float height = foregroundSize.y / backgroundSize.y;
                    float width = height;

                    objBackground.transform.localScale = new Vector3(width, height, 1);
                }
                else if (wipeSettings.backgroundScaling == WipeSettings.SizeOption.Fill)
                {
                    float height = foregroundSize.y / backgroundSize.y;
                    float width = foregroundSize.x / backgroundSize.x;

                    objBackground.transform.localScale = new Vector3(width, height, 1);
                }
            }
        }
    }

    [System.Serializable]
    public class WipeSettings
    {
        public Sprite wipeBrush;

        [Range(10, 200)]
        [Tooltip("The size of the brush.")]
        public int brushSize = 1;

        [Range(50, 100)]
        [Tooltip("Percentage of the front image which must be removed for the wipe to be complete.")]
        public int wipePercentage = 80;

        [Range(0.0f, 3f)]
        [Tooltip("How long it takes for the front image to fade when wipe is complete.")]
        public float fadeOutDuration = 1;

        [Tooltip("")]
        public WipeOption wipeOption;

        [Tooltip("Crop - Maintain aspect ratio and match foreground height, Fill - Scale height and width to match foreground.")]
        public SizeOption backgroundScaling;

        [Header("Optional")]
        public ParticleSystem touchParticleSystem;
        [Tooltip("Multiplier which effects how many particles are generated.")]
        public float emisionRateMultiplier = 30;


        [Space(10)]

        public ImageProperty brushSprite;

        [VectorRange(0.1f, 5.0f, 0.1f, 5.0f, true)]
        public Vector2 scale = new Vector2(1, 1);

        [VectorRange(-1.0f, 1.0f, -1.0f, 1.0f, true)]
        public Vector2 offset = Vector2.zero;

        public ComputeShader computeShader;

        public enum SizeOption
        {
            Crop,
            Fill
        }

        public enum WipeOption
        {
            BackgroundForeground,
            Foreground
        }
    }
}