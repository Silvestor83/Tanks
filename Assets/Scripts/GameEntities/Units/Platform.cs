using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.Infrastructure.Enums;

namespace Assets.Scripts.GameEntities.Units
{
    [Serializable]
    public class Platform : MechanicalPart
    {
        public PlatformName Name;
    }
}
