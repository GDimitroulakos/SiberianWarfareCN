using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunicationNetwork.Algorithms;
using CommunicationNetwork.Graph;

namespace CommunicationNetwork.Algorithm {
    public class TopologicalSort :BaseAlgorithm, IDataConsumer,IDataProvider {
        IGraph graph;
        protected Dictionary<string, object> _outputDataLinks = new Dictionary<string, object>();
        public TopologicalSort(string name) : base(name) {
            this.graph = graph;
            // Initialize the output data links
            _outputDataLinks["TOPOLOGICAL_ORDER"] = K_TOPOLOGICALORDER;
        }

        public void SetGraph(IGraph graph) {
            this.graph = graph;
        }

        public object GetDatakey(string key) {
            if (_outputDataLinks.TryGetValue(key, out var value)) {
                return value;
            }
            throw new KeyNotFoundException($"Key '{key}' not found in output data links.");
        }

        public void RegisterInput(IGraph graph, string inputkey, 
            IDataProvider provider, string PublicProviderkey) {
            if (inputkey == "K_DFSFINISHEDTIMES") {
                K_DFSFINISHEDTIMES =provider.GetDatakey(PublicProviderkey) as string;
            }
        }

        // Input data Metadata keys
        public string K_DFSFINISHEDTIMES;

        // Output data Metadata keys
        public readonly string K_TOPOLOGICALORDER = "TOPOLOGICAL_ORDER";


        public override void Execute() {
            Initialize();
            List<Node> sortedNodes = new List<Node>();
            sortedNodes = graph.Nodes.OrderBy(node => node.MetaData[K_DFSFINISHEDTIMES] as int?)
                .ToList();
        }

        public override void Initialize() {
            
        }
    }
}
