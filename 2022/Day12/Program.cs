using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Shared;

namespace Day12
{
	public class PathNode
	{
		public PathNode Parent { get; set; }

		public Location Location { get; set; }

		public int Steps { get; set; }

		public PathNode(Location loc)
		{
			Parent = null!;
			Location = loc;
			Steps = 0;
		}

		public PathNode(PathNode parent, Location location)
		{
			Parent = parent;
			Location = location;
			Steps = parent.Steps + 1;
		}

		public override string ToString() => $"{Location}";
	}

	public class Program
	{
		private static Grid<int> Heightmap = null!;

		private static Location Start;

		private static Location End;

		public static void Main(string[] args)
		{
			Stopwatch stopwatch = Stopwatch.StartNew();

			Heightmap = new Grid<int>(InputReader.Read<Program>()
				.Select(line => line
					.Select(c => (c - 'a')))
				.ToList());
			Heightmap.FormatCell = (i => "" + (char)('a' + i));

			Start = Heightmap.Locations.Where(loc => Heightmap[loc] == ('S' - 'a')).First();
			Heightmap[Start] = 0;
			End = Heightmap.Locations.Where(loc => Heightmap[loc] == ('E' - 'a')).First();
			Heightmap[End] = 'z' - 'a';

			Part1();
			Part2();

			stopwatch.Stop();
			Console.WriteLine($"Elapsed time: {stopwatch.ElapsedMilliseconds}ms ({stopwatch.ElapsedTicks} ticks).");
		}

		private static void Part1()
		{
			PathNode? result = FindShortestPath(Start);
			Console.WriteLine($"The result of part 1 is: {result!.Steps}");
		}

		private static void Part2()
		{
			List<Location> lowestLocations = Heightmap.Locations
				.Where(loc => Heightmap[loc] == 0)
				.ToList();

			int iteration = 0;
			int shortestPathLength = Int32.MaxValue;
			foreach(Location start in lowestLocations)
			{
				iteration++;

				PathNode? path = FindShortestPath(start);
				if (path == null)
					continue;

				if(path.Steps < shortestPathLength)
				{
					shortestPathLength = path.Steps;
					Console.WriteLine($"Iteration {iteration}: Found a new shortest path of length {shortestPathLength}.");
				}
			}

			Console.WriteLine($"The result of part 2 is: {shortestPathLength}");
		}

		private static PathNode? FindShortestPath(Location start)
		{
			HashSet<Location> visited = new HashSet<Location>();
			PriorityQueue<PathNode, int> explore = new PriorityQueue<PathNode, int>();
			explore.Enqueue(new PathNode(start), 0);

			while(explore.Count > 0)
			{
				PathNode current = explore.Dequeue();
				if (visited.Contains(current.Location))
					continue;

				visited.Add(current.Location);
				foreach (Location nb in current.Location.CoreNeighbours)
				{
					if (!Heightmap.IsInBounds(nb) || visited.Contains(nb))
						continue;

					if (Heightmap[nb] <= Heightmap[current.Location] + 1) 
					{
						PathNode neighbouringNode = new PathNode(current, nb);
						int dist = nb.ManhattanDistanceTo(End);
						if (dist > 0)
							explore.Enqueue(neighbouringNode, current.Steps + 1 + dist);
						else
							return neighbouringNode;
					}
				}
			}

			return null;
		}


	}
}
