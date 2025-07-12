using CommunicationNetwork.Graph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunicationNetwork.Algorithm {
    public class BFS :BaseAlgorithm{

        // 2. Metadata structure for BFS results
        public class BFS_NodeMetaData {
            public string Color;      // WHITE, GRAY, BLACK
            public int Distance;      // Distance from source
            public INode Parent;      // Parent in BFS tree

            public override string ToString() {
                string parentName = Parent?.Name ?? "null";
                return $"BFS Results\nColor={Color}\nDistance={Distance}\nParent={parentName}";
            }
        }

        public class BFS_GraphMetaData {
            // Dictionary to store paths from source to each node. The key is the targer node,
            // and the value is the list of nodes in the path.
            public Dictionary<INode, List<INode>> Paths;
            public INode Source; // The source node from which BFS was initiated

            public override string ToString() {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("BFS Graph MetaData:");
                sb.AppendLine($"Source: {Source?.Name}");
                sb.AppendLine("Paths:");
                foreach (var kvp in Paths) {
                    sb.AppendLine($"{kvp.Key.Name}: {string.Join(" -> ", kvp.Value.Select(n => n.Name))}");
                }
                return sb.ToString();
            }
        }

        // 3. Algorithm state
        private IGraph _graph;
        private INode _start=null;
        private Queue<INode> queue;

        public BFS(string name) {
            this.Name = name;
            MetadataKey = name;
        }

        // 4. Accessor methods
        public string Color(INode node) {
            return ((BFS_NodeMetaData)node.MetaData[MetadataKey]).Color;
        }

        public int Distance(INode node) {
            return ((BFS_NodeMetaData)node.MetaData[MetadataKey]).Distance;
        }

        public INode Parent(INode node) {
            return ((BFS_NodeMetaData)node.MetaData[MetadataKey]).Parent;
        }

        // 5. Setter methods
        private void SetColor(INode node, string color) {
            var metaData = (BFS_NodeMetaData)node.MetaData[MetadataKey];
            metaData.Color = color;
        }

        private void SetDistance(INode node, int distance) {
            var metaData = (BFS_NodeMetaData)node.MetaData[MetadataKey];
            metaData.Distance = distance;
        }

        private void SetParent(INode node, INode parent) {
            var metaData = (BFS_NodeMetaData)node.MetaData[MetadataKey];
            metaData.Parent = parent;
        }

        // 6. Graph initialization setters
        public void SetGraph(IGraph graph) {
            _graph = graph;
        }

        public void SetSource(INode start) {
            _start = start;
        }

        // 7. Required overrides
        public override void Initialize() {
            // Check if the graph and source node are set
            if (_graph == null) {
                throw new InvalidOperationException("Graph is not set. Use SetGraph method to set the graph.");
            }

            if (_start == null) {
                throw new InvalidOperationException("Source node is not set. Use SetSource method to set the source node.");
            }

            // Initialize the queue
            if (queue == null) {
                queue = new Queue<INode>();
            } else {
                queue.Clear();
            }

            // Initialize metadata for the graph and nodes
            _graph.MetaData[MetadataKey] = new BFS_GraphMetaData() {
                Paths = new Dictionary<INode, List<INode>>(),
                Source = _start
            };

            foreach (INode node in _graph.Nodes) {
                if (node != _start) {
                    node.MetaData[MetadataKey] = new BFS_NodeMetaData() {
                        Color = "WHITE",
                        Distance = int.MaxValue, // Infinity
                        Parent = null
                    };
                } else {
                    node.MetaData[MetadataKey] = new BFS_NodeMetaData() {
                        Color = "GRAY", // Start node is initially gray
                        Distance = 0,   // Distance from itself is 0
                        Parent = null   // No parent for the start node
                    };
                    queue.Enqueue(_start);
                }
            }
        }

        public override void Execute() {
            Initialize();

            // Current node in the BFS traversal
            INode current = null;

            while (queue.Count != 0) {
                // Dequeue the next node
                current = queue.Dequeue();
                foreach (INode neighbor in _graph.GetNeighbors(current)) {
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
            BFS_GraphMetaData graphMetaData = (BFS_GraphMetaData)_graph.MetaData[MetadataKey];
            if (graphMetaData != null) {
                foreach (INode node in _graph.Nodes) {
                    if (node != _start) {
                        // Reconstruct the path from source to this node
                        List<INode> path = new List<INode>();
                        INode currentNode = node;
                        while (currentNode != null) {
                            path.Add(currentNode);
                            currentNode = Parent(currentNode);
                        }
                        path.Reverse(); // Reverse to get the path from source to node
                        graphMetaData.Paths[node] = path;
                    }
                }
            }
        }
        // 8. Algorithm-specific methods


    }
}
