using System.Collections;
using System.Collections.Generic;
using Com.Immersive.Hotspots;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEditor;
using Immersive.Properties;

namespace Com.Immersive.Hotspots
{
    [ExecuteInEditMode]
    public class TextHotspot : MonoBehaviour
    {
        public bool isBackground;
        public ImageProperty imageProperty;
        public TextProperty textProperty;
        public int margine;

        public TMP_FontAsset fontAsset;
        public TextMeshPro textGlow, textHotspot;
        public SpriteRenderer spriteRendererBackground;

        void Awake()
        {
            Init();
            SetGlow();
        }

        /// <summary>
        /// Create Text Hotspot with defalt settings
        /// </summary>
        public void Init()
        {
            if (GetComponent<BoxCollider>() == null)
            {
                gameObject.AddComponent<BoxCollider>();
            }

            SetText();
            SetBackground();
        }

        /// <summary>
        /// Create Glow effect for Text and Background image
        /// </summary>
        public void SetGlow()
        {
            HotspotController hotspotController = this.gameObject.GetComponentInParent<HotspotController>();

            if (hotspotController != null && hotspotController.hotspotEffects == HotspotController.HotspotEffects.Glow)
            {
                textGlow.gameObject.AddComponent<PulseAnimation>().InitSettings(hotspotController.hotspotGlowSettings);
                textGlow.gameObject.AddComponent<DisableGlowWhenHotspotDisabled>().Init(this.GetComponentInParent<IHotspot>());

                if (spriteRendererBackground.sprite != null && isBackground && imageProperty.color.a >= 1.0)
                {
                    spriteRendererBackground.gameObject.AddComponent<CreateHotspotGlow>().SetValue(hotspotController.hotspotGlowSettings);
                }
            }
        }

        /// <summary>
        /// Set Text
        /// </summary>
        public void SetText()
        {
            textHotspot.font = fontAsset;
            textHotspot.text = textProperty.Text;
            textHotspot.fontSize = textProperty.FontSize;
            textHotspot.color = textProperty.Color;

            textGlow.text = "<material=\"" + fontAsset.name + " Glow\">" + textProperty.Text + "</material >"; //set glow material preset for the Glow Text. Preset need to be created from Unity editor
            textGlow.fontSize = textHotspot.fontSize;
            textGlow.font = fontAsset;

            textHotspot.ForceMeshUpdate();
        }

        /// <summary>
        /// Set Background and resize according to Text
        /// </summary>
        public void SetBackground()
        {
            if (isBackground)
            {
                spriteRendererBackground.gameObject.SetActive(true);

                if (imageProperty.sprite != null)
                {
                    spriteRendererBackground.sprite = imageProperty.sprite;
                    spriteRendererBackground.color = imageProperty.color;

                    spriteRendererBackground.drawMode = SpriteDrawMode.Simple;

                    if (imageProperty.type == Image.Type.Sliced)
                    {
                        spriteRendererBackground.drawMode = SpriteDrawMode.Sliced;
                    }

                    Vector2 textSize = (textHotspot.bounds.size * spriteRendererBackground.sprite.pixelsPerUnit) + Vector3.one * margine * textProperty.FontSize;
                    Vector2 spriteSize = spriteRendererBackground.sprite.rect.size;

                    spriteRendererBackground.transform.localScale = new Vector3(textSize.x / spriteSize.x, textSize.y / spriteSize.y, 1);
                }
                else
                {
                    spriteRendererBackground.sprite = null;
                }
            }
            else
            {
                spriteRendererBackground.gameObject.SetActive(false);
            }

            gameObject.GetComponent<BoxCollider>().size = textHotspot.bounds.size;
        }

        private void Update()
        {
            if (!Application.isPlaying)
            {
                SetText();
                SetBackground();
            }
        }
    }

#if UNITY_EDITOR

    /// <summary>
    /// This implements a custom editor for a TextHotspot
    /// </summary>
    [CustomEditor(typeof(TextHotspot))]
    public class TextHotspotEditor : Editor
    {
        private int currentTab = 0;

        private TextHotspot textHotspotScript;

        private SerializedProperty isBackground;
        private SerializedProperty imageProperty;
        private SerializedProperty textProperty;
        private SerializedProperty margine;

        private SerializedProperty textGlow;
        private SerializedProperty textHotspot;
        private SerializedProperty spriteRendererBackground;
        private SerializedProperty fontAsset;

        private void OnEnable()
        {
            textHotspotScript = (TextHotspot)target;

            isBackground = serializedObject.FindProperty("isBackground");
            imageProperty = serializedObject.FindProperty("imageProperty");
            textProperty = serializedObject.FindProperty("textProperty");
            margine = serializedObject.FindProperty("margine");

            fontAsset = serializedObject.FindProperty(nameof(textHotspotScript.fontAsset));
            textGlow = serializedObject.FindProperty(nameof(textHotspotScript.textGlow));
            textHotspot = serializedObject.FindProperty(nameof(textHotspotScript.textHotspot));
            spriteRendererBackground = serializedObject.FindProperty(nameof(textHotspotScript.spriteRendererBackground));
        }


        public override void OnInspectorGUI()
        {
            EditorGUILayout.Space();
            currentTab = GUILayout.Toolbar(currentTab, new string[] { "Settings", "Prefabs" });
            EditorGUILayout.Space();

            switch (currentTab)
            {
                case 0:
                    OnInspectorGUISettings();
                    break;
                case 1:
                    OnInspectorGUIPopUpPrefabs();
                    break;
            }

            serializedObject.ApplyModifiedProperties();
        }

        private void OnInspectorGUISettings()
        {
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(textProperty, new GUIContent("Text Property"), true);

            EditorGUILayout.PropertyField(isBackground, new GUIContent("Show Background"));

            if (textHotspotScript.isBackground)
            {
                EditorGUILayout.PropertyField(imageProperty, new GUIContent("Background Property"), true);
            }


            EditorGUILayout.PropertyField(margine, new GUIContent("Margine"));
        }

        private void OnInspectorGUIPopUpPrefabs()
        {
            EditorGUILayout.PropertyField(fontAsset, new GUIContent("Font"));
            EditorGUILayout.PropertyField(textGlow, new GUIContent("Text Glow"));
            EditorGUILayout.PropertyField(textHotspot, new GUIContent("Text Hotspot"));
            EditorGUILayout.PropertyField(spriteRendererBackground, new GUIContent("Background"));
        }
    }
#endif
}