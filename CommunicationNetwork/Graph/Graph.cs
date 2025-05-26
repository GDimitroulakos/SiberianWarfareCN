using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    /// storing, creating graphs and modifying graphs.
    /// </summary>
    public interface IGraphStorage{
        List<INode> Nodes { get; set; }
        List<IEdge> Edges { get; set; }
        Dictionary<INode, IEdge> OutgoingEdge { get; set; }
        Dictionary<INode, IEdge> IncomingEdge { get; set; }

        void AddNode(INode node);
        void AddEdge(IEdge edge);
        void RemoveNode(INode node);
        void RemoveEdge(IEdge edge);
        bool AreConnected(INode source, INode target);
    }

    public class AdjacencyListStorage : IGraphStorage{
        public List<INode> Nodes { get; set; } = new List<INode>();
        public List<IEdge> Edges { get; set; } = new List<IEdge>();
        public Dictionary<INode, IEdge> OutgoingEdge { get; set; } = new Dictionary<INode, IEdge>();
        public Dictionary<INode, IEdge> IncomingEdge { get; set; } = new Dictionary<INode, IEdge>();
        public void AddNode(INode node) {
            Nodes.Add(node);
        }
        public void AddEdge(IEdge edge) {
            Edges.Add(edge);
            if (!OutgoingEdge.ContainsKey(edge.Source)) {
                OutgoingEdge[edge.Source] = edge;
            }
            if (!IncomingEdge.ContainsKey(edge.Target)) {
                IncomingEdge[edge.Target] = edge;
            }
        }
        public void RemoveNode(INode node) {
            Nodes.Remove(node);
            OutgoingEdge.Remove(node);
            IncomingEdge.Remove(node);
        }
        public void RemoveEdge(IEdge edge) {
            Edges.Remove(edge);
            OutgoingEdge.Remove(edge.Source);
            IncomingEdge.Remove(edge.Target);
        }
        public bool AreConnected(INode source, INode target) {
            return OutgoingEdge.ContainsKey(source) && IncomingEdge.ContainsKey(target);
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
