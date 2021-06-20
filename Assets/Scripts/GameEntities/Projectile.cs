using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.Infrastructure.Enums;

namespace Assets.Scripts.GameEntities
{
    [Serializable]
    public class Projectile
    {
        public ProjectileType type;
        public ExplosionType explosionType;
        public int Damage;
        public float Speed;
    }
}
