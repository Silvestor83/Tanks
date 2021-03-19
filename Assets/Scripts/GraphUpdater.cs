using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using ModestTree;
using Pathfinding;
using UnityEngine;

public class GraphUpdater : MonoBehaviour
{
    private NavGraph graph;

    // Start is called before the first frame update
    async void Start()
    {
        // This holds all graph data
        AstarData data = AstarPath.active.data;

        // This creates a Grid Graph
        graph = data.graphs.Single();
        

        await UpdateGraph();
    }

    private async UniTask UpdateGraph()
    {
        while (true)
        {
            await UniTask.Delay(2000);

            graph.Scan();
        }
    }
}
