using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
