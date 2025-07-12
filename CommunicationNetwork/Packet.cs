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

		public Packet()
		{
			Payload = string.Empty;
		}

		public Packet(string payload)
		{
			Payload = payload;
		}

		public void ChangePayload(string newPayload)
		{
			if (string.IsNullOrEmpty(newPayload))
			{
				throw new ArgumentException("Payload cannot be null or empty.");
			}
			Payload = newPayload;
			Console.WriteLine($"Packet payload changed to: {Payload}");
		}
	}

	public interface ITransmitter
	{
		void Transmit(Packet packet, List<INode> path);
	}
}
