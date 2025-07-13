using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace CommunicationNetwork.Graph {
    
    public class Node : IGraphElement {
        public virtual string ID { get; }
        public Type ElementType { get; }
        public Dictionary<object, object> MetaData { get; }
        public int Serial => m_serialNumber;
        
        public readonly int m_serialNumber;
        private static int ms_TnodeCounter = 0;
        
        public Node() {
            m_serialNumber = Interlocked.Increment(ref ms_TnodeCounter);
            ID = "Node_" + m_serialNumber;
            MetaData = new Dictionary<object, object>();
            ElementType = typeof(Node);
        }
    }
}
