using System;
using System.Collections.Generic;
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
			Heightmap = new Grid<int>(InputReader.Read<Program>()
				.Select(line => line
					.Select(c => (c - 'a')))
				.ToList());
			Heightmap.FormatCell = (i => "" + (char)('a' + i));

			Start = Heightmap.Locations.Where(loc => Heightmap[loc] == ('S' - 'a')).First();
			Heightmap[Start] = 0;
			End = Heightmap.Locations.Where(loc => Heightmap[loc] == ('E' - 'a')).First();
			Heightmap[End] = 'z' - 'a';

			PathNode path = Part1();
			Console.WriteLine($"The result of part 1 is: {path.Steps}");
			Part2();
		}

		private static PathNode Part1()
		{
			HashSet<Location> visited = new HashSet<Location>();
			PriorityQueue<PathNode, int> explore = new PriorityQueue<PathNode, int>();
			explore.Enqueue(new PathNode(Start), 0);

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

			return null!;
		}

		private static void Part2()
		{
			var result = Heightmap.Locations.Count(loc => Heightmap[loc] == 0);

			Console.WriteLine($"The result of part 2 is: {result}");
		}

	}
}
