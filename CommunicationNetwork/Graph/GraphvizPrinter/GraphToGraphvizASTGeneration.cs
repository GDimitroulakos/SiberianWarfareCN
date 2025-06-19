using CommunicationNetwork.Graph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace CommunicationNetwork.Graph.GraphvizPrinter {
    public class GraphToGraphvizASTGeneration {
        private GraphvizFileLayout _dotFileAST;
        private List<object> _nodeMetadataKeys = new List<object>();
        private List<object> _graphMetadataKeys = new List<object>();


        public GraphvizFileLayout DotFileAst => _dotFileAST;

        public void AddNodeMetadataKey(object key) {
            if (key == null) throw new ArgumentNullException(nameof(key));
            if (!_nodeMetadataKeys.Contains(key)) {
                _nodeMetadataKeys.Add(key);
            }
        }
        public void AddGraphMetadataKey(object key) {
            if (key == null) throw new ArgumentNullException(nameof(key));
            if (!_graphMetadataKeys.Contains(key)) {
                _graphMetadataKeys.Add(key);
            }
        }
        public void RemoveNodeMetadataKey(object key) {
            if (key == null) throw new ArgumentNullException(nameof(key));
            _nodeMetadataKeys.Remove(key);
        }
        public void RemoveGraphMetadataKey(object key) {
            if (key == null) throw new ArgumentNullException(nameof(key));
            _graphMetadataKeys.Remove(key);
        }

        public void ClearNodeMetadataKeys() {
            _nodeMetadataKeys.Clear();
        }
        public void ClearGraphMetadataKeys() {
            _graphMetadataKeys.Clear();
        }

        public void ToAST(IGraph graph, string dotFileName) {

            CreateGraphvizLayout(graph, dotFileName);

            // Print nodes
            foreach (var node in graph.Nodes) {
                // Create a new GraphvizNode for each node in the graph and add it to the AST
                var newGraphvizNode = CreateNewGraphvizNodeToAST(node);

                // For each node add a properties node as a child
                var newProperties = CreateNodePropertiesAndAttachToNode(newGraphvizNode);

                // Create a label property for the node
                var labelProperty = CreateNodeLabelProperty(newProperties, node);

                //  Augment the label property with metadata if available
                AugmentNodeLabelPropertyWithMetadata(node, labelProperty);
            }

            // Print edges
            foreach (var edge in graph.Edges) {
                var newGraphvicEdge = CreateNewGraphvicEdgeToAST(edge);

                var edgeProperties = CreateEdgePropertiesAndAttachToEdge(newGraphvicEdge);

                CreateEdgeLabelProperty(edgeProperties, edge);
            }
        }

        private static void CreateEdgeLabelProperty(GraphvizEdgeProperties edgeProperties, IEdge edge) {
            // A. Label Property
            GraphvizEdgeProperty labelProperty = new GraphvizEdgeProperty("label");
            edgeProperties.AddChild(GraphvizEdge.ATTRIBUTE_LIST, labelProperty);

            // A1. Add the edge ID as the first value of the label property
            GraphvizEdgePropertyValue labelValue =
                new GraphvizEdgePropertyValue(edge.Serial.ToString());
            labelProperty.AddChild(GraphvizEdgeProperty.PROPERTY_VALUES, labelValue);
        }

        private static GraphvizEdgeProperties CreateEdgePropertiesAndAttachToEdge(GraphvizEdge newGraphvicEdge) {
            // For each edge add a properties node as a child
            GraphvizEdgeProperties edgeProperties = new GraphvizEdgeProperties();
            newGraphvicEdge.AddChild(GraphvizEdge.ATTRIBUTE_LIST, edgeProperties);
            return edgeProperties;
        }

        private GraphvizEdge CreateNewGraphvicEdgeToAST(IEdge edge) {
            int source = edge.Source.Serial;
            int target = edge.Target.Serial;
            // Create a new GraphvizEdge for each edge in the graph and add it to the AST
            var newGraphvicEdge = new GraphvizEdge(source.ToString(), target.ToString());
            _dotFileAST.AddChild(GraphvizFileLayout.EDGE_DEFINITIONS, newGraphvicEdge);
            return newGraphvicEdge;
        }

        private void AugmentNodeLabelPropertyWithMetadata(INode node, GraphvizNodeProperty labelProperty) {
            // A2. Add property values to the label property
            foreach (var key in _nodeMetadataKeys) {
                GraphvizNodePropertyValue newValue =
                    new GraphvizNodePropertyValue(node.MetaData[key].ToString());
                labelProperty.AddChild(GraphvizNodeProperty.PROPERTY_VALUES, newValue);
            }
        }

        private void AugmentGraphLabelPropertyWithMetadata(IGraph graph, GraphvizGraphProperty labelProperty) {
            // A2. Add property values to the label property
            foreach (var key in _graphMetadataKeys) {
                GraphvizGraphPropertyValue newValue =
                    new GraphvizGraphPropertyValue(graph.MetaData[key].ToString());
                labelProperty.AddChild(GraphvizNodeProperty.PROPERTY_VALUES, newValue);
            }
        }

        private static GraphvizNodeProperty CreateNodeLabelProperty(GraphvizNodeProperties newProperties, INode node) {
            // A. Create a label property for the node
            GraphvizNodeProperty labelProperty = new GraphvizNodeProperty("label");
            // Add the label property to the node properties
            newProperties.AddChild(GraphvizNode.ATTRIBUTE_LIST, labelProperty);

            // Create the label property value 
            GraphvizNodePropertyValue labelValue =
                new GraphvizNodePropertyValue(node.Serial.ToString());
            // Add the label value to the label property
            labelProperty.AddChild(GraphvizNodeProperty.PROPERTY_VALUES, labelValue);
            return labelProperty;
        }

        private static GraphvizGraphProperty CreateGraphLabelProperty(GraphvizGraphProperties newProperties, IGraph graph) {
            // A. Create a label property for the node
            GraphvizGraphProperty labelProperty = new GraphvizGraphProperty("label");
            // Add the label property to the node properties
            newProperties.AddChild(GraphvizFileLayout.GLOBAL_ATTRIBUTES, labelProperty);

            // Create the label property value 
            GraphvizGraphPropertyValue labelValue =
                new GraphvizGraphPropertyValue(graph.Serial.ToString());

            // Add the label value to the label property
            labelProperty.AddChild(GraphvizGraphProperty.PROPERTY_VALUES, labelValue);
            return labelProperty;
        }

        private static GraphvizNodeProperties CreateNodePropertiesAndAttachToNode(GraphvizNode newGraphvizNode) {
            // For each node add a properties node as a child
            GraphvizNodeProperties newProperties = new GraphvizNodeProperties();
            // Add the properties node to the new GraphvizNode
            newGraphvizNode.AddChild(GraphvizNode.ATTRIBUTE_LIST, newProperties);
            // return the new properties node to be used later for extending the graph
            return newProperties;
        }

        private static GraphvizGraphProperties CreateGraphPropertiesAndAttachToGraph(
            GraphvizFileLayout graphvizFileLayout) {
            // For each node add a properties node as a child
            GraphvizGraphProperties newProperties = new GraphvizGraphProperties();
            // Add the properties node to the new GraphvizNode
            graphvizFileLayout.AddChild(GraphvizFileLayout.GLOBAL_ATTRIBUTES, newProperties);
            // return the new properties node to be used later for extending the graph
            return newProperties;
        }


        private GraphvizNode CreateNewGraphvizNodeToAST(INode node) {
            // Create a new GraphvizNode for the given node 
            GraphvizNode newGraphvizNode = new GraphvizNode(node.Serial.ToString());
            // Add the new GraphvizNode to the AST
            _dotFileAST.AddChild(GraphvizFileLayout.NODE_DEFINITIONS,
                newGraphvizNode);
            // return the new GraphvizNode to be used later for extending the graph 
            return newGraphvizNode;
        }

        private void CreateGraphvizLayout(IGraph graph, string dotFileName) {
            // Check if the graph is null
            if (graph == null) throw new ArgumentNullException(nameof(graph));
            // Get the filename without extension
            string dotfilename = Path.GetFileNameWithoutExtension(dotFileName);

            // Get the graph type
            GraphvizFileLayout.GRAPHTYPE gtype = GetGraphType(graph);

            // Create the GraphvizFileLayout AST Node
            _dotFileAST = new GraphvizFileLayout(dotfilename, gtype);

            // Add properties to the GraphvizFileLayout
            var newProperties = CreateGraphPropertiesAndAttachToGraph(_dotFileAST);

            // Create a label property for the graph
            var labelProperty = CreateGraphLabelProperty(newProperties,graph);

            // Augment the label property with metadata if available
            AugmentGraphLabelPropertyWithMetadata(graph, labelProperty);

        }

        /// <summary>
        /// Determines the type of the graph (directed or undirected) based on the IGraph interface.
        /// </summary>
        /// <param name="graph"></param>
        /// <returns>The type of graph</returns>
        /// <exception cref="ArgumentException"></exception>
        private static GraphvizFileLayout.GRAPHTYPE GetGraphType(IGraph graph) {
            if (graph is IDirectedGraph) {
                return GraphvizFileLayout.GRAPHTYPE.DIGRAPH;
            } else if (graph is IUndirectedGraph) {
                return GraphvizFileLayout.GRAPHTYPE.GRAPH;
            } else {
                throw new ArgumentException("Graph must be either a directed or undirected graph.");
            }
        }
    }
}
