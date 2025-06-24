using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunicationNetwork.Algorithm {
    public abstract class BaseAlgorithm {
        public object MetadataKey { get; init; }
        public string Name { get; init; }
        public abstract void Execute();
        public abstract void Initialize();
    }
}
