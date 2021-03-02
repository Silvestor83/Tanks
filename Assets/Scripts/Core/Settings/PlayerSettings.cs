using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.GameEntities;
using Assets.Scripts.GameEntities.Units;

namespace Assets.Scripts.Core.Settings
{
    [Serializable]
    public class PlayerSettings
    {
        public List<Hull> Hulls;
        public List<Tower> Towers;
        public List<Track> Tracks;
        public List<Gun> Guns;
        public List<Projectile> Projectiles;
    }
}
