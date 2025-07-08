using CommunicationNetwork.Graph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunicationNetwork.Nodes
{
	/// <summary>
	/// Represents a wireless node in a communication network graph.
	/// Wireless nodes communicate over radio waves or other wireless technologies.
	/// </summary>
	public class WirelessNode : Node, ITransmitter
	{
		public WirelessNode() : base()
		{
			Console.WriteLine($"{Name} is a wireless node.");
		}
		public void Connect()
		{
			Console.WriteLine($"{Name} is connecting wirelessly to the network.");
			// Logic for establishing a wireless connection
		}
		public void Disconnect()
		{
			Console.WriteLine($"{Name} is disconnecting from the wireless network.");
			// Logic for terminating a wireless connection
		}

		public void Transmit(Packet packet)
		{
			throw new NotImplementedException();
		}
	}
}
