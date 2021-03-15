using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.Core.GameData;
using Assets.Scripts.GameEntities.Units;
using Assets.Scripts.Infrastructure.Enums;
using Assets.Scripts.Services;
using Pathfinding;
using UnityEngine;
using UnityEngine.Events;
using Zenject;
using ZLogger;
using Random = UnityEngine.Random;

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

        private Seeker seeker;
        private Path path;
        private float nextWaypointDistance = 3;
        private int currentWaypoint;
        private float repathRate = 0.5f;
        private float lastRepath = float.NegativeInfinity;

        public bool reachedEndOfPath;


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

            seeker = GetComponent<Seeker>();
            var p = seeker.StartPath(transform.position, playerData.position, OnPathCompleted);
            
            Debug.Log("Yay, we got a path back. Did it have an error? " + p.error);
        }

        public void OnPathCompleted(Path p)
        {
            Debug.Log("A path was calculated. Did it fail with an error? " + p.error);

            // Path pooling. To avoid unnecessary allocations paths are reference counted.
            // Calling Claim will increase the reference count by 1 and Release will reduce
            // it by one, when it reaches zero the path will be pooled and then it may be used
            // by other scripts. The ABPath.Construct and Seeker.StartPath methods will
            // take a path from the pool if possible. See also the documentation page about path pooling.
            p.Claim(this);
            if (!p.error)
            {
                if (path != null) path.Release(this);
                path = p;
                // Reset the waypoint counter so that we start to move towards the first point in the path
                currentWaypoint = 0;
            }
            else
            {
                p.Release(this);
            }
        }

        void FixedUpdate()
        {
            var startingSpeed = currentSpeed;
            var startingRotationSpeed = currentRotationSpeed;

            Rotate();
            Move();

            EventsInvocation(startingSpeed, startingRotationSpeed);
        }

        public void Update()
        {
            if (Time.time > lastRepath + repathRate && seeker.IsDone())
            {
                lastRepath = Time.time;

                // Start a new path to the targetPosition, call the the OnPathComplete function
                // when the path has been calculated (which may take a few frames depending on the complexity)
                seeker.StartPath(transform.position, playerData.position, OnPathCompleted);
            }

            if (path == null)
            {
                // We have no path to follow yet, so don't do anything
                return;
            }

            // Check in a loop if we are close enough to the current waypoint to switch to the next one.
            // We do this in a loop because many waypoints might be close to each other and we may reach
            // several of them in the same frame.
            reachedEndOfPath = false;
            // The distance to the next waypoint in the path
            float distanceToWaypoint;
            while (true)
            {
                // If you want maximum performance you can check the squared distance instead to get rid of a
                // square root calculation. But that is outside the scope of this tutorial.
                distanceToWaypoint = Vector3.Distance(transform.position, path.vectorPath[currentWaypoint]);
                if (distanceToWaypoint < nextWaypointDistance)
                {
                    // Check if there is another waypoint or if we have reached the end of the path
                    if (currentWaypoint + 1 < path.vectorPath.Count)
                    {
                        currentWaypoint++;
                    }
                    else
                    {
                        // Set a status variable to indicate that the agent has reached the end of the path.
                        // You can use this to trigger some special code if your game requires that.
                        reachedEndOfPath = true;
                        break;
                    }
                }
                else
                {
                    break;
                }
            }

            // Slow down smoothly upon approaching the end of the path
            // This value will smoothly go from 1 to 0 as the agent approaches the last waypoint in the path.
            var speedFactor = reachedEndOfPath ? Mathf.Sqrt(distanceToWaypoint / nextWaypointDistance) : 1f;

            // Direction to the next waypoint
            // Normalize it so that it has a length of 1 world unit
            Vector3 dir = (path.vectorPath[currentWaypoint] - transform.position).normalized;
            // Multiply the direction by our desired speed to get a velocity
            Vector3 velocity = dir * (track.MaxSpeed * speedFactor);

            // Move the agent using the CharacterController component
            // Note that SimpleMove takes a velocity in meters/second, so we should not multiply by Time.deltaTime
            //controller.SimpleMove(velocity);

            // If you are writing a 2D game you may want to remove the CharacterController and instead modify the position directly
            // transform.position += velocity * Time.deltaTime;
            transform.Translate(velocity * Time.deltaTime, Space.World);
            transform.Rotate(Vector3.forward, rotateSpeed * Time.unscaledDeltaTime);
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
