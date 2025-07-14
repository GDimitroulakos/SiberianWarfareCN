using CommunicationNetwork.Graph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunicationNetwork.Nodes
{
	public class Traversal
	{
		public static void Traverse(Packet packet, List<SWCommunicationNode> path)
		{
			if (path == null || path.Count == 0)
			{
				Console.WriteLine("No path to traverse.");
				return;
			}
			Console.WriteLine("Starting traversal with packet: " + packet.Payload);
			foreach (SWCommunicationNode node in path)
			{
				node.Trasmit(packet);
				if (packet.IsDropped)
				{
					Console.WriteLine("Packet was dropped during traversal. Stopping...");
					return;
				}
			}
			Console.WriteLine("Traversal completed successfully.");
		}
	}
}
