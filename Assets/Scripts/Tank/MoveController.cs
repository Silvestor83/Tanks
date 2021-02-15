using System;
using Assets.Scripts.Services;
using Microsoft.Extensions.Logging;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Composites;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Zenject;
using ZLogger;
using Button = UnityEngine.UIElements.Button;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

namespace Assets.Scripts.Tank
{
    public class MoveController : MonoBehaviour
    {
        public float MaxSpeed = 4f;
        public float MinSpeed = -2f;

        public int ForwardAccelerate = 8;
        public int RearAccelerate = -8;
        public int BrakingAcceleration = 16;

        private int rotateSpeed = 120;
        private int currentRotationSpeed;
        private float rotateInputValue;
        private float moveInputValue;

        public float currentSpeed;
        private bool isInit;
        public readonly UnityEvent<float,float> StateChanged = new UnityEvent<float, float>();
        
        private LogService logService;

        [Inject]
        public void Init(LogService logService)
        {
            this.logService = logService;
        }

        // Start is called before the first frame update
        void Start()
        {
            Debug.DrawRay(gameObject.transform.position, gameObject.transform.up * 5, Color.green, 60f);
            Debug.DrawRay(gameObject.transform.position, new Vector3(2.05f, -1.66f, 0) * 5, Color.blue, 60f);
        }

        void FixedUpdate()
        {
            var startingSpeed = currentSpeed;
            var startingRotationSpeed = currentRotationSpeed;

            Rotate();
            Move();
            //OtherActions();

            EventsInvocation(startingSpeed, startingRotationSpeed);
        }

        private void Rotate()
        {
            currentRotationSpeed = rotateSpeed;

            if (rotateInputValue > 0)
            {
                transform.Rotate(Vector3.forward, -rotateSpeed * Time.fixedUnscaledDeltaTime);
            }
            else if (rotateInputValue < 0)
            {
                transform.Rotate(Vector3.forward, rotateSpeed * Time.fixedUnscaledDeltaTime);
            }
            else
            {
                currentRotationSpeed = 0;
            }
        }

        private void Move()
        {
            float accelerate;
            float tempSpeed;

            if (moveInputValue > 0)
            {
                if (currentSpeed <= MaxSpeed)
                {
                    accelerate = currentSpeed > 0 ? ForwardAccelerate : BrakingAcceleration;
                    tempSpeed = accelerate * Time.fixedUnscaledDeltaTime + currentSpeed;
                    currentSpeed = tempSpeed > MaxSpeed ? MaxSpeed : tempSpeed;
                }
            }
            else if (moveInputValue < 0)
            {
                if (currentSpeed >= MinSpeed)
                {
                    accelerate = currentSpeed < 0 ? RearAccelerate : -BrakingAcceleration;
                    tempSpeed = accelerate * Time.fixedUnscaledDeltaTime + currentSpeed;
                    currentSpeed = tempSpeed < MinSpeed ? MinSpeed : tempSpeed;
                }
            }
            else
            {
                accelerate = currentSpeed > 0 ? -BrakingAcceleration : BrakingAcceleration;
                tempSpeed = accelerate * Time.fixedUnscaledDeltaTime + currentSpeed;
                currentSpeed = (currentSpeed > 0 && tempSpeed > 0) || (currentSpeed < 0 && tempSpeed < 0) ? tempSpeed : 0;
            }

            transform.Translate(currentSpeed * Time.fixedDeltaTime * transform.up, Space.World);
        }

        private void OtherActions()
        {
            if (Input.GetKey(KeyCode.Space))
            {
                var posX = gameObject.transform.position.x;
                var posY = gameObject.transform.position.y;
                var posZ = gameObject.transform.position.z;

                Debug.Log($"X: {posX}, Y: {posY}, Z: {posZ}");
            }

            if (Input.GetKey(KeyCode.Escape))
            {
                logService.Loggger.ZLogTrace("Application quite");
                Application.Quit();
            }
        }

        private void EventsInvocation(float startingSpeed, float startingRotationSpeed)
        {
            if (startingRotationSpeed != currentRotationSpeed || startingSpeed != currentSpeed)
            {
                StateChanged.Invoke(currentSpeed, currentRotationSpeed);
            }
        }

        private void OnMove(InputValue value)
        {
            moveInputValue = value.Get<float>();
        }

        private void OnRotate(InputValue value)
        {
            rotateInputValue = value.Get<float>();
        }
    }
}
