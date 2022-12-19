using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Shared;

namespace Day19
{
	public static class MaterialType
	{
		public const int Ore = 0;
		public const int Clay = 1;
		public const int Obsidian = 2;
		public const int Geode = 3;
	}

	public record struct Materials
	{
		public int[] Counts { get; set; } = new int[4];

		public int Ore { 
			get => Counts[MaterialType.Ore];
			set => Counts[MaterialType.Ore] = value;
		}

		public int Clay {
			get => Counts[MaterialType.Clay];
			set => Counts[MaterialType.Clay] = value;
		}

		public int Obsidian
		{
			get => Counts[MaterialType.Obsidian];
			set => Counts[MaterialType.Obsidian] = value;
		}

		public int Geode {
			get => Counts[MaterialType.Geode];
			set => Counts[MaterialType.Geode] = value;
		}

		public Materials(int ore = 0, int clay = 0, int obsidian = 0, int geode = 0)
		{
			Ore = ore;
			Clay = clay;
			Obsidian = obsidian;
			Geode = geode;
		}
	}

	public class Blueprint
	{
		public int Id { get; set; }

		public Materials OreRobotCost { get; set; }

		public Materials ClayRobotCost { get; set; }

		public Materials ObsidianRobotCost { get; set; }

		public Materials GeodeRobotCost { get; set; }

		private static readonly Regex CostRegex = new Regex(@"Each (?<name>\S+) robot costs (?<count1>\d+) (\S+)(?: and (?<count2>\d+) (\S+))?");

		public static Blueprint Parse(string line)
		{
			string[] parts = line.Split(new char[] { '.', ':' });

			Blueprint result = new Blueprint()
			{
				Id = ExtractNumbers(parts[0])[0],
				OreRobotCost = new Materials(ore: ExtractNumbers(parts[1])[0]),
				ClayRobotCost = new Materials(ore: ExtractNumbers(parts[2])[0]),
				ObsidianRobotCost = new Materials(ore: ExtractNumbers(parts[3])[0], clay: ExtractNumbers(parts[3])[1]),
				GeodeRobotCost = new Materials(ore: ExtractNumbers(parts[4])[0], obsidian: ExtractNumbers(parts[4])[1]),
			};

			return result;
		}

		private static int[] ExtractNumbers(string text)
		{
			return text.Split(' ', '.')
				.Where(part => !string.IsNullOrEmpty(part) && part.All(c => (c >= '0' && c <= '9')))
				.Select(part => Int32.Parse(part))
				.ToArray();
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
			List<Blueprint> blueprints = InputReader.Read<Program>()
				.Select(line => Blueprint.Parse(line))
				.ToList();

			int result = 0;
			Console.WriteLine($"The result of part 1 is: {result}");
		}

		private static void Part2()
		{
			var result = InputReader.Read<Program>();

			Console.WriteLine($"The result of part 2 is: {result}");
		}
	}
}
