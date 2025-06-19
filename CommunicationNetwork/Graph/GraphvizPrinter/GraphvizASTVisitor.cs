using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace CommunicationNetwork.Graph.GraphvizPrinter {
    public interface IGraphvizASTVisitor {
        void Visit(ASTLeaf leaf);
        void Visit(GraphvizFileLayout layout);
        void Visit(GraphvizNode node);
        void Visit(GraphvizEdge edge);
        void Visit(GraphvizNodeProperties properties);
        void Visit(GraphvizEdgeProperties properties);
        void Visit(GraphvizGraphProperties properties);
        void Visit(GraphvizNodeProperty property);
        void Visit(GraphvizGraphProperty property);
        void Visit(GraphvizGraphPropertyValue value);
        void Visit(GraphvizNodePropertyValue value);
        void Visit(GraphvizEdgeProperty property);
        void Visit(GraphvizEdgePropertyValue value);
    }
    public class BaseASTVisitor : IGraphvizASTVisitor {
        public virtual void Visit(ASTLeaf leaf) {
            // Default implementation does nothing
        }
        public virtual void Visit(GraphvizFileLayout layout) {
            VisitChildren(layout);
        }
        public virtual void Visit(GraphvizNode node) {
            VisitChildren(node);
        }
        public virtual void Visit(GraphvizEdge edge) {
            // Default implementation does nothing
            VisitChildren(edge);
        }
        public virtual void Visit(GraphvizNodeProperties property) {
            // Default implementation does nothing
            VisitChildren(property);
        }
        public virtual void Visit(GraphvizNodeProperty property) {
            VisitChildren(property);
        } public virtual void Visit(GraphvizGraphProperty property) {
            // Default implementation does nothing
            VisitChildren(property);
        }
        public virtual void Visit(GraphvizNodePropertyValue value) {
            // Default implementation does nothing
        }
        public virtual void Visit(GraphvizEdgePropertyValue value) {
            // Default implementation does nothing
        }
        public virtual void Visit(GraphvizGraphPropertyValue value) {
            // Default implementation does nothing
        }
        public virtual void Visit(GraphvizEdgeProperties property) {
            // Default implementation does nothing
            VisitChildren(property);
        }
        public virtual void Visit(GraphvizGraphProperties properties) {
            // Default implementation does nothing
            VisitChildren(properties);
        }
        public virtual void Visit(GraphvizEdgeProperty property) {
            // Default implementation does nothing
        }

        public virtual void VisitChildren(ASTComposite composite) {
            foreach (var childList in composite.Children.Values) {
                foreach (var child in childList) {
                    child.Accept(this);
                }
            }
        }
    }

    public class GraphvizFileLayoutVisitor : BaseASTVisitor {

        StreamWriter _writer;
        private string _filename;
        GraphvizFileLayout _graphLayout;

        public GraphvizFileLayoutVisitor() {
            
        }

        public void GenerateDot(string filename, GraphvizFileLayout graph) {
            _writer = new StreamWriter(filename);
            _graphLayout = graph;
            _filename = filename;
            Visit(graph);
            _writer.Close();
        }

        public void GenerateGIF() {
            // Extract the filename without extension and augment with .dot
            string dotFilePath = Path.ChangeExtension(_filename, ".dot");
            string outputGifPath = Path.ChangeExtension(_filename, ".gif");

            // Configure the process start info for running the dot command
            var processStartInfo = new System.Diagnostics.ProcessStartInfo {
                FileName = "dot",
                Arguments = $"-Tgif \"{dotFilePath}\" -o \"{outputGifPath}\"",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            // Start the process and wait for it to complete. In case of an error, throw an exception with the error message.
            using (var process = new System.Diagnostics.Process { StartInfo = processStartInfo }) {
                process.Start();
                string stdOut = process.StandardOutput.ReadToEnd();
                string stdErr = process.StandardError.ReadToEnd();
                process.WaitForExit();

                if (process.ExitCode != 0) {
                    throw new InvalidOperationException(
                        $"dot process failed with exit code {process.ExitCode}: {stdErr}");
                }
            }
        }

        public override void Visit(GraphvizFileLayout layout) {
            _writer.WriteLine($"{layout.GraphType} {layout.GraphName} {{");
            VisitChildren(layout);
            _writer.Write("}");
        }

        public override void Visit(GraphvizNode node) {
            _writer.Write($" {node.NodeID} ");
            VisitChildren(node);
            _writer.WriteLine(";");
        }

        public override void Visit(GraphvizEdge edge) {
            switch (_graphLayout.GraphType) {
                case GraphvizFileLayout.GRAPHTYPE.DIGRAPH:
                    _writer.Write($" {edge.SourceNodeID} -> {edge.TargetNodeID} ");
                    break;
                case GraphvizFileLayout.GRAPHTYPE.GRAPH:
                    _writer.Write($" {edge.SourceNodeID} -- {edge.TargetNodeID} ");
                    break;
            }
            VisitChildren(edge);
            _writer.WriteLine(";");
        }

        
        public override void Visit(GraphvizNodeProperties properties) {
            // Custom logic for visiting node properties
            _writer.Write($" [ ");
            int i;

            // Iterate through each property in the node properties and output comma 
            // separated values. VisitChildren cannot do that
            foreach (var property in
                     properties.
                         Children[GraphvizNodeProperties.PROPERTIES].
                         Cast<GraphvizNodeProperty>()) {
                i = 0;
                foreach (var value in property.
                             Children[GraphvizNodeProperty.PROPERTY_VALUES].
                             Cast<GraphvizNodePropertyValue>()) {
                    if (i++ > 0) _writer.Write(", ");
                    Visit(property);
                }
            }
            _writer.Write(" ]");
        }
        public override void Visit(GraphvizEdgeProperties properties) {
            // Custom logic for visiting edge properties
            _writer.Write($" [ ");
            int i;
            // Iterate through each property in the edge properties and output comma
            // separated values. VisitChildren cannot do that
            foreach (var property in
                     properties.
                         Children[GraphvizEdgeProperties.PROPERTIES].
                         Cast<GraphvizEdgeProperty>()) {
                i = 0;
                foreach (var value in property.
                             Children[GraphvizEdgeProperty.PROPERTY_VALUES].
                             Cast<GraphvizEdgePropertyValue>()) {
                    if (i++ > 0) _writer.Write(", ");
                    Visit(property);
                }
            }
            _writer.Write(" ]");
        }
        public override void Visit(GraphvizNodeProperty property) {
            // Custom logic for visiting a single node property
            _writer.Write($"{property.PropertyName}= \"");
            int i = 0;
            // Iterate through each value in the node property and output them
            // its in own line. VisitChildren cannot do that
            foreach (GraphvizNodePropertyValue value in property.
                         Children[GraphvizNodeProperty.PROPERTY_VALUES].
                         Cast<GraphvizNodePropertyValue>()) {
                if (i++ > 0) _writer.Write("\n");
                Visit(value);
            }
            _writer.Write("\" ");
        }

        public override void Visit(GraphvizGraphProperty property) {
            // Custom logic for visiting a single node property
            _writer.Write($"{property.PropertyName}= \"");
            int i = 0;
            // Iterate through each value in the node property and output them
            // its in own line. VisitChildren cannot do that
            foreach (GraphvizGraphPropertyValue value in property.
                         Children[GraphvizFileLayout.GLOBAL_ATTRIBUTES].
                         Cast<GraphvizGraphPropertyValue>()) {
                if (i++ > 0) _writer.Write("\n");
                Visit(value);
            }
            _writer.WriteLine("\" ");
        }

        public override void Visit(GraphvizGraphPropertyValue value) {
            // Custom logic for visiting a single node property value
            _writer.Write($"{value.PropertyValue}");
        }

        public override void Visit(GraphvizNodePropertyValue value) {
            // Custom logic for visiting a single node property value
            _writer.Write($"{value.PropertyValue}");
        }

        public override void Visit(GraphvizEdgePropertyValue value) {
            // Custom logic for visiting a single edge property value
            _writer.Write($"{value.PropertyValue}");
        }

        public override void Visit(GraphvizEdgeProperty property) {
            // Custom logic for visiting a single edge property
            _writer.Write($"{property.PropertyName}= \"");
            int i = 0;
            // Iterate through each value in the edge property and output them
            // its in own line. VisitChildren cannot do that
            foreach (GraphvizEdgePropertyValue value in property.
                         Children[GraphvizEdgeProperty.PROPERTY_VALUES].
                         Cast<GraphvizEdgePropertyValue>()) {
                if (i++ > 0) _writer.Write("\n");
                Visit(value);
            }
            _writer.Write("\" ");
        }
    }

}
