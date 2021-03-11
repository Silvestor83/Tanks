using System;
using Assets.Scripts.GameEntities.Creators;
using Assets.Scripts.Infrastructure.Enums;
using UnityEngine;
using Zenject;

namespace Assets.Scripts
{
    public class MakeEnemy : MonoBehaviour
    {
        private TankCreator tankCreator;

        [Inject]
        public void Init(TankCreator creator)
        {
            this.tankCreator = creator;
        }

        private async void Awake()
        {
            var childTransforms = GetComponentsInChildren<Transform>();

            foreach (var childTransform in childTransforms)
            {
                if (childTransform.CompareTag(GameObjectTag.Portal.ToString()))
                {
                    await tankCreator.CreateTankAsync(HullName.SmallA, TowerName.SmallA, TrackName.TrackB, GunName.SmallA, childTransform.position, "EnemyTank", GameObjectTag.Enemy);
                }
            }
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
