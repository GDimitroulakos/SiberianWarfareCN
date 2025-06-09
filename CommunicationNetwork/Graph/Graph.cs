using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.VisualBasic;
using System.Threading;
using CommunicationNetwork.Algorithms;

namespace CommunicationNetwork.Graph {
    
    public interface IGraphElement {
        Type ElementType { get; } // Type of the graph element (Node, Edge, Graph)}
        Dictionary<object, object> MetaData { get; }
        string Name { get; }
        int Serial { get; }
    }

    /// <summary>
    /// ===Purpose=== : The graph storage interface provides methods for
    /// storing, creating graphs and modifying graphs. It represents classes
    /// that store the graph data in a specific way, such as an adjacency list
    /// The methods include adding and removing nodes and edges,
    /// checking for the existence of nodes and edges.
    /// === Usage=== : This interface is used by graph classes to manage
    /// the underlying data structure of the graph. What is placed inside must
    /// first verified, so that the graph is always in a valid state. Query
    /// the inserted nodes and edges using the methods provided. Provide
    /// the necessary methods to add and remove nodes and edges
    /// </summary>
    public interface IGraphStorage {
        IReadOnlyList<INode> Nodes { get; }
        IReadOnlyList<IEdge> Edges { get; }

        void AddNode(INode node);
        void AddEdge(IEdge edge);
        void RemoveNode(INode node);
        void RemoveEdge(IEdge edge);
        bool HasEdge(IEdge edge);
        bool HasNode(INode node);
        bool AreConnected(INode source, INode target);
    }

    public interface IDirectedGraphStorage : IGraphStorage {
        IReadOnlyList<IEdge> GetOutgoingEdges(INode node);
        IReadOnlyList<IEdge> GetIncomingEdges(INode node);
    }

    public interface IUndirectedGraphStorage : IGraphStorage {
        IReadOnlyList<IEdge> GetEdges(INode node);
        IReadOnlyList<INode> GetNeighbors(INode node);
    }

    // <summary>
    /// ===Purpose=== : The adjacency list storage class implements the IGraphStorage interface
    /// The graph is consistent by construction by the provided methods. Deletion methods assume
    /// consistency of the graph, so that the graph is always in a valid state.
    public class AdjacencyListStorage : IGraphStorage {
        public IReadOnlyList<INode> Nodes => nodes.AsReadOnly();
        public IReadOnlyList<IEdge> Edges => edges.AsReadOnly();


        List<INode> nodes = new List<INode>();
        List<IEdge> edges = new List<IEdge>();

        public virtual void AddNode(INode node) {
            if (node == null) { // Check for null node
                throw new ArgumentNullException(nameof(node));
            } else if (nodes.Contains(node)) { // Check if the node already exists in the graph
                throw new ArgumentException("Node already exists in the graph.", nameof(node));
            }
            nodes.Add(node); // Add the node to the list of nodes
        }
        public virtual void AddEdge(IEdge edge) {
            if (edge == null) { // Check for null edge
                throw new ArgumentNullException(nameof(edge));
            } else if (edges.Contains(edge)) { // Check if the edge already exists in the graph
                throw new ArgumentException("Edge already exists in the graph.", nameof(edge));
            } else if (edge.Source == null ||
                       edge.Target == null) { // Check if the edge has valid source and target nodes
                throw new ArgumentException("Edge must have a valid source and target node.", nameof(edge));
            } else if (!nodes.Contains(edge.Source) ||
                       !nodes.Contains(edge.Target)) { // Check if the source and target nodes exist in the graph
                throw new ArgumentException("Source or target node does not exist in the graph.", nameof(edge));
            }
            edges.Add(edge); // Add the edge to the list of edges
        }

        public virtual bool HasNode(INode node) {
            return nodes.Contains(node);
        }
        public virtual bool HasEdge(IEdge edge) {
            return edges.Contains(edge);
        }
        public virtual void RemoveNode(INode node) {
            if (node == null) { // Check for null node
                throw new ArgumentNullException(nameof(node));
            } else if (!nodes.Contains(node)) { // Check if the node exists in the graph
                throw new ArgumentException("Node does not exist in the graph.", nameof(node));
            }
            nodes.Remove(node);
        }
        public virtual void RemoveEdge(IEdge edge) {
            if (edge == null) { // Check for null edge
                throw new ArgumentNullException(nameof(edge));
            } else if (!edges.Contains(edge)) { // Check if the edge exists in the graph
                throw new ArgumentException("Edge does not exist in the graph.", nameof(edge));
            }
            edges.Remove(edge); // Remove the edge from the list of edges
        }

        public virtual bool AreConnected(INode source, INode target) {
            if (source == null || target == null)
                throw new ArgumentNullException("Source and target nodes cannot be null.");
            return edges.Any(edge => edge.Source.Equals(source) && edge.Target.Equals(target));
        }
    }

    public class DirectedAdjacencyListStorage : AdjacencyListStorage, IDirectedGraphStorage {
        Dictionary<INode, List<IEdge>> outgoingEdge = new Dictionary<INode, List<IEdge>>();
        Dictionary<INode, List<IEdge>> incomingEdge = new Dictionary<INode, List<IEdge>>();

        public IReadOnlyList<IEdge> GetOutgoingEdges(INode node) {
            ValidateNodeExists(node);
            // Return the outgoing edges for the node
            return outgoingEdge[node].AsReadOnly();
        }

        public IReadOnlyList<IEdge> GetIncomingEdges(INode node) {
            ValidateNodeExists(node);
            // Return the incoming edges for the node
            return incomingEdge[node].AsReadOnly();
        }

        public override void AddNode(INode node) {
            base.AddNode(node);
            outgoingEdge[node] = new List<IEdge>(); // Initialize the outgoing edges list for the node
            incomingEdge[node] = new List<IEdge>(); // Initialize the incoming edges list for the node
        }

        public override void AddEdge(IEdge edge) {
            base.AddEdge(edge);
            outgoingEdge[edge.Source].Add(edge); // Add the edge to the outgoing edges of the source node
            incomingEdge[edge.Target].Add(edge); // Add the edge to the incoming edges of the target node
        }

        public override void RemoveNode(INode node) {
            base.RemoveNode(node);
            // Remove all outgoing edges associated with the node
            foreach (var edge in outgoingEdge[node]) { // Check for outgoing edges
                incomingEdge[edge.Target].Remove(edge); // Remove the edge from the incoming edges of the target node
                base.RemoveEdge(edge); // Remove the edge from the list of edges
            }
            outgoingEdge.Remove(node); // Remove the node from the outgoing edges dictionary

            // Remove all incoming edges associated with the node
            foreach (var edge in incomingEdge[node]) { // Check for incoming edges
                outgoingEdge[edge.Source].Remove(edge); // Remove the edge from the outgoing edges of the source node
                base.RemoveEdge(edge); // Remove the edge from the list of edges
            }
            incomingEdge.Remove(node); // Remove the node from the incoming edges dictionary
        }
        public override void RemoveEdge(IEdge edge) {
            base.RemoveEdge(edge);
            outgoingEdge[edge.Source].Remove(edge); // Remove the edge from the outgoing edges of the source node
            incomingEdge[edge.Target].Remove(edge); // Remove the edge from the incoming edges of the target node
        }
        private void ValidateNodeExists(INode node) {
            if (node == null) { // Check for null node
                throw new ArgumentNullException(nameof(node));
            } else if (!Nodes.Contains(node)){// Check if the node exists in the graph
                throw new ArgumentException("Node does not exist in the graph.", nameof(node));
            } else if (!outgoingEdge.ContainsKey(node)) { // Check if the node data in the graph are consistent
                throw new ArgumentException("Inconsisent graph state. Uninitialize list of Outgoing edges",
                    nameof(node));
            }
        }
    }

    public class UndirectedAdjacencyListStorage : AdjacencyListStorage, IUndirectedGraphStorage {
        Dictionary<INode, List<IEdge>> edgesByNode = new Dictionary<INode, List<IEdge>>();

        public IReadOnlyList<IEdge> GetEdges(INode node) {
            ValidateNodeExists(node);
            return edgesByNode[node].AsReadOnly();
        }
        // Alternative one-liner approach using LINQ Distinct()
        public IReadOnlyList<INode> GetNeighbors(INode node) {
            ValidateNodeExists(node);
            return edgesByNode[node]
                .Select(edge => edge.Source.Equals(node) ? edge.Target : edge.Source)
                .Distinct() // ← This removes duplicates
                .ToList()
                .AsReadOnly();
        }
        public override void AddNode(INode node) {
            base.AddNode(node);
            edgesByNode[node] = new List<IEdge>(); // Initialize the edges list for the node
        }
        public override void AddEdge(IEdge edge) {
            base.AddEdge(edge);
            edgesByNode[edge.Source].Add(edge); // Add the edge to the source node's edges
            edgesByNode[edge.Target].Add(edge); // Add the edge to the target node's edges
        }
        public override void RemoveNode(INode node) {
            ValidateNodeExists(node);

            // Create a copy to avoid modification during iteration
            var edgesToRemove = new List<IEdge>(edgesByNode[node]);

            foreach (var edge in edgesToRemove) {
                RemoveEdge(edge); // Use RemoveEdge method to handle cleanup properly
            }
            edgesByNode.Remove(node);
            base.RemoveNode(node);
        }
        public override void RemoveEdge(IEdge edge) {
            ValidateEdgeExists(edge);
            base.RemoveEdge(edge);
            edgesByNode[edge.Source].Remove(edge); // Remove the edge from the source node's edges
            edgesByNode[edge.Target].Remove(edge); // Remove the edge from the target node's edges
        }
        public override bool AreConnected(INode source, INode target) {
            if (source == null || target == null)
                throw new ArgumentNullException("Source and target nodes cannot be null.");

            // Check if nodes are neighbors (more efficient than scanning all edges)
            return edgesByNode.ContainsKey(source) &&
                   edgesByNode[source].Any(edge =>
                       (edge.Source.Equals(source) && edge.Target.Equals(target)) ||
                       (edge.Source.Equals(target) && edge.Target.Equals(source))
                   );
        }

        private void ValidateNodeExists(INode node) {
            if (node == null) {
                throw new ArgumentNullException(nameof(node));
            } else if (!Nodes.Contains(node)) {
                throw new ArgumentException("Node does not exist in the graph.", nameof(node));
            } else if (!edgesByNode.ContainsKey(node)) {
                throw new ArgumentException("Inconsisent graph state. Uninitialize list of Edges", nameof(node));
            }
        }
        private void ValidateEdgeExists(IEdge edge) {
            if (edge == null) {
                throw new ArgumentNullException(nameof(edge));
            } else if (!Edges.Contains(edge)) {
                throw new ArgumentException("Edge does not exist in the graph.", nameof(edge));
            }
        }
    }

    public interface IGraph : IGraphElement {
        /// <summary>
        /// ===Purpose=== : Provides access to methods for adding and removing nodes and edges,
        /// and query the existence of nodes and edges in the graph. 
        /// </summary>
        void AddNode(INode node);
        void AddEdge(IEdge edge);
        void RemoveNode(INode node);
        void RemoveEdge(IEdge edge);
        bool AreConnected(INode source, INode target);
        bool HasNode(INode node);
        bool HasEdge(IEdge edge);
        IReadOnlyList<INode> Nodes { get; }
        IReadOnlyList<IEdge> Edges { get; }
        string Name { get; set; }
        Dictionary<object, object> MetaData { get; set; }
    }

    public interface IGraph<T> : IGraph {
        T Value { get; set; } // Generic value associated with the graph}
    }

    public abstract class BaseGraph : IGraph {
        protected IGraphStorage storage;
        private static int serialCounter = 0;
        public string Name { get; set; }
        public Type ElementType { get; }
        public int Serial { get; init; }
        public Dictionary<object, object> MetaData { get; set; }
        public int NodeCount => storage.Nodes.Count;
        public int EdgeCount => storage.Edges.Count;
        public IReadOnlyList<INode> Nodes => storage.Nodes;
        public IReadOnlyList<IEdge> Edges => storage.Edges;

        public bool IsEmpty => NodeCount == 0;

        public override string ToString() {
            return $"{GetType().Name} '{Name}' [Nodes: {NodeCount}, Edges: {EdgeCount}]";
        }


        protected BaseGraph(IGraphStorage storage, string name = null) {
            this.storage = storage ?? throw new ArgumentNullException(nameof(storage));
            Serial = Interlocked.Increment(ref serialCounter);
            Name = name ?? $"{GetType().Name}_{Serial}";
            MetaData = new Dictionary<object, object>();
            ElementType = typeof(BaseGraph);
        }

        public void AddNode(INode node) {
            storage.AddNode(node);
        }

        public void AddEdge(IEdge edge) {
            storage.AddEdge(edge);
        }

        public void RemoveNode(INode node) {
            storage.RemoveNode(node);
        }

        public void RemoveEdge(IEdge edge) {
            storage.RemoveEdge(edge);
        }

        public virtual bool AreConnected(INode source, INode target) {
            return storage.AreConnected(source, target);
        }

        public bool HasNode(INode node) {
            return storage.HasNode(node);
        }

        public bool HasEdge(IEdge edge) {
            return storage.HasEdge(edge);
        }
    }

    public class BaseGraph<T> : BaseGraph, IGraph<T> {
        public T Value { get; set; } // Generic value associated with the graph
        public BaseGraph(IGraphStorage storage, string name = null) : base(storage, name) {
            Value = default(T);
        }
        public override string ToString() {
            return $"{base.ToString()}, Value: {Value}";
        }
    }

    public interface IDirectedGraph : IGraph {
        IEnumerable<INode> GetPredecessors(INode node);
        IEnumerable<INode> GetSuccessors(INode node);
        IEnumerable<IEdge> GetOutgoingEdges(INode node);
        IEnumerable<IEdge> GetIncomingEdges(INode node);
    }

    public interface IUndirectedGraph : IGraph {
        IEnumerable<INode> GetNeighbors(INode node);
        IEnumerable<IEdge> GetEdges(INode node);
    }

    public class UnDirectedGraph : BaseGraph, IUndirectedGraph {
        readonly IUndirectedGraphStorage undirectedStorage;

        public UnDirectedGraph(IUndirectedGraphStorage storage, string name = null) : base(storage, name) {
            undirectedStorage = storage ;
        }
        public IEnumerable<INode> GetNeighbors(INode node) {
            if (node == null) throw new ArgumentNullException(nameof(node));
            if (!storage.HasNode(node))
                throw new ArgumentException("Node does not exist in the graph.", nameof(node));
            // Use the IUndirectedGraphStorage interface to get neighbors
            return undirectedStorage.GetNeighbors(node);
        }
        public IEnumerable<IEdge> GetEdges(INode node) {
            if (node == null) throw new ArgumentNullException(nameof(node));
            if (!storage.HasNode(node))
                throw new ArgumentException("Node does not exist in the graph.", nameof(node));
            // Use the IUndirectedGraphStorage interface to get edges
            return undirectedStorage.GetEdges(node);
        }

        // Override AreConnected for undirected behavior
        public override bool AreConnected(INode source, INode target) {
            if (source == null || target == null)
                throw new ArgumentNullException("Source and target nodes cannot be null.");

            // In undirected graphs, check connection in both directions
            return storage.AreConnected(source, target) ||
                   storage.AreConnected(target, source);
        }
    }

    public class UnDirectedGraph<T> : UnDirectedGraph, IGraph<T> {
        public T Value { get; set; } // Generic value associated with the graph
        public UnDirectedGraph(IUndirectedGraphStorage storage, string name = null) :
            base(storage, name) {
            Value = default(T);
        }
        public override string ToString() {
            return $"{base.ToString()}, Value: {Value}";
        }
    }

    public class DirectedGraph : BaseGraph, IDirectedGraph {
        IDirectedGraphStorage directedStorage;

        public DirectedGraph(IDirectedGraphStorage storage, string name = null) : base(storage, name) {
            directedStorage = storage ;
        }

        // Update your DirectedGraph.GetSuccessors method
        public IEnumerable<INode> GetSuccessors(INode node) {
            if (node == null) throw new ArgumentNullException(nameof(node));
            if (!storage.HasNode(node))
                throw new ArgumentException("Node does not exist in the graph.", nameof(node));

            // Return distinct successor nodes (remove duplicates)
            return directedStorage.GetOutgoingEdges(node)
                .Select(edge => edge.Target)
                .Distinct(); // ← This fixes the issue
        }

        // Similarly, fix GetPredecessors method
        public IEnumerable<INode> GetPredecessors(INode node) {
            if (node == null) throw new ArgumentNullException(nameof(node));
            if (!storage.HasNode(node))
                throw new ArgumentException("Node does not exist in the graph.", nameof(node));

            // Return distinct predecessor nodes (remove duplicates)
            return directedStorage.GetIncomingEdges(node)
                .Select(edge => edge.Source)
                .Distinct(); // ← Also fix this one
        }

        public IEnumerable<IEdge> GetOutgoingEdges(INode node) {
            if (node == null) throw new ArgumentNullException(nameof(node));
            if (!storage.HasNode(node))
                throw new ArgumentException("Node does not exist in the graph.", nameof(node));
            return directedStorage.GetOutgoingEdges(node);
        }

        public IEnumerable<IEdge> GetIncomingEdges(INode node) {
            if (node == null) throw new ArgumentNullException(nameof(node));
            if (!storage.HasNode(node))
                throw new ArgumentException("Node does not exist in the graph.", nameof(node));
            return directedStorage.GetIncomingEdges(node);
        }
    }

    public class DirectedGraph<T> : DirectedGraph, IGraph<T> {
        public T Value { get; set; } // Generic value associated with the graph
        public DirectedGraph(IDirectedGraphStorage storage, string name = null) :
            base(storage, name) {
            Value = default(T);
        }
        public override string ToString() {
            return $"{base.ToString()}, Value: {Value}";
        }
    }
    
    public class GraphvizPrinterSettings {
        private bool _showNodeLabels = true;
        private bool _showEdgeLabels = false;
        private bool _showNodeProperties;
        private bool _showEdgeProperties;

        public bool ShowNodeLabels {
            get => _showNodeLabels;
            set => _showNodeLabels = value;
        }

        public bool ShowEdgeLabels {
            get => _showEdgeLabels;
            set => _showEdgeLabels = value;
        }

        public bool ShowNodeProperties {
            get => _showNodeProperties;
            set => _showNodeProperties = value;
        }

        public bool ShowEdgeProperties {
            get => _showEdgeProperties;
            set => _showEdgeProperties = value;
        }
    }

    public static class UndirectedGraphGraphvizPrinter {
        /// <summary>
        /// Generates a Graphviz DOT representation of an undirected graph.
        /// </summary>
        /// <param name="graph">The undirected graph to print.</param>
        /// <param name="dotFileName">The file name to write the DOT output to.</param>
        /// <returns>DOT format string.</returns>
        public static string ToDot(IUndirectedGraph graph, string dotFileName,
            GraphvizPrinterSettings printSettings) {
            if (graph == null) throw new ArgumentNullException(nameof(graph));

            var sb = new StringBuilder();
            sb.AppendLine("graph G {");

            // Print nodes
            foreach (var node in graph.Nodes) {
                CreateAugmentedGraphvizNode(node, sb,printSettings);
            }

            // Print edges (avoid duplicates by only printing edge if Source.Serial <= Target.Serial)
            var printed = new HashSet<(int, int)>();
            foreach (var edge in graph.Edges) {
                int source = edge.Source.Serial;
                int target = edge.Target.Serial;
                string edgeLabel = edge.Name;

                if (printSettings.ShowEdgeLabels) {
                    if (printSettings.ShowEdgeProperties) {
                        sb.AppendLine($"  \"{source}\" -- \"{target}\"  [label=\"{Escape(edgeLabel)}\"," +
                                      $" xlabel=\"NA\"];");
                    } else {
                        sb.AppendLine($"    \"{source}\" -- \"{target}\" [label=\"{Escape(edgeLabel)}\"]");
                    }
                } else {
                        sb.AppendLine($"    \"{source}\" -- \"{target}\";");
                }
            }

            sb.AppendLine("}");
            using (StreamWriter dotFile = new StreamWriter(dotFileName)) {
                dotFile.Write(sb);
            }
            return sb.ToString();
        }

        private static void CreateAugmentedGraphvizNode(INode node, 
            StringBuilder sb, GraphvizPrinterSettings printSettings) {
            string nodeLabel = node.Name ?? node.Serial.ToString();
            var TimeDiscovered = node.MetaData.ContainsKey("DFSUndirected")
                ? DFSUndirected.TimeDiscovered(node).ToString()
                : "N/A";
            var TimeFinished = node.MetaData.ContainsKey("DFSUndirected")
                ? DFSUndirected.TimeFinished(node).ToString()
                : "N/A";
            if (printSettings.ShowNodeLabels) {
                if (printSettings.ShowNodeProperties) {
                    sb.AppendLine($"    \"{node.Serial}\" [fixedsized=false, label=\"{Escape(nodeLabel)} \nTD:{TimeDiscovered} \nTF:{TimeFinished}\"];");
                }
                else {
                    sb.AppendLine($"    \"{node.Serial}\" [label=\"{Escape(nodeLabel)}\"]");
                }
            } else {
                if (printSettings.ShowNodeProperties) {
                    sb.AppendLine($"    \"{node.Serial}\" [xlabel=\"TD:{TimeDiscovered}\nTF:{TimeFinished}\"];");
                }
                else {
                    sb.AppendLine($"    \"{node.Serial}\";");
                }
            }
        }

        public static void GenerateGraphGif(string dotFilePath, string outputGifPath) {
            if (string.IsNullOrWhiteSpace(dotFilePath))
                throw new ArgumentException("DOT file path must be provided.", nameof(dotFilePath));
            if (string.IsNullOrWhiteSpace(outputGifPath))
                throw new ArgumentException("Output GIF path must be provided.", nameof(outputGifPath));

            var processStartInfo = new System.Diagnostics.ProcessStartInfo {
                FileName = "dot",
                Arguments = $"-Tgif \"{dotFilePath}\" -o \"{outputGifPath}\"",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using (var process = new System.Diagnostics.Process { StartInfo = processStartInfo }) {
                process.Start();
                string stdOut = process.StandardOutput.ReadToEnd();
                string stdErr = process.StandardError.ReadToEnd();
                process.WaitForExit();

                if (process.ExitCode != 0) {
                    throw new InvalidOperationException(
                        $"dot process failed with exit code {process.ExitCode}: {stdErr}");
                }
            }
        }

        private static string Escape(string label) {
            return label?.Replace("\"", "\\\"") ?? string.Empty;
        }
    }
    public static class DirectedGraphGraphvizPrinter {
        /// <summary>
        /// Generates a Graphviz DOT representation of a directed graph.
        /// </summary>
        /// <param name="graph">The directed graph to print.</param>
        /// <param name="dotFileName">The file name to write the DOT output to.</param>
        /// <param name="printSettings">Settings for node/edge labels and properties.</param>
        /// <returns>DOT format string.</returns>
        public static string ToDot(IDirectedGraph graph, string dotFileName, GraphvizPrinterSettings printSettings) {
            if (graph == null) throw new ArgumentNullException(nameof(graph));

            var sb = new StringBuilder();
            sb.AppendLine("digraph G {");

            // Print nodes
            foreach (var node in graph.Nodes) {
                CreateAugmentedGraphvizNode(node, sb, printSettings);
            }

            // Print edges
            foreach (var edge in graph.Edges) {
                int source = edge.Source.Serial;
                int target = edge.Target.Serial;
                string edgeLabel = edge.Name;

                if (printSettings.ShowEdgeLabels) {
                    if (printSettings.ShowEdgeProperties) {
                        sb.AppendLine($"  \"{source}\" -> \"{target}\" [label=\"{Escape(edgeLabel)}\", xlabel=\"NA\"];");
                    } else {
                        sb.AppendLine($"    \"{source}\" -> \"{target}\" [label=\"{Escape(edgeLabel)}\"]");
                    }
                } else {
                    sb.AppendLine($"    \"{source}\" -> \"{target}\";");
                }
            }

            sb.AppendLine("}");
            using (StreamWriter dotFile = new StreamWriter(dotFileName)) {
                dotFile.Write(sb);
            }
            return sb.ToString();
        }

        private static void CreateAugmentedGraphvizNode(INode node, StringBuilder sb, GraphvizPrinterSettings printSettings) {
            string nodeLabel = node.Name ?? node.Serial.ToString();
            var TimeDiscovered = node.MetaData.ContainsKey("DFSDirected")
                ? DFSDirected.TimeDiscovered(node).ToString()
                : "N/A";
            var TimeFinished = node.MetaData.ContainsKey("DFSDirected")
                ? DFSDirected.TimeFinished(node).ToString()
                : "N/A";
            if (printSettings.ShowNodeLabels) {
                if (printSettings.ShowNodeProperties) {
                    sb.AppendLine($"    \"{node.Serial}\" [fixedsized=false, label=\"{Escape(nodeLabel)} \\nTD:{TimeDiscovered} \\nTF:{TimeFinished}\"];");
                } else {
                    sb.AppendLine($"    \"{node.Serial}\" [label=\"{Escape(nodeLabel)}\"]");
                }
            } else {
                if (printSettings.ShowNodeProperties) {
                    sb.AppendLine($"    \"{node.Serial}\" [xlabel=\"TD:{TimeDiscovered}\\nTF:{TimeFinished}\"];");
                } else {
                    sb.AppendLine($"    \"{node.Serial}\";");
                }
            }
        }

        public static void GenerateGraphGif(string dotFilePath, string outputGifPath) {
            if (string.IsNullOrWhiteSpace(dotFilePath))
                throw new ArgumentException("DOT file path must be provided.", nameof(dotFilePath));
            if (string.IsNullOrWhiteSpace(outputGifPath))
                throw new ArgumentException("Output GIF path must be provided.", nameof(outputGifPath));

            var processStartInfo = new System.Diagnostics.ProcessStartInfo {
                FileName = "dot",
                Arguments = $"-Tgif \"{dotFilePath}\" -o \"{outputGifPath}\"",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using (var process = new System.Diagnostics.Process { StartInfo = processStartInfo }) {
                process.Start();
                string stdOut = process.StandardOutput.ReadToEnd();
                string stdErr = process.StandardError.ReadToEnd();
                process.WaitForExit();

                if (process.ExitCode != 0) {
                    throw new InvalidOperationException(
                        $"dot process failed with exit code {process.ExitCode}: {stdErr}");
                }
            }
        }

        private static string Escape(string label) {
            return label?.Replace("\"", "\\\"") ?? string.Empty;
        }
    }

    

    



}
