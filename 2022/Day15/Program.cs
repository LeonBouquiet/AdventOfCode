using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Shared;

namespace Day15
{
	public class Diamond
	{
		public Location Center { get; set; }

		public int Radius { get; set; }

		public Diamond(Location center, int radius)
		{
			Center = center;
			Radius = radius;
		}

		public (int, int)? GetCoverageAt(int y)
		{
			if (Center.Y - Radius > y || Center.Y + Radius < y)
				return null;

			var widthAtY = Radius - Math.Abs(y - Center.Y);
			return new(Center.X - widthAtY, Center.X + widthAtY);
		}

		public override string ToString()
		{
			return $"Center at {Center}, radius {Radius}";
		}
	}

	public class NumberRange
	{
		public int Start { get; set; }
		public int End { get; set; }        //Inclusive

		public NumberRange(int start, int end)
		{
			Start = start;
			End = end;
		}

		public override string ToString() => $"[{Start}, {End}]";

		public bool Overlaps(NumberRange other)
		{
			return !(this.End < other.Start || other.End < this.Start);
		}

		public NumberRange Union(NumberRange other)
		{
			return new NumberRange(Math.Min(this.Start, other.Start), Math.Max(this.End, other.End));
		}

		public static List<NumberRange> UnionAll(IEnumerable<NumberRange> numberRanges)
		{
			List<NumberRange> merged = numberRanges.ToList();

			//Not sure if O(n^2) is needed here, but it works well as long as the counts are low...
			for (int targetIndex = 0; targetIndex < merged.Count; targetIndex++)
			{
				for (int index = 0; index < merged.Count; index++)
				{
					if (index != targetIndex && merged[index].Overlaps(merged[targetIndex]))
					{
						merged[index] = merged[index].Union(merged[targetIndex]);
						merged.RemoveAt(targetIndex);

						//Continue with this combined NumberRange, don't advance
						index--;
						targetIndex = (targetIndex > 0) ? targetIndex - 1: targetIndex;
					}
				}
			}

			return merged;
		}

		public NumberRange Intersection(NumberRange other)
		{
			return new NumberRange(Math.Max(this.Start, other.Start), Math.Min(this.End, other.End));
		}

	}

	public class Program
	{
		public static void Main(string[] args)
		{
			Stopwatch stopwatch = Stopwatch.StartNew();

			List<Diamond> diamonds = InputReader.Read<Program>()
				.Select(line => ParseBeaconSensorPair(line))
				.Select(pair => new Diamond(pair.Item1, pair.Item1.ManhattanDistanceTo(pair.Item2)))
				.ToList();

			Part1(diamonds);
			Part2(diamonds);

			stopwatch.Stop();
			Console.WriteLine($"Elapsed time: {stopwatch.ElapsedMilliseconds}ms ({stopwatch.ElapsedTicks} ticks).");
		}

		private static (Location, Location) ParseBeaconSensorPair(string line)
		{
			int[] numbers = line
				.Split('=', ',', ':')
				.Where(part => !string.IsNullOrWhiteSpace(part) && part.All(c => ((c >= '0' && c <= '9') || c == '-')))
				.Select(part => Int32.Parse(part))
				.ToArray();

			return new(new Location(numbers[0], numbers[1]), new Location(numbers[2], numbers[3]));
		}

		private static void Part1(List<Diamond> diamonds)
		{
			var result = diamonds
				.Select(d => d.GetCoverageAt(2000000))
				.Where(pair => pair != null)
				.SelectMany(pair => Enumerable.Range(pair.Value.Item1, pair.Value.Item2 - pair.Value.Item1))
				.Distinct()
				.Count();

			Console.WriteLine($"The result of part 1 is: {result}");
		}

		private static void Part2(List<Diamond> diamonds)
		{
			for (int y = 0; y < 4000000; y++)
			{
				List<NumberRange> numberRanges = diamonds
					.Select(d => d.GetCoverageAt(y))
					.Where(pair => pair != null)
					.Select(pair => new NumberRange(pair.Value.Item1, pair.Value.Item2))
					.OrderBy(rng => rng.Start)
					.ToList();

				List<NumberRange> merged = NumberRange.UnionAll(numberRanges);
				if(merged.Count > 1)
				{
					merged = merged
						.Select(rng => rng.Intersection(new NumberRange(0, 4000000)))
						.ToList();

					int xGap = merged[0].End + 1;
					long result = (long)xGap * 4000000 + y;
					Console.WriteLine($"The result of part 2 is: {result}");

					return;
				}
			}
		}

	}
}
