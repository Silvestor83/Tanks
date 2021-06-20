using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.Controllers;
using Assets.Scripts.Core.Settings;
using Assets.Scripts.Infrastructure.Enums;
using Assets.Scripts.Services;
using Cysharp.Threading.Tasks;
using UnityEngine;
using ZLogger;

namespace Assets.Scripts.GameEntities.Creators
{
    public class CannonCreator : BasePartsCreator
    {
        private PlayerSettings playerSettings;
        private const string CANNON_ROOT_PREFAB = "Assets/Prefabs/Tanks/Cannon.prefab";


        public CannonCreator(PlayerSettings settings, LogService logService, MechanicalPartsBuilder builder) : base(logService, builder)
        {
            this.playerSettings = settings;
        }

        public async UniTask CreateCannonAsync(PlatformName platformName, TowerName towerName, GunName gunName, Vector3 position, string name, GameObjectTag tag)
        {
            var platform = playerSettings.Platforms.First(p => p.Name == platformName);
            var tower = playerSettings.Towers.First(t => t.Name == towerName);
            var gun = playerSettings.Guns.First(g => g.Name == gunName);
            var health = platform.Durability + tower.Durability;

            var cannonRoot = await builder.CreateCannonRootAsync(CANNON_ROOT_PREFAB, name, tag, health, position, false);
            logService.Loggger.ZLogTrace($"Cannon Root was created.");

            var platformGO = await builder.CreatePlatformAsync(platform.PrefabName, platformName.ToString(), cannonRoot.transform, tag);
            logService.Loggger.ZLogTrace($"Platform was created.");
            
            var towerGO = await CreateTowerAsync(tower, platformGO.transform, tag);
            await CreateGunAsync(gun, towerGO, tag);

            logService.Loggger.ZLogTrace($"GameObject ({name}) was created.");
            cannonRoot.SetActive(true);
        }
    }
}
