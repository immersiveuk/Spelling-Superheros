using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


namespace Com.Immersive.Cameras.PostProcessing
{
    public class FadeInAndOut : MonoBehaviour
    {
        //Static variables
        public static FadeInAndOut CurrentFadeInAndOut;
        //bool fadeIn, float duration, Color fadeColor, bool fadeInAudio
        private static (bool, float, Color, bool) FadeInSceneInfo = (false, 0, Color.black, true);
        public static void FadeInNextScene(float duration, Color color, bool fadeInAudio) { FadeInSceneInfo = (true, duration, color, fadeInAudio); }

        //SETTINGS
        public bool fadeAudio = true;
        [ColorUsage(false)]
        public Color fadeColor = Color.black;
        public float fadeDuration = 3;
        public bool fadeInAtStart = false;

        public Material fadeToBlackMaterial;

        [NonSerialized]
        public bool active = false;
        private bool _active = false;

        //Internal Settings
        private float currentFadeLevel = 0;

        private FadeInfo currentFade = null;
        private bool IsFading => currentFade != null;



        private List<ApplyFadeInOutMaterial> fadeInOutScripts;

        private bool InHotspotPlacementMode
        {
            get
            {
#if UNITY_EDITOR
                if (EditorPrefs.HasKey("PlaceHotspotMode") && EditorPrefs.GetBool("PlaceHotspotMode"))
                {
                    return true;
                }
#endif
                return false;
            }
        }

        // Start is called before the first frame update
        void Start()
        {
            CurrentFadeInAndOut = this;

            AddEffectToEachCamera();

            //If not in hotspot placement mode
            if (!InHotspotPlacementMode)
            {
                if (fadeInAtStart)
                {
                    FadeInNextScene(fadeDuration, fadeColor, fadeAudio);
                }

                //Fade In At Start
                if (FadeInSceneInfo.Item1 == true)
                {
                    active = true;
                    currentFadeLevel = 1;
                    fadeColor = FadeInSceneInfo.Item3;
                    fadeAudio = FadeInSceneInfo.Item4;
                    FadeIn(FadeInSceneInfo.Item2, FadeInCompletionHandler);
                    FadeInSceneInfo = (false, 0, Color.black, true);
                }
            }
        }

        private void AddEffectToEachCamera()
        {
            //Add fade out script to each camera.
            //Use UI camera so its the top camera that fades out.
            var uiCameras = AbstractImmersiveCamera.CurrentImmersiveCamera.uiCameras;
            fadeInOutScripts = new List<ApplyFadeInOutMaterial>();
            foreach (Camera cam in uiCameras)
            {
                var fadeScript = cam.gameObject.AddComponent<ApplyFadeInOutMaterial>() as ApplyFadeInOutMaterial;
                fadeScript.SetMaterial(fadeToBlackMaterial);
                fadeInOutScripts.Add(fadeScript);
            }
        }

        private void FadeInCompletionHandler() { active = false; }

        // Update is called once per frame
        void Update()
        {
            //Activate and Deactivate
            if (active != _active)
            {
                _active = active;
                foreach (var fadeScript in fadeInOutScripts)
                {
                    fadeScript.active = active;
                }
            }

            ////Fade In and Out
            if (IsFading)
            {
                currentFadeLevel = currentFade.GetCurrentFadeLevel();
                foreach (var fadeScript in fadeInOutScripts)
                {
                    fadeScript.SetFadeLevel(currentFadeLevel);
                }
                //Fade Audio
                if (fadeAudio) AudioListener.volume = 1 - currentFadeLevel;
                
                //End Fade
                if (currentFade.IsFadeComplete)
                {
                    currentFade.CompleteFade();
                    currentFade = null;
                    active = false;
                }
            }
        }

        #region FadeOut
        //Fade Out
        public void FadeOut(float fadeOutDuration)
        {
            active = true;
            currentFade = new FadeInfo();
            currentFade.StartFadeOut(fadeOutDuration, currentFadeLevel);
            SetFadeColor(fadeColor);
        }

        public void FadeOut(float fadeOutLength, Action completionHandler)
        {
            FadeOut(fadeOutLength);
            currentFade.SetCompletionHandler(completionHandler);
        }

        public void FadeOut(float fadeOutLength, Color fadeOutColor, bool fadeOutAudio, Action completionHandler)
        {
            fadeColor = fadeOutColor;
            fadeAudio = fadeOutAudio;
            FadeOut(fadeOutLength, completionHandler);
        }
        #endregion

        #region FadeIn
        //Fade In
        public void FadeIn(float fadeInDuration)
        {
            active = true;
            currentFade = new FadeInfo();
            currentFade.StartFadeIn(fadeInDuration, currentFadeLevel);
            SetFadeColor(fadeColor);
        }

        public void FadeIn(float fadeInLength, Action completionHandler)
        {
            FadeIn(fadeInLength);
            currentFade.SetCompletionHandler(completionHandler);
        }
        #endregion


        public void DarkenBackground()
        {
            currentFadeLevel = 0.5f;
            active = true;
            foreach (var fadeScript in fadeInOutScripts)
            {
                fadeScript.SetFadeLevel(currentFadeLevel);
            }
        }

        private void SetFadeColor(Color color)
        {
            foreach (var fadeScript in fadeInOutScripts)
            {
                fadeScript.SetFadeColor(color);
            }
        }

        private class FadeInfo
        {
            private float fadeStartTime, fadeEndTime;
            private float startFadeLevel, targetFadeLevel;
            private Action completionHandler = null;

            public bool IsFadeComplete => Time.time >= fadeEndTime;

            public float GetCurrentFadeLevel()
            {
                var t = Mathf.InverseLerp(fadeStartTime, fadeEndTime, Time.time);
                return Mathf.Lerp(startFadeLevel, targetFadeLevel, t);
            }

            public void CompleteFade()
            {
                completionHandler?.Invoke();
            }

            public void StartFadeOut(float fadeDuration, float currentFadeLevel)
            {
                fadeStartTime = Time.time;
                fadeEndTime = fadeStartTime + fadeDuration;
                startFadeLevel = currentFadeLevel;
                targetFadeLevel = 1;
            }

            public void StartFadeIn(float fadeDuration, float currentFadeLevel)
            {
                fadeStartTime = Time.time;
                fadeEndTime = fadeStartTime + fadeDuration;
                startFadeLevel = currentFadeLevel;
                targetFadeLevel = 0;
            }

            public void SetCompletionHandler(Action completionHandler)
            {
                this.completionHandler = completionHandler;
            }
        }
    }
}
