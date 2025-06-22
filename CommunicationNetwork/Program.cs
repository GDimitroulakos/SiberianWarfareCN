using CommunicationNetwork.Algorithm;
using CommunicationNetwork.Algorithms;
using CommunicationNetwork.Graph;
using CommunicationNetwork.Graph.GraphvizPrinter;

namespace CommunicationNetwork {
    internal class Program {
        static void Main(string[] args) {
            UnDirectedGraph graph = new UnDirectedGraph(new UndirectedAdjacencyListStorage(), "test");
            Node node1 = new Node();
            Node node2 = new Node();
            Node node3 = new Node();
            Node node4 = new Node();
            Node node5 = new Node();
            Edge edge1 = new Edge(node1, node2);
            Edge edge2 = new Edge(node2, node3);
            Edge edge3 = new Edge(node1, node4);
            Edge edge4 = new Edge(node1, node5);
            Edge edge5 = new Edge(node4, node5);
            Edge edge6 = new Edge(node3, node5);
            graph.AddNode(node1);
            graph.AddNode(node2);
            graph.AddNode(node3);
            graph.AddNode(node4);
            graph.AddNode(node5);
            graph.AddEdge(edge1);
            graph.AddEdge(edge2);
            graph.AddEdge(edge3);
            graph.AddEdge(edge4);
            graph.AddEdge(edge5);

            DirectedGraph directedGraph = new DirectedGraph(new DirectedAdjacencyListStorage(), "test_directed");
            directedGraph.AddNode(node1);
            directedGraph.AddNode(node2);
            directedGraph.AddNode(node3);
            directedGraph.AddNode(node4);
            directedGraph.AddNode(node5);
            directedGraph.AddEdge(edge1);
            directedGraph.AddEdge(edge2);
            directedGraph.AddEdge(edge3);
            directedGraph.AddEdge(edge4);
            directedGraph.AddEdge(edge5);
            directedGraph.AddEdge(edge6);

            /*  DFSUndirected dfs = new DFSUndirected();
                  dfs.SetUnDirectedGraph(graph);
                  dfs.Execute();
             */

            // Run DFS on directed graph
            DFS dfsDirected = new DFS();
            dfsDirected.SetGraph(directedGraph);
            dfsDirected.Execute();

            BFS bfsDirected = new BFS();
            bfsDirected.SetGraph(directedGraph);
            bfsDirected.SetSource(node1);
            bfsDirected.Execute();

            BellmanFord.SetWeight(edge1,2);
            BellmanFord.SetWeight(edge2, 3);
            BellmanFord.SetWeight(edge3, 1);
            BellmanFord.SetWeight(edge4, 4);
            BellmanFord.SetWeight(edge5, 2);
            BellmanFord.SetWeight(edge6, 5);


            BellmanFord bellmanFordDirected = new BellmanFord();
            bellmanFordDirected.SetStart(node1);
            bellmanFordDirected.SetGraph(directedGraph);
            bellmanFordDirected.Execute();


            GraphToGraphvizASTGeneration graphToDOTGeneration = new GraphToGraphvizASTGeneration();
            graphToDOTGeneration.AddNodeMetadataKey(DFS.MetadataKey);
            graphToDOTGeneration.AddNodeMetadataKey(BFS.MetadataKey);
            graphToDOTGeneration.AddGraphMetadataKey(BFS.MetadataKey);
            graphToDOTGeneration.AddEdgeMetadataKey(BellmanFord.MetadataKey);
            graphToDOTGeneration.AddNodeMetadataKey(BellmanFord.MetadataKey);
            graphToDOTGeneration.AddGraphMetadataKey(BellmanFord.MetadataKey);
            graphToDOTGeneration.ToAST(directedGraph, "test_directed.dot");
            GraphvizFileLayoutVisitor graphvizFileLayoutVisitor = new GraphvizFileLayoutVisitor();

            graphvizFileLayoutVisitor.GenerateDot("test_directed.dot", graphToDOTGeneration.DotFileAst);
            graphvizFileLayoutVisitor.GenerateGIF();

            GraphvizASTPrinter graphvizASTPrinter = new GraphvizASTPrinter();
            graphvizASTPrinter.GenerateDot(graphToDOTGeneration.DotFileAst, "AST.dot");






        }
    }
}
