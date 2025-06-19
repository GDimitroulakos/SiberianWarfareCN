using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace CommunicationNetwork.Graph.GraphvizPrinter {
    public class GraphvizASTPrinter :BaseASTVisitor {
        private StreamWriter _dotStreamWriter;
        Stack<ASTNode> _parents = new Stack<ASTNode>();
        public GraphvizASTPrinter() { }

        public void GenerateDot(GraphvizFileLayout root,string filename) {
            if (root == null) throw new ArgumentNullException(nameof(root));
            if (string.IsNullOrEmpty(filename)) throw new ArgumentException("Filename cannot be null or empty.", nameof(filename));
            _parents.Clear();
            _dotStreamWriter = new StreamWriter(filename);
            Visit(root);
            _dotStreamWriter.Close();
            GenerateGIF(filename);
        }

        public void GenerateGIF(string _filename) {
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
            _dotStreamWriter.WriteLine($"digraph AST {{");
            _dotStreamWriter.WriteLine($"\"GraphvizFileLayout{layout.Serial.ToString()}\"");
            m_parents.Push(layout);
            base.Visit(layout);
            m_parents.Pop();
            _dotStreamWriter.WriteLine("}");
        }

        public override void Visit(GraphvizNode node) {
            ASTNode parent =  m_parents.Peek();
            _dotStreamWriter.WriteLine($"\"{parent.Type}{parent.Serial.ToString()}\" -> \"{node.Type}{node.Serial}\"");
            m_parents.Push(node);
            base.Visit(node);
            m_parents.Pop();
        }

        public override void Visit(GraphvizEdge edge) {
            ASTNode parent = m_parents.Peek();
            _dotStreamWriter.WriteLine($"\"{parent.Type}{parent.Serial.ToString()}\" -> \"{edge.Type}{edge.Serial}\"");
            m_parents.Push(edge);
            base.Visit(edge);
            m_parents.Pop();
        }

        public override void Visit(GraphvizProperties property) {
            ASTNode parent = m_parents.Peek();
            _dotStreamWriter.WriteLine($"\"{parent.Type}{parent.Serial.ToString()}\" -> \"{property.Type}{property.Serial}\"");
            m_parents.Push(property);
            base.Visit(property);
            m_parents.Pop();
        }

        public override void Visit(GraphvizProperty property) {
            ASTNode parent = m_parents.Peek();
            _dotStreamWriter.WriteLine($"\"{parent.Type}{parent.Serial.ToString()}\" -> \"{property.Type}{property.Serial}\"");
            m_parents.Push(property);
            base.Visit(property);
            m_parents.Pop();

        }

        public override void Visit(GraphvizPropertyValue value) {
            ASTNode parent = m_parents.Peek();
            _dotStreamWriter.WriteLine($"\"{parent.Type}{parent.Serial.ToString()}\" -> \"{value.Type}{value.Serial}\n{value.PropertyValue}\"");
        }
    }
}
