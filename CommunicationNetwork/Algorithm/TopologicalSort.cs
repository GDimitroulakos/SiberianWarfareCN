using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunicationNetwork.Algorithms;
using CommunicationNetwork.Graph;

namespace CommunicationNetwork.Algorithm {
    public class TopologicalSort :BaseAlgorithm {
        private IGraph _graph;
        private DFS _dfs;
        private List<Node> _topologicalOrderedNodes;
        public IEnumerable<Node> TopologicalOrderedNodes => _topologicalOrderedNodes.AsReadOnly();

        public TopologicalSort(string name) :base(name){
        }

        public void SetGraph(IGraph graph) {
            _graph = graph;
        }

        public void SetDFS(string key) {
            if ( AlgorithmScope.Instance.GetAlgorithm(key) == null) {
                throw new ArgumentException($"No DFS algorithm found with key '{key}'.");
            } else if (AlgorithmScope.Instance.GetAlgorithm(key) is not DFS) {
                throw new ArgumentException($"Algorithm with key '{key}' is not of type DFS.");
            }
            _dfs = (DFS)AlgorithmScope.Instance.GetAlgorithm(key);
            AddInput("DFS", _dfs);
        }
        

        private int TimeFinished(Node node) {
            
        }

        public override void Execute() {
            // 0.Initialize the algorithm
            Initialize();

            // 1. Call DFS to perform a depth-first search on the graph
            _dfs.Execute();

            // 2. Create the topological sorted list of nodes in the order they are visited
            _topologicalOrderedNodes = _graph.Nodes.
                OrderByDescending(node => TimeFinished(node)) // Sort by finish time in descending order
                .ToList();
        }

        public override void Initialize() {
            
        }
    }
}
