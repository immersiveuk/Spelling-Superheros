using Com.Immersive.Cameras;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Immersive.SuperHero
{
    public class SelectedSuperHeroData : Singleton<SelectedSuperHeroData>
    {
        public delegate void OnSuperHeroPartSelected(SuperHeroCreatorStages stage, bool completed);
        public static event OnSuperHeroPartSelected OnSuperHeroPartSelectedEvent;

        Dictionary<WallType, SelectedSuperHero> createdSuperHeros = new Dictionary<WallType, SelectedSuperHero>();
        public Dictionary<WallType, bool> selectedWalls = new Dictionary<WallType, bool>();

        public SuperHeroCreatorStages currentStage;

        void Inisialize()
        {
            createdSuperHeros.Add(WallType.Left, new SelectedSuperHero());
            createdSuperHeros.Add(WallType.Center, new SelectedSuperHero());
            createdSuperHeros.Add(WallType.Right, new SelectedSuperHero());

            selectedWalls.Add(WallType.Left, false);
            selectedWalls.Add(WallType.Center, false);
            selectedWalls.Add(WallType.Right, false);
        }

        #region SuperHero Creator
        public SelectedSuperHero GetSuperHero(WallType wallType)
        {
            if (createdSuperHeros.Count <= 0)
            {
                Inisialize();
            }

            return createdSuperHeros[wallType];
        }

        public void SetSuperHeroData(WallType wallType, SelectedSuperHero selectedSuperHero)
        {
            AbstractImmersiveCamera.PlayAudio(SuperHeroCreatorManager.Instance.selectClip);

            createdSuperHeros[wallType] = selectedSuperHero;
            selectedWalls[wallType] = true;

            if (selectedWalls[WallType.Left] && selectedWalls[WallType.Center] && selectedWalls[WallType.Right])
            {
                bool completed = false;
                if (currentStage == SuperHeroCreatorStages.Stage3)
                {
                    completed = true;
                }
                else
                {
                    currentStage++;
                }

                if (OnSuperHeroPartSelectedEvent != null)
                    OnSuperHeroPartSelectedEvent(currentStage, completed);
            }
        }

        public void ResetWallSelected()
        {
            selectedWalls[WallType.Left] = selectedWalls[WallType.Center] = selectedWalls[WallType.Right] = false;
        }

        public void ResetData()
        {
            currentStage = SuperHeroCreatorStages.Stage1;
            createdSuperHeros.Clear();
            selectedWalls.Clear();
        }
        #endregion
    }
}