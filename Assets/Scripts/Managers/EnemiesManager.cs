using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.Core.GameData;
using Assets.Scripts.Core.Settings;
using Assets.Scripts.GameEntities.Creators;
using Assets.Scripts.Infrastructure.Enums;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Random = System.Random;
using Vector3 = UnityEngine.Vector3;

namespace Assets.Scripts.Managers
{
    public class EnemiesManager
    {
        public List<Vector3> SpawnPoints { get; set; }
        
        private readonly PlayerSettings playerSettings;
        private readonly LevelData levelData;
        private readonly TankCreator tankCreator;
        private int activeEnemies;
        private int remainingEnemies;
        private List<HullName> hulls;
        private List<TowerName> towers;
        private List<TrackName> tracks;
        private List<GunName> guns;
        private Random random = new Random();
        private int enemyIndex;

        public EnemiesManager(PlayerSettings playerSettings, LevelData levelData, TankCreator tankCreator)
        {
            this.playerSettings = playerSettings;
            this.levelData = levelData;
            this.tankCreator = tankCreator;
            activeEnemies = 0;
            remainingEnemies = levelData.TotalEnemies;
            SpawnPoints = new List<Vector3>();
        }

        public async UniTask SpawnEnemies()
        {
            while (remainingEnemies > 0)
            {
                if (remainingEnemies == levelData.TotalEnemies)
                {
                    enemyIndex = 0;
                    UpdateMechanicalParts();

                    foreach (var spawnPoint in SpawnPoints.Take(remainingEnemies))
                    {
                        await SpawnEnemy(spawnPoint);
                    }

                    await UniTask.Delay(TimeSpan.FromSeconds(10));
                }

                await UniTask.Delay(TimeSpan.FromSeconds(5));

                if (activeEnemies < levelData.MaxEnemiesOnScene)
                {
                    await TrySpawnEnemy();
                }
            }
        }

        /// <summary>
        /// Fill collections for mechanical parts with elements available for the given Level
        /// </summary>
        private void UpdateMechanicalParts()
        {
            hulls = playerSettings.Hulls.Where(h => h.Size == levelData.MaxEnemiesSize).Select(h => h.Name).ToList();
            towers = playerSettings.Towers.Where(t => t.Size == levelData.MaxEnemiesSize).Select(t => t.Name).ToList();
            tracks = playerSettings.Tracks.Where(t => t.Size == levelData.MaxEnemiesSize).Select(t => t.Name).ToList();
            guns = playerSettings.Guns.Where(g => g.Size == levelData.MaxEnemiesSize).Select(g => g.Name).ToList();
        }

        private async UniTask SpawnEnemy(Vector3 position)
        {
            enemyIndex++;

            await tankCreator.CreateTankAsync(GetRandomHull(),
                GetRandomTower(),
                GetRandomTrack(),
                GetRandomGun(),
                position,
                "EnemyTank_" + enemyIndex,
                GameObjectTag.Enemy);

            activeEnemies++;
            remainingEnemies--;
        }

        public void ExcludeEnemy()
        {
            activeEnemies--;
        }

        private async UniTask TrySpawnEnemy()
        {
            var randomSpawnPoint = SpawnPoints[random.Next(0, SpawnPoints.Count)];
            var collider = Physics2D.OverlapCircle(randomSpawnPoint, 1f);

            if (collider == null)
            {
                await SpawnEnemy(randomSpawnPoint);
            }
        }

        private HullName GetRandomHull()
        {
            return hulls[random.Next(0, hulls.Count)];
        }

        private TowerName GetRandomTower()
        {
            return towers[random.Next(0, towers.Count)];
        }

        private TrackName GetRandomTrack()
        {
            return tracks[random.Next(0, tracks.Count)];
        }

        private GunName GetRandomGun()
        {
            return guns[random.Next(0, guns.Count)];
        }
    }
}
