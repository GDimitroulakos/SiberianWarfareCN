using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunicationNetwork.Algorithms;
using CommunicationNetwork.Graph;

namespace CommunicationNetwork.Algorithm {
    public class TopologicalSort :BaseAlgorithm {
        private DFS _dfs;
        private IGraph _graph;
        private List<INode> _topologicalOrderedNodes;
        public IEnumerable<INode> TopologicalOrderedNodes => _topologicalOrderedNodes.AsReadOnly();

        public TopologicalSort(string name) {
            this.Name = name;
            MetadataKey = this;
        }

        public void SetGraph(IGraph graph) {
            _graph = graph;
        }

        public void SetDFS(DFS dfs) {
            _dfs = dfs;
        }

        private int TimeFinished(INode node) {
            return _dfs.TimeFinished(node);
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
