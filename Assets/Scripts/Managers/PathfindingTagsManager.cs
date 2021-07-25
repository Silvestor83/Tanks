using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Pathfinding;

namespace Assets.Scripts.Managers
{
    public class PathfindingTagsManager
    {
        // max number of tags for pathfinding system + 1 not used
        private Dictionary<uint, bool> tankNumbers = new Dictionary<uint, bool>(31);

        public PathfindingTagsManager()
        {
            for (uint i = 1; i <= 31; i++)
            {
                tankNumbers.Add(i, false);
            }
        }

        /// <summary>
        /// This method gather 9 nodes under the object to use them as obstacles for other objects.
        /// For this aim we mark them with tags, with number of object.
        /// And clear old marked nodes to default tag state. 
        /// </summary>
        public void UpdateTags(Vector3 currentPosition, List<GraphNode> nodes, uint nodetag)
        {
            var nodeInfo = AstarPath.active.GetNearest(currentPosition);

            foreach (var node in nodes)
            {
                if (node.Tag == nodetag)
                {
                    node.Tag = 0;
                }
            }

            nodes.Clear();

            nodes.Add(nodeInfo.node);
            nodeInfo.node.GetConnections(node => nodes.Add(node));

            foreach (var node in nodes)
            {
                if (node.Walkable)
                {
                    node.Tag = nodetag;
                }
            }
        }

        public uint GetFreeNumber()
        {
            var key = tankNumbers.First(n => n.Value == false).Key;
            tankNumbers[key] = true;
            return key;
        }

        public void ReleaseTag(uint tag)
        {
            tankNumbers[tag] = false;
        }
    }
}
