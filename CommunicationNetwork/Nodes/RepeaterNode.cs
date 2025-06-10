using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunicationNetwork.Graph;

namespace CommunicationNetwork.Nodes
{
	public class RepeaterNode : Node
	{
		/// <summary>
		/// Represents a repeater node in a communication network graph.
		/// Repeater nodes are used to extend the range of communication by amplifying or broadcasting signals.
		/// </summary>
		public RepeaterNode() : base()
		{
			
		}

		public void BroadcastSignal()
		{
			Console.WriteLine($"{Name} is broadcasting the signal.");
			// Logic for amplifying the signal
		}

		public void AmplifySignal()
		{
			Console.WriteLine($"{Name} is amplifying the signal.");
			// Logic for regenerating the signal
		}
	}
}
