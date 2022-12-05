using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Shared;

namespace Day04
{
	public class Range
	{
		public int Start { get; set; }
		public int End { get; set; }

		public Range(int start, int end)
		{
			Start = start;
			End = end;
		}

		public bool PartiallyOverlaps(Range other)
		{
			return !(this.End < other.Start || other.End < this.Start);
		}

		public bool FullyContains(Range other)
		{
			return (this.Start <= other.Start && this.End >= other.End);
		}

		public static Range Parse(string s)
		{
			int[] numbers = s
				.Split('-')
				.Select(s => Int32.Parse(s))
				.ToArray();

			return new Range(numbers[0], numbers[1]);
		}
	}

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
				.Select(line => line
					.Split(',')
					.Select(s => Range.Parse(s))
					.ToArray())
				.Count(ranges => ranges[0].FullyContains(ranges[1]) || ranges[1].FullyContains(ranges[0]));

			Console.WriteLine($"The result of part 1 is: {result}");
		}

		private static void Part2()
		{
			var result = InputReader.Read<Program>()
				.Select(line => line
					.Split(',')
					.Select(s => Range.Parse(s))
					.ToArray())
				.Count(ranges => ranges[0].PartiallyOverlaps(ranges[1]));

			Console.WriteLine($"The result of part 2 is: {result}");
		}

	}
}
