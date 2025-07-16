using System;
using System.Collections.Generic;
using System.Linq;
using CommunicationNetwork.Algorithm;
using CommunicationNetwork.Algorithms;
using CommunicationNetwork.Graph;

namespace CommunicationNetwork.Algorithms {


    public class DFS : BaseAlgorithm, IDataProvider {
        protected Dictionary<string, object> _outputDataLinks = new Dictionary<string, object>();

        IGraph _graph;
        int time = 0;

        public IGraph Graph() {
            return _graph;
        }
        public void SetGraph(IGraph graph) {
            _graph = graph;
        }

        // Input data Metadata keys
        // None


        // Output data Metadata keys
        public readonly string K_COLOR = "COLOR";
        public readonly string K_TIMEDISCOVERY = "TIME_DISCOVERED";
        public readonly string K_TIMEFINISHED = "TIME_FINISHED";


        public DFS(string name):base() {
            // Initialize the output data links
            _outputDataLinks["COLOR"] = K_COLOR;
            _outputDataLinks["TIME_DISCOVERED"] = K_TIMEDISCOVERY;
            _outputDataLinks["TIME_FINISHED"] = K_TIMEFINISHED;
        }
        
        public void SetColor(Node node, string color) {
            node.MetaData[K_COLOR] = color;
        }
        public string Color(Node node) {
            if (node.MetaData.TryGetValue(K_COLOR, out var color)) {
                return color as string;
            }
            throw new InvalidOperationException($"Color metadata not found for node {node.ID}.");
        }
        public void SetTimeDiscovered(Node node, int time) {
            node.MetaData[K_TIMEDISCOVERY] = time;
        }
        public int TimeDiscovered(Node node) {
            if (node.MetaData.TryGetValue(K_TIMEDISCOVERY, out var time)) {
                return (int)time;
            }
            throw new InvalidOperationException($"TimeDiscovered metadata not found for node {node.ID}.");
        }
        public void SetTimeFinished(Node node, int time) {
            node.MetaData[K_TIMEFINISHED] = time;
        }
        public int TimeFinished(Node node) {
            if (node.MetaData.TryGetValue(K_TIMEFINISHED, out var time)) {
                return (int)time;
            }
            throw new InvalidOperationException($"TimeFinished metadata not found for node {node.ID}.");
        }
        
        public object GetDatakey(string key) {
            if (_outputDataLinks.TryGetValue(key, out var value)) {
                return value;
            }
            throw new KeyNotFoundException($"Data key '{key}' not found in DFS algorithm.");
        }

        public override void Initialize() {
            time = 0;
            foreach (Node node in _graph.Nodes) {
                // Initialize metadata for each node
                SetColor(node,"WHITE");
                SetTimeDiscovered(node,-1);
                SetTimeFinished(node, -1);
            }
        }

        public override void Execute() {
            Initialize();
            if (_graph == null || !_graph.Nodes.Any()) {
                throw new InvalidOperationException("Graph is not set or is empty.");
            }

            foreach (Node node in _graph.Nodes) {
                if (Color(node) == "WHITE") {
                    DFSVisit(node);
                }
            }
        }

        private void DFSVisit(Node node) {
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