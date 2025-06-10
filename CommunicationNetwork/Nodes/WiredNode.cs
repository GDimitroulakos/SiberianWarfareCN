using CommunicationNetwork.Graph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunicationNetwork.Nodes
{
	public class WiredNode : Node
	{
		
		public WiredNode() : base()
		{
			// Initialization logic for wired node
			Console.WriteLine($"{Name} is a wired node.");
		}
		public void Connect()
		{
			Console.WriteLine($"{Name} is connecting to the network.");
			// Logic for establishing a connection
		}
		public void Disconnect()
		{
			Console.WriteLine($"{Name} is disconnecting from the network.");
			// Logic for terminating a connection
		}
	}
}
