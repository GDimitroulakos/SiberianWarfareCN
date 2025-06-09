using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunicationNetwork.Graph;

namespace CommunicationNetwork.Nodes
{
	/// <summary>
	/// Represents a terminal node in a communication network graph.
	/// Terminal nodes either pull or push orders and are typically the endpoints of a communication path.
	/// </summary>
	public class TerminalNode : Node
	{
		private enum NodeType
		{
			Troop,
			Vehicle,
			Building,
			Aircraft,
			Ship,
			Station,
			Drone
		};

		private NodeType nodeType;

		public TerminalNode() : base()
		{
			nodeType = NodeType.Troop; // Default type, can be changed as needed
		}

		public void PullOrder()
		{
			// Logic for pulling an order
			Console.WriteLine($"{Name} is pulling an order.");
		}

		public void PushOrder()
		{
			// Logic for pushing an order
			Console.WriteLine($"{Name} is pushing an order.");
		}
	}
}
