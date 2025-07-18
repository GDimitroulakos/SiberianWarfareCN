using CommunicationNetwork.Algorithm;
using CommunicationNetwork.Algorithm.TestingAlgorithms;
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
            DFS dfsDirected = new DFS("dfs1");
            dfsDirected.SetGraph(directedGraph);
            dfsDirected.Execute();

            BFS bfsDirected = new BFS("bfs1");
            bfsDirected.SetGraph(directedGraph);
            bfsDirected.SetSource(node1);
            bfsDirected.Execute();

            CreateSampleWeightsAlgorithm SW = new CreateSampleWeightsAlgorithm("sampleWeights");
            SW.SetGraph(graph);
            SW.Execute();

            // Create Bellman-Ford algorithm instance
            BellmanFord bellmanFord = new BellmanFord("bell1");
            // Set the input graph for the Bellman-Ford algorithm assuming that 
            // the weights have been set by the CreateSampleWeightsAlgorithm
            bellmanFord.SetGraph(graph);
            // Set the start node for the Bellman-Ford algorithm
            bellmanFord.SetStart(node1);
            // Set the key to access the weights in the graph given by the CreateSampleWeightsAlgorithm
            // The weight are also assumed to be set in the edges by the CreateSampleWeightsAlgorithm.
            // Bellman-Ford algorithm requires the weights to be set in the edges. It is the programmer's
            // responsibility to ensure that the weights are set before executing the algorithm and given
            // through the input graph.
            // However, there is a problem when we have two instances of the CreateSampleWeightsAlgorithm.
            // Here we may have the problem
            // 1. We sent a different graph from the one manipulated by the CreateSampleWeightsAlgorithm
            // NO problem the key is unique for the algorithm instance so the algorithm cannot access false data.
            // However, the algorithm can access absent data so **A CHECK MUST BE MADE** to ensure that the
            // weights are set in the edges of the graph.
            // 2. If the algorithm takes multiple graphs how can we ensure that the proper graph is used?
            // We must have a way to link the input graph with the key to access the weights in the edges
            // together.
            bellmanFord.RegisterInput(graph,nameof(bellmanFord.K_WEIGHT), SW,SW.K_WEIGHT);
            // Execute the Bellman-Ford algorithm
            bellmanFord.Execute();

            /*TopologicalSort topologicalSort = new TopologicalSort("topo1");
            topologicalSort.SetGraph(directedGraph);
            topologicalSort.RegisterInput(directedGraph,nameof(topologicalSort.K_DFSFINISHEDTIMES),
                dfsDirected,dfsDirected.K_TIMEFINISHED);
            topologicalSort.Execute();*/


            GraphToGraphvizASTGeneration graphToDOTGeneration = new GraphToGraphvizASTGeneration();
            graphToDOTGeneration.AddNodeMetadataKey(dfsDirected.K_TIMEFINISHED);
            graphToDOTGeneration.AddNodeMetadataKey(dfsDirected.K_TIMEDISCOVERY);
            graphToDOTGeneration.AddNodeMetadataKey(dfsDirected.K_COLOR);
            graphToDOTGeneration.ToAST(directedGraph, "test_directed.dot");
            GraphvizFileLayoutVisitor graphvizFileLayoutVisitor = new GraphvizFileLayoutVisitor();

            graphvizFileLayoutVisitor.GenerateDot("test_directed.dot", graphToDOTGeneration.DotFileAst);
            graphvizFileLayoutVisitor.GenerateGIF();

            GraphvizASTPrinter graphvizASTPrinter = new GraphvizASTPrinter();
            graphvizASTPrinter.GenerateDot(graphToDOTGeneration.DotFileAst, "AST.dot");
            
        }
    }
}
