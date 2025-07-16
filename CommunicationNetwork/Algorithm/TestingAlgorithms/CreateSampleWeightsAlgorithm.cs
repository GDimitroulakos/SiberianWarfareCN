using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunicationNetwork.Graph;

namespace CommunicationNetwork.Algorithm.TestingAlgorithms {
    public class CreateSampleWeightsAlgorithm: BaseAlgorithm, IDataProvider {
        protected Dictionary<string, object> _outputDataLinks = new Dictionary<string, object>();
        IGraph _graph;
        
        // Input data Metadata keys
        // None
        public readonly string K_INPUTGRAPH = "INPUTGRAPH";

        // Output data Metadata keys
        public readonly string K_WEIGHT = "WEIGHT";

        public void SetGraph(IGraph graph) {
            _graph = graph;
        }

        public CreateSampleWeightsAlgorithm() : base() {
            // Initialize the output data links
            _outputDataLinks["WEIGHT"] = K_WEIGHT;
        }

        public double Weight(Edge edge) {
            if (edge.MetaData.TryGetValue(K_WEIGHT, out var weight)) {
                return (double)weight;
            }
            throw new InvalidOperationException($"Weight metadata not found for edge {edge.ID}.");
        }
        public void SetWeight(Edge edge, double weight) {
            edge.MetaData[K_WEIGHT] = weight;
        }

        public override void Execute() {
            Initialize();

            if (_graph == null)
                throw new InvalidOperationException("Graph is not set.");

            var random = new Random();
            foreach (var edge in _graph.Edges) {
                SetWeight(edge,random.Next(1, 11)); // 1 to 10 inclusive
            }
        }

        public override void Initialize() {
            
        }

        public object GetDatakey(string key) {
            if (_outputDataLinks.TryGetValue(key, out var value)) {
                return value;
            }
            throw new KeyNotFoundException($"Key '{key}' not found in output data links.");
        }
    }
}
