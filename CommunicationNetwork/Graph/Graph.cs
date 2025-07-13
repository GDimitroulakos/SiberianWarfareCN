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
        string ID { get; }
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
        IReadOnlyList<Node> Nodes { get; }
        IReadOnlyList<Edge> Edges { get; }

        void AddNode(Node node);
        void AddEdge(Edge edge);
        void RemoveNode(Node node);
        void RemoveEdge(Edge edge);
        bool HasEdge(Edge edge);
        bool HasNode(Node node);
        bool AreConnected(Node source, Node target);
    }

    public interface IDirectedGraphStorage : IGraphStorage {
        IReadOnlyList<Edge> GetOutgoingEdges(Node node);
        IReadOnlyList<Edge> GetIncomingEdges(Node node);
    }

    public interface IUndirectedGraphStorage : IGraphStorage {
        IReadOnlyList<Edge> GetEdges(Node node);
        IReadOnlyList<Node> GetNeighbors(Node node);
    }

    // <summary>
    /// ===Purpose=== : The adjacency list storage class implements the IGraphStorage interface
    /// The graph is consistent by construction by the provided methods. Deletion methods assume
    /// consistency of the graph, so that the graph is always in a valid state.
    public class AdjacencyListStorage : IGraphStorage {
        public IReadOnlyList<Node> Nodes => nodes.AsReadOnly();
        public IReadOnlyList<Edge> Edges => edges.AsReadOnly();


        List<Node> nodes = new List<Node>();
        List<Edge> edges = new List<Edge>();

        public virtual void AddNode(Node node) {
            if (node == null) { // Check for null node
                throw new ArgumentNullException(nameof(node));
            } else if (nodes.Contains(node)) { // Check if the node already exists in the graph
                throw new ArgumentException("Node already exists in the graph.", nameof(node));
            }
            nodes.Add(node); // Add the node to the list of nodes
        }
        public virtual void AddEdge(Edge edge) {
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

        public virtual bool HasNode(Node node) {
            return nodes.Contains(node);
        }
        public virtual bool HasEdge(Edge edge) {
            return edges.Contains(edge);
        }
        public virtual void RemoveNode(Node node) {
            if (node == null) { // Check for null node
                throw new ArgumentNullException(nameof(node));
            } else if (!nodes.Contains(node)) { // Check if the node exists in the graph
                throw new ArgumentException("Node does not exist in the graph.", nameof(node));
            }
            nodes.Remove(node);
        }
        public virtual void RemoveEdge(Edge edge) {
            if (edge == null) { // Check for null edge
                throw new ArgumentNullException(nameof(edge));
            } else if (!edges.Contains(edge)) { // Check if the edge exists in the graph
                throw new ArgumentException("Edge does not exist in the graph.", nameof(edge));
            }
            edges.Remove(edge); // Remove the edge from the list of edges
        }

        public virtual bool AreConnected(Node source, Node target) {
            if (source == null || target == null)
                throw new ArgumentNullException("Source and target nodes cannot be null.");
            return edges.Any(edge => edge.Source.Equals(source) && edge.Target.Equals(target));
        }
    }

    public class DirectedAdjacencyListStorage : AdjacencyListStorage, IDirectedGraphStorage {
        Dictionary<Node, List<Edge>> outgoingEdge = new Dictionary<Node, List<Edge>>();
        Dictionary<Node, List<Edge>> incomingEdge = new Dictionary<Node, List<Edge>>();

        public IReadOnlyList<Edge> GetOutgoingEdges(Node node) {
            ValidateNodeExists(node);
            // Return the outgoing edges for the node
            return outgoingEdge[node].AsReadOnly();
        }

        public IReadOnlyList<Edge> GetIncomingEdges(Node node) {
            ValidateNodeExists(node);
            // Return the incoming edges for the node
            return incomingEdge[node].AsReadOnly();
        }

        public override void AddNode(Node node) {
            base.AddNode(node);
            outgoingEdge[node] = new List<Edge>(); // Initialize the outgoing edges list for the node
            incomingEdge[node] = new List<Edge>(); // Initialize the incoming edges list for the node
        }

        public override void AddEdge(Edge edge) {
            base.AddEdge(edge);
            outgoingEdge[edge.Source].Add(edge); // Add the edge to the outgoing edges of the source node
            incomingEdge[edge.Target].Add(edge); // Add the edge to the incoming edges of the target node
        }

        public override void RemoveNode(Node node) {
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
        public override void RemoveEdge(Edge edge) {
            base.RemoveEdge(edge);
            outgoingEdge[edge.Source].Remove(edge); // Remove the edge from the outgoing edges of the source node
            incomingEdge[edge.Target].Remove(edge); // Remove the edge from the incoming edges of the target node
        }
        private void ValidateNodeExists(Node node) {
            if (node == null) { // Check for null node
                throw new ArgumentNullException(nameof(node));
            } else if (!Nodes.Contains(node)) {// Check if the node exists in the graph
                throw new ArgumentException("Node does not exist in the graph.", nameof(node));
            } else if (!outgoingEdge.ContainsKey(node)) { // Check if the node data in the graph are consistent
                throw new ArgumentException("Inconsisent graph state. Uninitialize list of Outgoing edges",
                    nameof(node));
            }
        }
    }
    public class UndirectedAdjacencyListStorage : AdjacencyListStorage, IUndirectedGraphStorage {
        Dictionary<Node, List<Edge>> edgesByNode = new Dictionary<Node, List<Edge>>();

        public IReadOnlyList<Edge> GetEdges(Node node) {
            ValidateNodeExists(node);
            return edgesByNode[node].AsReadOnly();
        }
        // Alternative one-liner approach using LINQ Distinct()
        public IReadOnlyList<Node> GetNeighbors(Node node) {
            ValidateNodeExists(node);
            return edgesByNode[node]
                .Select(edge => edge.Source.Equals(node) ? edge.Target : edge.Source)
                .Distinct() // ← This removes duplicates
                .ToList()
                .AsReadOnly();
        }
        public override void AddNode(Node node) {
            base.AddNode(node);
            edgesByNode[node] = new List<Edge>(); // Initialize the edges list for the node
        }
        public override void AddEdge(Edge edge) {
            base.AddEdge(edge);
            edgesByNode[edge.Source].Add(edge); // Add the edge to the source node's edges
            edgesByNode[edge.Target].Add(edge); // Add the edge to the target node's edges
        }
        public override void RemoveNode(Node node) {
            ValidateNodeExists(node);

            // Create a copy to avoid modification during iteration
            var edgesToRemove = new List<Edge>(edgesByNode[node]);

            foreach (var edge in edgesToRemove) {
                RemoveEdge(edge); // Use RemoveEdge method to handle cleanup properly
            }
            edgesByNode.Remove(node);
            base.RemoveNode(node);
        }
        public override void RemoveEdge(Edge edge) {
            ValidateEdgeExists(edge);
            base.RemoveEdge(edge);
            edgesByNode[edge.Source].Remove(edge); // Remove the edge from the source node's edges
            edgesByNode[edge.Target].Remove(edge); // Remove the edge from the target node's edges
        }
        public override bool AreConnected(Node source, Node target) {
            if (source == null || target == null)
                throw new ArgumentNullException("Source and target nodes cannot be null.");

            // Check if nodes are neighbors (more efficient than scanning all edges)
            return edgesByNode.ContainsKey(source) &&
                   edgesByNode[source].Any(edge =>
                       (edge.Source.Equals(source) && edge.Target.Equals(target)) ||
                       (edge.Source.Equals(target) && edge.Target.Equals(source))
                   );
        }

        private void ValidateNodeExists(Node node) {
            if (node == null) {
                throw new ArgumentNullException(nameof(node));
            } else if (!Nodes.Contains(node)) {
                throw new ArgumentException("Node does not exist in the graph.", nameof(node));
            } else if (!edgesByNode.ContainsKey(node)) {
                throw new ArgumentException("Inconsisent graph state. Uninitialize list of Edges", nameof(node));
            }
        }
        private void ValidateEdgeExists(Edge edge) {
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
        void AddNode(Node node);
        void AddEdge(Edge edge);
        void RemoveNode(Node node);
        void RemoveEdge(Edge edge);
        bool AreConnected(Node source, Node target);
        IReadOnlyList<Node> GetNeighbors(Node node);
        bool HasNode(Node node);
        bool HasEdge(Edge edge);
        Edge GetEdge(Node source, Node target);
        IReadOnlyList<Node> Nodes { get; }
        IReadOnlyList<Edge> Edges { get; }
        string ID { get; }
        Dictionary<object, object> MetaData { get; set; }
    }

    public interface IGraph<T> : IGraph {
        T Value { get; set; } // Generic value associated with the graph}
    }

    public abstract class BaseGraph : IGraph {
        protected IGraphStorage storage;
        private static int serialCounter = 0;
        public virtual string ID { get; set; }
        public Type ElementType { get; }
        public int Serial { get; init; }
        public Dictionary<object, object> MetaData { get; set; }
        public int NodeCount => storage.Nodes.Count;
        public int EdgeCount => storage.Edges.Count;
        public IReadOnlyList<Node> Nodes => storage.Nodes;
        public IReadOnlyList<Edge> Edges => storage.Edges;

        public bool IsEmpty => NodeCount == 0;

        public override string ToString() {
            return $"{GetType().Name} '{ID}' [Nodes: {NodeCount}, Edges: {EdgeCount}]";
        }

        protected BaseGraph(IGraphStorage storage, string name = null) {
            this.storage = storage ?? throw new ArgumentNullException(nameof(storage));
            Serial = Interlocked.Increment(ref serialCounter);
            ID = name ?? $"{GetType().Name}_{Serial}";
            MetaData = new Dictionary<object, object>();
            ElementType = typeof(BaseGraph);
        }

        public void AddNode(Node node) {
            storage.AddNode(node);
        }

        public void AddEdge(Edge edge) {
            storage.AddEdge(edge);
        }

        public void RemoveNode(Node node) {
            storage.RemoveNode(node);
        }

        public void RemoveEdge(Edge edge) {
            storage.RemoveEdge(edge);
        }

        public virtual bool AreConnected(Node source, Node target) {
            return storage.AreConnected(source, target);
        }

        public Edge GetEdge(Node source, Node target) {
            if (source == null || target == null)
                throw new ArgumentNullException("Source and target nodes cannot be null.");
            return storage.Edges.FirstOrDefault(edge => edge.Source.Equals(source) && edge.Target.Equals(target));
        }

        public bool HasNode(Node node) {
            return storage.HasNode(node);
        }

        public bool HasEdge(Edge edge) {
            return storage.HasEdge(edge);
        }

        public abstract IReadOnlyList<Node> GetNeighbors(Node node);
    }

    public abstract class BaseGraph<T> : BaseGraph, IGraph<T> {
        public T Value { get; set; } // Generic value associated with the graph
        public BaseGraph(IGraphStorage storage, string name = null) : base(storage, name) {
            Value = default(T);
        }
        public override string ToString() {
            return $"{base.ToString()}, Value: {Value}";
        }
    }

    public interface IDirectedGraph : IGraph {
        IEnumerable<Node> GetPredecessors(Node node);
        IEnumerable<Node> GetSuccessors(Node node);
        IEnumerable<Edge> GetOutgoingEdges(Node node);
        IEnumerable<Edge> GetIncomingEdges(Node node);
    }

    public interface IUndirectedGraph : IGraph {
        IReadOnlyList<Node> GetNeighbors(Node node);
        IEnumerable<Edge> GetEdges(Node node);
    }

    public class UnDirectedGraph : BaseGraph, IUndirectedGraph {
        readonly IUndirectedGraphStorage undirectedStorage;

        public UnDirectedGraph(IUndirectedGraphStorage storage, string name = null) :
            base(storage, name) {
            undirectedStorage = storage;
        }
        public override IReadOnlyList<Node> GetNeighbors(Node node) {
            if (node == null) throw new ArgumentNullException(nameof(node));
            if (!storage.HasNode(node))
                throw new ArgumentException("Node does not exist in the graph.", nameof(node));
            // Use the IUndirectedGraphStorage interface to get neighbors
            return undirectedStorage.GetNeighbors(node);
        }
        public IEnumerable<Edge> GetEdges(Node node) {
            if (node == null) throw new ArgumentNullException(nameof(node));
            if (!storage.HasNode(node))
                throw new ArgumentException("Node does not exist in the graph.", nameof(node));
            // Use the IUndirectedGraphStorage interface to get edges
            return undirectedStorage.GetEdges(node);
        }

        // Override AreConnected for undirected behavior
        public override bool AreConnected(Node source, Node target) {
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
            directedStorage = storage;
        }

        // Update your DirectedGraph.GetSuccessors method
        public IEnumerable<Node> GetSuccessors(Node node) {
            if (node == null) throw new ArgumentNullException(nameof(node));
            if (!storage.HasNode(node))
                throw new ArgumentException("Node does not exist in the graph.", nameof(node));

            // Return distinct successor nodes (remove duplicates)
            return directedStorage.GetOutgoingEdges(node)
                .Select(edge => edge.Target)
                .Distinct(); // ← This fixes the issue
        }

        // Similarly, fix GetPredecessors method
        public IEnumerable<Node> GetPredecessors(Node node) {
            if (node == null) throw new ArgumentNullException(nameof(node));
            if (!storage.HasNode(node))
                throw new ArgumentException("Node does not exist in the graph.", nameof(node));

            // Return distinct predecessor nodes (remove duplicates)
            return directedStorage.GetIncomingEdges(node)
                .Select(edge => edge.Source)
                .Distinct(); // ← Also fix this one
        }

        public IEnumerable<Edge> GetOutgoingEdges(Node node) {
            if (node == null) throw new ArgumentNullException(nameof(node));
            if (!storage.HasNode(node))
                throw new ArgumentException("Node does not exist in the graph.", nameof(node));
            return directedStorage.GetOutgoingEdges(node);
        }

        public override IReadOnlyList<Node> GetNeighbors(Node node) {
            List<Node> neighbors = GetSuccessors(node).ToList(); // For directed graphs, neighbors are successors
            return neighbors;
        }

        public IEnumerable<Edge> GetIncomingEdges(Node node) {
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
}
