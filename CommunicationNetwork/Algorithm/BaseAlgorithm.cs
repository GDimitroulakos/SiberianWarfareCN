using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunicationNetwork.Algorithm {
    public abstract class BaseAlgorithm {
        public string Name { get; }
        public abstract void Execute();
        public abstract void Initialize();
    }
}
