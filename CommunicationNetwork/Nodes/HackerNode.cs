using CommunicationNetwork.Graph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace CommunicationNetwork.Nodes
{
	public class HackerNode : Node
	{
		private double _modifyProbability;
		private double _dropProbability;
		private Random _rng = new Random();

		/// <param name="modifyProbability">Chance [0..1] to alter packet payload</param>
		/// <param name="dropProbability">Chance [0..1] to drop packet entirely</param>
		public HackerNode(double modifyProbability = 0.5, double dropProbability = 0.2) : base()
		{
			_modifyProbability = modifyProbability;
			_dropProbability = dropProbability;
		}


		public string IntegrityAttack(string payload)
		{
			// alter the payload to simulate a data integrity attack with random letters
			var alteredPayload = new StringBuilder();
			foreach (char c in payload)
			{
				if (char.IsLetter(c))
				{
					// Randomly change the character to a different letter
					char newChar = (char) ('A' + _rng.Next(0, 26));
					alteredPayload.Append(newChar);
				}
				else
				{
					// Non-letter characters remain unchanged
					alteredPayload.Append(c);
				}
			}
			return alteredPayload.ToString();
		}

		public override void Trasmit(Packet packet)
		{
			Console.WriteLine("Reached Hacker Node:");
			Console.WriteLine($"\t{Name} intercepted packet: {packet.Payload}");

			// Decide whether to drop
			if (_rng.NextDouble() < _dropProbability)
			{
				Console.WriteLine($"\t{Name} dropped the packet.");
				packet.IsDropped = true;
				return;
			}

			// Decide whether to modify
			if (_rng.NextDouble() < _modifyProbability)
			{
				var original = packet.Payload;
				packet.Payload = $"{packet.Payload}-[HACKED]";
				Console.WriteLine($"\t{Name} modified payload from '{original}' to '{packet.Payload}'.");
			}

			Console.WriteLine();
		}
	}
}
