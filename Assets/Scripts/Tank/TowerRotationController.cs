using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Assets.Scripts.Tank
{
    public class TowerRotationController : MonoBehaviour
    {
        // in degrees per second
        private float rotationSpeed = 90f;
        private Camera mainCamera;

        void Awake()
        {
            mainCamera = Camera.main;
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
