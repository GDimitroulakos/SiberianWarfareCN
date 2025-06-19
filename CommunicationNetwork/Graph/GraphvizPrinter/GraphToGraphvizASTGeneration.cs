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
        private List<object> _edgeMetadataKeys = new List<object>();



        public GraphvizFileLayout DotFileAst => _dotFileAST;

        public void AddNodeMetadataKey(object key) {
            if (key == null) throw new ArgumentNullException(nameof(key));
            if (!_nodeMetadataKeys.Contains(key)) {
                _nodeMetadataKeys.Add(key);
            }
        }
        public void AddEdgeMetadataKey(object key) {
            if (key == null) throw new ArgumentNullException(nameof(key));
            if (!_edgeMetadataKeys.Contains(key)) {
                _edgeMetadataKeys.Add(key);
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
        public void RemoveEdgeMetadataKey(object key) {
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

            // Print nodes
            foreach (var node in graph.Nodes) {
                // Create a new GraphvizNode for each node in the graph and add it to the AST
                var newGraphvizNode = CreateNewGraphvizNodeToAST(node);

                // For each node add a properties node as a child
                var newProperties = CreatePropertiesAndAttach(newGraphvizNode);

                // Create a label property for the node
                var labelProperty = CreateLabelProperty(newProperties, node);

                //  Augment the label property with metadata if available
                AugmentNodeLabelPropertyWithMetadata(node, labelProperty);
            }

            // Print edges
            foreach (var edge in graph.Edges) {
                var newGraphvicEdge = CreateNewGraphvicEdgeToAST(edge);

                var edgeProperties = CreatePropertiesAndAttach(newGraphvicEdge);

                CreateLabelProperty(edgeProperties, edge);
            }
        }
        
        private static GraphvizProperty CreateLabelProperty(GraphvizProperties newProperties, IGraphElement element) {
            // A. Create a label property for the node
            GraphvizProperty labelProperty = new GraphvizProperty("label");
            // Add the label property to the node properties
            newProperties.AddChild(GraphvizFileLayout.PROPERTIES, labelProperty);

            // Create the label property value 
            GraphvizPropertyValue labelValue =
                new GraphvizPropertyValue(element.Serial.ToString());

            // Add the label value to the label property
            labelProperty.AddChild(GraphvizProperty.PROPERTY_VALUES, labelValue);
            return labelProperty;
        }


        private GraphvizEdge CreateNewGraphvicEdgeToAST(IEdge edge) {
            int source = edge.Source.Serial;
            int target = edge.Target.Serial;
            // Create a new GraphvizEdge for each edge in the graph and add it to the AST
            var newGraphvicEdge = new GraphvizEdge(source.ToString(), target.ToString());
            _dotFileAST.AddChild(GraphvizFileLayout.EDGE_DEFINITIONS, newGraphvicEdge);
            return newGraphvicEdge;
        }

        private void AugmentNodeLabelPropertyWithMetadata(INode node, GraphvizProperty labelProperty) {
            // A2. Add property values to the label property
            foreach (var key in _nodeMetadataKeys) {
                GraphvizPropertyValue newValue =
                    new GraphvizPropertyValue(node.MetaData[key].ToString());
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

        private static GraphvizProperties CreatePropertiesAndAttach(ASTComposite astCompositeNode) {
            // For each node add a properties node as a child
            GraphvizProperties newProperties = new GraphvizProperties();
            // Add the properties node to the new GraphvizNode
            astCompositeNode.AddChild(GraphvizNode.ATTRIBUTE_LIST, newProperties);
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
            var newProperties = CreatePropertiesAndAttach(_dotFileAST);

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
