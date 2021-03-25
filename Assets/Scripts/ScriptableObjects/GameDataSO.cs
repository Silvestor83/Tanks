using Assets.Scripts.Core.GameData;
using Assets.Scripts.Providers;
using Assets.Scripts.Services;
using UnityEngine;
using Zenject;

namespace Assets.Scripts.ScriptableObjects
{
    [CreateAssetMenu(fileName = "GameDataSO", menuName = "Installers/GameData")]
    class GameDataSO : ScriptableObjectInstaller<GameDataSO>
    {
        [SerializeField]
        private PlayerData playerData;
        [SerializeField]
        private LevelData levelData;

        public override void InstallBindings()
        {
            Container.BindInstance(playerData);
            Container.BindInstance(levelData);
            Container.Bind<PathfindingService>().AsSingle();
            Container.Bind<PathfindingProvider>().AsTransient();
        }
    }
}
