using CommunicationNetwork.Graph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunicationNetwork.Nodes
{
	public class FirewallNode : Node, ITransmitter
	{
		private List<ITransmitter> _links = new List<ITransmitter>();
		private Func<Packet, bool> _filter;

		public FirewallNode(Func<Packet, bool> filter)
		{
			_filter = filter ?? (_ => true);
		}

		public void AddLink(ITransmitter node) => _links.Add(node);

		public void Transmit(Packet packet)
		{
			Console.WriteLine($"{Name} inspecting packet with payload '{packet.Payload}'.");
			if (_filter(packet))
			{
				Console.WriteLine($"{Name} allowed the packet.");
				foreach (var link in _links)
				{
					Console.WriteLine($"{Name} forwarding packet to {((Node) link).Name}.");
					link.Transmit(packet);
				}
			}
			else
			{
				Console.WriteLine($"{Name} dropped the packet.");
			}
		}
	}
}
