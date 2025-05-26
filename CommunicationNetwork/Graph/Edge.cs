using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunicationNetwork.Graph {
    public interface IEdge {
        string Name { get; }
        Type Type { get; }
        Dictionary<string, object> MetaData { get; }
        INode Source { get; }
        INode Target { get; }
        int Serial { get; } // Serial number is not needed for edges, as they are not uniquely identified by a serial number
    }

    public interface IEdge<T> : IEdge {
        T Value { get; }
    }

    public class Edge<T> : IEdge<T>{
        public T Value { get; set; }
        public string Name { get; }
        public Type Type { get; }
        public Dictionary<string, object> MetaData { get; }
        public INode Source { get; }
        public INode Target { get; }
        public int Serial => m_serialNumber; // Serial number is not needed for edges, as they are not uniquely identified by a serial number

        public int m_serialNumber; // Serial number is not needed for edges, as they are not uniquely identified by a serial number
        static int ms_TedgeCounter = 0;

        public Edge(INode source, INode target) {
            Value = default(T);
            m_serialNumber = ms_TedgeCounter++;
            Name = "Edge" + typeof(T).Name + m_serialNumber;
            Type = typeof(T);
            MetaData = new Dictionary<string, object>();
            Source = source ?? throw new ArgumentNullException();
            Target = target ?? throw new ArgumentNullException();

        }
    }
}
