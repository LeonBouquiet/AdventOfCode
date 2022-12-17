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

		public int Potential { get; private set; }

		public int Priority { get; private set; }

		public PathNode(int startingIndex)
		{
			Parent = null!;
			Minute = 0;
			CurrentIndex = startingIndex;
			FlowsPerValve = Program.ValveMap.Select(v => v.InitialFlowRate).ToArray();
		}

		public PathNode(PathNode parent, int moveToIndex)
		{
			Parent = parent;
			Minute = parent.Minute + 1;
			CurrentIndex = moveToIndex;
			FlowsPerValve = parent.FlowsPerValve.ToArray();
			TotalPressureRelease = parent.TotalPressureRelease;

			DeterminePriorityAndPotential();
		}

		public PathNode(PathNode parent, int moveToIndex, int valveToClose)
		{
			Parent = parent;
			Minute = parent.Minute + 1;
			CurrentIndex = moveToIndex;
			FlowsPerValve = parent.FlowsPerValve.ToArray();
			TotalPressureRelease = parent.TotalPressureRelease + ((30 - Minute) * parent.FlowsPerValve[valveToClose]);
			FlowsPerValve[valveToClose] = 0;

			DeterminePriorityAndPotential();
		}

		private void DeterminePriorityAndPotential()
		{
			int minutesRemaining = 30 - Minute;
			Potential = FlowsPerValve
				.Where(i => i > 0)
				.OrderByDescending(i => i)
				.Select(i => i * minutesRemaining--)
				.Where(i => i > 0)
				.Sum();

			Priority = -(TotalPressureRelease + (Potential / 2));
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
			PathNode root = new PathNode(ValveMap.First(v => v.Name == "AA").Index);
			PriorityQueue<PathNode, int> queue = new PriorityQueue<PathNode, int>();
			queue.Enqueue(root, root.Priority);

			int iteration = 0;
			PathNode? bestSolution = null;
			while (queue.Count > 0)
			{
				PathNode currentNode = queue.Dequeue();
				if (currentNode.Minute == 30)
				{
					if (bestSolution == null) 
					{
						Console.WriteLine($"Found the first solution! {currentNode}");
						bestSolution = currentNode;
					}
					else if(currentNode.TotalPressureRelease > bestSolution.TotalPressureRelease)
					{
						Console.WriteLine($"Found a better solution: {currentNode}");
						bestSolution = currentNode;
					}
				}
				else
				{
					var childNodes = GetChildNodes(currentNode);
					queue.EnqueueRange(childNodes.Select(n => (n, n.Priority)));
				}

				if(iteration++ % 100000 == 0)
					Console.WriteLine($"Iteration {iteration}, currently {queue.Count} elements queued.");
			}

			//Console.WriteLine($"The result of part 1 is: {result}");
		}

		private static void Part2()
		{
			var result = InputReader.Read<Program>();

			Console.WriteLine($"The result of part 2 is: {result}");
		}

		private static List<PathNode> GetChildNodes(PathNode pathNode)
		{
			if (pathNode.Minute >= 30)
				return new List<PathNode>();

			//Start with all possible moves to the neighbouring Valves
			Valve current = ValveMap[pathNode.CurrentIndex];
			List<PathNode> result = current.Tunnels
				.Select(v => new PathNode(pathNode, v.Index))
				.ToList();

			if (pathNode.FlowsPerValve[current.Index] > 0)
			{
				//Valve is still open, close it.
				result.Add(new PathNode(pathNode, pathNode.CurrentIndex, current.Index));
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

			List<Valve> valveMap = valveDescriptions
				.Select(pair => pair.Item1)
				.OrderBy(v => v.Name)
				.ToList();

			for (int index = 0; index < valveMap.Count; index++)
				valveMap[index].Index = index;

			return valveMap;
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
