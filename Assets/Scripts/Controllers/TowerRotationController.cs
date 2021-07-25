using System;
using System.Linq;
using Assets.Scripts.GameEntities.Units;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

namespace Assets.Scripts.Controllers
{
    public class TowerRotationController : MonoBehaviour
    {
        // in degrees per second
        private float rotationSpeed;
        private Camera mainCamera;

        [Inject]
        public void Init(Tower tower)
        {
            rotationSpeed = tower.RotationSpeed;
        }

        void Awake()
        {
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
