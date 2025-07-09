using CommunicationNetwork.Graph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace CommunicationNetwork.Nodes
{
	public class HackerNode : Node, ITransmitter
	{
		private List<ITransmitter> _compromisedLinks = new List<ITransmitter>();
		private double _modifyProbability;
		private double _dropProbability;
		private Random _rng = new Random();

		/// <param name="modifyProbability">Chance [0..1] to alter packet payload</param>
		/// <param name="dropProbability">Chance [0..1] to drop packet entirely</param>
		public HackerNode(double modifyProbability = 0.5, double dropProbability = 0.2) : base()
		{
			_modifyProbability = modifyProbability;
			_dropProbability = dropProbability;
			Console.WriteLine($"{Name} is a hacker node (modify {_modifyProbability:P0}, drop {_dropProbability:P0}).");
		}

		public void Connect(ITransmitter other)
		{
			if (!_compromisedLinks.Contains(other))
			{
				_compromisedLinks.Add(other);
				Console.WriteLine($"{Name} compromised link to {((Node) other).Name}.");
			}
		}

		public void Disconnect(ITransmitter other)
		{
			if (_compromisedLinks.Remove(other))
			{
				Console.WriteLine($"{Name} released link from {((Node) other).Name}.");
			}
		}

		public string IntegrityAttack(string payload)
		{
			// alter the payload to simulate a data integrity attack with random letters
			var alteredPayload = new StringBuilder();
			foreach (char c in payload)
			{
				if (char.IsLetter(c))
				{
					// Randomly change the character to a different letter
					char newChar = (char) ('A' + _rng.Next(0, 26));
					alteredPayload.Append(newChar);
				}
				else
				{
					// Non-letter characters remain unchanged
					alteredPayload.Append(c);
				}
			}
			return alteredPayload.ToString();
		}

		public void Transmit(Packet packet)
		{
			packet.CurrentNode = this;
			Console.WriteLine($"{Name} intercepted packet: {packet.Payload}");

			// Decide whether to drop
			if (_rng.NextDouble() < _dropProbability)
			{
				Console.WriteLine($"{Name} dropped the packet.");
				return;
			}

			// Decide whether to modify
			if (_rng.NextDouble() < _modifyProbability)
			{
				var original = packet.Payload;
				packet.Payload = $"{packet.Payload}-[HACKED]";
				Console.WriteLine($"{Name} modified payload from '{original}' to '{packet.Payload}'.");
			}

			// Forward to intended target(s)
			foreach (var link in _compromisedLinks)
			{
				Console.WriteLine($"{Name} forwarding packet to {((Node) link).Name}.");
				link.Transmit(packet);
			}
		}
	}
}
