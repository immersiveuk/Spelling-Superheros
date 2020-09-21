using System;
using UnityEngine;

namespace Com.Immersive.Cameras.PostProcessing
{
    public class ApplyFadeInOutMaterial : MonoBehaviour
    {
        [NonSerialized]
        public bool active = false;

        private Material fadeInOutMaterial;

        private void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            if (active)
            {
                Graphics.Blit(source, destination, fadeInOutMaterial);
            }
            else
            {
                Graphics.Blit(source, destination);
            }
        }

        public void SetMaterial(Material material)
        {
            fadeInOutMaterial = Instantiate(material);
        }

        public void SetFadeLevel(float fadeLevel)
        {
            fadeInOutMaterial.SetFloat("_FadeLevel", fadeLevel);
        }

        public void SetFadeColor(Color color)
        {
            fadeInOutMaterial.SetColor("_TintColor", color);
        }
    }
}