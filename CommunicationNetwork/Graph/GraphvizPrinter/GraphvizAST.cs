using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using CommunicationNetwork.Graph.GraphvizPrinter;


namespace CommunicationNetwork.Graph.GraphvizPrinter {
    public abstract class ASTNode {
        public int Serial { get; }

        public string Type { get; set; }

        static int serialCounter = 0;

        public ASTNode(string type) {
            Type = type;
            Serial = serialCounter++;
        }

        public abstract void Accept(IGraphvizASTVisitor visitor);

    }

    public abstract class ASTComposite : ASTNode {
        public Dictionary<int, List<ASTNode>> Children { get; set; }
        public ASTComposite( string type) : base(type) {
            Children = new Dictionary<int, List<ASTNode>>();
        }
        public void AddChild(int context, ASTNode child) {
            if (!Children.ContainsKey(context)) {
                Children[context] = new List<ASTNode>();
            }
            Children[context].Add(child);
        }
    }

    public abstract class ASTLeaf : ASTNode {
        public ASTLeaf( string type) : base(type) {
        }
    }

    public class GraphvizFileLayout : ASTComposite {

        public enum GRAPHTYPE {
            DIGRAPH,
            GRAPH
        }
        private GRAPHTYPE _graphType;
        public GRAPHTYPE GraphType => _graphType;

        private string _graphName;
        public string GraphName => _graphName;


        public const int GLOBAL_ATTRIBUTES = 0, NODE_DEFINITIONS = 1,
            EDGE_DEFINITIONS = 2, SUBGRAPH_DEFINITIONS = 3;

        
        public GraphvizFileLayout(string name, GRAPHTYPE graphType) :
            base("GraphvizFileLayout") {
            _graphType = graphType;
            _graphName = name;
        }

        public override void Accept(IGraphvizASTVisitor visitor) {
            visitor.Visit(this);
        }
    }

    public class GraphvizNode : ASTComposite {
        string _nodeID;
        public string NodeID => _nodeID;

        public const int ATTRIBUTE_LIST = 0;

        public GraphvizNode(string nodeID) : base("GraphvizNode") {
            _nodeID = nodeID;
        }

        public override void Accept(IGraphvizASTVisitor visitor) {
            visitor.Visit(this);
        }
    }

    public class GraphvizNodeProperties : ASTComposite {
        
        public const int PROPERTIES = 0;

        public GraphvizNodeProperties() : base("GraphvizNodeProperties") {
        }
        public override void Accept(IGraphvizASTVisitor visitor) {
            visitor.Visit(this);
        }
    }

    public class GraphvizEdgeProperties : ASTComposite {

        public const int PROPERTIES = 0;

        public GraphvizEdgeProperties() : base("GraphvizEdgeProperties") {
        }
        public override void Accept(IGraphvizASTVisitor visitor) {
            visitor.Visit(this);
        }
    }

    public class GraphvizEdgeProperty : ASTComposite {
        string _propertyName;
        public string PropertyName => _propertyName;
        public const int PROPERTY_VALUES = 0;

        public GraphvizEdgeProperty(string propertyName) :
            base("GraphvizEdgeProperty") {
            _propertyName = propertyName;
        }
        public override void Accept(IGraphvizASTVisitor visitor) {
            visitor.Visit(this);
        }
    }

    public class GraphvizEdgePropertyValue : ASTLeaf {
        private string _propertyValue;
        public string PropertyValue => _propertyValue;
        public GraphvizEdgePropertyValue(string propertyValue) :
            base("GraphvizEdgePropertyValue") {
            _propertyValue = propertyValue;
        }
        public override void Accept(IGraphvizASTVisitor visitor) {
            visitor.Visit(this);
        }
    }

    public class GraphvizNodeProperty : ASTComposite {
        string _propertyName;
        public string PropertyName => _propertyName;

        public const int PROPERTY_VALUES = 0;

        public GraphvizNodeProperty(string propertyName) :
            base("GraphvizNodeProperty") {
            _propertyName = propertyName;
        }
        public override void Accept(IGraphvizASTVisitor visitor) {
            visitor.Visit(this);
        }
    }

    public class GraphvizNodePropertyValue : ASTLeaf {
        private string _propertyValue;
        public string PropertyValue => _propertyValue;
        public GraphvizNodePropertyValue(string propertyValue) :
            base("GraphvizNodePropertyValue") {
            _propertyValue = propertyValue;
        }
        public override void Accept(IGraphvizASTVisitor visitor) {
            visitor.Visit(this);
        }
    }
    
    public class GraphvizEdge : ASTComposite {
        string _sourceNodeID;
        string _targetNodeID;
        public string SourceNodeID => _sourceNodeID;
        public string TargetNodeID => _targetNodeID;
        public const int ATTRIBUTE_LIST = 0;
        public GraphvizEdge(string sourceNodeID, string targetNodeID) :
            base("GraphvizEdge") {
            _sourceNodeID = sourceNodeID;
            _targetNodeID = targetNodeID;
        }
        public override void Accept(IGraphvizASTVisitor visitor) {
            visitor.Visit(this);
        }
    }
}
