using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunicationNetwork.Graph;
using System.Security.Cryptography;

namespace CommunicationNetwork.Nodes
{
	/// <summary>
	/// Represents a terminal node in a communication network graph.
	/// Terminal nodes either pull or push orders and are typically the endpoints of a communication path.
	/// </summary>
	public abstract class TerminalNode : Node
	{ 
		public enum TerminalType { Sender, Receiver, TwoWay }

		public abstract void Receive(Packet packet);

		public override abstract void Trasmit(Packet packet);
		//{
			//Console.WriteLine($"Reached Terminal {Type} Node:");
			//if (Type == TerminalType.Sender)
			//{
			//	Console.WriteLine($"\t{Name} is sending packet '{packet.Payload}' to network.");

			//}
			//else if (Type == TerminalType.Receiver)
			//{
			//	Console.WriteLine($"\t{Name} received packet with payload '{packet.Payload}'.");
			//	string oldSignature = packet.Signature;
			//	string newSignature = Packet.HashSHA256(packet.Payload);

			//	if (oldSignature != newSignature)
			//	{
			//		Console.WriteLine($"\t The original packet has been hacked");
			//		Console.WriteLine($"\t The original signature was: " + oldSignature);
			//		Console.WriteLine($"\t The new signature is: " + newSignature);
			//	}
			//	else
			//	{
			//		Console.WriteLine($"\t The packet is intact and has not been altered.");
			//		Console.WriteLine($"\t The packet payload is: {packet.Payload}");
			//	}
				
			//}
			//else if (Type == TerminalType.TwoWay)
			//{

			//}
			//	Console.WriteLine();
		//}
	}
}
