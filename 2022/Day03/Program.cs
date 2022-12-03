using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Shared;

namespace Day03
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
				.Select(line => new { First = line.Substring(0, line.Length / 2), Second = line.Substring(line.Length / 2) })
				.Select(pair => pair.First.Intersect(pair.Second).Single())
				.Select(c => (c < 'a') ? (c - 'A' + 27) : (c - 'a' + 1))
				.Sum();

			Console.WriteLine($"The result of part 1 is: {result}");
		}

		private static void Part2()
		{
			var result = InputReader.Read<Program>()
				.PartitionIntoRangesOfN(3)
				.Select(range => (range[0].Intersect(range[1])).Intersect(range[2]).Single())
				.Select(c => (c < 'a') ? (c - 'A' + 27) : (c - 'a' + 1))
				.Sum();

			Console.WriteLine($"The result of part 2 is: {result}");
		}

	}
}
