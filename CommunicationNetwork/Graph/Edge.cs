using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace CommunicationNetwork.Graph {

    public class Edge : IGraphElement {
        public virtual string ID { get; }
        public Dictionary<object, object> MetaData { get; }
        public Node Source { get; }
        public Node Target { get; }
        public Type ElementType { get; }
        public int Serial => m_serialNumber; // Serial number is not needed for edges, as they are not uniquely identified by a serial number
        public readonly int m_serialNumber; // Serial number is not needed for edges, as they are not uniquely identified by a serial number
        private static int ms_TedgeCounter = 0;
        public Edge(Node source, Node target) {
            m_serialNumber = Interlocked.Increment(ref ms_TedgeCounter);
            ID = "Edge" + m_serialNumber;
            MetaData = new Dictionary<object, object>();
            Source = source ?? throw new ArgumentNullException(nameof(source));
            Target = target ?? throw new ArgumentNullException(nameof(target));
            ElementType = typeof(Edge);
        }
    }
    
}
