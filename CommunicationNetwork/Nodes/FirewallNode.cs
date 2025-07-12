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

		public override void Trasmit(Packet packet, List<Node> path)
		{
			Console.WriteLine($"{Name} inspecting packet with payload '{packet.Payload}'.");
			if (_filter(packet))
			{
				Console.WriteLine($"{Name} allowed the packet.");

			}
			else
			{
				Console.WriteLine($"{Name} dropped the packet.");
			}
		}
	}
}
