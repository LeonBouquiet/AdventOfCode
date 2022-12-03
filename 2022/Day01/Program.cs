using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Shared;

namespace Day01
{
	public class Program
	{
		public static void Main(string[] args)
		{
			Part1();
			Part2();

			Console.WriteLine("Press enter to exit...");
			Console.ReadLine();
		}

		private static void Part1()
		{
			int? result = InputReader.Read<Program>()
				.Select(s => (int?)(!string.IsNullOrWhiteSpace(s) ? Int32.Parse(s) : null))
				.PartitionIntoRangesBy(i => i == null, includeDelimiters: false)
				.Select(rng => rng.Sum())
				.Max();

			Console.WriteLine($"The result of part 1 is: {result}");
		}

		private static void Part2()
		{
			int? result = InputReader.Read<Program>()
				.Select(s => (int?)(!string.IsNullOrWhiteSpace(s) ? Int32.Parse(s) : null))
				.PartitionIntoRangesBy(i => i == null, includeDelimiters: false)
				.Select(rng => rng.Sum())
				.OrderByDescending(i => i)
				.Take(3)
				.Sum();

			Console.WriteLine($"The result of part 2 is: {result}");
		}

	}
}
