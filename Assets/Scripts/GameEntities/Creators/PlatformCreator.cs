using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.Core.Settings;
using Assets.Scripts.Infrastructure.Enums;
using Assets.Scripts.Services;
using Assets.Scripts.Tank;
using Cysharp.Threading.Tasks;
using UnityEngine;
using ZLogger;

namespace Assets.Scripts.GameEntities.Creators
{
    public class CannonCreator
    {
        private PlayerSettings playerSettings;
        private LogService logService;
        private MechanicalPartsBuilder builder;
        private const string TANK_ROOT_PREFAB = "Assets/Prefabs/Tanks/Tank.prefab";
        private const string CANNON_ROOT_PREFAB = "Assets/Prefabs/Tanks/Cannon.prefab";


        public CannonCreator(PlayerSettings settings, LogService logService, MechanicalPartsBuilder builder)
        {
            this.playerSettings = settings;
            this.logService = logService;
            this.builder = builder;
        }

        public async UniTask CreateCannonAsync(PlatformName platformName, TowerName towerName, GunName gunName, Vector3 position, string name = "Cannon_Clone", string tag = "Untagged")
        {
            var platform = playerSettings.Platforms.First(p => p.Name == platformName);
            var tower = playerSettings.Towers.First(t => t.Name == towerName);
            var gun = playerSettings.Guns.First(g => g.Name == gunName);
            var health = platform.Durability + tower.Durability;

            var cannonRoot = await builder.CreateCannonRoot(CANNON_ROOT_PREFAB, name, tag, health, position, false);
            logService.Loggger.ZLogTrace($"Cannon Root was created.");

            var platformGO = await builder.CreateHull(platform.PrefabName, platformName.ToString(), cannonRoot.transform);
            logService.Loggger.ZLogTrace($"Platform was created.");

            var towerGO = await builder.CreateTower(tower.PrefabName, towerName.ToString(), platformGO.transform, tower);
            logService.Loggger.ZLogTrace($"Tower was created.");

            var towerBindings = towerGO.GetComponent<TowerBindings>();
            var gunPosition = towerGO.transform.position + (Vector3)towerBindings.GunPosition;
            await builder.CreateGun(gun, gunName.ToString(), gunPosition, towerGO.transform);
            logService.Loggger.ZLogTrace($"Gun was created.");

            cannonRoot.SetActive(true);
        }
    }
}
