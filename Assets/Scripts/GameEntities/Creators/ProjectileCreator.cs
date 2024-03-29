﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.Controllers;
using Assets.Scripts.Core.Settings;
using Assets.Scripts.Infrastructure.Enums;
using Assets.Scripts.Managers;
using Assets.Scripts.Services;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Zenject;
using ZLogger;
using Object = UnityEngine.Object;

namespace Assets.Scripts.GameEntities.Creators
{
    public class ProjectileCreator : IDisposable
    {
        [Inject]
        private DiContainer container;
        private IEnumerable<GameObject> projectilePrefabs;
        private PlayerSettings settings;
        private LogService logService;
        private const string LABLE_PREFAB = "Projectile";
        private const string PROJECTILES_NAME_GO = "Projectiles";
        private AsyncOperationHandle<IList<GameObject>> handle;
        private GameObject projectilesGO;

        public ProjectileCreator(PlayerSettings settings, LogService logService)
        {
            this.logService = logService;
            this.settings = settings;

            projectilesGO = GameObject.Find(PROJECTILES_NAME_GO);

            handle = Addressables.LoadAssetsAsync<GameObject>(LABLE_PREFAB, (n) => { });
            handle.Completed += HandleOnCompleted;
        }

        /// <param name="root">Root GameObject that contains all Colliders which we need to exclude from collisions</param>
        public void CreateProjectile(ProjectileType type, Vector2 position, Quaternion rotation, Vector2 direction, Transform root)
        {
            if (projectilePrefabs != null)
            {
                var projectile = settings.Projectiles.First(p => p.type == type);
                var projectilePrefab = projectilePrefabs.First(p => p.name == type.ToString());
                var projectileGO = Object.Instantiate(projectilePrefab, position, rotation, projectilesGO.transform);

                container.InjectGameObjectForComponent<ShotController>(projectileGO, new object[] { projectile, direction });
                IgnoreСolliders(projectileGO, root);
            }
            else if (handle.Status == AsyncOperationStatus.Failed)
            {
                logService.Loggger.ZLogError("Failed to get projectiles prefab");
            }
        }

        private void IgnoreСolliders(GameObject baseGO, Transform root)
        {
            var baseCollider = baseGO.GetComponent<Collider2D>();
            var colliders = root.GetComponentsInChildren<Collider2D>();

            foreach (var collider2D in colliders)
            {
                Physics2D.IgnoreCollision(baseCollider, collider2D);
            }
        }

        private void HandleOnCompleted(AsyncOperationHandle<IList<GameObject>> obj)
        {
            projectilePrefabs = obj.Result;
        }

        public void Dispose()
        {
            handle.Completed -= HandleOnCompleted;
        }
    }
}
