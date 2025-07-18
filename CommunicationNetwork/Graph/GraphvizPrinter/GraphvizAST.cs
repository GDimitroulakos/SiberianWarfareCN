using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml.Linq;

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


    // Graphviz File Layout class represents the structure of a Graphviz file (.dot)
    // It contains 4 sections 
    // 1. Properties refers to global properties of the graph that are deposited in the
    // first section of the file.
    // 2. Node Definitions refers to the nodes of the graph, which are defined in the second section.
    // Nodes are followed by their attributes enclosed in square brackets.
    // 3. Edge Definitions refers to the edges of the graph, which are defined in the third section.
    // Edges are defined by the source and target nodes, and can also have attributes that follow
    // the edge definition.
    // 4. Subgraph Definitions refers to subgraphs, which are defined in the fourth section.
    // 5. Default Node Attributes refers to the default attributes that are applied to all nodes in the graph.
    // 6. Default Edge Attributes refers to the default attributes that are applied to all edges in the graph.
    public class GraphvizFileLayout : ASTComposite {

        public enum GRAPHTYPE {
            DIGRAPH,
            GRAPH
        }
        private GRAPHTYPE _graphType;
        public GRAPHTYPE GraphType => _graphType;

        private string _graphName;
        public string GraphName => _graphName;


        public const int GRAPH_LEVEL_ATTRIBUTES = 0,
                            DEFAULT_NODE_ATTRIBUTES = 1,
                            DEFAULT_EDGE_ATTRIBUTES = 2, 
                            NODE_DECLARATIONS = 3,
                            EDGE_DECLARATIONS = 4,
                            SUBGRAPH_DECLARATIONS = 5;

        
        public GraphvizFileLayout(string name, GRAPHTYPE graphType) :
            base("GraphvizFileLayout") {
            _graphType = graphType;
            _graphName = name;
        }

        public override void Accept(IGraphvizASTVisitor visitor) {
            visitor.Visit(this);
        }
    }

    // Graphviz Node represents a Graphviz Node Definition in the AST. Graphviz node
    // has one type of children, which is physically a list of attributes that is 
    // enclosed in square brackets in the Graphviz file.
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


    // Graphviz Edge represents a Graphviz Edge Definition in the AST. Graphviz edge
    // has one type of children, which is physically a list of attribute collections
    // Normally the attributes are placed in a single GraphvizProperties node.
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


    // GraphvizProperties represents a collection of properties that can be applied
    // to a Graphviz Node or Edge. Since the syntax of Node and Edge properties
    // is the same, this class is used for both GraphvizNode and GraphvizEdge.
    public class GraphvizProperties : ASTComposite {
        
        public const int PROPERTIES = 0;

        public GraphvizProperties() : base("GraphvizNodeProperties") {
        }
        public override void Accept(IGraphvizASTVisitor visitor) {
            visitor.Visit(this);
        }
    }

    // GraphvizProperty represents a single property in a collection of properties.
    // The property may have one or more values, which are represented by
    // GraphvizPropertyValue leaf nodes. The property name is stored in the PropertyName
    // For example, the graphviz property "label" represents literal text that is printed
    // aside of the node or edge in the Graphviz graph and could contain multiple values
    // originating from different attributes that the node or edge has.
    public class GraphvizProperty : ASTComposite {
        string _propertyName;
        public string PropertyName => _propertyName;
        public const int PROPERTY_VALUES = 0;

        public GraphvizProperty(string propertyName) :
            base("GraphvizEdgeProperty") {
            _propertyName = propertyName;
        }
        public override void Accept(IGraphvizASTVisitor visitor) {
            visitor.Visit(this);
        }
    }

    // GraphvizPropertyValue represents a single value of a property
    public class GraphvizPropertyValue : ASTLeaf {
        private string _propertyValue;
        public string PropertyValue => _propertyValue;
        public GraphvizPropertyValue(string propertyValue) :
            base("GraphvizPropertyValue") {
            _propertyValue = propertyValue;
        }
        public override void Accept(IGraphvizASTVisitor visitor) {
            visitor.Visit(this);
        }
    }
}
