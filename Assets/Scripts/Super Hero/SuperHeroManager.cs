using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum FillInTheBlankStages { Stage1, Stage2, Stage3 }

namespace Immersive.SuperHero
{
    public class SuperHeroManager : Singleton<SuperHeroManager>
    {
        Dictionary<WallType, SelectedSuperHero> createdSuperHeros = new Dictionary<WallType, SelectedSuperHero>();
        public Dictionary<WallType, bool> selectedWalls = new Dictionary<WallType, bool>();

        public FillInTheBlankStages currentStage;

        public void ResetManager()
        {
            wallCompleted = 0;
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

        #region SuperHero Game
        int wallCompleted = 0;
        public void OnAllEnemiesDestroyedOfWall()
        {
            wallCompleted++;

            if (wallCompleted > 2)
            {
                LoadScene("End Scene");
            }
        }
        #endregion

        public void LoadScene(string sceneName)
        {
            SceneManager.LoadScene(sceneName);
        }

        private void OnApplicationQuit()
        {
            PlayerPrefs.SetInt("GameMode", -1);
        }
    }
}