using System;
using Assets.Scripts.Infrastructure.Enums;
using Assets.Scripts.Managers;
using Assets.Scripts.Services;
using UnityEngine;
using Zenject;

namespace Assets.Scripts.Controllers
{
    public class HealthController : MonoBehaviour
    {
        public int CurrentHealth { get; private set; }
        public int MaxHealth { get; private set; }

        private EnemiesManager enemiesManager;
        private HealthService healthService; 

        public void Init(int health)
        {
            MaxHealth = CurrentHealth = health;
        }

        [Inject]
        public void InitWithInjection(EnemiesManager enemiesManager, HealthService healthService)
        {
            this.enemiesManager = enemiesManager;
            this.healthService = healthService;
        }

        public void ChangeHealth(int currentHealth, int? maxHealth = null)
        {
            CurrentHealth = currentHealth;

            if (maxHealth != null)
            {
                MaxHealth = maxHealth.Value;
            }

            if (CompareTag(GameObjectTag.Player.ToString()))
            {
                healthService.PlayerHealthChanged(CurrentHealth, MaxHealth);
            }
        }

        private void OnDestroy()
        {
            if (CompareTag(GameObjectTag.Enemy.ToString()))
            {
                enemiesManager.ExcludeEnemy();
            }
        }
    }
}
