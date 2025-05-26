using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunicationNetwork.Graph {

    public interface INode {
        string Name { get; }
        Type Type { get; }
        Dictionary<string, object> MetaData { get; }
        int Serial { get; }
    }

    public interface INode<T> : INode {
        T Value { get; }
    }

    public class Node<T> : INode<T> {
        public T Value { get; set; }
        public string Name { get; }
        public Type Type { get; }
        public Dictionary<string, object> MetaData { get; }

        public int Serial => m_serialNumber;
        public int m_serialNumber;
        static int ms_TnodeCounter = 0;

        public Node() {
            Value = default(T);
            m_serialNumber = ms_TnodeCounter++;
            Name = "Node_"+typeof(T).Name+"_"+m_serialNumber;
            Type = typeof(T);
            MetaData = new Dictionary<string, object>();
        }
    }

    
}
