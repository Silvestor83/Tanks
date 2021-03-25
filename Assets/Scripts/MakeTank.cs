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

        [Inject]
        public void Init(TankCreator creator)
        {
            this.tankCreator = creator;
        }

        private async void Start()
        {
            var playerTank = await tankCreator.CreateTankAsync(HullName.SmallC, TowerName.SmallC, TrackName.TrackA, GunName.SmallC, 
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
