using CommunicationNetwork.Graph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Security;

namespace CommunicationNetwork.Nodes
{
	public class VulnerableNode : Node
	{
		private double _plaintextProbability = 0.3;
		private Random _rng = new Random();

		public VulnerableNode(double plaintextProbability = 0.3)
		{
			_plaintextProbability = plaintextProbability;
		}

		public string EncryptPayload(string payload)
		{
			var encrypted = new StringBuilder();
			int shift = _rng.Next(1, 26); // Random shift for encryption
			foreach (char c in payload)
			{
				if (char.IsLetter(c))
				{
					char offset = char.IsUpper(c) ? 'A' : 'a';
					encrypted.Append((char) ((((c + shift) - offset) % 26) + offset));
				}
				else
				{
					encrypted.Append(c); // Non-letter characters remain unchanged
				}
			}
			return encrypted.ToString();
		}

		public override void Trasmit(Packet packet)
		{
			Console.WriteLine("Reached Vulnerable Node:");
			Console.WriteLine($"\t{Name} received packet with payload '{packet.Payload}'.");
			// Confidentiality attack: log plaintext with some probability
			if (_rng.NextDouble() < _plaintextProbability)
			{
				Console.WriteLine($"\t{Name} logged plaintext data: {packet.Payload}");
			}
			else
			{
				var encryptedPayload = EncryptPayload(packet.Payload);
				Console.WriteLine($"\t{Name} encrypted payload: {encryptedPayload}");
				packet.Payload = encryptedPayload; // Update packet with encrypted payload
			}
			Console.WriteLine();
		}
	}
}

