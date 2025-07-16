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
		
		public abstract void Receive(Packet packet);

		public override abstract void Trasmit(Packet packet);

		private protected abstract void CheckSignature(string oldSignature, string newSignature);
	}
}
