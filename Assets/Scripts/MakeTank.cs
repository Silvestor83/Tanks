using Assets.Scripts.Core.GameData;
using Assets.Scripts.GameEntities.Creators;
using Assets.Scripts.GameEntities.Units;
using Assets.Scripts.Infrastructure.Enums;
using Cinemachine;
using UnityEngine;
using Zenject;

namespace Assets.Scripts
{
    public class MakeTank : MonoBehaviour
    {
        private TankCreator tankCreator;
        private PlayerData playerData;

        [Inject]
        public void Init(TankCreator creator, PlayerData playerData)
        {
            this.tankCreator = creator;
            this.playerData = playerData;
        }

        private async void Start()
        {
            playerData.hullName = HullName.SmallA;
            playerData.trackName = TrackName.TrackA;
            playerData.towerName = TowerName.SmallA;
            playerData.gunName = GunName.SmallA;

            var playerTank = await tankCreator.CreateTankAsync(playerData.hullName, playerData.towerName, playerData.trackName, playerData.gunName, 
                transform.position, "PlayerTank", GameObjectTag.Player);

            var camera = Camera.main;
            var brain = camera.GetComponent<CinemachineBrain>();
            var vcam = brain.ActiveVirtualCamera as CinemachineVirtualCamera;
            vcam.Follow = playerTank.transform;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(transform.position, 1);
        }
    }
}
