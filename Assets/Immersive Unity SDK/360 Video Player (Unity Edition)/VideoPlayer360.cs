/* Copyright (C) Immersive Interactive, Ltd - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Luke Bissell <luke@immersive.co.uk>, July 2019
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using UnityEngine.Video;
using Com.Immersive.Cameras;

namespace Com.Immersive.Video360
{
    public class VideoPlayer360 : MonoBehaviour
    {
        [HideInInspector]
        public string URLEditorOnly;
        public GameObject ErrorObj;


        private VideoPlayer videoPlayer;
        public Material skyboxMaterial;
        public UnityEngine.Rendering.TextureDimension videoType;
        public enum VideoType { ThreeD, TwoD };

        public ColourPalette colourPalette = ColourPalette.Standard;
        private ColourPalette _colourPalette = ColourPalette.Standard;

        private AbstractImmersiveCamera cam;

        // Start is called before the first frame update
        void Start()
        {
            cam = FindObjectOfType<AbstractImmersiveCamera>();

            //Create Video Player
            videoPlayer = gameObject.AddComponent<VideoPlayer>();
            videoPlayer.prepareCompleted += SetupVideo;
            videoPlayer.errorReceived += VideoPlayer_errorRecieved;
            videoPlayer.playOnAwake = false;
            videoPlayer.isLooping = true;

            //Get URL
            string url = "";
            if (Application.isEditor)
            {
                url = URLEditorOnly;
            }
            else
            {
                url = "..\\"+ReadParameters.Settings.FilePath;
            }

            print("URL: " + url);

            //Play video if it exists
            if (File.Exists(url))
            {
                //videoPlayer.url = "..\\" + url;
                videoPlayer.url = url;
                videoPlayer.Play();
            }
            //Present warning that video doesn't exist.
            else
            {
                ErrorObj.SetActive(true);
                Debug.LogError("File: " + url + " does not exist.");
            }
        }

        private void SetupVideo(VideoPlayer source)
        {
            //Create new render texture for the video to be rendered to
            RenderTexture renderTexture =
                new RenderTexture((int)source.width, (int)source.height, 0)
                {
                    //Set Video Type
                    dimension = videoType
                };

            //Tell the video to render to render texture
            source.targetTexture = renderTexture;

            //Set the texture of the skybox
            skyboxMaterial.mainTexture = renderTexture;
            RenderSettings.skybox = skyboxMaterial;

            SetColourPalette();
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                colourPalette = ColourPalette.Standard;
                cam.TurnOffWeatherEffect();
            }
            if (Input.GetKeyDown(KeyCode.Alpha2)) {
                colourPalette = ColourPalette.Night;
                cam.TurnOffWeatherEffect();
            }
            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                colourPalette = ColourPalette.Sunset;
                cam.TurnOffWeatherEffect();
            }
            if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                cam.TurnOnWeatherEffect();
                colourPalette = ColourPalette.Rain;
            }

            if (colourPalette != _colourPalette)
            {
                _colourPalette = colourPalette;
                SetColourPalette();
            }    
        }

        private void SetColourPalette()
        {
            switch (colourPalette)
            {
                case ColourPalette.Standard:
                    SetStandardColourPalette();
                    break;
                case ColourPalette.Night:
                    SetNightColourPalette();
                    break;
                case ColourPalette.Sunset:
                    SetSunsetColourPalette();
                    break;
                case ColourPalette.Rain:
                    SetRainColourPalette();
                    break;
            }
        }

        private void SetStandardColourPalette()
        {
            skyboxMaterial.SetFloat("_Exposure", 0.62f);
            skyboxMaterial.SetColor("_Tint", new Color(1, 1, 1));
        }

        private void SetNightColourPalette()
        {
            skyboxMaterial.SetFloat("_Exposure", 0.25f);
            skyboxMaterial.SetColor("_Tint", new Color(0.55f, 0.67f, 0.75f));
        }

        private void SetSunsetColourPalette()
        {
            skyboxMaterial.SetFloat("_Exposure", 1.5f);
            skyboxMaterial.SetColor("_Tint", new Color(0.55f, 0.35f, 0.22f));
        }

        private void SetRainColourPalette()
        {
            skyboxMaterial.SetFloat("_Exposure", 0.62f);
            skyboxMaterial.SetColor("_Tint", new Color(0.6f, 0.6f, 0.6f));
        }


        private void VideoPlayer_errorRecieved (VideoPlayer source, string message)
        {
            print("Video Player Error: " + message);
            videoPlayer.errorReceived -= VideoPlayer_errorRecieved;
        }

        public enum ColourPalette { Standard, Night, Sunset, Rain };

    }

    //==============================================================
    // CUSTOM EDITOR
    //==============================================================

#if UNITY_EDITOR

    [CustomEditor(typeof(VideoPlayer360))]
    public class Video360Editor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            VideoPlayer360 videoPlayer = (VideoPlayer360)target;

            EditorGUILayout.LabelField("URL (Editor Only)", videoPlayer.URLEditorOnly);
            if (GUILayout.Button("Browse..."))
            {
                videoPlayer.URLEditorOnly = EditorUtility.OpenFilePanel("Choose Video", "", "*");
                EditorUtility.SetDirty(videoPlayer);
            }
        }
    }

#endif

}