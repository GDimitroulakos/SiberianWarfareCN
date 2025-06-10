using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunicationNetwork.Graph.GraphvizPrinter {
    public class GraphToGraphvizASTGeneration {
        private GraphvizFileLayout _dotFileAST;

        public GraphvizFileLayout DotFileAst => _dotFileAST;

        public void ToAST(IDirectedGraph graph, string dotFileName) {
            if (graph == null) throw new ArgumentNullException(nameof(graph));

            string dotfilename = System.IO.Path.GetFileNameWithoutExtension(dotFileName);
            _dotFileAST = new GraphvizFileLayout(dotfilename, "directed");
            
            // Print nodes
            foreach (var node in graph.Nodes) {
                _dotFileAST.AddChild(GraphvizFileLayout.NODE_DEFINITIONS,
                    new GraphvizNode(node.Serial.ToString()));
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
