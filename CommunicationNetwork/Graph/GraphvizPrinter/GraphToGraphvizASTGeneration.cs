using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunicationNetwork.Graph.GraphvizPrinter {
    public class GraphToGraphvizASTGeneration {
        private GraphvizFileLayout _dotFileAST;
        private List<object> _nodeMetadataKeys = new List<object>();

        public GraphvizFileLayout DotFileAst => _dotFileAST;

        public void AddNodeMetadataKey(object key) {
            if (key == null) throw new ArgumentNullException(nameof(key));
            if (!_nodeMetadataKeys.Contains(key)) {
                _nodeMetadataKeys.Add(key);
            }
        }
        public void RemoveNodeMetadataKey(object key) {
            if (key == null) throw new ArgumentNullException(nameof(key));
            _nodeMetadataKeys.Remove(key);
        }

        public void ClearNodeMetadataKeys() {
            _nodeMetadataKeys.Clear();
        }

        public void ToAST(IDirectedGraph graph, string dotFileName) {
            if (graph == null) throw new ArgumentNullException(nameof(graph));

            string dotfilename = System.IO.Path.GetFileNameWithoutExtension(dotFileName);
            _dotFileAST = new GraphvizFileLayout(dotfilename, "digraph");
            
            // Print nodes
            foreach (var node in graph.Nodes) {
                // Create a new GraphvizNode for each node in the graph and add it to the AST
                GraphvizNode newGraphvizNode = new GraphvizNode(node.Serial.ToString());
                _dotFileAST.AddChild(GraphvizFileLayout.NODE_DEFINITIONS,
                    newGraphvizNode);

                // For each node add a properties node as a child
                GraphvizNodeProperties newProperties = new GraphvizNodeProperties();
                newGraphvizNode.AddChild(GraphvizNode.ATTRIBUTE_LIST, newProperties);


                // A. Label Property
                GraphvizNodeProperty labelProperty = new GraphvizNodeProperty("label");
                newProperties.AddChild(GraphvizNode.ATTRIBUTE_LIST, labelProperty);

                // A1. Add the node ID as the first value of the label property
                GraphvizNodePropertyValue labelValue =
                    new GraphvizNodePropertyValue(node.Serial.ToString());
                labelProperty.AddChild(GraphvizNodeProperty.PROPERTY_VALUES, labelValue);

                // A2. Add property values to the label property
                foreach (var key in _nodeMetadataKeys) {
                    GraphvizNodePropertyValue newValue =
                        new GraphvizNodePropertyValue(node.MetaData[key].ToString());
                    labelProperty.AddChild(GraphvizNodeProperty.PROPERTY_VALUES, newValue);
                }
            }

            // Print edges
            foreach (var edge in graph.Edges) {
                int source = edge.Source.Serial;
                int target = edge.Target.Serial;
                var edgeNode = new GraphvizEdge(source.ToString(), target.ToString());
                _dotFileAST.AddChild(GraphvizFileLayout.EDGE_DEFINITIONS, edgeNode);
            }
        }
    }
}
