using System;
using System.Linq;
using Assets.Scripts.GameEntities.Creators;
using Assets.Scripts.Infrastructure.Enums;
using Assets.Scripts.Managers;
using UnityEngine;
using Zenject;

namespace Assets.Scripts
{
    public class MakeEnemy : MonoBehaviour
    {
        private EnemiesManager enemiesManager;

        [Inject]
        public void Init(EnemiesManager enemiesManager)
        {
            this.enemiesManager = enemiesManager;
        }

        private void Awake()
        {
            var childTransforms = GetComponentsInChildren<Transform>();
            enemiesManager.SpawnPoints.AddRange(childTransforms.Where(c => c.CompareTag(GameObjectTag.Portal.ToString())).Select(c => c.position));
        }

        private async void Start()
        {
            await enemiesManager.SpawnEnemies();
        }

        private void OnDrawGizmos()
        {
            var childTransforms = GetComponentsInChildren<Transform>();

            Gizmos.color = Color.red;

            foreach (var childTransform in childTransforms)
            {
                if (childTransform.CompareTag(GameObjectTag.Portal.ToString()))
                {
                    Gizmos.DrawSphere(childTransform.position, 1);
                }
            }
        }
    }
}
