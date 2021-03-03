using Assets.Scripts.GameEntities.Creators;
using Assets.Scripts.Infrastructure.Enums;
using UnityEngine;
using Zenject;

namespace Assets.Scripts
{
    public class MakeEnemy : MonoBehaviour
    {
        private TankCreator tankCreator;

        [Inject]
        public void Init(TankCreator creator)
        {
            this.tankCreator = creator;
        }

        private async void Start()
        {
            Debug.Log("Creating enemy!!!");
            await tankCreator.CreateTankAsync(HullName.SmallA, TowerName.SmallA, TrackName.TrackB, GunName.SmallA, new Vector3(5f, 0), "EnemyTank", GameObjectTag.Enemy);
        }
    }
}
