using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace CommunicationNetwork.Nodes
{
	public class CommandCenter : TerminalNode
	{
		public string Name { get; set; }

		public CommandCenter()
		{
			Name = "Command Center";
		}

		public CommandCenter(string name)
		{
			Name = name;
		}

		private protected override void CheckSignature(string oldSignature, string newSignature)
		{
			if (oldSignature != newSignature)
			{
				Console.WriteLine($"\t The original packet has been hacked");
				Console.WriteLine($"\t The original signature was: " + oldSignature);
				Console.WriteLine($"\t The new signature is: " + newSignature);
			}
			else
			{
				Console.WriteLine($"\t The packet is intact and has not been altered.");
			}
		}

		public override void Receive(Packet packet)
		{
			Console.WriteLine("Reached Command Center Node:");
			Console.WriteLine($"\t{Name} received packet with payload '{packet.Payload}'.");
			if (packet.IsEncrypted)
			{
				string decryptedPayload = Cipher.Decrypt(packet.Payload, packet.Key);
				Console.WriteLine($"\t{Name} decrypted payload: {decryptedPayload}");
				string oldSignature = packet.Signature;
				string newSignature = Packet.HashSHA256(decryptedPayload);

				CheckSignature(oldSignature, newSignature);
			} 
			else
			{
				string oldSignature = packet.Signature;
				string newSignature = Packet.HashSHA256(packet.Payload);

				CheckSignature(oldSignature, newSignature);
			}
		}

		public override void Trasmit(Packet packet)
		{
			Console.WriteLine($"Reached Terminal {Name} Node:");
			
			Console.WriteLine($"\t{Name} is sending packet '{packet.Payload}' to network.");

			
			Console.WriteLine();
		}
	}
}
