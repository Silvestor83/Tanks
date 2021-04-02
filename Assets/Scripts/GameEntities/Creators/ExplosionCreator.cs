using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.Core.Settings;
using Assets.Scripts.Infrastructure.Enums;
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
    public class ExplosionCreator : IDisposable
    {
        [Inject]
        private DiContainer container;
        private const string LABLE_PREFAB = "Explosion";
        private const string EXPLOSIONS_NAME_GO = "Explosions";
        private PlayerSettings settings;
        private LogService logService;
        private AsyncOperationHandle<IList<GameObject>> handle;
        private IEnumerable<GameObject> explosionPrefabs;
        private GameObject explosionsGO;

        public ExplosionCreator(PlayerSettings settings, LogService logService)
        {
            this.logService = logService;
            this.settings = settings;

            explosionsGO = GameObject.Find(EXPLOSIONS_NAME_GO);

            handle = Addressables.LoadAssetsAsync<GameObject>(LABLE_PREFAB, (n) => { });
            handle.Completed += HandleOnCompleted;
        }

        public void CreateExplosion(ExplosionType type, Vector2 position)
        {
            if (explosionPrefabs != null)
            {
                var explosionPrefab = explosionPrefabs.First(p => p.name == type.ToString());
                var explosionGO = Object.Instantiate(explosionPrefab, position, Quaternion.identity, explosionsGO.transform);

                //container.InjectGameObjectForComponent<ShotController>(projectileGO, new object[] { projectile, position, direction });
            }
            else if (handle.Status == AsyncOperationStatus.Failed)
            {
                logService.Loggger.ZLogError("Failed to get explosion prefab");
            }
        }

        private void HandleOnCompleted(AsyncOperationHandle<IList<GameObject>> obj)
        {
            explosionPrefabs = obj.Result;
        }

        public void Dispose()
        {
            handle.Completed -= HandleOnCompleted;
        }
    }
}
