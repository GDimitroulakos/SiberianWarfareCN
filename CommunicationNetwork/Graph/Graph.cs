using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.VisualBasic;

namespace CommunicationNetwork.Graph {

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

        IReadOnlyList<IEdge> GetOutgoingEdges(INode node);
        IReadOnlyList<IEdge> GetIncomingEdges(INode node);
        void AddNode(INode node);
        void AddEdge(IEdge edge);
        void RemoveNode(INode node);
        void RemoveEdge(IEdge edge);
        bool HasEdge(IEdge edge);
        bool HasNode(INode node);
        bool AreConnected(INode source, INode target);
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
        Dictionary<INode, List<IEdge>> outgoingEdge = new Dictionary<INode, List<IEdge>>();
        Dictionary<INode, List<IEdge>> incomingEdge = new Dictionary<INode, List<IEdge>>();

        public IReadOnlyList<IEdge> GetOutgoingEdges(INode node) {
            if (node == null) { // Check for null node
                throw new ArgumentNullException(nameof(node));
            }
            else if(!nodes.Contains(node)) // Check if the node exists in the graph
            {
                throw new ArgumentException("Node does not exist in the graph.", nameof(node));
            }
            else if (!outgoingEdge.ContainsKey(node)) { // Check if the node data in the graph are consistent
                throw new ArgumentException("Inconsisent graph state. Uninitialize list of Outgoing edges",
                    nameof(node));
            }
            // Return the outgoing edges for the node
            return outgoingEdge[node].AsReadOnly();
        }

        public IReadOnlyList<IEdge> GetIncomingEdges(INode node) {
            if (node == null) {
                throw new ArgumentNullException(nameof(node));
            }
            else if (!nodes.Contains(node)) {
                throw new ArgumentException("Node does not exist in the graph.", nameof(node));
            }
            else if (!incomingEdge.ContainsKey(node)) {
                throw new ArgumentException("Inconsisent graph state. Uninitialize list of Incoming edges",
                    nameof(node));
            }
            // Return the incoming edges for the node
            return incomingEdge[node].AsReadOnly();
        }

        public void AddNode(INode node) {
            if (node == null) { // Check for null node
                throw new ArgumentNullException(nameof(node));
            }
            else if (nodes.Contains(node)) { // Check if the node already exists in the graph
                throw new ArgumentException("Node already exists in the graph.", nameof(node));
            }
            nodes.Add(node); // Add the node to the list of nodes
            outgoingEdge[node] = new List<IEdge>(); // Initialize the outgoing edges list for the node
            incomingEdge[node] = new List<IEdge>(); // Initialize the incoming edges list for the node
        }
        public void AddEdge(IEdge edge) {
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
            outgoingEdge[edge.Source].Add(edge); // Add the edge to the outgoing edges of the source node
            incomingEdge[edge.Target].Add(edge); // Add the edge to the incoming edges of the target node
        }

        public bool HasNode(INode node) {
            return nodes.Contains(node);
        }
        public bool HasEdge(IEdge edge) {
            return edges.Contains(edge);
        }
        public void RemoveNode(INode node) {
            if (node == null) { // Check for null node
                throw new ArgumentNullException(nameof(node));
            } else if (!nodes.Contains(node)) { // Check if the node exists in the graph
                throw new ArgumentException("Node does not exist in the graph.", nameof(node));
            }
            nodes.Remove(node);
            // Remove all outgoing edges associated with the node
            foreach (var edge in outgoingEdge[node]) { // Check for outgoing edges
                incomingEdge[edge.Target].Remove(edge); // Remove the edge from the incoming edges of the target node
                edges.Remove(edge); // Remove the edge from the list of edges
            }
            outgoingEdge.Remove(node); // Remove the node from the outgoing edges dictionary

            // Remove all incoming edges associated with the node
            foreach (var edge in incomingEdge[node]) { // Check for incoming edges
                outgoingEdge[edge.Source].Remove(edge); // Remove the edge from the outgoing edges of the source node
                edges.Remove(edge); // Remove the edge from the list of edges
            }
            incomingEdge.Remove(node); // Remove the node from the incoming edges dictionary
        }
        public void RemoveEdge(IEdge edge) {
            if (edge == null) { // Check for null edge
                throw new ArgumentNullException(nameof(edge));
            } else if (!edges.Contains(edge)) { // Check if the edge exists in the graph
                throw new ArgumentException("Edge does not exist in the graph.", nameof(edge));
            } 
            edges.Remove(edge); // Remove the edge from the list of edges
            outgoingEdge[edge.Source].Remove(edge); // Remove the edge from the outgoing edges of the source node
            incomingEdge[edge.Target].Remove(edge); // Remove the edge from the incoming edges of the target node
        }
        public bool AreConnected(INode source, INode target) {
            if (source == null || target == null)
                throw new ArgumentNullException("Source and target nodes cannot be null.");
            if (!outgoingEdge.ContainsKey(source)) return false;
            return outgoingEdge[source].Any(edge => edge.Target.Equals(target));
        }
    }
    
    public interface IGraph {
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
        string Name { get; set; }
        UInt32 SerialNumber { get; init; }
        Dictionary<string, object> MetaData { get; set; }
    }

    public abstract class BaseGraph : IGraph {
        protected IGraphStorage storage;
        private static uint serialCounter = 0;
        public string Name { get; set; }
        public uint SerialNumber { get; init; }
        public Dictionary<string, object> MetaData { get; set; }
        public int NodeCount => storage.Nodes.Count;
        public int EdgeCount => storage.Edges.Count;
        public bool IsEmpty => NodeCount == 0;
        public override string ToString() {
            return $"{GetType().Name} '{Name}' [Nodes: {NodeCount}, Edges: {EdgeCount}]";
        }


        protected BaseGraph(IGraphStorage storage, string name = null) {
            this.storage = storage ?? throw new ArgumentNullException(nameof(storage));
            SerialNumber = serialCounter++;
            Name = name ?? $"{GetType().Name}_{SerialNumber}";
            MetaData = new Dictionary<string, object>();
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

        public UnDirectedGraph(IGraphStorage storage, string name = null) : base(storage, name) {
        }
        public IEnumerable<INode> GetNeighbors(INode node) {
            if (node == null) throw new ArgumentNullException(nameof(node));

            var neighbors = storage.GetOutgoingEdges(node)
                .Select(edge => edge.Target)
                .Concat(storage.GetIncomingEdges(node).Select(edge => edge.Source))
                .ToHashSet();
          
            return neighbors; 
        }
        public IEnumerable<IEdge> GetEdges(INode node) {
            if (node == null) throw new ArgumentNullException(nameof(node));

            var edges = storage.GetOutgoingEdges(node)
                .Concat(storage.GetIncomingEdges(node))
                .ToList();

            return edges;
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

    public class DirectedGraph : BaseGraph, IDirectedGraph {

        public DirectedGraph(IGraphStorage storage, string name=null) : base(storage,name) {
        }

        public IEnumerable<INode> GetPredecessors(INode node) {
            if (node == null) throw new ArgumentNullException(nameof(node));
            if (!storage.HasNode(node))
                throw new ArgumentException("Node does not exist in the graph.", nameof(node));
            return storage.GetIncomingEdges(node).Select(edge => edge.Source);
        }

        public IEnumerable<INode> GetSuccessors(INode node) {
            if (node == null) throw new ArgumentNullException(nameof(node));
            if (!storage.HasNode(node))
                throw new ArgumentException("Node does not exist in the graph.", nameof(node));
            return storage.GetOutgoingEdges(node).Select(edge => edge.Target);
        }

        public IEnumerable<IEdge> GetOutgoingEdges(INode node) {
            if (node == null) throw new ArgumentNullException(nameof(node));
            if (!storage.HasNode(node))
                throw new ArgumentException("Node does not exist in the graph.", nameof(node));
            return storage.GetOutgoingEdges(node);
        }

        public IEnumerable<IEdge> GetIncomingEdges(INode node) {
            if (node == null) throw new ArgumentNullException(nameof(node));
            if (!storage.HasNode(node))
                throw new ArgumentException("Node does not exist in the graph.", nameof(node));
            return storage.GetIncomingEdges(node);
        }

    }
}
