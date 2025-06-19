using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace CommunicationNetwork.Graph.GraphvizPrinter {
    
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
        
        public override void Visit(GraphvizProperties properties) {
            // Custom logic for visiting node properties
            if (m_parents.Peek() is not GraphvizFileLayout) {
                _writer.Write($" [ ");
            }

            int i;

            // Iterate through each property in the node properties and output comma 
            // separated values. VisitChildren cannot do that
            foreach (var property in
                     properties.
                         Children[GraphvizProperties.PROPERTIES].
                         Cast<GraphvizProperty>()) {
                i = 0;
                foreach (var value in property.
                             Children[GraphvizProperty.PROPERTY_VALUES].
                             Cast<GraphvizPropertyValue>()) {
                    if (i++ > 0) _writer.Write(", ");
                    m_parents.Push(properties);
                    Visit(property);
                    m_parents.Pop();
                }
            }
            // If the parent is not a GraphvizFileLayout, close the properties with a bracket
            if (m_parents.Peek() is not GraphvizFileLayout) {
                _writer.Write(" ]");
            }

        }
        
        public override void Visit(GraphvizProperty property) {
            // Custom logic for visiting a single node property
            _writer.Write($"{property.PropertyName}= \"");
            int i = 0;
            // Iterate through each value in the node property and output them
            // its in own line. VisitChildren cannot do that
            foreach (GraphvizPropertyValue value in property.
                         Children[GraphvizFileLayout.PROPERTIES].
                         Cast<GraphvizPropertyValue>()) {
                if (i++ > 0) _writer.Write("\n");
                m_parents.Push(property);
                Visit(value);
                m_parents.Pop();
            }
            _writer.WriteLine("\" ");
        }

        public override void Visit(GraphvizPropertyValue value) {
            // Custom logic for visiting a single node property value
            _writer.Write($"{value.PropertyValue}");
        }
    }

}
