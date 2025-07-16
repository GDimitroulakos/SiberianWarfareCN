using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunicationNetwork
{
	public class Cipher
	{
		public static string Encrypt(string input, int key)
		{
			StringBuilder encrypted = new StringBuilder();
			foreach (char c in input)
			{
				if (char.IsLetter(c))
				{
					char offset = char.IsUpper(c) ? 'A' : 'a';
					encrypted.Append((char) ((((c - offset) + key) % 26) + offset));
				}
				else
				{
					encrypted.Append(c); // Non-letter characters remain unchanged
				}
			}
			return encrypted.ToString();
		}

		public static string Decrypt(string input, int key)
		{
			StringBuilder decrypted = new StringBuilder();
			foreach (char c in input)
			{
				if (char.IsLetter(c))
				{
					char offset = char.IsUpper(c) ? 'A' : 'a';
					decrypted.Append((char) ((((c - offset) - key + 26) % 26) + offset));
				}
				else
				{
					decrypted.Append(c); // Non-letter characters remain unchanged
				}
			}
			return decrypted.ToString();
		}
	}
}
