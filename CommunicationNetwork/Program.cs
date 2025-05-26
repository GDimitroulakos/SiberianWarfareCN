using CommunicationNetwork.Graph;

namespace CommunicationNetwork
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello, World!");
            AdjacencyListStorage graphStorage = new AdjacencyListStorage();
            Node<int> node1 = new Node<int>();
            Node<string> node2 = new Node<string>();
            Edge<int> edge1 = new Edge<int>(node1, node2);
            graphStorage.AddNode(node1);
            graphStorage.AddNode(node2);
            graphStorage.AddEdge(edge1);
            Console.WriteLine($"Node1: {node1.Name}, Type: {node1.Type}, Serial: {node1.Serial}");
            Console.WriteLine($"Node2: {node2.Name}, Type: {node2.Type}, Serial: {node2.Serial}");
            Console.WriteLine($"Edge1: {edge1.Name}, Type: {edge1.Type}, Serial: {edge1.Serial}");
            Console.WriteLine($"Are Node1 and Node2 connected? {graphStorage.AreConnected(node1, node2)}");


        }
    }
}
