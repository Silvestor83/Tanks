using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.Infrastructure.Enums;

namespace Assets.Scripts.GameEntities.Units
{
    [Serializable]
    public abstract class MechanicalPart
    {
        // Path for addressables
        public string PrefabName;
        public int Durability;
        public int Cost;
        public MechanicalPartSize Size;
    }
}
