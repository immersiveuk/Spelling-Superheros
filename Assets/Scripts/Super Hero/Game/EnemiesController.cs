using Com.Immersive.Cameras;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Immersive.SuperHero
{
    public class EnemiesController : SuperHeroGame
    {
        [Header("Enemy Objects")]
        public Transform enemyParent;
        public Enemies enemies;
        public Enemy prefabEnemy;

        float enemyRange;

        new void Start()
        {
            base.Start();
            enemyRange = AbstractImmersiveCamera.CurrentImmersiveCamera.cameras[0].aspect / 2.5f;

            CreateEnemy();
        }

        void CreateEnemy()
        {
            Vector3 startPosition = new Vector3(Random.Range(-enemyRange, enemyRange), 1, 0);
            Vector3 endPosition = new Vector3(Random.Range(-enemyRange, enemyRange), Random.Range(0.2f, 0.4f), 0);

            Enemy objEnemy = Instantiate(prefabEnemy, enemyParent, false);
            objEnemy.transform.localPosition = startPosition;

            objEnemy.Init(enemies.EnemyList[0], endPosition);
        }
    }
}