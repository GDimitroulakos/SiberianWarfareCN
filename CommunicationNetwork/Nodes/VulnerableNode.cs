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
		private int _shift = 0;

		public VulnerableNode(double plaintextProbability = 0.3)
		{
			_plaintextProbability = plaintextProbability;
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
				// Encrypt the payload using a simple Caesar cipher with random key its time
				string encryptedPayload = Cipher.Encrypt(packet.Payload, _rng.Next(1, 26));
				Console.WriteLine($"\t{Name} encrypted payload: {encryptedPayload}");
				packet.Payload = encryptedPayload; // Update packet with encrypted payload
				packet.IsEncrypted = true;
				packet.Key = _shift; // Store the encryption key (shift value)
			}
			Console.WriteLine();
		}
	}
}

