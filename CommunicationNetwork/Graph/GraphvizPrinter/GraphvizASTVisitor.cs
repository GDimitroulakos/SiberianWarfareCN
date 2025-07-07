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
        void Visit(GraphvizNodeProperty property);
        void Visit(GraphvizNodePropertyValue value);
        void Visit(GraphvizEdgeProperty property);
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
        }
        public virtual void Visit(GraphvizNodePropertyValue value) {
            // Default implementation does nothing
        }
        public virtual void Visit(GraphvizEdgeProperties property) {
            // Default implementation does nothing
            VisitChildren(property);
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
        private Dictionary<string, List<string>> _nodePropertiesText;
        private Dictionary<string, List<string>> _edgePropertiesText;

        public GraphvizFileLayoutVisitor() {
            _nodePropertiesText = new Dictionary<string, List<string>>();
            _edgePropertiesText = new Dictionary<string, List<string>>();
        }

        public void GenerateDot(string filename, GraphvizFileLayout graph) {
            _writer = new StreamWriter(filename);
            _filename = filename;
            Visit(graph);
            _writer.Close();
        }

        public void GenerateGIF() {
            // Extract the filename without extension and augment with .dot
            string dotFilePath = System.IO.Path.ChangeExtension(_filename, ".dot");
            string outputGifPath = System.IO.Path.ChangeExtension(_filename, ".gif");

            var processStartInfo = new System.Diagnostics.ProcessStartInfo {
                FileName = "dot",
                Arguments = $"-Tgif \"{dotFilePath}\" -o \"{outputGifPath}\"",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

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
            _writer.Write($" {edge.SourceNodeID} -> {edge.TargetNodeID} ");
            VisitChildren(edge);
            _writer.WriteLine(";");
        }

        public override void Visit(GraphvizNodeProperties properties) {
            // Custom logic for visiting node properties
            _writer.Write($" [ ");
            int i;
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
            VisitChildren(properties);
            _writer.Write(" ]");
        }
        public override void Visit(GraphvizNodeProperty property) {
            // Custom logic for visiting a single node property
            _writer.Write($"{property.PropertyName}= \"");
            VisitChildren(property);
            _writer.Write("\" ");
        }

        public override void Visit(GraphvizNodePropertyValue value) {
            // Custom logic for visiting a single node property value
            _writer.Write($"{value.PropertyValue}\n");
        }

        public override void Visit(GraphvizEdgeProperty property) {
            // Custom logic for visiting a single edge property
            _writer.Write($"{property.PropertyName}= \"");
            VisitChildren(property);
            _writer.Write("\" ");
        }
    }

}
