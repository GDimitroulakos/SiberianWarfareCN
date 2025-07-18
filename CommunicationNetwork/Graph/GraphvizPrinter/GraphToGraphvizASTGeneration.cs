using CommunicationNetwork.Graph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using CommunicationNetwork.Algorithm;

namespace CommunicationNetwork.Graph.GraphvizPrinter {
    public class GraphToGraphvizASTGeneration {
        private GraphvizFileLayout _dotFileAST;
        private List<InfoKey> _nodeMetadataKeys = new List<InfoKey>();
        private List<InfoKey> _graphMetadataKeys = new List<InfoKey>();
        private List<InfoKey> _edgeMetadataKeys = new List<InfoKey>();

        public GraphvizFileLayout DotFileAst => _dotFileAST;

        public void AddNodeMetadataKey(InfoKey key) {
            if (key == null) throw new ArgumentNullException(nameof(key));
            if (!_nodeMetadataKeys.Contains(key)) {
                _nodeMetadataKeys.Add(key);
            }
        }
        public void AddEdgeMetadataKey(InfoKey key) {
            if (key == null) throw new ArgumentNullException(nameof(key));
            if (!_edgeMetadataKeys.Contains(key)) {
                _edgeMetadataKeys.Add(key);
            }
        }
        public void AddGraphMetadataKey(InfoKey key) {
            if (key == null) throw new ArgumentNullException(nameof(key));
            if (!_graphMetadataKeys.Contains(key)) {
                _graphMetadataKeys.Add(key);
            }
        }
        public void RemoveNodeMetadataKey(InfoKey key) {
            if (key == null) throw new ArgumentNullException(nameof(key));
            _nodeMetadataKeys.Remove(key);
        }
        public void RemoveGraphMetadataKey(InfoKey key) {
            if (key == null) throw new ArgumentNullException(nameof(key));
            _graphMetadataKeys.Remove(key);
        }
        public void RemoveEdgeMetadataKey(InfoKey key) {
            if (key == null) throw new ArgumentNullException(nameof(key));
            _edgeMetadataKeys.Remove(key);
        }

        public void ClearNodeMetadataKeys() {
            _nodeMetadataKeys.Clear();
        }
        public void ClearGraphMetadataKeys() {
            _graphMetadataKeys.Clear();
        }
        public void ClearEdgeMetadataKeys() {
            _edgeMetadataKeys.Clear();
        }

        public void ToAST(IGraph graph, string dotFileName) {

            CreateGraphvizLayout(graph, dotFileName);

            // This loop creates the Graphviz AST subgraph that refers to the nodes declarations
            // and their properties.
            foreach (var node in graph.Nodes) {
                // Create a new GraphvizNode for each node in the graph and add it to the 
                // GraphvizFileLayout AST Node
                var newGraphvizNode = CreateNewGraphvizNodeToAST(node);

                // For each node add a properties node as a child of the GraphvizNode
                var newProperties = CreatePropertiesAndAttach(newGraphvizNode,
                    GraphvizNode.ATTRIBUTE_LIST);

                // Create a label property for the node and attach it to the GraphvizProperties
                // node that was created in the previous step ( newProperties )
                var labelProperty = CreateLabelProperty(newProperties, node);

                //  For each registered metadata key, add to the GraphvizProperty node 
                //  the corresponding GraphvizPropertyValue
                AugmentNodeLabelPropertyWithMetadata(node, labelProperty);
            }

            // Print edges
            foreach (var edge in graph.Edges) {
                // Create a new GraphvizEdge for each edge in the graph and add it to the AST
                var newGraphvicEdge = CreateNewGraphvicEdgeToAST(edge);

                // For each edge add a properties node as a child
                var edgeProperties = CreatePropertiesAndAttach(newGraphvicEdge,
                    GraphvizEdge.ATTRIBUTE_LIST);

                // Create a label property for the edge
                var labelProperty = CreateLabelProperty(edgeProperties, edge);

                // Augment the label property with metadata if available
                AugmentEdgeLabelPropertyWithMetadata(edge, labelProperty);



            }
        }
        
        private static GraphvizProperty CreateLabelProperty(GraphvizProperties newProperties, IGraphElement element) {
            // A. Create a label property for the node
            GraphvizProperty labelProperty = new GraphvizProperty("label");
            // Add the label property to the node properties
            newProperties.AddChild(GraphvizFileLayout.GRAPH_LEVEL_ATTRIBUTES, labelProperty);

            // Create the label property value 
            GraphvizPropertyValue labelValue =
                new GraphvizPropertyValue(element.Serial.ToString());

            // Add the label value to the label property
            labelProperty.AddChild(GraphvizProperty.PROPERTY_VALUES, labelValue);
            return labelProperty;
        }


        private GraphvizEdge CreateNewGraphvicEdgeToAST(Edge edge) {
            int source = edge.Source.Serial;
            int target = edge.Target.Serial;
            // Create a new GraphvizEdge for each edge in the graph and add it to the AST
            var newGraphvicEdge = new GraphvizEdge(source.ToString(), target.ToString());
            _dotFileAST.AddChild(GraphvizFileLayout.EDGE_DECLARATIONS, newGraphvicEdge);
            return newGraphvicEdge;
        }

        private void AugmentNodeLabelPropertyWithMetadata(Node node, GraphvizProperty labelProperty) {
            // A2. Add property values to the label property
            foreach (InfoKey key in _nodeMetadataKeys) {
                GraphvizPropertyValue newValue =
                    new GraphvizPropertyValue($"({key.InfoOwner}):{key.AttributeKeyID})={node.MetaData[key.AttributeKeyID].ToString()}");
                labelProperty.AddChild(GraphvizProperty.PROPERTY_VALUES, newValue);
            }
        }

        private void AugmentEdgeLabelPropertyWithMetadata(Edge edge, GraphvizProperty labelProperty) {
            // A2. Add property values to the label property
            foreach (var key in _edgeMetadataKeys) {
                GraphvizPropertyValue newValue =
                    new GraphvizPropertyValue(edge.MetaData[key].ToString());
                labelProperty.AddChild(GraphvizProperty.PROPERTY_VALUES, newValue);
            }
        }

        private void AugmentGraphLabelPropertyWithMetadata(IGraph graph, GraphvizProperty labelProperty) {
            // A2. Add property values to the label property
            foreach (var key in _graphMetadataKeys) {
                GraphvizPropertyValue newValue =
                    new GraphvizPropertyValue(graph.MetaData[key].ToString());
                labelProperty.AddChild(GraphvizProperty.PROPERTY_VALUES, newValue);
            }
        }

        private static GraphvizProperties CreatePropertiesAndAttach(ASTComposite astCompositeNode, int context) {
            // For each node add a properties node as a child
            GraphvizProperties newProperties = new GraphvizProperties();
            // Add the properties node to the new GraphvizNode
            astCompositeNode.AddChild(context, newProperties);
            // return the new properties node to be used later for extending the graph
            return newProperties;
        }
        
        private GraphvizNode CreateNewGraphvizNodeToAST(Node node) {
            // Create a new GraphvizNode for the given node 
            GraphvizNode newGraphvizNode = new GraphvizNode(node.Serial.ToString());
            // Add the new GraphvizNode to the AST
            _dotFileAST.AddChild(GraphvizFileLayout.NODE_DECLARATIONS,
                newGraphvizNode);
            // return the new GraphvizNode to be used later for extending the graph 
            return newGraphvizNode;
        }


        // This method creates the GraphvizFileLayout AST Node based on the provided graph
        // and dot file name. Each tasks include 
        // 1. Create the GraphvizFileLayout AST Node
        //      a. Acquire the graphviz file name without extension.
        //      b. Determine the type of the graph (directed or undirected).
        // 2. Create the children of the GraphvizFileLayout AST Node in the
        // GraphvizFileLayout.GRAPH_LEVEL_ATTRIBUTES context
        //      a. Create a GraphvizProperties node and attach it to the GraphvizFileLayout AST Node.
        //      b. Create a label property for the graph and attach it to the GraphvizProperties node.
        //      c. Augment the label property with metadata as GraphvizPropertyValue objects that are
        //         registered using the AddGraphMetadataKey.
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
            var newProperties = CreatePropertiesAndAttach(_dotFileAST,
                GraphvizFileLayout.GRAPH_LEVEL_ATTRIBUTES);

            // Create a label property for the graph
            var labelProperty = CreateLabelProperty(newProperties,graph);

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
