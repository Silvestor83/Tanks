using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pathfinding;
using UnityEngine;

namespace Assets.Scripts.Providers
{
    public class PathfindingProvider
    {
        private OnPathDelegate callback;
        private Path path; 

        public void GeneratePath(Seeker seeker, Vector2 from, Vector2 to, OnPathDelegate callback)
        {
            this.callback = callback;
            seeker.StartPath(from, to, OnPathCompleted);
        }

        private void OnPathCompleted(Path p)
        {
            if (p.error)
            {
                Debug.Log("A path was calculated. Did it fail with an error? " + p.error);

                return;
            }

            // Path pooling. To avoid unnecessary allocations paths are reference counted.
            // Calling Claim will increase the reference count by 1 and Release will reduce
            // it by one, when it reaches zero the path will be pooled and then it may be used
            // by other scripts. The ABPath.Construct and Seeker.StartPath methods will
            // take a path from the pool if possible. See also the documentation page about path pooling.
            path?.Release(this);
            p.Claim(this);
            path = p;

            callback(path);
        }

        
    }
}
