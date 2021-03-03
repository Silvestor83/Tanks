using Assets.Scripts.GameEntities.Creators;
using Assets.Scripts.GameEntities.Units;
using Assets.Scripts.Infrastructure.Enums;
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
            Debug.Log("Creating tank!!!");
            await tankCreator.CreateTankAsync(HullName.SmallC, TowerName.SmallC, TrackName.TrackA, GunName.SmallB, new Vector3(5f, 5f), "PlayerTank", GameObjectTag.Player.ToString());
        }
    }
}
