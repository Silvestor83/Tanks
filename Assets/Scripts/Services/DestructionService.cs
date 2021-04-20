using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.Controllers;
using Assets.Scripts.GameEntities;
using Assets.Scripts.GameEntities.Creators;
using Assets.Scripts.Infrastructure.Enums;
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

        public DestructionService(ExplosionCreator explosionCreator, EnhancementsCreator enhancementsCreator, LogService logService)
        {
            this.explosionCreator = explosionCreator;
            this.enhancementsCreator = enhancementsCreator;
            this.logService = logService;
        }

        public async UniTask CheckDestruction(GameObject collisionGameObject, Projectile projectile)
        {
            if (collisionGameObject.CompareTag(GameObjectTag.Player.ToString())
                || collisionGameObject.CompareTag(GameObjectTag.Enemy.ToString())
                || collisionGameObject.CompareTag(GameObjectTag.Cannon.ToString()))
            {
                var healthController = collisionGameObject.GetComponent<HealthController>();

                healthController.ChangeHealth(healthController.CurrentHealth - projectile.Damage);

                //if (collisionGameObject.CompareTag(GameObjectTag.Player.ToString()))
                //{
                //    OnDamageDone(new DamageEventArgs(healthController.MaxHealth, healthController.CurrentHealth));
                //}

                if (healthController.CurrentHealth <= 0 && !collisionGameObject.CompareTag(GameObjectTag.Player.ToString()))
                {
                    var position = collisionGameObject.transform.position;

                    explosionCreator.CreateExplosion(ExplosionType.TankExplosion, position);

                    if (collisionGameObject != null)
                    {
                        logService.Loggger.ZLogTrace($"GameObject {collisionGameObject.name} was destroyed");
                        Object.Destroy(collisionGameObject);
                        await enhancementsCreator.TryCreateEnhancement(position);
                    }
                }
            }
        }
    }
}
