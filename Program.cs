using CommunicationNetwork.Graph;
using System;
using System.Collections.Generic;

namespace CommunicationNetwork
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Communication Network Analysis");
            Console.WriteLine("==============================\n");

            // Create a new network
            var network = new DirectedGraph<string>();
            
            // Add communication nodes
            network.AddNode("Router A");
            network.AddNode("Router B");
            network.AddNode("Router C");
            network.AddNode("Server 1");
            network.AddNode("Server 2");
            network.AddNode("Client 1");
            network.AddNode("Client 2");
            network.AddNode("Client 3");
            
            // Add connections with latency weights (in ms)
            network.AddEdge("Router A", "Router B", 5);
            network.AddEdge("Router B", "Router A", 5);
            network.AddEdge("Router A", "Router C", 10);
            network.AddEdge("Router C", "Router A", 10);
            network.AddEdge("Router B", "Router C", 7);
            network.AddEdge("Router C", "Router B", 7);
            network.AddEdge("Router A", "Server 1", 3);
            network.AddEdge("Server 1", "Router A", 3);
            network.AddEdge("Router B", "Server 2", 4);
            network.AddEdge("Server 2", "Router B", 4);
            network.AddEdge("Router C", "Client 1", 2);
            network.AddEdge("Client 1", "Router C", 2);
            network.AddEdge("Router C", "Client 2", 3);
            network.AddEdge("Client 2", "Router C", 3);
            network.AddEdge("Router A", "Client 3", 6);
            network.AddEdge("Client 3", "Router A", 6);
            
            // Display the network
            Console.WriteLine("Network Nodes:");
            foreach (var node in network.Nodes)
            {
                Console.WriteLine($"- {node}");
            }
            
            Console.WriteLine("\nNetwork Connections:");
            foreach (var node in network.Nodes)
            {
                var edges = network.GetEdges(node);
                foreach (var edge in edges)
                {
                    Console.WriteLine($"- {node} -> {edge.Target} (Latency: {edge.Weight}ms)");
                }
            }
            
            // Find shortest path from Client 1 to Server 2
            Console.WriteLine("\nFinding shortest path from Client 1 to Server 2:");
            var shortestPath = network.Dijkstra("Client 1", "Server 2");
            
            if (shortestPath != null)
            {
                Console.WriteLine("Path found: " + string.Join(" -> ", shortestPath));
                
                // Calculate total latency
                int totalLatency = 0;
                for (int i = 0; i < shortestPath.Count - 1; i++)
                {
                    var edge = network.GetEdge(shortestPath[i], shortestPath[i + 1]);
                    totalLatency += edge?.Weight ?? 0;
                }
                Console.WriteLine($"Total latency: {totalLatency}ms");
            }
            else
            {
                Console.WriteLine("No path found");
            }
            
            // Check network connectivity
            Console.WriteLine("\nChecking network connectivity:");
            bool isConnected = IsFullyConnected(network);
            Console.WriteLine($"Network is {(isConnected ? "fully connected" : "not fully connected")}");
            
            // Find critical communication nodes
            Console.WriteLine("\nIdentifying critical nodes:");
            var criticalNodes = FindCriticalNodes(network);
            Console.WriteLine("Critical nodes that if removed would disconnect the network:");
            foreach (var node in criticalNodes)
            {
                Console.WriteLine($"- {node}");
            }
            
            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
        }
        
        // Helper method to check if the network is fully connected
        private static bool IsFullyConnected<T>(DirectedGraph<T> graph)
        {
            if (graph.Nodes.Count == 0) return true;
            
            var visited = new HashSet<T>();
            var first = graph.Nodes[0];
            
            // Perform DFS from first node
            DFS(graph, first, visited);
            
            // If all nodes were visited, the graph is connected
            return visited.Count == graph.Nodes.Count;
        }
        
        private static void DFS<T>(DirectedGraph<T> graph, T current, HashSet<T> visited)
        {
            visited.Add(current);
            
            foreach (var edge in graph.GetEdges(current))
            {
                if (!visited.Contains(edge.Target))
                {
                    DFS(graph, edge.Target, visited);
                }
            }
        }
        
        // Find nodes that would disconnect the network if removed
        private static List<T> FindCriticalNodes<T>(DirectedGraph<T> graph)
        {
            var criticalNodes = new List<T>();
            
            foreach (var node in graph.Nodes)
            {
                // Create a copy of the graph without this node
                var tempGraph = new DirectedGraph<T>();
                
                // Add all nodes except the current one
                foreach (var otherNode in graph.Nodes)
                {
                    if (!otherNode.Equals(node))
                    {
                        tempGraph.AddNode(otherNode);
                    }
                }
                
                // Add all edges that don't involve the current node
                foreach (var sourceNode in graph.Nodes)
                {
                    if (sourceNode.Equals(node)) continue;
                    
                    foreach (var edge in graph.GetEdges(sourceNode))
                    {
                        if (!edge.Target.Equals(node))
                        {
                            tempGraph.AddEdge(sourceNode, edge.Target, edge.Weight);
                        }
                    }
                }
                
                // If removing this node disconnects the graph, it's critical
                if (tempGraph.Nodes.Count > 1 && !IsFullyConnected(tempGraph))
                {
                    criticalNodes.Add(node);
                }
            }
            
            return criticalNodes;
        }
    }
}
