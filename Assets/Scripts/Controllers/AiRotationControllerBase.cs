using Assets.Scripts.Core.GameData;
using Assets.Scripts.GameEntities.Units;
using UnityEngine;
using Zenject;

namespace Assets.Scripts.Controllers
{
    public class AiRotationControllerBase : MonoBehaviour
    {
        // in degrees per second
        protected float rotationSpeed;
        protected PlayerData playerData;
        protected LevelData levelData;
        protected Tower tower;
        protected Transform parent;

        protected float distanceToPlayer;
        protected float distanceToAkvila;
        protected float currentAngle;
        public bool InLineOfSight = false;

        [Inject]
        public void Init(PlayerData playerData, LevelData levelData, Tower tower)
        {
            this.playerData = playerData;
            this.levelData = levelData;
            this.tower = tower;
        }
    }
}
