using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.Infrastructure.Enums;
using UnityEngine.Serialization;

namespace Assets.Scripts.GameEntities.Units
{
    [Serializable]
    public class Gun : MechanicalPart
    {
        public GunName Name;
        public int FiringRate;
        public ProjectileType ProjectileType;
    }
}
