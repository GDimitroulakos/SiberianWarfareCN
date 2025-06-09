using CommunicationNetwork.Graph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunicationNetwork
{
	public class Packet
	{
		/// <summary>
		/// Represents a packet in a communication network.
		/// A packet is a formatted unit of data carried by a packet-switched network.
		/// </summary>
		public string Payload { get; set; }
		public Node Source { get; set; }
		public Node Destination { get; set; }
		public Node CurrentNode { get; set; }

		public Packet()
		{
			Payload = string.Empty;
		}

		public bool HasReachedDestination()
		{
			if (CurrentNode == null || Destination == null)
			{
				return false; // Cannot determine if the packet has reached its destination
			}
			if (CurrentNode.Equals(Destination))
			{
				Console.WriteLine($"Packet has reached its destination: {Destination.Name}");
				return true; // Packet has reached its destination
			}
			else
			{
				Console.WriteLine($"Packet is at {CurrentNode.Name}, not at destination {Destination.Name}.");
				return false; // Packet has not reached its destination
			}
		}
	}
}
