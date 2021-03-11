using System.Collections.Generic;
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
        private const string TANK_ROOT_PREFAB = "Assets/Prefabs/Tanks/Tank.prefab";


        public TankCreator(PlayerSettings settings, LogService logService, MechanicalPartsBuilder builder)
        {
            this.playerSettings = settings;
            this.logService = logService;
            this.builder = builder;
        }

        public async UniTask<GameObject> CreateTankAsync(HullName hullName, TowerName towerName, TrackName trackName, GunName gunName, Vector3 position, string name, GameObjectTag tag)
        {
            var hull = playerSettings.Hulls.First(h => h.Name == hullName);
            var tower = playerSettings.Towers.First(t => t.Name == towerName);
            var track = playerSettings.Tracks.First(t => t.Name == trackName);
            var gun = playerSettings.Guns.First(g => g.Name == gunName);
            var health = hull.Durability + tower.Durability + track.Durability;

            // Create root
            var tankRoot = await builder.CreateTankRoot(TANK_ROOT_PREFAB, name, tag, health, track, position, false);
            logService.Loggger.ZLogTrace($"Tank Root was created.");
            
            // Create hull
            var hullGO = await builder.CreateHull(hull.PrefabName, hullName, tankRoot.transform);
            logService.Loggger.ZLogTrace($"Hull was created.");

            // Create tracks
            var hullBindings = hullGO.GetComponent<HullBindings>();
            var leftPosition = hullGO.transform.position + (Vector3)hullBindings.LeftTruckPosition;
            var rightPosition = hullGO.transform.position + (Vector3)hullBindings.RightTruckPosition;
            await builder.CreateTracks(track.PrefabName, trackName, leftPosition, rightPosition, hullGO.transform);
            logService.Loggger.ZLogTrace($"Tracks was created.");

            // Create tower
            var towerGO = await builder.CreateTower(tower.PrefabName, towerName, hullGO.transform, tower, tag);
            logService.Loggger.ZLogTrace($"Tower was created.");

            // Create gun
            var towerBindings = towerGO.GetComponent<TowerBindings>();
            var gunPosition = towerGO.transform.position + (Vector3)towerBindings.GunPosition;
            await builder.CreateGun(gun, gunName, gunPosition, towerGO.transform);
            logService.Loggger.ZLogTrace($"Gun was created.");

            tankRoot.SetActive(true);

            return tankRoot;
        }
    }
}
