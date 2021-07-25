using Assets.Scripts.Core.GameData;
using Assets.Scripts.GameEntities.Units;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using Zenject;
using Vector3 = UnityEngine.Vector3;

namespace Assets.Scripts.Controllers
{
    public class MoveController : MoveBaseController
    {
        private float rotateInputValue;
        private float moveInputValue;
        private PlayerData playerData;

        [Inject]
        public void Init(PlayerData playerData, Track track)
        {
            this.playerData = playerData;
            UpdateTrackTraits(track);
        }

        void FixedUpdate()
        {
            var startingSpeed = currentSpeed;
            var startingRotationSpeed = currentRotationSpeed;

            Rotate();
            Move();
            SavePlayerData();
            EventsInvocation(track, startingSpeed, startingRotationSpeed);
        }

        private void SavePlayerData()
        {
            playerData.position = transform.position;
        }

        protected override void Rotate()
        {
            currentRotationSpeed = rotationSpeed;

            if (rotateInputValue > 0)
            {
                transform.Rotate(Vector3.forward, -rotationSpeed * Time.fixedUnscaledDeltaTime);
            }
            else if (rotateInputValue < 0)
            {
                transform.Rotate(Vector3.forward, rotationSpeed * Time.fixedUnscaledDeltaTime);
            }
            else
            {
                currentRotationSpeed = 0;
            }
        }

        protected override void Move()
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
