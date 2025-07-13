using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunicationNetwork.Graph;

namespace CommunicationNetwork.Algorithm {
    public class BellmanFord : BaseAlgorithm {
        public BellmanFord(string name):base() {
        }

        public class BellmanFord_NodeMetadata {
            public double Distance;
            public Node Parent;
            public BellmanFord_NodeMetadata(double distance, Node parent) {
                Distance = distance;
                Parent = parent;
            }
            public override string ToString() {
                string parentName = Parent?.ID ?? "null";
                return $"Bellman-Ford Results\nDistance={Distance}\nParent={parentName}";
            }
        }
        

        public double Distance(Node node) {
            return ((BellmanFord_NodeMetadata)node.MetaData[MetadataKey]).Distance;
        }
        public void SetDistance(Node node, double distance) {
            var metaData = (BellmanFord_NodeMetadata)node.MetaData[MetadataKey];
            metaData.Distance = distance;
        }
        public Node Parent(Node node) {
            return ((BellmanFord_NodeMetadata)node.MetaData[MetadataKey]).Parent;
        }
        public  void SetParent(Node node, Node parent) {
            var metaData = (BellmanFord_NodeMetadata)node.MetaData[MetadataKey];
            metaData.Parent = parent;
        }

        public class BellmanFord_EdgeMetadata {
            public double _weight;
            public BellmanFord_EdgeMetadata(double weight) {
                _weight = weight;
            }

            public override string ToString() {
                return $"Weight: {_weight}";
            }
        }

        public double Weight(Edge edge) {
            return ((BellmanFord_EdgeMetadata)edge.MetaData[MetadataKey])._weight;
        }

        public void SetWeight(Edge edge, double weight) {
            if (!edge.MetaData.ContainsKey(MetadataKey)) {
                edge.MetaData[MetadataKey] = new BellmanFord_EdgeMetadata(weight);
            } else {
                ((BellmanFord_EdgeMetadata)edge.MetaData[MetadataKey])._weight = weight;
            }
        }

        public class BellmanFord_GraphMetadata {
            public Dictionary<Node, List<Node>> _paths;
            public BellmanFord_GraphMetadata() {
                _paths = new Dictionary<Node, List<Node>>();
            }
            public override string ToString() {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("Bellman-Ford Graph MetaData:");
                foreach (var kvp in _paths) {
                    sb.AppendLine($"{kvp.Key.ID}: {string.Join(" -> ", kvp.Value.Select(n => n.ID))}");
                }
                return sb.ToString();
            }
        }

        private IGraph _graph;
        private Node _start;

        public void SetGraph(IGraph graph) {
            _graph = graph ?? throw new ArgumentNullException(nameof(graph));
        }
        public void SetStart(Node start) {
            _start = start;
        }

        public override void Initialize() {
            // Initialize graph metadata
            if (!_graph.MetaData.ContainsKey(MetadataKey)) {
                _graph.MetaData[MetadataKey] = new BellmanFord_GraphMetadata();

            }

            // Initialize node metadata
            foreach (var node in _graph.Nodes) {
                if (!node.MetaData.ContainsKey(MetadataKey)) {
                    if (node == _start) {
                        node.MetaData[MetadataKey] = new BellmanFord_NodeMetadata(0, null);
                    } else {
                        node.MetaData[MetadataKey] = new BellmanFord_NodeMetadata(double.MaxValue, null);
                    }
                }
            }
        }

        public override void Execute() {
            Initialize();

            for (int i = 0; i < _graph.Nodes.Count - 1; i++) {
                foreach (var edge in _graph.Edges) {
                    Relax(edge.Source, edge.Target);
                }
            }

            // Check for negative weight cycles
            foreach (var edge in _graph.Edges) {
                if (Distance(edge.Source) + Weight(edge) < Distance(edge.Target)) {
                    throw new InvalidOperationException("Graph contains a negative weight cycle.");
                }
            }

            // Optionally, you can store the paths in the graph metadata
            AssemblePathData();
        }

        private void AssemblePathData() {
            var graphMetaData = (BellmanFord_GraphMetadata)_graph.MetaData[MetadataKey];
            foreach (var node in _graph.Nodes) {
                if (!graphMetaData._paths.ContainsKey(node)) {
                    graphMetaData._paths[node] = new List<Node>();
                }
                Node current = node;
                while (current != null) {
                    graphMetaData._paths[node].Add(current);
                    current = Parent(current);
                }
                graphMetaData._paths[node].Reverse(); // Reverse to get the path from start to node
            }
        }




        public void Relax(Node u, Node v) {
            Edge edge = _graph.GetEdge(u, v);
            if (edge == null) {
                throw new InvalidOperationException($"Edge from {u.ID} to {v.ID} does not exist.");
            }
            if (Distance(u) + Weight(edge) < Distance(v)) {
                SetDistance(v, Distance(u) + Weight(edge));
                SetParent(v, u);
            }
        }
    }
}
