using System;
using System.Collections.Generic;
using System.Linq;
using CommunicationNetwork.Algorithms;
using CommunicationNetwork.Graph;

namespace CommunicationNetwork.Algorithms {

    public abstract class BaseAlgorithm{
        Dictionary<object, object> _metaData = new Dictionary<object, object>();
        Dictionary<object,IGraph> _inputGraphs = new Dictionary<object, IGraph>();
        Dictionary<object,IGraph> _outputGraphs = new Dictionary<object, IGraph>();


        public void AddMapping(string parameter, object key) {
            if ( parameter == null || key == null) {
                throw new ArgumentNullException("Parameter and key cannot be null.");
            }
            if (!_metaData.ContainsKey(parameter)) {
                _metaData[parameter] = key;
            } else {
                throw new ArgumentException($"Parameter '{parameter}' already exists.");
            }
        }
        public object GetMapping(string parameter) {
            if (parameter == null) {
                throw new ArgumentNullException("Parameter cannot be null.");
            }
            if (_metaData.ContainsKey(parameter)) {
                return _metaData[parameter];
            } else {
                throw new KeyNotFoundException($"Parameter '{parameter}' not found.");
            }
        }

        public void AddInputGraph(string name, IGraph graph) {
            if (name == null || graph == null) {
                throw new ArgumentNullException("Name and graph cannot be null.");
            }
            if (!_inputGraphs.ContainsKey(name)) {
                _inputGraphs[name] = graph;
            } else {
                throw new ArgumentException($"Input graph '{name}' already exists.");
            }
        }
        public IGraph GetInputGraph(string name) {
            if (name == null) {
                throw new ArgumentNullException("Name cannot be null.");
            }
            if (_inputGraphs.ContainsKey(name)) {
                return _inputGraphs[name];
            } else {
                throw new KeyNotFoundException($"Input graph '{name}' not found.");
            }
        }

        public void AddOutputGraph(string name, IGraph graph) {
            if (name == null || graph == null) {
                throw new ArgumentNullException("Name and graph cannot be null.");
            }
            if (!_outputGraphs.ContainsKey(name)) {
                _outputGraphs[name] = graph;
            } else {
                throw new ArgumentException($"Output graph '{name}' already exists.");
            }
        }

        public IGraph GetOutputGraph(string name) {
            if (name == null) {
                throw new ArgumentNullException("Name cannot be null.");
            }
            if (_outputGraphs.ContainsKey(name)) {
                return _outputGraphs[name];
            } else {
                throw new KeyNotFoundException($"Output graph '{name}' not found.");
            }
        }

        public string Name { get; }
       
       public abstract void Execute();
    }

    public class DFSUndirected : BaseAlgorithm {
        
        UnDirectedGraph _graph;
        int time = 0;

        public string []Input = new string[] { "Graph","Color", "TimeDiscovered", "TimeFinished" };

        public DFSUndirected() {
            AddMapping("Color", "DFSUndirected");
            AddMapping("TimeDiscovered", "DFSUndirected");
            AddMapping("TimeFinished", "DFSUndirected");
        }

        public class DFSUndirected_NodeMetaData {
            public string Color; // WHITE, GRAY, BLACK
            public int TimeDiscovered; // Time when the node was discovered
            public int TimeFinished; // Time when the node was finished
        }
        
        private string Color(INode node) {
            return ((DFSUndirected_NodeMetaData)node.MetaData[GetMapping("Color")]).Color;
        }
        private void SetColor(INode node, string color) {
            var metaData = (DFSUndirected_NodeMetaData)node.MetaData[GetMapping("Color")];
            metaData.Color = color;
        }
        public int TimeDiscovered(INode node) {
            return ((DFSUndirected_NodeMetaData)node.MetaData["DFSUndirected"]).TimeDiscovered;
        }
        private void SetTimeDiscovered(INode node, int t) {
            var metaData = (DFSUndirected_NodeMetaData)node.MetaData["DFSUndirected"];
            metaData.TimeDiscovered = t;
        }
        public int TimeFinished(INode node) {
            return ((DFSUndirected_NodeMetaData)node.MetaData["DFSUndirected"]).TimeFinished;
        } private void SetTimeFinished(INode node, int t) {
            var metaData = (DFSUndirected_NodeMetaData)node.MetaData["DFSUndirected"];
            metaData.TimeFinished = t;
        }


        
        public override void Execute() {
            time = 0;
            _graph = (UnDirectedGraph)GetInputGraph("Graph");
            foreach (INode node in _graph.Nodes) {
                // Initialize metadata for each node
                node.MetaData["DFSUndirected"] = new DFSUndirected_NodeMetaData() {
                    Color = "WHITE",  // Unvisited
                    TimeDiscovered = -1, // Not discovered
                    TimeFinished = -1 // Not finished
                };
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
    public class DFSUndirectedWithDuration : DFSUndirected {
        UnDirectedGraph _graph;
        public DFSUndirectedWithDuration(UnDirectedGraph graph) {
            _graph = graph;
            AddMapping("Duration", "DFSUndirectedWithDuration");

            foreach (INode node in _graph.Nodes) {
                // Initialize duration metadata for each node
                node.MetaData["DFSUndirectedWithDuration"] = new DFSUndirectedWithDuration_NodeMetaData() {
                    Duration = -1
                };
            }
        }

        public struct DFSUndirectedWithDuration_NodeMetaData {
            public int Duration; // TimeFinished - TimeDiscovered
        }

        public override void Execute() {
            base.Execute();
            foreach (INode node in _graph.Nodes) {
                int discovered = TimeDiscovered(node);
                int finished = TimeFinished(node);
                int duration = (discovered >= 0 && finished >= 0) ? (finished - discovered) : -1;
                SetDuration(node, duration);
            }
        }

        public int Duration(INode node) {
            return ((DFSUndirectedWithDuration_NodeMetaData)node.MetaData["DFSUndirectedWithDuration"]).Duration;
        }

        private void SetDuration(INode node, int duration) {
            var metaData = (DFSUndirectedWithDuration_NodeMetaData)node.MetaData["DFSUndirectedWithDuration"];
            metaData.Duration = duration;
            node.MetaData["DFSUndirectedWithDuration"] = metaData;
        }
    }
    

    



}