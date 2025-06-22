using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunicationNetwork.Graph;

namespace CommunicationNetwork.Algorithm {
    public class BellmanFord : BaseAlgorithm {


        public class BellmanFord_NodeMetadata {
            public double Distance;
            public INode Parent;
            public BellmanFord_NodeMetadata(double distance, INode parent) {
                Distance = distance;
                Parent = parent;
            }
            public override string ToString() {
                string parentName = Parent?.Name ?? "null";
                return $"Bellman-Ford Results\nDistance={Distance}\nParent={parentName}";
            }
        }

        public static double Distance(INode node) {
            return ((BellmanFord_NodeMetadata)node.MetaData[MetadataKey]).Distance;
        }
        public static void SetDistance(INode node, double distance) {
            var metaData = (BellmanFord_NodeMetadata)node.MetaData[MetadataKey];
            metaData.Distance = distance;
        }
        public static INode Parent(INode node) {
            return ((BellmanFord_NodeMetadata)node.MetaData[MetadataKey]).Parent;
        }
        public static void SetParent(INode node, INode parent) {
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

        public static double Weight(IEdge edge) {
            return ((BellmanFord_EdgeMetadata)edge.MetaData[MetadataKey])._weight;
        }

        public static void SetWeight(IEdge edge, double weight) {
            if (!edge.MetaData.ContainsKey(MetadataKey)) {
                edge.MetaData[MetadataKey] = new BellmanFord_EdgeMetadata(weight);
            } else {
                ((BellmanFord_EdgeMetadata)edge.MetaData[MetadataKey])._weight = weight;
            }
        }

        public class BellmanFord_GraphMetadata {
            public Dictionary<INode, List<INode>> _paths;
            public BellmanFord_GraphMetadata() {
                _paths = new Dictionary<INode, List<INode>>();
            }
            public override string ToString() {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("Bellman-Ford Graph MetaData:");
                foreach (var kvp in _paths) {
                    sb.AppendLine($"{kvp.Key.Name}: {string.Join(" -> ", kvp.Value.Select(n => n.Name))}");
                }
                return sb.ToString();
            }
        }

        public static string MetadataKey => "BellmanFord";
        private IGraph _graph;
        private INode _start;

        public void SetGraph(IGraph graph) {
            _graph = graph ?? throw new ArgumentNullException(nameof(graph));
        }
        public void SetStart(INode start) {
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
                    graphMetaData._paths[node] = new List<INode>();
                }
                INode current = node;
                while (current != null) {
                    graphMetaData._paths[node].Add(current);
                    current = Parent(current);
                }
                graphMetaData._paths[node].Reverse(); // Reverse to get the path from start to node
            }
        }




        public void Relax(INode u, INode v) {
            IEdge edge = _graph.GetEdge(u, v);
            if (edge == null) {
                throw new InvalidOperationException($"Edge from {u.Name} to {v.Name} does not exist.");
            }
            if (Distance(u) + Weight(edge) < Distance(v)) {
                SetDistance(v, Distance(u) + Weight(edge));
                SetParent(v, u);
            }
        }
    }
}
