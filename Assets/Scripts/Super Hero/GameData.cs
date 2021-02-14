using Com.Immersive.Cameras;
using Immersive.FillInTheBlank;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum FillInTheBlankStages { Stage1, Stage2, Stage3, None }

namespace Immersive.SuperHero
{
    public class GameData : Singleton<GameData>
    {
        private const string arg = "-wordsJSON=";

        Dictionary<WallType, SelectedSuperHero> createdSuperHeros = new Dictionary<WallType, SelectedSuperHero>();
        public Dictionary<WallType, bool> selectedWalls = new Dictionary<WallType, bool>();

        public FillInTheBlankStages currentStage;

        [Header("SFX")]
        public AudioClip selectClip;
        public AudioClip switchClip;

        public AudioSource audioSource;
        public AudioSource labAmbienceAudioSource;

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
        }

        [ContextMenu("JSON")]
        void CreateJson()
        {
            //Debug.Log(JsonConvert.SerializeObject(fillInTheBlanksDataStages));
        }

        public void ResetManager()
        {
            currentStage = FillInTheBlankStages.Stage1;
            createdSuperHeros.Clear();
            selectedWalls.Clear();
        }

        void Inisialize()
        {
            createdSuperHeros.Add(WallType.Left, new SelectedSuperHero());
            createdSuperHeros.Add(WallType.Center, new SelectedSuperHero());
            createdSuperHeros.Add(WallType.Right, new SelectedSuperHero());

            selectedWalls.Add(WallType.Left, false);
            selectedWalls.Add(WallType.Center, false);
            selectedWalls.Add(WallType.Right, false);
        }

        public FillInTheBlanksDataStage GetWords()
        {
            FillInTheBlanksDataStage stage = fillInTheBlanksDataStages.stage1;

            switch (currentStage)
            {
                case FillInTheBlankStages.Stage1:
                    stage = fillInTheBlanksDataStages.stage1;
                    break;

                case FillInTheBlankStages.Stage2:
                    stage = fillInTheBlanksDataStages.stage2;
                    break;

                case FillInTheBlankStages.Stage3:
                    stage = fillInTheBlanksDataStages.stage3;
                    break;
            }

            return stage;
        }

        public SelectedSuperHero GetSuperHero(WallType wallType)
        {
            if (createdSuperHeros.Count <= 0)
            {
                Inisialize();
            }

            return createdSuperHeros[wallType];
        }

        #region SuperHero Creator
        public void SetSuperHeroData(WallType wallType, SelectedSuperHero selectedSuperHero)
        {
            createdSuperHeros[wallType] = selectedSuperHero;
            selectedWalls[wallType] = true;

            if(selectedWalls[WallType.Left] && selectedWalls[WallType.Center]&& selectedWalls[WallType.Right])
            {
                if(currentStage == FillInTheBlankStages.Stage3)
                {
                    LoadScene("Super Hero Game");
                }
                else
                {
                    currentStage++;
                    LoadScene("" + currentStage);
                }               
            }
        }

        public void ResetWallSelected()
        {
            selectedWalls[WallType.Left] = selectedWalls[WallType.Center] = selectedWalls[WallType.Right] = false;
        }
        #endregion

        public void LoadScene(string sceneName)
        {
            if (audioSource)
                audioSource.Stop();

            SceneManager.LoadScene(sceneName);
        }

        public void PlaySelect()
        {
            AbstractImmersiveCamera.PlayAudio(selectClip);
        }

        public void PlaySwitch()
        {
            AbstractImmersiveCamera.PlayAudio(switchClip);
        }

        private void OnApplicationQuit()
        {
            PlayerPrefs.SetInt("GameMode", -1);
        }
    }
}