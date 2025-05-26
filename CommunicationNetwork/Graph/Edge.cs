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

        static int ms_TedgeCounter = 0;

        public Edge(INode source, INode target) {
            Value = default(T);
            Name = "Edge" + typeof(T).Name + ms_TedgeCounter++;
            Type = typeof(T);
            MetaData = new Dictionary<string, object>();
            Source = source ?? throw new ArgumentNullException();
            Target = target ?? throw new ArgumentNullException();
        }
    }
}
