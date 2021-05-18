using Com.Immersive.Cameras;
using Immersive.FillInTheBlank;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;


namespace Immersive.SuperHero
{
    public class GameData : Singleton<GameData>
    {
        private const string arg = "-wordsJSON=";


        public SuperHeroCreatorStages currentStage;

        public AudioSource audioSource;
        public AudioSource labAmbienceAudioSource;

        [Header("Level")]
        public SpriteRenderer levelLeft;
        public SpriteRenderer levelCenter;
        public SpriteRenderer levelRight;

        public Sprite level_1;
        public Sprite level_2;

        [Header("Testing")]
        public TextAsset json;

        public FillInTheBlanksDataStages fillInTheBlanksDataStages = new FillInTheBlanksDataStages();

        private void Start()
        {
#if !UNITY_EDITOR
            string filePath = ReadParameters.Settings.FilePath;
            Debug.LogError(filePath);

            RuntimeLoading.Instance.LoadJson("file:///"+filePath, (string jsonText, bool success)=>
            {
                //Debug.LogError(jsonText);
                fillInTheBlanksDataStages = JsonConvert.DeserializeObject<FillInTheBlanksDataStages>(jsonText);
            });      
#endif

            SelectedSuperHeroData.OnSuperHeroPartSelectedEvent += OnSuperHeroPartSelected;

            SetLevel();
        }

        public void SetLevel()
        {
            if((GameMode)PlayerPrefs.GetInt("GameMode") == GameMode.Simple)
            {
                levelLeft.sprite = levelCenter.sprite = levelRight.sprite = level_1;
            }
            else
            {
                levelLeft.sprite = levelCenter.sprite = levelRight.sprite = level_2;
            }

            transform.localScale = new Vector3(1, 1, 1);
        }

        private void OnDestroy()
        {
            SelectedSuperHeroData.OnSuperHeroPartSelectedEvent -= OnSuperHeroPartSelected;
        }

        /// <summary>
        /// To Create JSON sample file from editor
        /// </summary>
        [ContextMenu("Load Json")]
        void LoadJson()
        {
            //Debug.Log(JsonConvert.SerializeObject(fillInTheBlanksDataStages));
            fillInTheBlanksDataStages = JsonConvert.DeserializeObject<FillInTheBlanksDataStages>(json.text);
        }

        /// <summary>
        /// To Create JSON sample file from editor
        /// </summary>
        [ContextMenu("Create Json")]
        void CreateJson()
        {
            Debug.Log(JsonConvert.SerializeObject(fillInTheBlanksDataStages));

            System.IO.File.WriteAllText(System.IO.Directory.GetCurrentDirectory() + "/FillInTheBlancks.json", JsonConvert.SerializeObject(fillInTheBlanksDataStages));
        }

        public void ResetManager()
        {
            currentStage = SuperHeroCreatorStages.Stage1;
            SelectedSuperHeroData.Instance.ResetData();
        }

        public FillInTheBlanksDataStage GetWords()
        {
            FillInTheBlanksDataStage stage = fillInTheBlanksDataStages.stage1;

            switch (currentStage)
            {
                case SuperHeroCreatorStages.Stage1:
                    stage = fillInTheBlanksDataStages.stage1;
                    break;

                case SuperHeroCreatorStages.Stage2:
                    stage = fillInTheBlanksDataStages.stage2;
                    break;

                case SuperHeroCreatorStages.Stage3:
                    stage = fillInTheBlanksDataStages.stage3;
                    break;
            }

            return stage;
        }

        public void LoadScene(string sceneName)
        {
            if (audioSource)
                audioSource.Stop();

            SceneManager.LoadScene(sceneName);
        }

        void OnSuperHeroPartSelected(SuperHeroCreatorStages stage, bool completed)
        {
            currentStage = stage;

            if (!completed)
                LoadScene("" + currentStage);
            else
                LoadScene("Super Hero Game");
        }
    }
}