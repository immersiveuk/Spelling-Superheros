using Com.Immersive.Cameras;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Immersive.SuperHero
{
    public class EnemiesController : SuperHeroGame
    {
        [Header("Enemy Objects")]
        public Transform enemyParent;
        public Enemies enemies;
        public Enemy prefabEnemy;
        public TextMeshPro textEnemy;

        float enemyRange;
        int enemyIndex;
        int totalEnemy;

        new void Start()
        {
            base.Start();

            totalEnemy = enemies.EnemyList.Count;
            enemyRange = AbstractImmersiveCamera.CurrentImmersiveCamera.cameras[0].aspect / 3.0f;
            
            CreateEnemy();

            Invoke("CreateEnemy", 2.0f);
        }

        void CreateEnemy()
        {
            Vector3 startPosition = new Vector3(Random.Range(-enemyRange, enemyRange), 1.5f, 0);
            Vector3 endPosition = new Vector3(Random.Range(-enemyRange, enemyRange), Random.Range(0.2f, 0.4f), 0);

            Enemy objEnemy = Instantiate(prefabEnemy, enemyParent, false);
            objEnemy.transform.localPosition = startPosition;
            objEnemy.transform.localScale = Vector3.one * 0.3f;

            objEnemy.Init(enemies.EnemyList[enemyIndex], endPosition);

            enemyIndex++;        
        }

        protected override void OnEnemyDestory()
        {
            
            if (enemyIndex < enemies.EnemyList.Count)
            {
                CreateEnemy();
            }

            totalEnemy--;

            textEnemy.text = "Enemy: " + (enemies.EnemyList.Count - totalEnemy);

            if (totalEnemy <= 0)
            {
                SuperHeroManager.Instance.OnAllEnemiesDestroyedOfWall();
            }
        }
    }
}