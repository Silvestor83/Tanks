using System.Collections.Generic;
using Assets.Scripts.Core.GameData;
using Assets.Scripts.GameEntities.Units;
using Assets.Scripts.Managers;
using Assets.Scripts.Providers;
using Assets.Scripts.Services;
using Pathfinding;
using UnityEngine;
using UnityEngine.Events;
using Zenject;

namespace Assets.Scripts.Controllers
{
    public class AiMoveController : MoveBaseController
    {
        private PlayerData playerData;
        private PathfindingTagsManager tagsManager;
        private PathfindingProvider pathProvider;
        private PathfindingService pathfindingService;
        private Seeker seeker;
        private Path path;

        // in seconds
        [SerializeField]
        private float pathRecalculatingRate = 0.5f;
        [SerializeField]
        private float endPointRecalculatingRate = 10f;
        // Must be more or equal to nextWaypointMinDistance
        [SerializeField]
        private float endWaypointMinDistance = 0.5f;
        [SerializeField]
        private float nextWaypointMinDistance = 0.1f;
        [SerializeField]
        private float distanceToPlayerToRecalculateEndPoint = 30f;
        private float lastRepath = float.NegativeInfinity;
        private float lastEndPointTime = float.NegativeInfinity;
        private Vector2 endPoint;
        private bool endOfPathReached = true;
        private Vector3 currentPosition;
        private int currentWaypoint;
        private float currentDistanceToWaypoint;
        private Vector3 directionFromTankToWaypoint;
        private float signedAngleFromTankToWaypoint;

        private List<GraphNode> nodes = new List<GraphNode>();
        [SerializeField]
        private uint nodetag;

        [Inject]
        public void Init(PlayerData playerData, PathfindingTagsManager tagsManager, PathfindingProvider pathProvider, PathfindingService pathfindingService, Track track, uint tag)
        {
            this.playerData = playerData;
            this.tagsManager = tagsManager;
            this.pathProvider = pathProvider;
            this.pathfindingService = pathfindingService;
            nodetag = tag;
            UpdateTrackTraits(track);
        }

        void Start()
        {
            seeker = GetComponent<Seeker>();
            seeker.traversableTags = (1 << 0) | (1 << (int)nodetag);
        }

        private void OnPathCompleted(Path p)
        {
            path = p;
            endOfPathReached = false;
            currentWaypoint = 0;
        }

        public void Update()
        {
            currentPosition = transform.position;
            UpdateEndPointOfPathIfNeeded();
            UpdatePathIfNeeded();

            if (path == null || endOfPathReached)
            {
                return;
            }

            var distanceToEndPoint = Vector3.Distance(currentPosition, path.vectorPath[path.vectorPath.Count - 1]);

            if (distanceToEndPoint < endWaypointMinDistance)
            {
                endOfPathReached = true;

                return;
            }

            currentDistanceToWaypoint = Vector3.Distance(currentPosition, path.vectorPath[currentWaypoint]);

            while (currentWaypoint < path.vectorPath.Count - 1)
            {
                if (currentDistanceToWaypoint < nextWaypointMinDistance)
                {
                    currentWaypoint++;
                    currentDistanceToWaypoint = Vector3.Distance(currentPosition, path.vectorPath[currentWaypoint]);
                }
                else
                {
                    break;
                }
            }

            directionFromTankToWaypoint = (path.vectorPath[currentWaypoint] - currentPosition).normalized;
            signedAngleFromTankToWaypoint = Vector3.SignedAngle(transform.up, directionFromTankToWaypoint, Vector3.forward);
        }

        private void UpdateEndPointOfPathIfNeeded()
        {
            var distanceToPlayer = Vector2.Distance(currentPosition, playerData.position);

            if (Time.time > lastEndPointTime + endPointRecalculatingRate || endOfPathReached)
            {
                lastEndPointTime = Time.time;

                // Enemy moves to a random point on the field. If hi is closer then distanceToPlayerToRecalculateEndPoint to the player he changes his end point to close to the player
                if (distanceToPlayer < distanceToPlayerToRecalculateEndPoint)
                {
                    endPoint = pathfindingService.GetRandomFreePointNearPlayer();
                }
                else
                {
                    endPoint = pathfindingService.GetRandomFreePoint(currentPosition);
                }
            }
        }

        private void UpdatePathIfNeeded()
        {
            if (Time.time > lastRepath + pathRecalculatingRate && seeker.IsDone())
            {
                lastRepath = Time.time;
                tagsManager.UpdateTags(currentPosition, nodes, nodetag);

                // Start a new path to the targetPosition, call the OnPathComplete function
                // when the path has been calculated (which may take a few frames depending on the complexity)
                pathProvider.GeneratePath(seeker, currentPosition, endPoint, OnPathCompleted);
            }
        }

        private void FixedUpdate()
        {
            var startingSpeed = currentSpeed;
            var startingRotationSpeed = currentRotationSpeed;

            Act();
            EventsInvocation(track, startingSpeed, startingRotationSpeed);
        }

        private void Act()
        {
            // Enemy can only move or rotate, not both simultaneously
            if (!endOfPathReached)
            {
                if (Mathf.Abs(signedAngleFromTankToWaypoint) > 0.1f)
                {
                    if (currentSpeed > 0)
                    {
                        // Enemy tank need to be stopped before it can rotate
                        Breaking();
                    }
                    else
                    {
                        Rotate();
                    }
                }
                else
                {
                    Move();
                }
            }
            else if (currentSpeed > 0)
            {
                // We need to be stopped at the end of the path
                Breaking();
            }
        }

        protected override void Rotate()
        {
            currentRotationSpeed = rotationSpeed;

            var maxPossibleAngle = rotationSpeed * Time.fixedDeltaTime * Mathf.Sign(signedAngleFromTankToWaypoint);

            if (Mathf.Abs(signedAngleFromTankToWaypoint) > Mathf.Abs(maxPossibleAngle))
            {
                transform.Rotate(Vector3.forward, maxPossibleAngle);
            }
            else
            {
                transform.Rotate(Vector3.forward, signedAngleFromTankToWaypoint);
                currentRotationSpeed = 0;
            }
        }

        protected override void Move()
        {
            var currentBreakingDistance = currentSpeed * currentSpeed / (2 * breakingAcceleration);

            if (currentDistanceToWaypoint <= currentBreakingDistance)
            {
                Breaking();
            }
            else if (currentSpeed < maxSpeed)
            {
                Acceleration();
            }
            else
            {
                ChangePosition();
            }
        }

        private void Acceleration()
        {
            var tempSpeed = forwardAcceleration * Time.fixedUnscaledDeltaTime + currentSpeed;
            currentSpeed = tempSpeed > maxSpeed ? maxSpeed : tempSpeed;

            ChangePosition();
        }

        private void Breaking()
        {
            var tempSpeed = -breakingAcceleration * Time.fixedDeltaTime + currentSpeed;
            currentSpeed = tempSpeed > 0 ? tempSpeed : 0;

            ChangePosition();
        }

        private void ChangePosition()
        {
            transform.Translate(currentSpeed * Time.fixedDeltaTime * transform.up, Space.World);
        }

        private void OnDestroy()
        {
            tagsManager.ReleaseTag(nodetag);
        }

        private void OnDrawGizmos()
        {
            foreach (var graphNode in nodes)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawSphere((Vector3)graphNode.position, 0.25f);
            }
        }
    }
}
