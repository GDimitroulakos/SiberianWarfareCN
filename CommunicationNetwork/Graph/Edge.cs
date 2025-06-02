using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace CommunicationNetwork.Graph {

    public interface IEdge : IGraphElement {
        INode Source { get; }
        INode Target { get; }
    }

    public interface IEdge<T> :IEdge {
        T Value { get; }
    }

    public class Edge : IEdge {
        public virtual string Name { get; }
        public Dictionary<object, object> MetaData { get; }
        public INode Source { get; }
        public INode Target { get; }
        public Type ElementType { get; }
        public int Serial => m_serialNumber; // Serial number is not needed for edges, as they are not uniquely identified by a serial number
        public readonly int m_serialNumber; // Serial number is not needed for edges, as they are not uniquely identified by a serial number
        private static int ms_TedgeCounter = 0;
        public Edge(INode source, INode target) {
            m_serialNumber = Interlocked.Increment(ref ms_TedgeCounter);
            Name = "Edge" + m_serialNumber;
            MetaData = new Dictionary<object, object>();
            Source = source ?? throw new ArgumentNullException(nameof(source));
            Target = target ?? throw new ArgumentNullException(nameof(target));
            ElementType = typeof(Edge);
        }
    }

    public class Edge<T> : Edge, IEdge<T>{
        public T Value { get; set; }
        public override string Name => "Edge_" + typeof(T).Name + "_" + m_serialNumber;
        public Edge(INode source, INode target) : base(source, target) {
            Value = default(T);
        }

    }
}
