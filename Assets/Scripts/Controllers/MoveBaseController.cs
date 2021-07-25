using Assets.Scripts.GameEntities.Units;
using UnityEngine;
using UnityEngine.Events;

namespace Assets.Scripts.Controllers
{
    public abstract class MoveBaseController : MonoBehaviour
    {
        public readonly UnityEvent<Track, float, float> StateChanged = new UnityEvent<Track, float, float>();

        protected Track track;
        protected float maxSpeed;
        protected float minSpeed;
        protected float forwardAcceleration;
        protected float rearAcceleration;
        protected float breakingAcceleration;
        protected float rotationSpeed;
        protected float currentSpeed;
        protected float currentRotationSpeed;

        public void UpdateTrackTraits(Track track)
        {
            this.track = track;
            maxSpeed = track.MaxSpeed;
            minSpeed = track.MinSpeed;
            forwardAcceleration = track.ForwardAcceleration;
            rearAcceleration = track.RearAcceleration;
            breakingAcceleration = track.BreakingAcceleration;
            rotationSpeed = track.rotateSpeed;
        }

        protected void EventsInvocation(Track track, float startingSpeed, float startingRotationSpeed)
        {
            if (startingRotationSpeed != currentRotationSpeed || startingSpeed != currentSpeed)
            {
                StateChanged.Invoke(track, Mathf.Abs(currentSpeed), currentRotationSpeed);
            }
        }

        protected abstract void Move();
        protected abstract void Rotate();
    }
}
