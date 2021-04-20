using System;
using Assets.Scripts.Infrastructure.Enums;
using UnityEngine;

namespace Assets.Scripts.Core.GameData
{
    [Serializable]
    public class PlayerData
    {
        public Vector2 position;
        public HullName hullName;
        public TowerName towerName;
        public TrackName trackName;
        public GunName gunName;
    }
}
