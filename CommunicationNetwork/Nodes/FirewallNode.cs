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
		/// <summary>
		/// Represents a firewall node in a communication network graph.
		/// Firewall nodes are used to filter and control the flow of data between different parts of the network.
		/// </summary>
		public FirewallNode() : base()
		{

		}

		public void FilterData()
		{
			Console.WriteLine($"{Name} is filtering data.");
			// Logic for filtering data packets
		}

		public void LogTraffic()
		{
			Console.WriteLine($"{Name} is logging traffic.");
			// Logic for logging network traffic
		}

		public void Transmit(Packet packet)
		{
			throw new NotImplementedException();
		}
	}
}
