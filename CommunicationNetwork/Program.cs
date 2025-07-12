using CommunicationNetwork.Algorithm;
using CommunicationNetwork.Algorithms;
using CommunicationNetwork.Graph;
using CommunicationNetwork.Graph.GraphvizPrinter;
using CommunicationNetwork.Nodes;
using System.Xml.Linq;
using static CommunicationNetwork.Algorithm.BellmanFord;

namespace CommunicationNetwork {
    internal class Program {
        static void Main(string[] args) {
            UnDirectedGraph graph = new UnDirectedGraph(new UndirectedAdjacencyListStorage(), "ug");
            Node tns = new TerminalNode(TerminalNode.TerminalType.Sender);
            Node hn = new HackerNode();
            Node vn = new VulnerableNode();
            Node fn = new FirewallNode(packet => (packet.Payload.Length >= 10 && packet.Payload.Length <=30));
            Node tnr = new TerminalNode(TerminalNode.TerminalType.Receiver);
            Edge edge1 = new Edge(tns, vn);
            Edge edge2 = new Edge(vn, fn);
            Edge edge3 = new Edge(fn, hn);
            Edge edge4 = new Edge(hn, tnr);

            graph.AddNode(tns);
            graph.AddNode(vn);
            graph.AddNode(fn);
			graph.AddNode(hn);
			graph.AddNode(tnr);
            graph.AddEdge(edge1);
            graph.AddEdge(edge2);
            graph.AddEdge(edge3);
            graph.AddEdge(edge4);

            DirectedGraph directedGraph = new DirectedGraph(new DirectedAdjacencyListStorage(), "dg");
            directedGraph.AddNode(tns);
            directedGraph.AddNode(vn);
            directedGraph.AddNode(fn);
            directedGraph.AddNode(hn);
            directedGraph.AddNode(tnr);
            directedGraph.AddEdge(edge1);
            directedGraph.AddEdge(edge2);
            directedGraph.AddEdge(edge3);
            directedGraph.AddEdge(edge4);


            /*  DFSUndirected dfs = new DFSUndirected();
                  dfs.SetUnDirectedGraph(graph);
                  dfs.Execute();
             */

            // Run DFS on directed graph
            DFS dfsDirected = new DFS("dfs");
            dfsDirected.SetGraph(directedGraph);
            dfsDirected.Execute();

            BFS bfsDirected = new BFS("bfs");
            bfsDirected.SetGraph(directedGraph);
            bfsDirected.SetSource(tns);
            bfsDirected.Execute();

            BellmanFord bellmanFordDirected = new BellmanFord("bf1");
            bellmanFordDirected.SetWeight(edge1, 1);
            bellmanFordDirected.SetWeight(edge2, 1);
            bellmanFordDirected.SetWeight(edge3, 1);
            bellmanFordDirected.SetWeight(edge4, 1);
            bellmanFordDirected.SetStart(tns);
            bellmanFordDirected.SetGraph(directedGraph);
            bellmanFordDirected.Execute();

            TopologicalSort topologicalSort = new TopologicalSort("ts1");
            topologicalSort.SetGraph(directedGraph);
            topologicalSort.SetDFS(dfsDirected);
            topologicalSort.Execute();

            // Traverse path with packet
            Packet packet = new Packet("Hello World");
            var path = bellmanFordDirected.Paths[tnr];
            foreach (Node node in path)
            {
                node.Trasmit(packet);
            }



            GraphToGraphvizASTGeneration graphToDOTGeneration = new GraphToGraphvizASTGeneration();
            graphToDOTGeneration.AddNodeMetadataKey(dfsDirected.MetadataKey);
            graphToDOTGeneration.AddNodeMetadataKey(bfsDirected.MetadataKey);
            graphToDOTGeneration.AddGraphMetadataKey(bfsDirected.MetadataKey);
            graphToDOTGeneration.AddEdgeMetadataKey(bellmanFordDirected.MetadataKey);
            graphToDOTGeneration.AddNodeMetadataKey(bellmanFordDirected.MetadataKey);
            graphToDOTGeneration.AddGraphMetadataKey(bellmanFordDirected.MetadataKey);
            graphToDOTGeneration.ToAST(directedGraph, "test_directed.dot");
            GraphvizFileLayoutVisitor graphvizFileLayoutVisitor = new GraphvizFileLayoutVisitor();

            graphvizFileLayoutVisitor.GenerateDot("test_directed.dot", graphToDOTGeneration.DotFileAst);
            graphvizFileLayoutVisitor.GenerateGIF();

            GraphvizASTPrinter graphvizASTPrinter = new GraphvizASTPrinter();
            graphvizASTPrinter.GenerateDot(graphToDOTGeneration.DotFileAst, "AST.dot");



            //DFSDirectedGraphVizNodeLabelPrinter ndp = new DFSDirectedGraphVizNodeLabelPrinter(
            //    new DFSDirectedGraphvizFixedSizePropertyPrinter(
            //        new DFSGraphvizNodePrinter()));
            //DFSGraphvizEdgePrinter dgep = new DFSGraphvizEdgePrinter();
            //DirectedGraphGraphvizPrinter dgp = new DirectedGraphGraphvizPrinter(ndp, dgep);

            //// Print the directed graph to DOT and generate GIF
            //dgp.ToDot(directedGraph, "test_directed.dot");
            //dgp.GenerateGraphGif("test_directed.dot", "test_directed.gif");


        }
    }
}
