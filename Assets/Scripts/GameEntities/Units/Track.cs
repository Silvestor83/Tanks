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
    public class Track : MechanicalPart
    {
        public TrackName Name;
        public float MaxSpeed;
        public float MinSpeed;
        public float ForwardAcceleration;
        public float RearAcceleration;
        // Breaking acceleration
        public float BreakingAcceleration;
        /// <summary>
        /// In degrees per second
        /// </summary>
        public float rotateSpeed = 120;
        /// <summary>
        /// distance between two animation frames in Unity units
        /// </summary>
        public float AnimationStep;
        public string EngineSoundAssetName;
    }
}
