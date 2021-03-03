using Assets.Scripts.GameEntities.Creators;
using Assets.Scripts.Infrastructure.Enums;
using UnityEngine;
using Zenject;

namespace Assets.Scripts
{
    public class MakeCannon : MonoBehaviour
    {
        private CannonCreator cannonCreator;

        [Inject]
        public void Init(CannonCreator creator)
        {
            this.cannonCreator = creator;
        }

        private async void Start()
        {
            Debug.Log("Creating cannon!!!");
            await cannonCreator.CreateCannonAsync(PlatformName.PlatformA, TowerName.SmallA, GunName.SmallC, new Vector3(2f, 2f), "Cannon", GameObjectTag.Enemy);
        }
    }
}
