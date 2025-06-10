using CommunicationNetwork.Graph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunicationNetwork.Nodes
{
	public class VulnerableNode : Node
	{
		private double plaintextProbability = 0.4; // Probability of plaintext data being logged

		public VulnerableNode() : base()
		{
			
		}

		public string LogPlaintext()
		{
			// Logic for logging plaintext data
			Console.WriteLine($"{Name} is logging plaintext data.");
			return "Plaintext data logged.";
		}

		public string LogEncrypted()
		{
			// Logic for logging encrypted data
			Console.WriteLine($"{Name} is logging encrypted data.");
			return "Encrypted data logged.";
		}
	}
}
