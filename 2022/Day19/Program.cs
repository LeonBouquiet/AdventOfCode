﻿using System;
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

		public bool IsNonNegative
		{
			get => Counts.All(c => c >= 0);
		}

		public Materials(IEnumerable<int> counts)
		{
			Counts = counts.ToArray();
		}

		public Materials(int ore = 0, int clay = 0, int obsidian = 0, int geode = 0)
		{
			Ore = ore;
			Clay = clay;
			Obsidian = obsidian;
			Geode = geode;
		}

		
		public static Materials operator* (int i, Materials materials)
		{
			return new Materials(materials.Counts.Select(c => i * c));
		}
		public static Materials operator +(Materials left, Materials right)
		{
			return new Materials(left.Counts
				.Zip(right.Counts)
				.Select(a => a.First + a.Second));
		}

		public static Materials operator -(Materials left, Materials right)
		{
			return new Materials(left.Counts
				.Zip(right.Counts)
				.Select(a => a.First - a.Second));
		}
	}

	public class Blueprint
	{
		public int Id { get; set; }

		public Materials OreRobotCost {
			get => CostsPerRobot[MaterialType.Ore]; 
			set => CostsPerRobot[MaterialType.Ore] = value;
		}

		public Materials ClayRobotCost {
			get => CostsPerRobot[MaterialType.Clay];
			set => CostsPerRobot[MaterialType.Clay] = value;
		}

		public Materials ObsidianRobotCost {
			get => CostsPerRobot[MaterialType.Obsidian];
			set => CostsPerRobot[MaterialType.Obsidian] = value;
		}

		public Materials GeodeRobotCost {
			get => CostsPerRobot[MaterialType.Geode];
			set => CostsPerRobot[MaterialType.Geode] = value;
		}

		public Materials[] CostsPerRobot { get; set; } = new Materials[4];

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

	public class GameState
	{
		public GameState Parent { get; set; }

		public int Minute { get; set;  }

		public Materials Robots { get; set; }

		public Materials Materials { get; set; }

		public GameState()
		{
			Parent = null!;
			Minute = 0;
			Robots = new Materials(ore: 1);
			Materials = new Materials();
		}

		public GameState(GameState parent)
		{
			Parent = parent;
			Minute = parent.Minute + 1;

			Robots = parent.Robots;
			Materials = parent.Materials;
		}

		public List<GameState> GenerateChildStates(Blueprint blueprint)
		{
			List<GameState> result = new List<GameState>();

			//There's always the option to not build any Robot and just collect the Materials
			GameState child = new GameState(this);
			child.Materials = child.Materials + child.Robots;
			result.Add(child);

			foreach (int materialType in new[] { MaterialType.Ore, MaterialType.Clay, MaterialType.Obsidian, MaterialType.Geode })
			{
				child = new GameState(this);

				//Buy a Robot of this type - We never try to buy more than 1, otherwise we would have bought it sooner.
				child.Materials = child.Materials - blueprint.CostsPerRobot[materialType];
				if (child.Materials.IsNonNegative)
				{
					//Collect materials from the Robots; every Robot produces one of its Materials.
					child.Materials = child.Materials + child.Robots;

					//Add the produced robot.
					child.Robots.Counts[materialType]++;
					result.Add(child);
				}
			}

			return result;
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

		private static int CalculateMaxGeodes(Blueprint blueprint)
		{
			Materials robots = new Materials(ore: 1);
			Materials materials = new Materials();

			//Have all robots produce their materials
			materials = materials + robots;


		}

		private static void Part2()
		{
			var result = InputReader.Read<Program>();

			Console.WriteLine($"The result of part 2 is: {result}");
		}
	}
}
