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
		//Index into the ValveMap and FlowsPerValve collections.
		public int Index { get; set; }

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

	public class PathNode
	{
		public PathNode Parent { get; set; }

		public int Minute { get; set; }

		public int CurrentIndex { get; set; }

		public int[] FlowsPerValve { get; set; }

		public int TotalPressureRelease { get; set; }

		public PathNode(PathNode parent, int currentIndex)
		{
			Parent = parent;
			Minute = parent.Minute + 1;
			CurrentIndex = currentIndex;
			FlowsPerValve = parent.FlowsPerValve.ToArray();
			TotalPressureRelease = parent.TotalPressureRelease;
		}

		public override string ToString()
		{
			return $"Minute {Minute} at valve {Program.ValveMap[CurrentIndex].Name}, total pressure released: {TotalPressureRelease}";
		}
	}

	public class Program
	{
		public static List<Valve> ValveMap = null!;

		public static void Main(string[] args)
		{
			ValveMap = ParseValveMap(InputReader.Read<Program>());

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

		private static List<PathNode> GetChildNodes(PathNode pathNode)
		{
			Valve current = ValveMap[pathNode.CurrentIndex];
			
			//Start with all possible moves to the neighbouring Valves
			List<PathNode> result = current.Tunnels
				.Select(v => new PathNode(pathNode, v.Index))
				.ToList();

			if (pathNode.FlowsPerValve[current.Index] > 0)
			{
				//Valve is still open, close it.
				PathNode openedValve = new PathNode(pathNode, pathNode.CurrentIndex);
				openedValve.TotalPressureRelease += ((30 - openedValve.Minute) * pathNode.FlowsPerValve[current.Index]);
				pathNode.FlowsPerValve[current.Index] = 0;

				result.Add(openedValve);
			}

			return result;
		}

		private static readonly Regex ValveDescriptionRegex = new Regex(@"Valve (.+) has flow rate=(\d+); tunnel(?:[s]?) lead(?:[s]?) to valve(?:[s]?) (.+)");

		private static List<Valve> ParseValveMap(IEnumerable<string> lines)
		{
			var valveDescriptions = lines
				.Select(line => ParseValveDescription(line))
				.ToList();

			var valvesByNameMap = valveDescriptions
				.Select(pair => pair.Item1)
				.ToDictionary(v => v.Name);

			foreach (var pair in valveDescriptions)
			{
				pair.Item1.Tunnels = pair.Item2
					.Select(name => valvesByNameMap[name])
					.ToList();
			}

			return valveDescriptions
				.Select(pair => pair.Item1)
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
