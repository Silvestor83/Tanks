using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.Core.GameData;
using Assets.Scripts.GameEntities.Units;
using Assets.Scripts.Infrastructure.Enums;
using Assets.Scripts.Services;
using UnityEngine;
using UnityEngine.Events;
using Zenject;
using ZLogger;

namespace Assets.Scripts.Tank
{
    public class AiMoveController : MonoBehaviour
    {
        private float maxSpeed;
        private float minSpeed;
        private float forwardAcceleration;
        private float rearAcceleration;
        private float breakingAcceleration;
        private float rotateSpeed;

        private float currentSpeed;
        private float currentRotationSpeed;
        private float rotateInputValue;
        private float moveInputValue;

        private Track track;
        private LogService logService;
        private PlayerData playerData;

        public readonly UnityEvent<float, float> StateChanged = new UnityEvent<float, float>();

        [Inject]
        public void Init(PlayerData playerData, LogService logService, Track track)
        {
            this.playerData = playerData;
            this.logService = logService;
            this.track = track;
        }

        // Start is called before the first frame updates
        void Start()
        {
            maxSpeed = track.MaxSpeed;
            minSpeed = track.MinSpeed;
            forwardAcceleration = track.ForwardAcceleration;
            rearAcceleration = track.RearAcceleration;
            breakingAcceleration = track.BreakingAcceleration;
            rotateSpeed = track.rotateSpeed;
        }

        void FixedUpdate()
        {
            var startingSpeed = currentSpeed;
            var startingRotationSpeed = currentRotationSpeed;

            Rotate();
            Move();

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
                if (currentSpeed <= maxSpeed)
                {
                    accelerate = currentSpeed >= 0 ? forwardAcceleration : breakingAcceleration;
                    tempSpeed = accelerate * Time.fixedUnscaledDeltaTime + currentSpeed;
                    currentSpeed = tempSpeed > maxSpeed ? maxSpeed : tempSpeed;
                }
            }
            else if (moveInputValue < 0)
            {
                if (currentSpeed >= minSpeed)
                {
                    accelerate = currentSpeed <= 0 ? -rearAcceleration : -breakingAcceleration;
                    tempSpeed = accelerate * Time.fixedUnscaledDeltaTime + currentSpeed;
                    currentSpeed = tempSpeed < minSpeed ? minSpeed : tempSpeed;
                }
            }
            else
            {
                accelerate = currentSpeed > 0 ? -breakingAcceleration : breakingAcceleration;
                tempSpeed = accelerate * Time.fixedUnscaledDeltaTime + currentSpeed;
                currentSpeed = (currentSpeed > 0 && tempSpeed > 0) || (currentSpeed < 0 && tempSpeed < 0) ? tempSpeed : 0;
            }

            transform.Translate(currentSpeed * Time.fixedDeltaTime * transform.up, Space.World);
        }

        private void EventsInvocation(float startingSpeed, float startingRotationSpeed)
        {
            if (startingRotationSpeed != currentRotationSpeed || startingSpeed != currentSpeed)
            {
                StateChanged.Invoke(Mathf.Abs(currentSpeed), currentRotationSpeed);
            }
        }
    }
}
