﻿using System;
using Assets.Scripts.GameEntities.Units;
using Assets.Scripts.Infrastructure;
using Assets.Scripts.Infrastructure.Enums;
using Assets.Scripts.Tank;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Zenject;
using Object = UnityEngine.Object;

namespace Assets.Scripts.GameEntities.Creators
{
    public class MechanicalPartsBuilder
    {
        [Inject]
        private DiContainer container;

        public async UniTask<GameObject> CreateTankRoot(string prefabKey, string name, GameObjectTag tag, int health, Track track, Vector3 position, bool isActive)
        {
            var tank = await Addressables.InstantiateAsync(prefabKey, position, Quaternion.identity).ToUniTask();
            tank.SetActive(isActive);
            tank.name = name;
            tank.tag = tag.ToString();

            if (!tank.HasComponent<MoveController>())
            {
                throw new Exception($"TankRoot GO from {prefabKey} prefab doesn't contain MoveController component.");
            }
            container.InjectGameObjectForComponent<MoveController>(tank, new object[] { track });

            if (!tank.HasComponent<HealthController>())
            {
                throw new Exception($"TankRoot GO from {prefabKey} prefab doesn't contain HealthController component.");
            }
            var healthController = tank.GetComponent<HealthController>();
            healthController.Init(health);

            return tank;
        }

        public async UniTask<GameObject> CreateCannonRoot(string prefabKey, string name, GameObjectTag tag, int health, Vector3 position, bool isActive)
        {
            var cannon = await Addressables.InstantiateAsync(prefabKey, position, Quaternion.identity).ToUniTask();
            cannon.SetActive(isActive);
            cannon.name = name;
            cannon.tag = tag.ToString();

            if (!cannon.HasComponent<HealthController>())
            {
                throw new Exception($"CannonRoot GO from {prefabKey} prefab doesn't contain HealthController component.");
            }
            var healthController = cannon.GetComponent<HealthController>();
            healthController.Init(health);

            return cannon;
        }

        public async UniTask<GameObject> CreateHull(string prefabKey, string name, Transform parentTransform)
        {
            var hullGO = await Addressables.InstantiateAsync(prefabKey, parentTransform).ToUniTask();
            hullGO.name = name;
            
            return hullGO;
        }

        public async UniTask<GameObject> CreateTower(string prefabKey, string name, Transform parentTransform, Tower tower, GameObjectTag tag)
        {
            var towerGO = await Addressables.InstantiateAsync(prefabKey, parentTransform).ToUniTask();
            towerGO.name = name;

            if (tag == GameObjectTag.Player)
            {
                towerGO.AddComponent<TowerRotationController>();
                container.InjectGameObjectForComponent<TowerRotationController>(towerGO, new object[] { tower });
            }
            else if (tag == GameObjectTag.Enemy)
            {
                towerGO.AddComponent<AiTowerRotationController>();
                container.InjectGameObjectForComponent<AiTowerRotationController>(towerGO, new object[] { tower });
            }
            else
            {
                throw new Exception($"Сan't create towerGO with '{tag.ToString()}' tag.");
            }

            return towerGO;
        }

        public async UniTask CreateTracks(string prefabKey, string name, Vector3 leftPosition, Vector3 rightPosition, Transform parentTransform)
        {
            var trackPrefab = await Addressables.LoadAssetAsync<GameObject>(prefabKey).ToUniTask();

            var leftTrack = Object.Instantiate(trackPrefab, leftPosition, Quaternion.identity, parentTransform);
            leftTrack.name = name + "Left";
            
            var rightTrack = Object.Instantiate(trackPrefab, rightPosition, Quaternion.identity, parentTransform);
            rightTrack.name = name + "Right";
        }

        public async UniTask CreateGun(Gun gun, string name, Vector3 position, Transform parentTransform)
        {
            var gunGO = await Addressables.InstantiateAsync(gun.PrefabName, position, Quaternion.identity, parentTransform).ToUniTask();
            gunGO.name = name;

            container.InjectGameObjectForComponent<FiringController>(gunGO, new object[] {gun.ProjectileType});
        }
    }
}
