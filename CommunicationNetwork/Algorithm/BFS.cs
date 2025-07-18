using CommunicationNetwork.Graph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunicationNetwork.Algorithm {
    public class BFS : BaseAlgorithm, IDataProvider {
        protected Dictionary<string, object> _outputDataLinks = new Dictionary<string, object>();

        // Input data Metadata keys
        // None


        // Output data Metadata keys
       public readonly string K_COLOR = "COLOR";
       public readonly string K_DISTANCE = "DISTANCE";
       public readonly string K_PARENT = "PARENT";
       public readonly string K_PATHS = "PATHS";

        // 3. Algorithm state
        private IGraph _graph;
        private Node _start = null;
        private Queue<Node> queue;

        public BFS(string name) : base(name) {
            // Initialize the output data links
            _outputDataLinks["COLOR"] = K_COLOR;
            _outputDataLinks["DISTANCE"] = K_DISTANCE;
            _outputDataLinks["PARENT"] = K_PARENT;
            _outputDataLinks["PATHS"] = K_PATHS;
        }

        public object GetDatakey(string key) {
            if (_outputDataLinks.TryGetValue(key, out var value)) {
                return value;
            }
            throw new KeyNotFoundException($"Key '{key}' not found in output data links.");
        }

        // 4. Accessor methods
        public string Color(Node node) {
            if (node.MetaData.TryGetValue(K_COLOR, out var color)) {
                return color as string;
            }
            throw new InvalidOperationException($"Color metadata not found for node {node.ID}.");
        }

        public int Distance(Node node) {
            if (node.MetaData.TryGetValue(K_DISTANCE, out var distance)) {
                return (int)distance;
            }
            throw new InvalidOperationException($"Distance metadata not found for node {node.ID}.");
        }

        public Node Parent(Node node) {
            if (node.MetaData.TryGetValue(K_PARENT, out var parent)) {
                return parent as Node;
            }
            throw new InvalidOperationException($"Parent metadata not found for node {node.ID}.");
        }

        // 5. Setter methods
        private void SetColor(Node node, string color) {
            node.MetaData[K_COLOR] = color;
        }

        private void SetDistance(Node node, int distance) {
            node.MetaData[K_DISTANCE] = distance;
        }

        private void SetParent(Node node, Node parent) {
            node.MetaData[K_PARENT] = parent;
        }

        // 6. Graph initialization setters
        public void SetGraph(IGraph graph) {
            _graph = graph;
        }

        public void SetSource(Node start) {
            _start = start;
        }

        public void AddPath(Node source, List<Node> path) {
            (_graph.MetaData[K_PATHS] as Dictionary<Node,List<Node>>)[source] = path;
        }

        // 7. Required overrides
        public override void Initialize() {
            // Initialize the queue
            if (queue == null) {
                queue = new Queue<Node>();
            } else {
                queue.Clear();
            }

            foreach (Node node in _graph.Nodes) {
                if (node != _start) {
                    // Initialize metadata for each node
                    SetColor(node, "WHITE");
                    SetDistance(node, -1);
                    SetParent(node, null);
                } else {
                    // Initialize the source node
                    SetColor(node, "GRAY");
                    SetDistance(node, 0);
                    SetParent(node, null);
                }
            }
            _graph.MetaData[K_PATHS] = new Dictionary<Node, List<Node>>();
            queue.Enqueue(_start);
        }

        public override void Execute() {
            Initialize();

            // Current node in the BFS traversal
            Node current = null;

            while (queue.Count != 0) {
                // Dequeue the next node
                current = queue.Dequeue();
                foreach (Node neighbor in _graph.GetNeighbors(current)) {
                    if (Color(neighbor) == "WHITE") {
                        SetColor(neighbor, "GRAY");
                        SetDistance(neighbor, Distance(current) + 1);
                        SetParent(neighbor, current);
                        queue.Enqueue(neighbor);
                    }

                    SetColor(current, "BLACK");
                }
            }

            // After BFS completes, we can store the paths from the source to each node
            AssemblePaths();
        }

        private void AssemblePaths() {
            foreach (Node node in _graph.Nodes) {
                if (node != _start) {
                    // Reconstruct the path from source to this node
                    List<Node> path = new List<Node>();
                    Node currentNode = node;
                    while (currentNode != null) {
                        path.Add(currentNode);
                        currentNode = Parent(currentNode);
                    }
                    path.Reverse(); // Reverse to get the path from source to node
                    AddPath(node, path);
                }
            }

        }
        // 8. Algorithm-specific methods


    }
}
