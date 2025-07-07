using System;
using System.Collections.Generic;
using System.Linq;
using CommunicationNetwork.Algorithms;
using CommunicationNetwork.Graph;
using static CommunicationNetwork.Algorithms.BFSUndirected;

namespace CommunicationNetwork.Algorithms {

    public abstract class BaseAlgorithm {
       public string Name { get; }
       public abstract void Execute();
       public abstract void Initialize();
    }


    /// <summary>
    /// I want to create a distributed memory model where each graph object (node,edge,graph) or scope ( global, local )
    /// will have its own metadata dictionary. Every scope has a set of visible dictionaries that can be used to store metadata.
    /// Every scope is initialized with the set of dictionaries that are visible to it. Each algorithm has its own scope
    /// For example, the DFS algorithm will have a scope that contains the following dictionaries:
    /// 1) Graph: The graph that is being traversed
    /// 2) Nodes: Each node in the graph will have its own metadata dictionary. The key for the metadata dictionary will be
    /// the name of the algorithm or the algorithm object itself, 
    /// 3) Edges: Each edge in the graph will have its own metadata dictionary. The key for the metadata dictionary will be
    /// the name of the algorithm or the algorithm object itself. 
    /// 4) Global: A global dictionary that can be used to store metadata that is visible to all algorithms.
    /// 5) Algorithm: A dictionary that contains the metadata for the algorithm itself, such as the time taken to execute
    /// the algorithm, the number of nodes visited, etc.
    /// </summary>
    public class DFSUndirected : BaseAlgorithm {
        private Dictionary<string, object> _dictData = new Dictionary<string, object>();
        UnDirectedGraph _graph;
        int time = 0;


        public static string MetadataKey => "DFSUndirected";
        public string []Variables = new string[] { "Graph","Color", "TimeDiscovered", "TimeFinished","Time" };

        public DFSUndirected() {
        }
        
        public class DFSUndirected_NodeMetaData {
            public string Color; // WHITE, GRAY, BLACK
            public int TimeDiscovered; // Time when the node was discovered
            public int TimeFinished; // Time when the node was finished

            public override string ToString() {
                return $"DFS Undirected_NodeMetaData \nColor={Color}\nTimeDiscovered={TimeDiscovered}\nTimeFinished={TimeFinished}";
            }
        }
        
        public UnDirectedGraph Graph() {
            return _graph;
        }
        public void SetUnDirectedGraph(UnDirectedGraph graph) {
            _graph = graph;
        }
        public static string Color(INode node) {
            return ((DFSUndirected_NodeMetaData)node.MetaData[MetadataKey]).Color;
        }
        public void SetColor(INode node, string color) {
            var metaData = (DFSUndirected_NodeMetaData)node.MetaData[MetadataKey];
            metaData.Color = color;
        }
        public static int TimeDiscovered(INode node) {
           return ((DFSUndirected_NodeMetaData)node.MetaData[MetadataKey]).TimeDiscovered;
        }
        private void SetTimeDiscovered(INode node, int t) {
            var metaData = (DFSUndirected_NodeMetaData)node.MetaData[MetadataKey];
            metaData.TimeDiscovered = t;
        }
        public static int TimeFinished(INode node) {
            return ((DFSUndirected_NodeMetaData)node.MetaData[MetadataKey]).TimeFinished;
        } private void SetTimeFinished(INode node, int t) {
            var metaData = (DFSUndirected_NodeMetaData)node.MetaData[MetadataKey];
            metaData.TimeFinished = t;
        }
        
        public override void Initialize() {
            time = 0;
            foreach (INode node in _graph.Nodes) {
                // Initialize metadata for each node
                node.MetaData[MetadataKey] = new DFSUndirected_NodeMetaData() {
                    Color = "WHITE",  // Unvisited
                    TimeDiscovered = -1, // Not discovered
                    TimeFinished = -1 // Not finished
                };
            }
        }

        
        public override void Execute() {
            Initialize();
            
            foreach (INode node in _graph.Nodes) {
                if (Color(node) == "WHITE") {
                    DFSVisit(node);
                }
            }
        }

        private void DFSVisit(INode node) {
            time = time + 1;
            SetTimeDiscovered(node,time);
            SetColor(node, "GRAY"); // Visiting
            
            foreach (var neighbor in _graph.GetNeighbors(node)) {
                if (Color(neighbor) == "WHITE") {
                    DFSVisit(neighbor);
                }
            }
            SetColor(node, "BLACK"); // Finished visiting
            time = time + 1;
            SetTimeFinished(node, time);
        }
    }

    public class DFSDirected : BaseAlgorithm {
        private Dictionary<string, object> _dictData = new Dictionary<string, object>();
        DirectedGraph _graph;
        int time = 0;

        public static string MetadataKey => "DFSDirected";
        public string[] Variables = new string[] { "Graph", "Color", "TimeDiscovered", "TimeFinished", "Time" };

        public DFSDirected() {
        }

        public class DFSDirected_NodeMetaData {
            public string Color; // WHITE, GRAY, BLACK
            public int TimeDiscovered; // Time when the node was discovered
            public int TimeFinished; // Time when the node was finished

            public override string ToString() {
                return $"DFS Directed_NodeMetaData \nColor={Color}\nTimeDiscovered={TimeDiscovered}\nTimeFinished={TimeFinished}";
            }
        }

        public DirectedGraph Graph() {
            return _graph;
        }

        public void SetDirectedGraph(DirectedGraph graph) {
            _graph = graph;
        }

        public static string Color(INode node) {
            return ((DFSDirected_NodeMetaData)node.MetaData[MetadataKey]).Color;
        }

        public void SetColor(INode node, string color) {
            var metaData = (DFSDirected_NodeMetaData)node.MetaData[MetadataKey];
            metaData.Color = color;
        }

        public static int TimeDiscovered(INode node) {
            return ((DFSDirected_NodeMetaData)node.MetaData[MetadataKey]).TimeDiscovered;
        }

        private void SetTimeDiscovered(INode node, int t) {
            var metaData = (DFSDirected_NodeMetaData)node.MetaData[MetadataKey];
            metaData.TimeDiscovered = t;
        }

        public static int TimeFinished(INode node) {
            return ((DFSDirected_NodeMetaData)node.MetaData[MetadataKey]).TimeFinished;
        }

        private void SetTimeFinished(INode node, int t) {
            var metaData = (DFSDirected_NodeMetaData)node.MetaData[MetadataKey];
            metaData.TimeFinished = t;
        }

        public override void Initialize() {
            time = 0;
            foreach (INode node in _graph.Nodes) {
                // Initialize metadata for each node
                node.MetaData[MetadataKey] = new DFSDirected_NodeMetaData() {
                    Color = "WHITE",  // Unvisited
                    TimeDiscovered = -1, // Not discovered
                    TimeFinished = -1 // Not finished
                };
            }
        }

        public override void Execute() {
            Initialize();

            foreach (INode node in _graph.Nodes) {
                if (Color(node) == "WHITE") {
                    DFSVisit(node);
                }
            }
        }

        private void DFSVisit(INode node) {
            time = time + 1;
            SetTimeDiscovered(node, time);
            SetColor(node, "GRAY"); // Visiting

            foreach (var edge in _graph.GetOutgoingEdges(node)) {
                if (Color(edge.Target) == "WHITE") {
                    DFSVisit(edge.Target);
                }
            }

            SetColor(node, "BLACK"); // Finished visiting
            time = time + 1;
            SetTimeFinished(node, time);
        }
    }



    public class BFSUndirected : BaseAlgorithm {
        private Dictionary<string, object> _dictData = new Dictionary<string, object>();
        private UnDirectedGraph _graph;
        private int time = 0;

        public static string MetadataKey => "BFSUndirected";
        public string[] Variables = new string[] { "Graph", "Color", "Distance", "Predecessor", "Time" };

        public BFSUndirected() {
        }

        public class BFSUndirected_NodeMetaData {
            public string Color; // WHITE, GRAY, BLACK
            public int Distance; // Distance from source
            public INode Predecessor; // Previous node in BFS path
        }
        

        public UnDirectedGraph Graph() {
            return _graph;
        }

        public void SetUnDirectedGraph(UnDirectedGraph graph) {
            _graph = graph;
        }

        public string Color(INode node) {
            return ((BFSUndirected_NodeMetaData)node.MetaData[MetadataKey]).Color;
        }

        public void SetColor(INode node, string color) {
            var metaData = (BFSUndirected_NodeMetaData)node.MetaData[MetadataKey];
            metaData.Color = color;
        }

        public int Distance(INode node) {
            return ((BFSUndirected_NodeMetaData)node.MetaData[MetadataKey]).Distance;
        }

        public void SetDistance(INode node, int distance) {
            var metaData = (BFSUndirected_NodeMetaData)node.MetaData[MetadataKey];
            metaData.Distance = distance;
        }

        public INode Predecessor(INode node) {
            return ((BFSUndirected_NodeMetaData)node.MetaData[MetadataKey]).Predecessor;
        }

        public void SetPredecessor(INode node, INode pred) {
            var metaData = (BFSUndirected_NodeMetaData)node.MetaData["BFSUndirected"];
            metaData.Predecessor = pred;
        }

        public override void Initialize() {
            foreach (INode node in _graph.Nodes) {
                node.MetaData[MetadataKey] = new BFSUndirected_NodeMetaData() {
                    Color = "WHITE",
                    Distance = int.MaxValue,
                    Predecessor = null
                };
            }
        }

        public override void Execute() {
            Initialize();
            foreach (INode node in _graph.Nodes) {
                if (Color(node) == "WHITE") {
                    BFSVisit(node);
                }
            }
        }

        private void BFSVisit(INode startNode) {
            SetColor(startNode, "GRAY");
            SetDistance(startNode, 0);
            SetPredecessor(startNode, null);

            var queue = new Queue<INode>();
            queue.Enqueue(startNode);

            while (queue.Count > 0) {
                var node = queue.Dequeue();
                foreach (var neighbor in _graph.GetNeighbors(node)) {
                    if (Color(neighbor) == "WHITE") {
                        SetColor(neighbor, "GRAY");
                        SetDistance(neighbor, Distance(node) + 1);
                        SetPredecessor(neighbor, node);
                        queue.Enqueue(neighbor);
                    }
                }
                SetColor(node, "BLACK");
            }
        }
    }


    public class DistanceTimeTuples : BaseAlgorithm {
        private UnDirectedGraph _graph;
        public static string MetadataKey => "DistanceTimeTuples";
        
        
        public class DistanceTimeTuples_NodeMetaData {
           public (int Distance, int TimeDiscovered) DistanceTimeDiscovered;
           public (int Distance, int TimeFinished) DistanceTimeFinished;
        }

        public DistanceTimeTuples() { }

        public void SetUnDirectedGraph(UnDirectedGraph graph) {
            _graph = graph;
        }

        public override void Initialize() {
            foreach (INode node in _graph.Nodes) {
                node.MetaData[MetadataKey] = new DistanceTimeTuples_NodeMetaData() {
                    DistanceTimeDiscovered = (-1, -1), // Distance, TimeDiscovered
                    DistanceTimeFinished = (-1, -1)      // Distance, TimeFinished
                };
            }
        }

        public void SetDistanceTimeDiscovered(INode node, (int distance, int timeDiscovered) distanceTimeDiscovered) {
            if (!node.MetaData.TryGetValue(MetadataKey, out var metaObj) ||
                metaObj is not DistanceTimeTuples_NodeMetaData metaData)
                throw new InvalidOperationException("DistanceTimeTuples metadata not found for node.");
            metaData.DistanceTimeDiscovered = (distanceTimeDiscovered.Item1, distanceTimeDiscovered.Item2);
        }

        public void SetDistanceTimeFinished(INode node, (int distance, int TimeFinished) distanceTimeFinished) {
            if (!node.MetaData.TryGetValue(MetadataKey, out var metaObj) ||
                metaObj is not DistanceTimeTuples_NodeMetaData metaData)
                throw new InvalidOperationException("DistanceTimeTuples metadata not found for node.");
            metaData.DistanceTimeFinished = (distanceTimeFinished.distance, distanceTimeFinished.TimeFinished);
        }

        public int Distance(INode node) {
            if ( !node.MetaData.TryGetValue("BFSUndirected", out var bfsMetaObj) ||
                 !(bfsMetaObj is BFSUndirected.BFSUndirected_NodeMetaData bfsMetadata))
                throw new InvalidOperationException("BFSUndirected metadata not found for node.");
            return bfsMetadata.Distance;
        }

        public int TimeFinished(INode node) {
            if (!node.MetaData.TryGetValue("DFSUndirected", out var dfsMetaObj) ||
                dfsMetaObj is not DFSUndirected.DFSUndirected_NodeMetaData dfsMetadata)
                throw new InvalidOperationException("DFSUndirected metadata not found for node.");
            return dfsMetadata.TimeFinished;
        }

        public int TimeDiscovered(INode node) {
            if (!node.MetaData.TryGetValue("DFSUndirected", out var dfsMetaObj) ||
                dfsMetaObj is not DFSUndirected.DFSUndirected_NodeMetaData dfsMetadata)
                throw new InvalidOperationException("DFSUndirected metadata not found for node.");
            return dfsMetadata.TimeDiscovered;
        }

        public override void Execute() {
            // Assumes DFSUndirected and BFSUndirected have already been executed on _graph
            foreach (INode node in _graph.Nodes) {
                SetDistanceTimeDiscovered(node,(Distance(node), TimeDiscovered(node)));
                SetDistanceTimeFinished(node,(Distance(node), TimeFinished(node)));
            }
        }
    }



}