﻿using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Core.Settings;
using Assets.Scripts.GameEntities.Units;
using Assets.Scripts.Infrastructure.Enums;
using Assets.Scripts.Services;
using Assets.Scripts.Tank;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Zenject;
using ZLogger;

namespace Assets.Scripts.GameEntities.Creators
{
    public class TankCreator
    {
        private PlayerSettings playerSettings;
        private LogService logService;
        private MechanicalPartsBuilder builder;
        private const string TANK_PREFAB = "Assets/Prefabs/Tanks/Tank.prefab";


        public TankCreator(PlayerSettings settings, LogService logService, MechanicalPartsBuilder builder)
        {
            this.playerSettings = settings;
            this.logService = logService;
            this.builder = builder;
        }

        public async UniTask CreateTankAsync(HullName hullName, TowerName towerName, TrackName trackName, GunName gunName, Vector3 position, string name = "Tank_clone")
        {
            var hull = playerSettings.Hulls.First(h => h.Name == hullName);
            var tower = playerSettings.Towers.First(t => t.Name == towerName);
            var track = playerSettings.Tracks.First(t => t.Name == trackName);
            var gun = playerSettings.Guns.First(g => g.Name == gunName);

            var tankRoot = await builder.CreateTankRoot(TANK_PREFAB, name, track, position, false);
            logService.Loggger.ZLogTrace($"Tank Root was created.");
            
            var hullGO = await builder.CreateHull(hull.PrefabName, hullName.ToString(), tankRoot.transform);
            logService.Loggger.ZLogTrace($"Hull was created.");

            var hullBindings = hullGO.GetComponent<HullBindings>();
            var leftPosition = hullGO.transform.position + (Vector3)hullBindings.LeftTruckPosition;
            var rightPosition = hullGO.transform.position + (Vector3)hullBindings.RightTruckPosition;
            await builder.CreateTracks(track.PrefabName, trackName.ToString(), leftPosition, rightPosition, hullGO.transform);
            logService.Loggger.ZLogTrace($"Tracks was created.");

            var towerGO = await builder.CreateTower(tower.PrefabName, towerName.ToString(), hullGO.transform, tower);
            logService.Loggger.ZLogTrace($"Tower was created.");

            var towerBindings = towerGO.GetComponent<TowerBindings>();
            var gunPosition = towerGO.transform.position + (Vector3)towerBindings.GunPosition;
            await builder.CreateGun(gun, gunName.ToString(), gunPosition, towerGO.transform);
            logService.Loggger.ZLogTrace($"Gun was created.");

            tankRoot.SetActive(true);
        }
    }
}