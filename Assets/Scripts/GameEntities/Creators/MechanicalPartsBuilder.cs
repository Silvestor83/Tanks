using System;
using Assets.Scripts.Controllers;
using Assets.Scripts.GameEntities.Units;
using Assets.Scripts.Infrastructure;
using Assets.Scripts.Infrastructure.Enums;
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
        private const string ENEMIES_NAME_GO = "Enemies";
        private const string CANNONS_NAME_GO = "Cannons";
        private GameObject enemiesGO;
        private GameObject cannonsGO;

        public MechanicalPartsBuilder()
        {
            enemiesGO = GameObject.Find(ENEMIES_NAME_GO);
            cannonsGO = GameObject.Find(CANNONS_NAME_GO);
        }

        public async UniTask<GameObject> CreateTankRoot(string prefabKey, string name, GameObjectTag tag, int health, Track track, Vector3 position, uint tankNumber, bool isActive)
        {
            var parentTransform = tag == GameObjectTag.Enemy ? enemiesGO.transform : null;
            var tank = await Addressables.InstantiateAsync(prefabKey, position, Quaternion.identity, parentTransform).ToUniTask();
            tank.SetActive(isActive);
            tank.name = name;
            tank.tag = tag.ToString();

            if (tag == GameObjectTag.Player)
            {
                tank.AddComponent<MoveController>();
                container.InjectGameObjectForComponent<MoveController>(tank, new object[] { track });
            }
            else if (tag == GameObjectTag.Enemy)
            {
                tank.AddComponent<AiMoveController>();
                container.InjectGameObjectForComponent<AiMoveController>(tank, new object[] { track, tankNumber });
            }
            else
            {
                throw new Exception($"Сan't create tank root with '{tag.ToString()}' tag.");
            }
            
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
            var cannon = await Addressables.InstantiateAsync(prefabKey, position, Quaternion.identity, cannonsGO.transform).ToUniTask();
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

        public async UniTask<GameObject> CreateHull(string prefabKey, HullName name, string engineSoundAssetName, Transform parentTransform)
        {
            var hullGO = await Addressables.InstantiateAsync(prefabKey, parentTransform).ToUniTask();
            hullGO.name = name.ToString();

            var engineAudioClip = await Addressables.LoadAssetAsync<AudioClip>(engineSoundAssetName);
            hullGO.GetComponent<AudioSource>().clip = engineAudioClip;
            
            return hullGO;
        }

        public async UniTask<GameObject> CreatePlatform(string prefabKey, PlatformName name, Transform parentTransform)
        {
            var platformGO = await Addressables.InstantiateAsync(prefabKey, parentTransform).ToUniTask();
            platformGO.name = name.ToString();

            return platformGO;
        }

        public async UniTask<GameObject> CreateTower(string prefabKey, TowerName name, Transform parentTransform, Tower tower, GameObjectTag tag)
        {
            var towerGO = await Addressables.InstantiateAsync(prefabKey, parentTransform).ToUniTask();
            towerGO.name = name.ToString();

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
            else if (tag == GameObjectTag.Cannon)
            {
                towerGO.AddComponent<AiPlatformRotationController>();
                container.InjectGameObjectForComponent<AiPlatformRotationController>(towerGO, new object[] { tower });
            }
            else
            {
                throw new Exception($"Сan't create towerGO with '{tag.ToString()}' tag.");
            }

            return towerGO;
        }

        public async UniTask CreateTracks(string prefabKey, TrackName name, Vector3 leftPosition, Vector3 rightPosition, Transform parentTransform)
        {
            var trackPrefab = await Addressables.LoadAssetAsync<GameObject>(prefabKey).ToUniTask();

            var leftTrack = Object.Instantiate(trackPrefab, leftPosition, Quaternion.identity, parentTransform);
            leftTrack.name = name + "Left";
            
            var rightTrack = Object.Instantiate(trackPrefab, rightPosition, Quaternion.identity, parentTransform);
            rightTrack.name = name + "Right";
        }

        public async UniTask CreateGun(Gun gun, GunName name, Vector3 position, Transform parentTransform, GameObjectTag tag)
        {
            var gunGO = await Addressables.InstantiateAsync(gun.PrefabName, position, Quaternion.identity, parentTransform).ToUniTask();
            gunGO.name = name.ToString();

            var gunBindings = gunGO.GetComponentInChildren<GunBindings>();
            var barrelGO = gunBindings.gameObject;

            if (tag == GameObjectTag.Player)
            {
                barrelGO.AddComponent<FiringController>();
                container.InjectGameObjectForComponent<FiringController>(barrelGO, new object[] { gun });
            }
            else if (tag == GameObjectTag.Enemy || tag == GameObjectTag.Cannon)
            {
                barrelGO.AddComponent<AiFiringController>();
                container.InjectGameObjectForComponent<AiFiringController>(barrelGO, new object[] { gun });
            }
            else
            {
                throw new Exception($"Сan't create towerGO with '{tag.ToString()}' tag.");
            }
        }
    }
}
