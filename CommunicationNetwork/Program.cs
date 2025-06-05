using CommunicationNetwork.Graph;

namespace CommunicationNetwork
{
    internal class Program {
        static void Main(string[] args) {
           DirectedGraph graph = new DirectedGraph(new DirectedAdjacencyListStorage(), "test");
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
            DirectedGraphGraphvizPrinter.ToDot(graph,"test.dot");
            DirectedGraphGraphvizPrinter.GenerateGraphGif("test.dot","test.gif");
        }
    }
}
