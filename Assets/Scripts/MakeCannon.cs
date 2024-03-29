﻿using System;
using Assets.Scripts.GameEntities.Creators;
using Assets.Scripts.Infrastructure.Enums;
using UnityEngine;
using Zenject;

namespace Assets.Scripts
{
    public class MakeCannon : MonoBehaviour
    {
        private CannonCreator cannonCreator;
        private Transform[] childTransforms;

        [Inject]
        public void Init(CannonCreator creator)
        {
            this.cannonCreator = creator;
        }

        private void Awake()
        {
            childTransforms = GetComponentsInChildren<Transform>();
        }

        private async void Start()
        {
            foreach (var childTransform in childTransforms)
            {
                if (childTransform.CompareTag(GameObjectTag.Cannon.ToString()))
                {
                    await cannonCreator.CreateCannonAsync(PlatformName.PlatformA, TowerName.SmallA, GunName.SmallB, childTransform.position, "Cannon", GameObjectTag.Cannon);
                }
            }
        }

        private void OnDrawGizmos()
        {
            var childTransforms = GetComponentsInChildren<Transform>();

            Gizmos.color = Color.yellow;

            foreach (var childTransform in childTransforms)
            {
                if (childTransform.CompareTag(GameObjectTag.Cannon.ToString()))
                {
                    Gizmos.DrawSphere(childTransform.position, 1);
                }
            }
        }
    }
}
