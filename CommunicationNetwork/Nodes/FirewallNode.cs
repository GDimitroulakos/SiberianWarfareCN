using CommunicationNetwork.Graph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunicationNetwork.Nodes
{
	public class FirewallNode : SWCommunicationNode
	{
		private Func<Packet, bool> _filter;

		public FirewallNode(Func<Packet, bool> filter)
		{
			_filter = filter ?? (_ => true);
		}
        
        public override void Trasmit(Packet packet)
		{
			Console.WriteLine("Reached Firewall Node: ");
			Console.WriteLine($"\t{ID} inspecting packet with payload '{packet.Payload}'.");
			if (_filter(packet))
			{
				Console.WriteLine($"\t{ID} allowed the packet.");

			}
			else
			{
				Console.WriteLine($"\t{ID} dropped the packet.");
				packet.IsDropped = true;
				return;
			}
			Console.WriteLine();
		}
	}
}
