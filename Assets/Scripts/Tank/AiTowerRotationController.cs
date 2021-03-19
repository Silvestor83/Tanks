using System;
using System.Linq;
using Assets.Scripts.Core.GameData;
using Assets.Scripts.Core.Settings;
using Assets.Scripts.GameEntities.Units;
using Assets.Scripts.Infrastructure.Enums;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

namespace Assets.Scripts.Tank
{
    public class AiTowerRotationController : MonoBehaviour
    {
        // in degrees per second
        private float rotationSpeed;
        private Camera mainCamera;
        private PlayerData playerData;
        private Tower tower;

        [Inject]
        public void Init(PlayerData playerData, Tower tower)
        {
            this.playerData = playerData;
            this.tower = tower;
        }

        void Start()
        {
            rotationSpeed = tower.RotationSpeed;
        }

        void FixedUpdate()
        {
            var direction = playerData.position - (Vector2)transform.position;

            var angle = -Vector2.SignedAngle(direction, transform.up);

            var maxPossibleAngle = rotationSpeed * Time.fixedDeltaTime * Mathf.Sign(angle);

            transform.Rotate(Vector3.forward, Mathf.Abs(angle) > Mathf.Abs(maxPossibleAngle) ? maxPossibleAngle : angle);
        }
    }
}
