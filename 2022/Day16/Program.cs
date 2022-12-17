using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Shared;

namespace Day16
{
	public class Valve
	{
		public string Name { get; set; }

		public List<Valve> Tunnels { get; set; } = new List<Valve>();

		public int InitialFlowRate { get; set; }

		public Valve(string name, int initialFlowRate)
		{
			Name = name;
			InitialFlowRate = initialFlowRate;
		}

		public override string ToString()
		{
			return $"Valve {Name} with initial flow rate {InitialFlowRate}, and {Tunnels.Count} tunnels.";
		}
	}

	public class Program
	{
		public static void Main(string[] args)
		{
			List<Valve> valves = ParseValveGraph(InputReader.Read<Program>());

			Part1();
			Part2();
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

		private static readonly Regex ValveDescriptionRegex = new Regex(@"Valve (.+) has flow rate=(\d+); tunnel(?:[s]?) lead(?:[s]?) to valve(?:[s]?) (.+)");

		private static List<Valve> ParseValveGraph(IEnumerable<string> lines)
		{
			var valveDescriptions = lines
				.Select(line => ParseValveDescription(line))
				.ToList();

			Dictionary<string, Valve> valveMap = valveDescriptions
				.Select(pair => pair.Item1)
				.ToDictionary(v => v.Name);

			foreach (var pair in valveDescriptions)
			{
				pair.Item1.Tunnels = pair.Item2
					.Select(name => valveMap[name])
					.ToList();
			}

			return valveMap.Values
				.OrderBy(v => v.Name)
				.ToList();
		}

		private static (Valve, string[]) ParseValveDescription(string line)
		{
			Match match = ValveDescriptionRegex.Match(line);
			if (match.Success == false)
				throw new Exception();

			string name = match.Groups[1].Value;
			int flowRate = Int32.Parse(match.Groups[2].Value);
			string[] tunnelNames = match.Groups[3].Value.Split(',').Select(s => s.Trim()).ToArray();

			Valve valve = new Valve(name, flowRate);
			return new(valve, tunnelNames);
		}
	}
}
