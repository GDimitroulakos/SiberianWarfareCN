using System;
using System.Collections.Generic;
using System.Linq;
using CommunicationNetwork.Algorithm;
using CommunicationNetwork.Algorithms;
using CommunicationNetwork.Graph;

namespace CommunicationNetwork.Algorithms {
    
    
    public class DFS : BaseAlgorithm {
        private Dictionary<string, object> _dictData = new Dictionary<string, object>();
        IGraph _graph;
        int time = 0;
        
        public static string MetadataKey => "DFS";
        public string []Variables = new string[] { "Graph","Color", "TimeDiscovered", "TimeFinished","Time" };

        public DFS() {
        }
        
        public class DFS_NodeMetaData {
            public string Color; // WHITE, GRAY, BLACK
            public int TimeDiscovered; // Time when the node was discovered
            public int TimeFinished; // Time when the node was finished

            public override string ToString() {
                return $"DFS NodeMetaData \nColor={Color}\nTimeDiscovered={TimeDiscovered}\nTimeFinished={TimeFinished}";
            }
        }
        
        public IGraph Graph() {
            return _graph;
        }
        public void SetGraph(IGraph graph) {
            _graph = graph;
        }
        public static string Color(INode node) {
            return ((DFS_NodeMetaData)node.MetaData[MetadataKey]).Color;
        }
        public void SetColor(INode node, string color) {
            var metaData = (DFS_NodeMetaData)node.MetaData[MetadataKey];
            metaData.Color = color;
        }
        public static int TimeDiscovered(INode node) {
           return ((DFS_NodeMetaData)node.MetaData[MetadataKey]).TimeDiscovered;
        }
        private void SetTimeDiscovered(INode node, int t) {
            var metaData = (DFS_NodeMetaData)node.MetaData[MetadataKey];
            metaData.TimeDiscovered = t;
        }
        public static int TimeFinished(INode node) {
            return ((DFS_NodeMetaData)node.MetaData[MetadataKey]).TimeFinished;
        } private void SetTimeFinished(INode node, int t) {
            var metaData = (DFS_NodeMetaData)node.MetaData[MetadataKey];
            metaData.TimeFinished = t;
        }
        
        public override void Initialize() {
            time = 0;
            foreach (INode node in _graph.Nodes) {
                // Initialize metadata for each node
                node.MetaData[MetadataKey] = new DFS_NodeMetaData() {
                    Color = "WHITE",  // Unvisited
                    TimeDiscovered = -1, // Not discovered
                    TimeFinished = -1 // Not finished
                };
            }
        }

        public override void Execute() {
            Initialize();
            if (_graph == null || !_graph.Nodes.Any()) {
                throw new InvalidOperationException("Graph is not set or is empty.");
            }

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

}