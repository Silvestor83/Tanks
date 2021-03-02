using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.Infrastructure.Enums;

namespace Assets.Scripts.GameEntities.Units
{
    public class DummyTank
    {
        public HullName HullName;
        public TowerName TowerName;
        public TrackName TrackName;
        public GunName GunName;
        
        public DummyTank(HullName hullName, TowerName towerName, TrackName trackName, GunName gunName)
        {
            HullName = hullName;
            TowerName = towerName;
            TrackName = trackName;
            GunName = gunName;
        }
    }
}
