using CommunicationNetwork.Graph;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace CommunicationNetwork
{
	public class Packet
	{
		/// <summary>
		/// Represents a packet in a communication network.
		/// A packet is a formatted unit of data carried by a packet-switched network.
		/// </summary>
		public string Payload { get; set; }
		public string Signature { get; }
		public bool IsDropped { get; set; } = false;
		public bool IsEncrypted { get; set; } = false;
		public int Key { get; set; } = 0;

		public Packet(string payload)
		{
			Payload = payload;
			Signature = HashSHA256(payload);
		}

		public static string HashSHA256(string input)
		{
			using (SHA256 sha256 = SHA256.Create())
			{
				byte[] inputBytes = Encoding.UTF8.GetBytes(input);
				byte[] hashBytes = sha256.ComputeHash(inputBytes);
				return Convert.ToHexString(hashBytes); 
			}
		}
	}
}
