using System;
using System.Collections.Generic;
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

		public override string ToString()
		{
			return $"Center at {Center}, radius {Radius}";
		}
	}

	public class Program
	{
		public static void Main(string[] args)
		{
			var result = InputReader.Read<Program>()
				.Select(line => ParseBeaconSensorPair(line))
				.Select(pair => new Diamond(pair.Item1, pair.Item1.ManhattanDistanceTo(pair.Item2)))
				.ToList();

			Part1();
			Part2();
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

		private static void Part1()
		{
			var result = InputReader.Read<Program>();

			Console.WriteLine($"The result of part 1 is: {result}");
		}

		private static void Part2()
		{
			var result = InputReader.Read<Program>();

			Console.WriteLine($"The result of part 2 is: {result}");
		}

	}
}
