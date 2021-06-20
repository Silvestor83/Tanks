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

        [Inject]
        public void InitWithInjection(EnemiesManager enemiesManager, HealthService healthService)
        {
            this.enemiesManager = enemiesManager;
            this.healthService = healthService;
        }

        private void Start()
        {
            if (CompareTag(GameObjectTag.Akvila.ToString()))
            {
                MaxHealth = CurrentHealth = 500;
            }
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

            if (CompareTag(GameObjectTag.Akvila.ToString()))
            {
                healthService.AkvilaHealthChanged(CurrentHealth, MaxHealth);
            }
        }
    }
}
