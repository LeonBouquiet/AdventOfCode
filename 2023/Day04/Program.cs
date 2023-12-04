using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.WebSockets;
using System.Runtime.ExceptionServices;
using Shared;

namespace Day04
{
	public class Program
	{
		public static void Main(string[] args)
		{
			Part1();
			Part2();
		}

		private static void Part1()
		{
			var result = InputReader.Read<Program>()
				.Select(line => ParseCard(line))
				.Select(card => card.Item2.Intersect(card.Item3).Count())
				.Select(matches => matches == 0 ? 0 : (1 << (matches - 1)))
				.Sum();

			Console.WriteLine($"The result of part 1 is: {result}");
		}

		private static void Part2()
		{
			var matchesPerCard = InputReader.Read<Program>()
				.Select(line => ParseCard(line))
				.Select(card => new { Id = card.Item1, Matches = card.Item2.Intersect(card.Item3).Count() })
				.ToList();

			int[] cardCopies = new int[matchesPerCard.Count];
			for(int index = 0; index < matchesPerCard.Count; index++)
			{
				int multiplier = ++cardCopies[index];

				for (int count = 1; count <= matchesPerCard[index].Matches; count++)
					cardCopies[index + count] += multiplier;
			}

			var result = cardCopies.Sum();
			Console.WriteLine($"The result of part 2 is: {result}");
		}

		private static Tuple<int, int[], int[]> ParseCard(string line)
		{
			string[] parts = line.Split(':', '|');
			int id = Int32.Parse(parts[0].Substring(4));

			int[] winningNumbers = parts[1]
				.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
				.Select(s => Int32.Parse(s))
				.ToArray();

			int[] numbersIHave = parts[2]
				.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
				.Select(s => Int32.Parse(s))
				.ToArray();

			return new Tuple<int, int[], int[]>(id, winningNumbers, numbersIHave);
		}
	}
}
