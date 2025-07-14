using CommunicationNetwork.Nodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunicationNetwork.Graph
{
	public class Traversal
	{
		public static void Traverse(Packet packet, List<INode> path)
		{
			if (path == null || path.Count == 0)
			{
				Console.WriteLine("No path to traverse.");
				return;
			}
			Console.WriteLine("Starting traversal with packet: " + packet.Payload);
			for (int i = 0; i < path.Count - 1; i++)
			{
				Node? node = path[i] as Node;
				node?.Trasmit(packet);
				if (packet.IsDropped)
				{
					Console.WriteLine("Packet was dropped during traversal. Stopping...");
					return;
				}
			}
			TerminalNode? lastNode = path.Last() as TerminalNode;
			lastNode?.Receive(packet);
			Console.WriteLine("Traversal completed successfully.");
		}
	}
}
