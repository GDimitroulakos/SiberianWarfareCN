using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunicationNetwork.Graph;

namespace CommunicationNetwork.Algorithm {
    public class BellmanFord : BaseAlgorithm, IDataProvider,IDataConsumer {
        protected Dictionary<string, object> _outputDataLinks = new Dictionary<string, object>();
        // Input data Metadata keys
        public string K_WEIGHT;

        // Output data Metadata keys
        readonly string K_PARENT = "PARENT";
        readonly string K_PATHS = "PATHS";
        readonly string K_DISTANCE = "DISTANCE";

        public BellmanFord(string name):base(name) {
            // Initialize the output data links
            _outputDataLinks["DISTANCE"] = K_DISTANCE;
            _outputDataLinks["PARENT"] = K_PARENT;
            _outputDataLinks["PATHS"] = K_PATHS;
        }
        
        public double Distance(Node node) {
            if (node.MetaData.TryGetValue(K_DISTANCE, out var distance)) {
                return (double)distance;
            }
            throw new InvalidOperationException($"Distance metadata not found for node {node.ID}.");
        }
        public void SetDistance(Node node, double distance) {
            node.MetaData[K_DISTANCE] = distance;
        }

        public Node Parent(Node node) {
            if (node.MetaData.TryGetValue(K_PARENT, out var parent)) {
                return parent as Node;
            }
            throw new InvalidOperationException($"Parent metadata not found for node {node.ID}.");
        }
        public void SetParent(Node node, Node parent) {
            node.MetaData[K_PARENT] = parent;
        }

        public double Weight(Edge edge) {
            if (edge.MetaData.TryGetValue(K_WEIGHT, out var weight)) {
                return (double)weight;
            }
            throw new InvalidOperationException($"Weight metadata not found for edge {edge.ID}.");
        }

        // no setter for weight as it is given by the graph creator
        
        public void AddPath(Node source, List<Node> path) {
            (_graph.MetaData[K_PATHS] as Dictionary<Node, List<Node>>)[source] = path;
        }

        private IGraph _graph;
        private Node _start;

        public void SetGraph(IGraph graph) {
            _graph = graph ?? throw new ArgumentNullException(nameof(graph));
        }
        public void SetStart(Node start) {
            _start = start;
        }

        public object GetDatakey(string key) {
            if (_outputDataLinks.TryGetValue(key, out var value)) {
                return value;
            }
            throw new KeyNotFoundException($"Key '{key}' not found in output data links.");
        }

        public void RegisterInput(IGraph graph,string inputkey,
            IDataProvider provider, string PublicProviderkey) {
            ///TODO : Verify that the graph has data in the given key
            if (inputkey == "K_WEIGHT") {
                K_WEIGHT = provider.GetDatakey(PublicProviderkey) as string;
            }
        }

        public override void Initialize() {
            // Initialize graph metadata
            _graph.MetaData[K_PATHS] = new Dictionary<Node, List<Node>>();

            foreach (Node node in _graph.Nodes) {
                if (node != _start) {
                    // Initialize metadata for each node
                    SetDistance(node, double.MaxValue);
                    SetParent(node, null);
                } else {
                    // Initialize the source node
                    SetDistance(node, 0);
                    SetParent(node, null);
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
            var graphMetaData = _graph.MetaData[K_PATHS] as Dictionary<Node, List<Node>>;
            foreach (var node in _graph.Nodes) {
                if (!graphMetaData.ContainsKey(node)) {
                    graphMetaData[node] = new List<Node>();
                }
                Node current = node;
                while (current != null) {
                    graphMetaData[node].Add(current);
                    current = Parent(current);
                }
                graphMetaData[node].Reverse(); // Reverse to get the path from start to node
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
