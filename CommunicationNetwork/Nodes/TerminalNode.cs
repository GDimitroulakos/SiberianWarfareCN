using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunicationNetwork.Graph;

namespace CommunicationNetwork.Nodes
{
	/// <summary>
	/// Represents a terminal node in a communication network graph.
	/// Terminal nodes either pull or push orders and are typically the endpoints of a communication path.
	/// </summary>
	public class TerminalNode : Node
	{ 
		public enum TerminalType { Sender, Receiver }
		private TerminalType _type;

		public TerminalNode(TerminalType type)
		{
			_type = type;
		}

		public override void Trasmit(Packet packet, List<Node> path)
		{
			if (_type == TerminalType.Sender)
			{
				Console.WriteLine($"{Name} is sending packet '{packet.Payload}' to network.");

			}
			else if (_type == TerminalType.Receiver)
			{
				Console.WriteLine($"{Name} received packet with payload '{packet.Payload}'.");
			}
		}
	}
}
