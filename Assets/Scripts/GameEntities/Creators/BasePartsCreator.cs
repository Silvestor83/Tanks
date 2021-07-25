using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.Controllers;
using Assets.Scripts.GameEntities.Units;
using Assets.Scripts.Infrastructure.Enums;
using Assets.Scripts.Services;
using Cysharp.Threading.Tasks;
using UnityEngine;
using ZLogger;

namespace Assets.Scripts.GameEntities.Creators
{
    public class BasePartsCreator
    {
        protected LogService logService;
        protected MechanicalPartsBuilder builder;

        protected BasePartsCreator(LogService logService, MechanicalPartsBuilder builder)
        {
            this.logService = logService;
            this.builder = builder;
        }

        protected async UniTask<GameObject> CreateHullAsync(Hull hull, Transform parentTransform, GameObjectTag tag)
        {
            var hullGO = await builder.CreateHullAsync(hull.PrefabName, hull.Name.ToString(), parentTransform, tag);
            logService.Loggger.ZLogTrace($"Hull was created.");

            return hullGO;
        }

        protected async UniTask<GameObject> CreateTowerAsync(Tower tower, Transform parentTransform, GameObjectTag tag)
        {
            var towerGO = await builder.CreateTowerAsync(tower.PrefabName, tower.Name.ToString(), parentTransform, tower, tag);
            logService.Loggger.ZLogTrace($"Tower was created.");

            return towerGO;
        }

        protected async UniTask CreateTracksAsync(Track track, GameObject parentGO, GameObjectTag tag)
        {
            var hullBindings = parentGO.GetComponent<HullBindings>();
            var leftPosition = parentGO.transform.position + parentGO.transform.rotation * hullBindings.LeftTruckPosition;
            var rightPosition = parentGO.transform.position + parentGO.transform.rotation * hullBindings.RightTruckPosition;
            await builder.CreateTracksAsync(track.PrefabName, track.Name.ToString(), track.EngineSoundAssetName, leftPosition, rightPosition, parentGO.transform, tag);
            logService.Loggger.ZLogTrace($"Tracks was created.");
        }

        protected async UniTask<GameObject> CreateGunAsync(Gun gun, GameObject parentGO, GameObjectTag tag)
        {
            var towerBindings = parentGO.GetComponent<TowerBindings>();
            var gunPosition = parentGO.transform.position + parentGO.transform.rotation * towerBindings.GunPosition;
            var gunGO = await builder.CreateGunAsync(gun, gun.Name.ToString(), gunPosition, parentGO.transform, tag);
            logService.Loggger.ZLogTrace($"Gun was created.");

            return gunGO;
        }
    }
}
