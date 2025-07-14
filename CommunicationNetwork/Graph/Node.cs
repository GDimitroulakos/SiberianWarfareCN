using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace CommunicationNetwork.Graph {

    public interface INode : IGraphElement{
        public Dictionary<object, object> MetaData { get; }
	}

    public interface INode<T> : INode {
        T Value { get; }
    }

    public abstract class Node : INode {
        public virtual string Name { get; }
        public Type ElementType { get; }
        public Dictionary<object, object> MetaData { get; }
        public int Serial => m_serialNumber;
        public readonly int m_serialNumber;
        private static int ms_TnodeCounter = 0;

		public Node() {
            m_serialNumber = Interlocked.Increment(ref ms_TnodeCounter);
            Name = "Node_" + m_serialNumber;
            MetaData = new Dictionary<object, object>();
            ElementType = typeof(Node);
        }

		public abstract void Trasmit(Packet packet);
	}

    public abstract class Node<T> : Node ,INode<T>{
        public T Value { get; set; }
        public override string Name => "Node_" + typeof(T).Name + "_" + m_serialNumber;
        public Node() : base() {
            Value = default(T);
        }
    }
}
