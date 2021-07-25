using Assets.Scripts.Core.GameData;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Assets.Scripts.Services
{
    public class PathfindingService
    {
        private LevelData levelData;
        private PlayerData playerData;

        public PathfindingService(LevelData levelData, PlayerData playerData)
        {
            this.levelData = levelData;
            this.playerData = playerData;
        }

        public Vector2 GetRandomFreePoint(Vector2 currentPosition)
        {
            bool walkable;
            Vector2 randomPoint;

            do
            {
                randomPoint = GetRandomPointInRadius(currentPosition);
                var nodeInfo = AstarPath.active.GetNearest(randomPoint);

                walkable = nodeInfo.node.Walkable;
            } while (!walkable);

            return randomPoint;
        }
        
        public Vector2 GetRandomFreePointNearPlayer()
        {
            bool walkable;
            Vector2 randomPoint;

            do
            {
                randomPoint = GetRandomPointNearPlayer();
                var nodeInfo = AstarPath.active.GetNearest(randomPoint);

                walkable = nodeInfo.node.Walkable;
            } while (!walkable);

            return randomPoint;
        }

        /// <summary>
        /// Method to receive random point in level area in given radius
        /// </summary>
        private Vector2 GetRandomPointInRadius(Vector2 currentPosition)
        {
            var randomPoint = GetGameFieldRandomPoint();

            var distance = Vector2.Distance(currentPosition, randomPoint);

            if (distance > levelData.PathMaxDistance)
            {
                var direction = randomPoint - currentPosition;
                randomPoint = (direction * (levelData.PathMaxDistance / distance)) + currentPosition;
            }

            return randomPoint;
        }

        private Vector2 GetRandomPointNearPlayer()
        {
            var randomPoint = GetGameFieldRandomPoint();

            var distance = Vector2.Distance(playerData.position, randomPoint);

            if (distance > levelData.PlayerTrackingDistance)
            {
                var direction = randomPoint - playerData.position;
                randomPoint = (direction * (levelData.PlayerTrackingDistance / distance)) + playerData.position;
            }

            return randomPoint;
        }

        /// <summary>
        /// Method to receive random point in level area
        /// </summary>
        private Vector2 GetGameFieldRandomPoint()
        {
            var randomXPoint = Random.Range(0, levelData.GameFieldWidth) + levelData.OriginOffset.x;
            var randomYPoint = Random.Range(0, levelData.GameFieldHeight) + levelData.OriginOffset.y;

            return new Vector2(randomXPoint, randomYPoint);
        }
    }
}
