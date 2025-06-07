using CommunicationNetwork.Algorithms;
using CommunicationNetwork.Graph;

namespace CommunicationNetwork
{
    internal class Program {
        static void Main(string[] args) {
           UnDirectedGraph graph = new UnDirectedGraph(new UndirectedAdjacencyListStorage(), "test");
            List<Node<int>> nodes = new List<Node<int>>();
            for (int i = 0; i < 10; i++) {
                Node<int> newNode = new Node<int>();
                nodes.Add(newNode);
                graph.AddNode(newNode);
            }
            // Create 20 directed edges between the nodes
            Random rand = new Random();
            HashSet<(int, int)> createdEdges = new HashSet<(int, int)>();
            int edgeCount = 0;
            while (edgeCount < 20) {
                int from = rand.Next(0, nodes.Count);
                int to = rand.Next(0, nodes.Count);
                if (from != to && !createdEdges.Contains((from, to))) {
                    Edge<int> newedge = new Edge<int>(nodes[from], nodes[to]);
                    graph.AddEdge(newedge);
                    createdEdges.Add((from, to));
                    edgeCount++;
                }
            }
            
            DFSUndirected dfs = new DFSUndirected();
            dfs.SetUnDirectedGraph(graph);
            dfs.Execute();

            UndirectedGraphGraphvizPrinter.ToDot(graph, "test.dot", new GraphvizPrinterSettings() {
                ShowEdgeLabels = false,
                ShowNodeLabels = false,
                ShowNodeProperties = true,
                ShowEdgeProperties = false
            });
            UndirectedGraphGraphvizPrinter.GenerateGraphGif("test.dot", "test.gif");


        }
    }
}
