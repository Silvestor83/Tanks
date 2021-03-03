using Assets.Scripts.Core.GameData;
using UnityEngine;
using Zenject;

namespace Assets.Scripts.ScriptableObjects
{
    [CreateAssetMenu(fileName = "GameDataSO", menuName = "Installers/GameData")]
    class GameDataSO : ScriptableObjectInstaller<GameDataSO>
    {
        [SerializeField]
        private PlayerData playerData;

        public override void InstallBindings()
        {
            Container.BindInstance(playerData);
        }
    }
}
