using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Controllers;
using Assets.Scripts.Core.GameData;
using Assets.Scripts.Core.Settings;
using Assets.Scripts.GameEntities.Units;
using Assets.Scripts.Infrastructure.Enums;
using Assets.Scripts.Managers;
using Assets.Scripts.Services;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Zenject;
using ZLogger;
using Object = UnityEngine.Object;

namespace Assets.Scripts.GameEntities.Creators
{
    public class TankCreator : BasePartsCreator
    {
        private PlayerSettings playerSettings;
        
        private const string TANK_ROOT_PREFAB = "Assets/Prefabs/Tanks/Tank.prefab";
        private PathfindingTagsManager tagsManager;
        private readonly PlayerData playerData;

        public TankCreator(PlayerSettings settings, LogService logService, MechanicalPartsBuilder builder, PathfindingTagsManager tagsManager, PlayerData playerData) : base(logService, builder)
        {
            this.playerSettings = settings;            
            this.tagsManager = tagsManager;
            this.playerData = playerData;
        }

        public async UniTask<GameObject> CreateTankAsync(HullName hullName, TowerName towerName, TrackName trackName, GunName gunName, Vector3 position, string name, GameObjectTag tag)
        {
            var hull = playerSettings.Hulls.First(h => h.Name == hullName);
            var tower = playerSettings.Towers.First(t => t.Name == towerName);
            var track = playerSettings.Tracks.First(t => t.Name == trackName);
            var gun = playerSettings.Guns.First(g => g.Name == gunName);
            var health = hull.Durability + tower.Durability + track.Durability;

            
            var tankRoot = await builder.CreateTankRootAsync(TANK_ROOT_PREFAB, name, tag, health, track, position, tagsManager.GetFreeNumber(), false);
            logService.Loggger.ZLogTrace($"Tank Root was created.");

            var hullGO = await CreateHullAsync(hull, tankRoot.transform, tag);
            await CreateTracksAsync(track, hullGO, tag);
            var towerGO = await CreateTowerAsync(tower, hullGO.transform, tag);
            await CreateGunAsync(gun, towerGO, tag);

            logService.Loggger.ZLogTrace($"GameObject ({name}) was created.");
            tankRoot.SetActive(true);

            return tankRoot;
        }

        public async UniTask UpgradePlayerTankAsync(MechanicalPart part, GameObject tank)
        {
            var hullGO = tank.GetComponentInChildren<HullBindings>().gameObject;
            var towerGOOld = tank.GetComponentInChildren<TowerRotationController>().gameObject;
            Gun gunOld;
            GameObject towerGO;

            switch (part)
            {
                case Hull hull:
                    Object.Destroy(hullGO);

                    var towerOld = playerSettings.Towers.First(t => t.Name == playerData.towerName);
                    var trackOld = playerSettings.Tracks.First(t => t.Name == playerData.trackName);
                    gunOld = playerSettings.Guns.First(g => g.Name == playerData.gunName);

                    var hullGONew = await CreateHullAsync(hull, tank.transform, GameObjectTag.Player);
                    hullGONew.SetActive(false);
                    await CreateTracksAsync(trackOld, hullGONew, GameObjectTag.Player);
                    towerGO = await CreateTowerAsync(towerOld, hullGONew.transform, GameObjectTag.Player);
                    await CreateGunAsync(gunOld, towerGO, GameObjectTag.Player);
                    hullGONew.SetActive(true);

                    playerData.hullName = hull.Name;
                    break;
                case Tower tower:
                    Object.Destroy(towerGOOld);

                    gunOld = playerSettings.Guns.First(g => g.Name == playerData.gunName);

                    towerGO = await CreateTowerAsync(tower, hullGO.transform, GameObjectTag.Player);
                    towerGO.SetActive(false);
                    await CreateGunAsync(gunOld, towerGO, GameObjectTag.Player);
                    towerGO.SetActive(true);

                    playerData.towerName = tower.Name;
                    break;
                case Track track:
                    var tracksOld = tank.GetComponentsInChildren<AnimationController>();

                    foreach (var trackController in tracksOld)
                    {
                        Object.Destroy(trackController.gameObject);
                    }

                    await CreateTracksAsync(track, hullGO, GameObjectTag.Player);

                    playerData.trackName = track.Name;
                    tank.GetComponent<MoveController>().UpdateTrackTraits(track);
                    break;
                case Gun gun:
                    var gunGOOld = tank.GetComponentInChildren<FiringController>().transform.parent.gameObject;
                    Object.Destroy(gunGOOld.gameObject);

                    await CreateGunAsync(gun, towerGOOld, GameObjectTag.Player);

                    playerData.gunName = gun.Name;

                    tank.GetComponent<GameController>().UpdatePlayersInput();
                    break;
                default:
                    throw new Exception("Wrong type of the MechanicalPart to create an enhancement.");
            }
        }
    }
}
