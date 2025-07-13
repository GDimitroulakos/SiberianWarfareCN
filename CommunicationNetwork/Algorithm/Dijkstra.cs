using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunicationNetwork.Graph;

namespace CommunicationNetwork.Algorithm {
    public class Dijkstra : BaseAlgorithm {

        IGraph _graph;
        public object MetadataKey { get; init; }

        public Dijkstra(string name) : base(name) {
        }

        public override void Execute() {
            throw new NotImplementedException();
        }

        public override void Initialize() {
            throw new NotImplementedException();
        }
    }
}
