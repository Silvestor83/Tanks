using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.GameEntities.Units
{
    public abstract class MechanicalPart
    {
        public int Durability { get; set; }
        public int Cost { get; set; }
    }
}
