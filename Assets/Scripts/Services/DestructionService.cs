using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.Controllers;
using Assets.Scripts.GameEntities;
using Assets.Scripts.Infrastructure.Enums;
using UnityEngine;

namespace Assets.Scripts.Services
{
    public class DestructionService
    {
        public event EventHandler<DamageEventArgs> DamageDone; 

        public void CheckDestruction(GameObject collisionGameObject, Projectile projectile)
        {
            if (collisionGameObject.CompareTag(GameObjectTag.Player.ToString())
                || collisionGameObject.CompareTag(GameObjectTag.Enemy.ToString())
                || collisionGameObject.CompareTag(GameObjectTag.Cannon.ToString()))
            {
                var healthController = collisionGameObject.GetComponent<HealthController>();
                healthController.CurrentHealth -= projectile.Damage;

                if (collisionGameObject.CompareTag(GameObjectTag.Player.ToString()))
                {
                    OnDamageDone(new DamageEventArgs(healthController.MaxHealth, healthController.CurrentHealth));
                }

                if (healthController.CurrentHealth <= 0 && !collisionGameObject.CompareTag(GameObjectTag.Player.ToString()))
                {
                    UnityEngine.Object.Destroy(collisionGameObject);
                }
            }
        }

        private void OnDamageDone(DamageEventArgs e)
        {
            if (DamageDone != null)
            {
                DamageDone(this, e);
            }
        }
    }

    public class DamageEventArgs : EventArgs
    {
        public readonly float CurrentHealth;
        public readonly float MaxHealth;

        public DamageEventArgs(float maxHealth, float currentHealth)
        {
            MaxHealth = maxHealth;
            CurrentHealth = currentHealth;
        }
    }
}
