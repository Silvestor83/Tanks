﻿using Assets.Scripts.Core.GameData;
using Assets.Scripts.GameEntities.Units;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using Zenject;
using Vector3 = UnityEngine.Vector3;

namespace Assets.Scripts.Controllers
{
    public class MoveController : MonoBehaviour
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
        private PlayerData playerData;

        public readonly UnityEvent<Track, float, float> StateChanged = new UnityEvent<Track, float, float>();

        [Inject]
        public void Init(PlayerData playerData, Track track)
        {
            this.playerData = playerData;
            this.track = track;
        }

        // Start is called before the first frame update
        void Start()
        {
            UpdateTrackTraits(track);
        }

        public void UpdateTrackTraits(Track track)
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
            SavePlayerData();

            EventsInvocation(track, startingSpeed, startingRotationSpeed);
        }

        private void SavePlayerData()
        {
            playerData.position = transform.position;
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

        private void EventsInvocation(Track track, float startingSpeed, float startingRotationSpeed)
        {
            if (startingRotationSpeed != currentRotationSpeed || startingSpeed != currentSpeed)
            {
                StateChanged.Invoke(track, Mathf.Abs(currentSpeed), currentRotationSpeed);
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
