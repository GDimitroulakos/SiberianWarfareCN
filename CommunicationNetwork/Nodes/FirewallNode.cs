using CommunicationNetwork.Graph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunicationNetwork.Nodes
{
	public class FirewallNode : Node
	{
		private Func<Packet, bool> _filter;

		public FirewallNode(Func<Packet, bool> filter)
		{
			_filter = filter ?? (_ => true);
		}

		public override void Trasmit(Packet packet)
		{
			Console.WriteLine("Reached Firewall Node: ");
			Console.WriteLine($"\t{Name} inspecting packet with payload '{packet.Payload}'.");
			if (_filter(packet))
			{
				Console.WriteLine($"\t{Name} allowed the packet.");

			}
			else
			{
				Console.WriteLine($"\t{Name} dropped the packet.");
				packet.IsDropped = true;
				return;
			}
			Console.WriteLine();
		}
	}
}
