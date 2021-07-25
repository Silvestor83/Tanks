using System;
using System.Linq;
using Assets.Scripts.Controllers;
using Assets.Scripts.Core.GameData;
using Assets.Scripts.Core.Settings;
using Assets.Scripts.GameEntities.Units;
using Assets.Scripts.Infrastructure.Enums;
using Assets.Scripts.Services;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Zenject;
using ZLogger;
using Random = UnityEngine.Random;

namespace Assets.Scripts.GameEntities.Creators
{
    public class EnhancementsCreator
    {
        [Inject]
        private DiContainer container;
        private const string ENHANCEMENTS_NAME_GO = "Enhancements";
        private GameObject enhancementsGO;
        private readonly PlayerSettings playerSettings;
        private readonly LevelData levelData;
        private readonly MechanicalPartsBuilder builder;
        private readonly LogService logService;
        private readonly Vector3 trackLeftPosition = new Vector3(-0.45f, 0, 0);
        private readonly Vector3 trackRightPosition = new Vector3(0.45f, 0, 0);
        private const string ENHANCEMENT_ROOT_PREFAB = "Assets/Prefabs/Tanks/Enhancement.prefab";
        private const string HEALTH_ENHANCEMENT_PREFAB = "Assets/Prefabs/Tanks/Enhancements/Health.prefab";
        private int count = 0;

        public EnhancementsCreator(PlayerSettings playerSettings, LevelData levelData, MechanicalPartsBuilder builder, LogService logService)
        {
            this.playerSettings = playerSettings;
            this.levelData = levelData;
            this.builder = builder;
            this.logService = logService;
            enhancementsGO = GameObject.Find(ENHANCEMENTS_NAME_GO);
        }

        private async UniTask CreateMechanicalEnhancement(MechanicalPart part, Transform transform)
        {
            switch (part)
            {
                case Hull hull:
                    await builder.CreateHullAsync(part.PrefabName, hull.Name.ToString(), transform, GameObjectTag.Enhancement);
                    break;
                case Tower tower:
                    await builder.CreateTowerAsync(part.PrefabName, tower.Name.ToString(), transform, tower, GameObjectTag.Enhancement);
                    break;
                case Track track:
                    var leftPosition = transform.position + trackLeftPosition;
                    var rightPosition = transform.position + trackRightPosition;
                    await builder.CreateTracksAsync(part.PrefabName, track.Name.ToString(), null, leftPosition, rightPosition, transform, GameObjectTag.Enhancement);
                    break;
                case Gun gun:
                    await builder.CreateGunAsync(gun, gun.Name.ToString(), transform.position - new Vector3(0, 0.5f, 0), transform, GameObjectTag.Enhancement);
                    break;
                default:
                    throw new Exception("Wrong type of the MechanicalPart to create enhancement.");
            }
        }

        private async UniTask CreateTraitEnhancement(EnhancementType type, Transform transform)
        {
            switch (type)
            {
                case EnhancementType.Health:
                    var healthGO = await Addressables.InstantiateAsync(HEALTH_ENHANCEMENT_PREFAB, transform).ToUniTask();
                    healthGO.name = EnhancementType.Health.ToString();
                    healthGO.tag = GameObjectTag.Enhancement.ToString();
                    break;
                default:
                    throw new Exception("Wrong type of the EnhancementType to create enhancement.");
            }
        }

        private async UniTask<GameObject> CreateEnhancementRootAsync(string prefabKey, EnhancementType enhancementType, MechanicalPart part,
            string name, Vector3 position, bool isActive)
        {
            var enhancement = await Addressables.InstantiateAsync(prefabKey, position, Quaternion.identity, enhancementsGO.transform).ToUniTask();
            container.InjectGameObjectForComponent<EnhancementController>(enhancement, new object[] { enhancementType });

            if (enhancementType == EnhancementType.MechanicalPart)
            {
                enhancement.GetComponent<EnhancementController>().MechanicalPart = part;
            }

            enhancement.SetActive(isActive);
            enhancement.name = name;
            enhancement.tag = GameObjectTag.Enhancement.ToString();

            return enhancement;
        }

        public async UniTask TryCreateEnhancement(Vector3 position)
        {
            if (levelData.ChanceCreateEnhancement - Random.Range(1, 101) >= 0)
            {
                var enhancementType = GetRandomEnhancementType();
                MechanicalPart part = null;
                GameObject rootGO;

                var name = "enhancement_" + ++count;
                

                switch (enhancementType)
                {
                    case EnhancementType.MechanicalPart:
                    {
                        part = GetRandomPart();

                        rootGO = await CreateEnhancementRootAsync(ENHANCEMENT_ROOT_PREFAB, enhancementType, part, name, position, false);

                        await CreateMechanicalEnhancement(part, rootGO.transform);

                        if (part is Track)
                        {
                            rootGO.transform.GetChild(0).localScale = new Vector3(0.70f, 0.70f);
                            rootGO.transform.GetChild(1).localScale = new Vector3(0.70f, 0.70f);
                        }

                        break;
                    }
                    case EnhancementType.Health:
                        rootGO = await CreateEnhancementRootAsync(ENHANCEMENT_ROOT_PREFAB, enhancementType, part, name, position, false);
                        await CreateTraitEnhancement(enhancementType, rootGO.transform);
                        break;
                    default:
                        throw new Exception("Wrong type of the EnhancementType to create enhancement root.");
                }

                logService.Loggger.ZLogTrace($"GameObject ({name}) was created.");
                rootGO.SetActive(true);
            }
        }

        private EnhancementType GetRandomEnhancementType()
        {
            int relativeChance = Random.Range(1, 5);

            switch (relativeChance)
            {
                case 1:
                    return EnhancementType.Health;
                case 2:
                case 3:
                case 4:
                    return EnhancementType.MechanicalPart;
                default:
                    throw new Exception("Can't find a match for the random EnhancementType.");
            }
        }

        private MechanicalPart GetRandomPart()
        {
            int randomPartNum = Random.Range(1, 5);
            MechanicalPart part;

            switch (randomPartNum)
            {
                case 1:
                    part = playerSettings.Hulls[Random.Range(0, playerSettings.Hulls.Count)];
                    break;
                case 2:
                    part = playerSettings.Tracks[Random.Range(0, playerSettings.Tracks.Count)];
                    break;
                case 3:
                    part = playerSettings.Towers[Random.Range(0, playerSettings.Towers.Count)];
                    break;
                case 4:
                    part = playerSettings.Guns[Random.Range(0, playerSettings.Guns.Count)];
                    break;
                default:
                    throw new Exception("Can't get random MechanicalPart.");
            }

            return part;
        }
    }
}
