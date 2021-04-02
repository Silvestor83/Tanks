using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.Infrastructure.Enums;
using UnityEngine;
using UnityEngine.Serialization;

namespace Assets.Scripts.Core.GameData
{
    [Serializable]
    public class LevelData
    {
        public float GameFieldWidth;
        public float GameFieldHeight;
        public Vector2 OriginOffset;
        public float PathMaxDistance;
        public float PlayerTrackingDistance;
        public float EnemyAimingDistance;
        public float CannonAimingDistance;
        public float EnemyFirerateFactor;
        public float EnemyBulletsSpreadAngle;
        public int TotalEnemies;
        public int MaxEnemiesOnScene;
        public MechanicalPartSize MaxEnemiesSize;
    }
}
