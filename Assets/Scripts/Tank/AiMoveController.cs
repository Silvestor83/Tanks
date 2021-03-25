using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Assets.Scripts.Core.GameData;
using Assets.Scripts.GameEntities.Units;
using Assets.Scripts.Infrastructure.Enums;
using Assets.Scripts.Managers;
using Assets.Scripts.Providers;
using Assets.Scripts.Services;
using Pathfinding;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
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
        private float rotationSpeed;
        private float distanceToFullBreaking;

        private float currentSpeed;
        private float currentRotationSpeed;

        private Track track;
        private LogService logService;
        private PlayerData playerData;
        private PathfindingTagsManager tagsManager;
        private PathfindingProvider pathProvider;
        private PathfindingService pathfindingService;

        private Seeker seeker;
        private Path path;
        // in seconds
        public float pathRecalculatingRate = 0.5f;
        public float endPointRecalculatingRate = 10f;
        private float lastRepath = float.NegativeInfinity;
        private float lastEndPointTime = float.NegativeInfinity;
        private Vector2 endPoint;
        private bool endOfPathReached = true;
        // Must be more or equal to nextWaypointMinDistance
        public float endWaypointMinDistance = 0.5f;
        public float nextWaypointMinDistance = 0.1f;
        public float distanceToPlayerToRecalculateEndPoint = 30f;
        private Vector3 currentPosition;
        private int currentWaypoint;
        private float currentDistanceToWaypoint;
        private Vector3 directionFromTankToWaypoint;
        private float signedAngleFromTankToWaypoint;
        
        private List<GraphNode> nodes = new List<GraphNode>();
        public uint nodetag;


        public readonly UnityEvent<float, float> StateChanged = new UnityEvent<float, float>();

        [Inject]
        public void Init(PlayerData playerData, LogService logService, PathfindingTagsManager tagsManager, PathfindingProvider pathProvider, PathfindingService pathfindingService, Track track, uint tag)
        {
            this.playerData = playerData;
            this.logService = logService;
            this.tagsManager = tagsManager;
            this.pathProvider = pathProvider;
            this.pathfindingService = pathfindingService;
            this.track = track;
            this.nodetag = tag;
        }

        void Start()
        {
            maxSpeed = track.MaxSpeed;
            minSpeed = track.MinSpeed;
            forwardAcceleration = track.ForwardAcceleration;
            rearAcceleration = track.RearAcceleration;
            breakingAcceleration = track.BreakingAcceleration;
            rotationSpeed = track.rotateSpeed;
            distanceToFullBreaking = maxSpeed * maxSpeed / (2 * breakingAcceleration);

            seeker = GetComponent<Seeker>();
            seeker.traversableTags = (1 << 0) | (1 << (int)nodetag);
        }

        private void OnPathCompleted(Path p)
        {
            path = p;
            endOfPathReached = false;
            // Reset the waypoint counter so that we start to move towards the first point in the path
            currentWaypoint = 0;
        }

        public void Update()
        {
            currentPosition = transform.position;

            UpdatePath();

            if (path == null || endOfPathReached)
            {
                // We have no path to follow yet, so don't do anything
                return;
            }

            var distanceToEndPoint = Vector3.Distance(currentPosition, path.vectorPath[path.vectorPath.Count - 1]);

            if (distanceToEndPoint < endWaypointMinDistance)
            {
                endOfPathReached = true;

                return;
            }

            // If you want maximum performance you can check the squared distance instead to get rid of a
            // square root calculation. But that is outside the scope of this tutorial.
            currentDistanceToWaypoint = Vector3.Distance(currentPosition, path.vectorPath[currentWaypoint]);

            while (currentWaypoint + 1 < path.vectorPath.Count)
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

            // Direction to the next waypoint
            // Normalize it so that it has a length of 1 world unit
            directionFromTankToWaypoint = (path.vectorPath[currentWaypoint] - currentPosition).normalized;
            signedAngleFromTankToWaypoint = Vector3.SignedAngle(transform.up, directionFromTankToWaypoint, Vector3.forward);
        }

        public void UpdatePath()
        {
            var distanceToPlayer = Vector2.Distance(currentPosition, playerData.position);

            if (distanceToPlayer < distanceToPlayerToRecalculateEndPoint)
            {
                // change target to random position near player
                pathfindingService.UpdateRandomFreePointNearPlayer(ref endPoint, endPointRecalculatingRate, ref lastEndPointTime, endOfPathReached);
            }
            else
            {
                // Update end point of path
                pathfindingService.UpdateRandomFreePoint(ref endPoint, endPointRecalculatingRate, currentPosition,
                    ref lastEndPointTime, endOfPathReached);
            }

            // Update path of object and obstacles for objects in scene
            if (Time.time > lastRepath + pathRecalculatingRate && seeker.IsDone())
            {
                lastRepath = Time.time;

                UpdateTags();
                // Start a new path to the targetPosition, call the the OnPathComplete function
                // when the path has been calculated (which may take a few frames depending on the complexity)

                pathProvider.GeneratePath(seeker, currentPosition, endPoint, OnPathCompleted);
            }
        }

        void FixedUpdate()
        {
            var startingSpeed = currentSpeed;
            var startingRotationSpeed = currentRotationSpeed;

            // Enemy can only move or rotate, not both simultaneously
            if (!endOfPathReached)
            {
                if (Mathf.Abs(signedAngleFromTankToWaypoint) > 0.1f)
                {
                    if (currentSpeed > 0)
                    {
                        // We need to be stopped before we can rotate
                        NaturalBreaking();
                    }
                    else
                    {
                        Rotate(signedAngleFromTankToWaypoint);
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
                NaturalBreaking();
            }

            // If we change speed or rotation in loop update we need to invoke events (e.g. Animations depends on it)
            if (startingRotationSpeed != currentRotationSpeed || startingSpeed != currentSpeed)
            {
                StateChanged.Invoke(Mathf.Abs(currentSpeed), currentRotationSpeed);
            }
        }

        private void Rotate(float angle)
        {
            currentRotationSpeed = rotationSpeed;

            var maxPossibleAngle = rotationSpeed * Time.fixedDeltaTime * Mathf.Sign(angle);

            if (Mathf.Abs(angle) > Mathf.Abs(maxPossibleAngle))
            {
                transform.Rotate(Vector3.forward, maxPossibleAngle);
            }
            else
            {
                transform.Rotate(Vector3.forward, angle);
                currentRotationSpeed = 0;
            }
        }

        private void Move()
        {
            var maxSpeedBetweenTwoPoints = Mathf.Sqrt(2 * distanceToFullBreaking * forwardAcceleration *
                breakingAcceleration / (forwardAcceleration + breakingAcceleration));
            var maxBreakingDistance = maxSpeed * maxSpeed / (2 * breakingAcceleration);
            var currentBreakingDistance = currentSpeed * currentSpeed / (2 * breakingAcceleration);

            //distanceToFullBreaking = currentSpeed * currentSpeed / (2 * breakingAcceleration);

            if (currentDistanceToWaypoint <= currentBreakingDistance)
            {
                Breaking();
            }
            else
            {
                if (currentSpeed < maxSpeed)
                {
                    Acceleration();
                }
            }

            transform.Translate(currentSpeed * Time.fixedDeltaTime * transform.up, Space.World);
        }

        private void NaturalBreaking()
        {
            Breaking();

            transform.Translate(currentSpeed * Time.fixedDeltaTime * transform.up, Space.World);
        }

        private void Acceleration()
        {
            var tempSpeed = forwardAcceleration * Time.fixedUnscaledDeltaTime + currentSpeed;
            currentSpeed = tempSpeed > maxSpeed ? maxSpeed : tempSpeed;
        }

        private void Breaking()
        {
            var tempSpeed = -breakingAcceleration * Time.fixedDeltaTime + currentSpeed;
            currentSpeed = tempSpeed > 0 ? tempSpeed : 0;
        }

        /// <summary>
        /// This method gather 9 nodes under the object to use them as obstacles for other objects.
        /// For this aim we mark them with tags, with number of object.
        /// And clear old marked nodes to default tag state. 
        /// </summary>
        private void UpdateTags()
        {
            var nodeInfo = AstarPath.active.GetNearest(transform.position);

            foreach (var node in nodes)
            {
                if (node.Tag == nodetag)
                {
                    node.Tag = 0;
                }
            }

            nodes.Clear();
            
            nodes.Add(nodeInfo.node);
            nodeInfo.node.GetConnections(AddNodes);

            foreach (var node in nodes)
            {
                if (node.Walkable)
                {
                    node.Tag = nodetag;
                }
            }
        }

        private void AddNodes(GraphNode node)
        {
            nodes.Add(node);
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
