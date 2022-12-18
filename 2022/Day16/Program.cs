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

		public int ElephantIndex { get; set; }

		public int[] FlowsPerValve { get; set; }

		public int TotalPressureRelease { get; set; }

		public int Potential { get; private set; }

		public int Priority { get; private set; }

		public PathNode(int startingIndex)
		{
			Parent = null!;
			Minute = 0;
			CurrentIndex = startingIndex;
			ElephantIndex = startingIndex;
			FlowsPerValve = Program.ValveMap.Select(v => v.InitialFlowRate).ToArray();
		}

		public PathNode(PathNode parent)
		{
			Parent = parent;
			Minute = parent.Minute + 1;
			CurrentIndex = parent.CurrentIndex;
			ElephantIndex = parent.ElephantIndex;
			FlowsPerValve = parent.FlowsPerValve.ToArray();
			TotalPressureRelease = parent.TotalPressureRelease;
		}

		public void CloseValve()
		{
			TotalPressureRelease += ((Program.TotalMinutes - Minute) * FlowsPerValve[CurrentIndex]);
			FlowsPerValve[CurrentIndex] = 0;
		}

		public void ElephantCloseValve()
		{
			TotalPressureRelease += ((Program.TotalMinutes - Minute) * FlowsPerValve[ElephantIndex]);
			FlowsPerValve[ElephantIndex] = 0;
		}


		public void DeterminePriorityAndPotential()
		{
			//In the ideal world, the Valve with the highest pressure is right next to the second highest, etc., and
			//we can walk one step, close one Valve, walk one step, etc. all Valves from high to low.
			//Calculate the pressure we would release this way as the Potential; this is an upper bound (i.e. we could
			//never do better than) on what we could achieve in the actual setting.
			int index = 0;
			Potential = FlowsPerValve
				.Where(i => i > 0)
				.OrderByDescending(i => i)
				.Select(i => (Program.TotalMinutes - Minute - (2 * index++)) * i)
				.Where(i => i > 0)
				.Sum();

			//Lower priority is better, the Potential is important but we care more about the TotalPressureRelease.
			Priority = -(TotalPressureRelease + (Potential / 2));
		}

		public override bool Equals(object? obj)
		{
			PathNode? other = obj as PathNode;
			if (other == null)
				return false;

			if (!(this.CurrentIndex == other.CurrentIndex && this.ElephantIndex == other.ElephantIndex && this.Minute == other.Minute 
				&& this.TotalPressureRelease == other.TotalPressureRelease && this.Priority == other.Priority && this.Potential == other.Potential))
				return false;

			return Enumerable.SequenceEqual(this.FlowsPerValve, other.FlowsPerValve);
		}

		public override int GetHashCode()
		{
			return (Minute * 519) ^ TotalPressureRelease ^ Potential; 
		}

		public override string ToString()
		{
			return $"Minute {Minute} at valve {Program.ValveMap[CurrentIndex].Name}, total pressure released: {TotalPressureRelease}";
		}
	}

	public class Program
	{
		public static List<Valve> ValveMap = null!;

		public static int TotalMinutes = 30;

		public static void Main(string[] args)
		{
			ValveMap = ParseValveMap(InputReader.Read<Program>());

			Part1();
			Part2();
		}

		private static PathNode? ExplorePaths(Func<PathNode, List<PathNode>> getChildNodesFunc)
		{
			HashSet<PathNode> nodesFound = new HashSet<PathNode>();

			PriorityQueue<PathNode, int> queue = new PriorityQueue<PathNode, int>();
			PathNode root = new PathNode(ValveMap.First(v => v.Name == "AA").Index);
			queue.Enqueue(root, root.Priority);

			int iteration = 0;
			PathNode? bestSolution = null;
			while (queue.Count > 0)
			{
				PathNode currentNode = queue.Dequeue();
				if (nodesFound.Contains(currentNode))
					continue;

				nodesFound.Add(currentNode);

				//If the currentNode can't possibly do better than the bestSolution, don't explore it.
				if (bestSolution != null && currentNode.TotalPressureRelease + currentNode.Potential <= bestSolution.TotalPressureRelease)
					continue;

				if (currentNode.Minute == TotalMinutes)
				{
					if (bestSolution == null) 
					{
						Console.WriteLine($"Found the first solution at iteration {iteration}! {currentNode}");
						bestSolution = currentNode;
					}
					else if(currentNode.TotalPressureRelease > bestSolution.TotalPressureRelease)
					{
						Console.WriteLine($"Found a better solution at iteration {iteration}: {currentNode}");
						bestSolution = currentNode;
					}
				}
				else
				{
					var childNodes = getChildNodesFunc(currentNode);
					queue.EnqueueRange(childNodes.Select(n => (n, n.Priority)));
				}

				if(iteration++ % 100000 == 0)
					Console.WriteLine($"Iteration {iteration}, currently {queue.Count} elements queued.");
			}

			return bestSolution;
		}

		private static void Part1()
		{
			PathNode? bestSolution = ExplorePaths(GetChildNodesForPart1);

			Console.WriteLine($"The result of part 1 is: {bestSolution!.TotalPressureRelease}");
		}

		private static void Part2()
		{
			PathNode? bestSolution = ExplorePaths(GetChildNodesForPart1);

			Console.WriteLine($"The result of part 2 is: {bestSolution!.TotalPressureRelease}");
		}

		private static List<PathNode> GetChildNodesForPart1(PathNode pathNode)
		{
			if (pathNode.Minute >= TotalMinutes)
				return new List<PathNode>();

			//Start with all possible moves to the neighbouring Valves
			Valve current = ValveMap[pathNode.CurrentIndex];
			List<PathNode> result = new List<PathNode>();
			foreach(int destIndex in current.Tunnels.Select(v => v.Index))
			{
				PathNode child = new PathNode(pathNode);
				child.CurrentIndex = destIndex;
				child.DeterminePriorityAndPotential();

				result.Add(child);
			}

			if (pathNode.FlowsPerValve[current.Index] > 0)
			{
				//Valve is still open, close it.
				PathNode child = new PathNode(pathNode);
				child.CloseValve();
				child.DeterminePriorityAndPotential();

				result.Add(child);
			}

			return result;
		}

		private static List<PathNode> GetChildNodesForPart2(PathNode pathNode)
		{
			if (pathNode.Minute >= TotalMinutes)
				return new List<PathNode>();

			//Start with all possible moves to the neighbouring Valves
			Valve current = ValveMap[pathNode.CurrentIndex];
			List<PathNode> result = new List<PathNode>();
			foreach (int destIndex in current.Tunnels.Select(v => v.Index))
			{
				PathNode child = new PathNode(pathNode);
				child.CurrentIndex = destIndex;
				child.DeterminePriorityAndPotential();

				result.Add(child);
			}

			if (pathNode.FlowsPerValve[current.Index] > 0)
			{
				//Valve is still open, close it.
				PathNode child = new PathNode(pathNode);
				child.CloseValve();
				child.DeterminePriorityAndPotential();

				result.Add(child);
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
