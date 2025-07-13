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
			Console.WriteLine($"{ID} is a wired node.");
		}
		public void Connect()
		{
			Console.WriteLine($"{ID} is connecting to the network.");
			// Logic for establishing a connection
		}
		public void Disconnect()
		{
			Console.WriteLine($"{ID} is disconnecting from the network.");
			// Logic for terminating a connection
		}
	}
}
