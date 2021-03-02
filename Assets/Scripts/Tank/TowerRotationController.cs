using System;
using System.Linq;
using Assets.Scripts.GameEntities.Units;
using Assets.Scripts.Infrastructure.Enums;
using Assets.Scripts.Services;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;
using PlayerSettings = Assets.Scripts.Core.Settings.PlayerSettings;

namespace Assets.Scripts.Tank
{
    public class TowerRotationController : MonoBehaviour
    {
        // in degrees per second
        private float rotationSpeed;
        private Camera mainCamera;
        private PlayerSettings playerSettings;
        private Tower tower;

        [Inject]
        public void Init(PlayerSettings playerSettings, Tower tower = null)
        {
            this.playerSettings = playerSettings;
            this.tower = tower;
        }

        void Start()
        {
            if (tower == null)
            {
                tower = playerSettings.Towers.First(t => t.Name == TowerName.SmallA);
            }
            rotationSpeed = tower.RotationSpeed;
        }

        void Awake()
        {
            //mainCamera = Camera.main;
            var cameras = GameObject.FindGameObjectsWithTag("MainCamera");
            mainCamera = cameras.First(c => c.name == "LevelCamera").GetComponent<Camera>();
        }

        void FixedUpdate()
        {
            var mouseScreenPosition = Mouse.current.position.ReadValue();
            var mouseWorldPosition = (Vector2)mainCamera.ScreenToWorldPoint(mouseScreenPosition);
            var objectPosition = (Vector2)transform.position;

            var direction = mouseWorldPosition - objectPosition;

            var angle = -Vector2.SignedAngle(direction, transform.up);

            var maxPossibleAngle = rotationSpeed * Time.fixedDeltaTime * Math.Sign(angle);

            transform.Rotate(Vector3.forward, Math.Abs(angle) > Math.Abs(maxPossibleAngle) ? maxPossibleAngle : angle);
        }
    }
}
