using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualBasic;

namespace CommunicationNetwork.Graph {

    /// <summary>
    /// ===Purpose=== : The graph could be a directed or undirected graph, depending
    /// on the implementation. The same graph storage can be used for both
    /// directed and undirected graphs.
    /// Interface for a graph view, which provides access to nodes and edges. 
    /// </summary>
    public interface IGraphView {
        IEnumerable<INode> Nodes { get; }
        IEnumerable<IEdge> Edges { get; }

        void AddNode(INode node);

        void AddEdge(IEdge edge);
    }

    /// <summary>
    /// ===Purpose=== : The graph storage interface provides methods for
    /// storing, creating graphs and modifying graphs. It represents classes
    /// that store the graph data in a specific way, such as an adjacency list 
    /// </summary>
    public interface IGraphStorage{
        IReadOnlyList<INode> Nodes { get;  }
        IReadOnlyList<IEdge> Edges { get; }
        IReadOnlyDictionary<INode, IEdge> OutgoingEdge { get;  }
        IReadOnlyDictionary<INode, IEdge> IncomingEdge { get;  }

        void AddNode(INode node);
        void AddEdge(IEdge edge);
        void RemoveNode(INode node);
        void RemoveEdge(IEdge edge);
        bool HasEdge(IEdge edge);
        bool HasNode(INode node);
        bool AreConnected(INode source, INode target);
    }
    
    public class AdjacencyListStorage : IGraphStorage {
        public IReadOnlyList<INode> Nodes => nodes.AsReadOnly(); 
        public IReadOnlyList<IEdge> Edges => edges.AsReadOnly();
        public IReadOnlyDictionary<INode, IEdge> OutgoingEdge => outgoingEdge.AsReadOnly();
        public IReadOnlyDictionary<INode, IEdge> IncomingEdge => incomingEdge.AsReadOnly();

        List<INode> nodes = new List<INode>();
        List<IEdge> edges = new List<IEdge>();
        Dictionary<INode, IEdge> outgoingEdge = new Dictionary<INode, IEdge>();
        Dictionary<INode, IEdge> incomingEdge = new Dictionary<INode, IEdge>();

        public void AddNode(INode node) {
            nodes.Add(node);
        }
        public void AddEdge(IEdge edge) {
            edges.Add(edge);
            if (!outgoingEdge.ContainsKey(edge.Source)) {
                outgoingEdge[edge.Source] = edge;
            }
            if (!incomingEdge.ContainsKey(edge.Target)) {
                incomingEdge[edge.Target] = edge;
            }
        }
        public bool HasNode(INode node) {
            return nodes.Contains(node);
        }
        public bool HasEdge(IEdge edge) {
            return edges.Contains(edge);
        }
        public void RemoveNode(INode node) {
            nodes.Remove(node);
            outgoingEdge.Remove(node);
            incomingEdge.Remove(node);
        }
        public void RemoveEdge(IEdge edge) {
            edges.Remove(edge);
            outgoingEdge.Remove(edge.Source);
            incomingEdge.Remove(edge.Target);
        }
        public bool AreConnected(INode source, INode target) {
            if (!outgoingEdge.ContainsKey(source) || !incomingEdge.ContainsKey(target)) {
                return false;
            }
            return outgoingEdge[source].Target == target &&
                   incomingEdge[target].Source == source;
        }
    }

    public interface IDirectedGraphView : IGraphView{
        IEnumerable<INode> Predecessors(INode node);
        IEnumerable<INode> Successors(INode node);
        IEnumerable<IEdge> IncomingEdges(INode node);
        IEnumerable<IEdge> OutgoingEdges(INode node);
        
    }

    public interface IUndirectedGraphView : IGraphView {

        IEnumerable<INode> Neighbors(INode node);
        IEnumerable<IEdge> IncidentEdges(INode node);
    }
    
    public interface IGraph {
        /// <summary>
        /// ===Purpose=== : Provides access to the graph view, which allows interaction
        /// with the graph's nodes and edges. We choose composition to allow changing
        /// of view at runtime depending on the choosen view. 
        /// </summary>
        void AddNode(INode node);
        void AddEdge(IEdge edge);
        void RemoveNode(INode node);
        void RemoveEdge(IEdge edge);
        bool AreConnected(INode source, INode target);
        IDirectedGraphView DirectedView { get; }
        IUndirectedGraphView UndirectedView { get; }
        string Name { get; set; }
        UInt32 SerialNumber { get; init; }
        Dictionary<string, object> MetaData { get; set; }
    }

    internal class Graph {
    }
}
