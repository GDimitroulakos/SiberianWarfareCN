using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunicationNetwork.Graph
{
	public class NetworkRegistry
	{
		private static Dictionary<string, Node> nodes = new();
		private static BaseGraph graph;

		public static void Initialize(BaseGraph baseGraph)
		{
			graph = baseGraph;
			nodes.Clear();
		}

		public static void RegisterNode(Node node)
		{
			nodes[node.Name] = node;
			graph.AddNode(node);
		}

		public static void RegisterLink(IEdge edge)
		{
			graph.AddEdge(edge);
		}

		public static Node GetNode(string name) => nodes.GetValueOrDefault(name);
	}
}
