using System;
using Assets.Scripts.Infrastructure.Enums;
using Assets.Scripts.Managers;
using UnityEngine;
using Zenject;

namespace Assets.Scripts.Controllers
{
    public class HealthController : MonoBehaviour
    {
        public int MaxHealth;
        public int CurrentHealth;

        private EnemiesManager enemiesManager;
        
        public void Init(int health)
        {
            MaxHealth = CurrentHealth = health;
        }

        [Inject]
        public void InitWithInjection(EnemiesManager enemiesManager)
        {
            this.enemiesManager = enemiesManager;
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
