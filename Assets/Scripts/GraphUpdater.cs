using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using Pathfinding;
using UnityEngine;

namespace Assets.Scripts
{
    public class GraphUpdater : MonoBehaviour
    {
        private NavGraph graph;
        private CancellationTokenSource cancelSource;

        private void Awake()
        {
            cancelSource = new CancellationTokenSource();
        }

        // Start is called before the first frame update
        async void Start()
        {
            // This holds all graph data
            AstarData data = AstarPath.active.data;

            // This creates a Grid Graph
            graph = data.graphs.Single();
        

            await UpdateGraph(cancelSource.Token);
        }

        private async UniTask UpdateGraph(CancellationToken token)
        {
            while (true)
            {
                await UniTask.Delay(2000);

                if (token.IsCancellationRequested)
                {
                    break;
                }

                graph.Scan();
            }
        }

        private void OnDestroy()
        {
            cancelSource.Cancel();
        }
    }
}
