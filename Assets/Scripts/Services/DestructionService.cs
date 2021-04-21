using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.Controllers;
using Assets.Scripts.GameEntities;
using Assets.Scripts.GameEntities.Creators;
using Assets.Scripts.Infrastructure.Enums;
using Assets.Scripts.Managers;
using Cysharp.Threading.Tasks;
using UnityEngine;
using ZLogger;
using Object = UnityEngine.Object;

namespace Assets.Scripts.Services
{
    public class DestructionService
    {
        private readonly ExplosionCreator explosionCreator;
        private readonly EnhancementsCreator enhancementsCreator;
        private readonly LogService logService;
        private readonly SceneManager sceneManager;
        private readonly EnemiesManager enemiesManager;

        public DestructionService(ExplosionCreator explosionCreator, EnhancementsCreator enhancementsCreator, LogService logService, SceneManager sceneManager, EnemiesManager enemiesManager)
        {
            this.explosionCreator = explosionCreator;
            this.enhancementsCreator = enhancementsCreator;
            this.logService = logService;
            this.sceneManager = sceneManager;
            this.enemiesManager = enemiesManager;
        }

        public async UniTask CheckDestruction(GameObject collisionGameObject, Projectile projectile)
        {
            if (collisionGameObject.CompareTag(GameObjectTag.Player.ToString())
                || collisionGameObject.CompareTag(GameObjectTag.Enemy.ToString())
                || collisionGameObject.CompareTag(GameObjectTag.Akvila.ToString())
                || collisionGameObject.CompareTag(GameObjectTag.Cannon.ToString()))
            {
                var healthController = collisionGameObject.GetComponent<HealthController>();

                healthController.ChangeHealth(healthController.CurrentHealth - projectile.Damage);

                if (healthController.CurrentHealth <= 0)
                {
                    var position = collisionGameObject.transform.position;

                    explosionCreator.CreateExplosion(ExplosionType.TankExplosion, position);
                    Object.Destroy(collisionGameObject);
                    logService.Loggger.ZLogTrace($"GameObject {collisionGameObject.name} was destroyed");

                    if (!collisionGameObject.CompareTag(GameObjectTag.Player.ToString())
                        && !collisionGameObject.CompareTag(GameObjectTag.Akvila.ToString()))
                    {
                        if (collisionGameObject.CompareTag(GameObjectTag.Enemy.ToString()))
                        {
                            await enemiesManager.ExcludeEnemy();
                        }

                        await enhancementsCreator.TryCreateEnhancement(position);
                    }
                    else
                    {
                        await UniTask.Delay(3000);
                        await sceneManager.LoadSceneAsync(SceneName.GameOver);
                    }
                }
            }
        }
    }
}
