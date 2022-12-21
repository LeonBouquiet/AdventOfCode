using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Shared;

namespace Day18
{
	public record struct Point3D
	{
		public int X { get; set; }
		public int Y { get; set; }
		public int Z { get; set; }

		private static readonly (int, int, int)[] neighbourDisplacements = new[]
			{ (-1, 0, 0), ( 1, 0, 0), (0, -1, 0), (0,  1, 0), (0, 0, -1), ( 0, 0,  1) };

		public IEnumerable<Point3D> Neighbours
		{
			get
			{
				foreach (var disp in neighbourDisplacements)
					yield return Displace(disp.Item1, disp.Item2, disp.Item3);
			}
		}

		public Point3D(int x, int y, int z)
		{
			X = x;
			Y = y;
			Z = z;
		}

		public Point3D Displace(int dx, int dy, int dz)
		{
			return new Point3D(X + dx, Y + dy, Z + dz);
		}
	}

	public class Program
	{
		public static void Main(string[] args)
		{
			Stopwatch stopwatch = Stopwatch.StartNew();

			List<Point3D> points = InputReader.Read<Program>()
				.Select(line => line.Split(',')
					.Select(s => Int32.Parse(s))
					.ToArray())
				.Select(arr => new Point3D(arr[0], arr[1], arr[2]))
				.ToList();

			Part1(points);
			Part2(points);

			stopwatch.Stop();
			Console.WriteLine($"Elapsed time: {stopwatch.ElapsedMilliseconds}ms ({stopwatch.ElapsedTicks} ticks).");
		}

		private static void Part1(List<Point3D> points)
		{
			HashSet<Point3D> droplet = new HashSet<Point3D>(points);
			
			int openSides = 0;
			foreach(Point3D point in droplet)
				openSides += (6 - point.Neighbours.Count(nb => droplet.Contains(nb)));

			Console.WriteLine($"The result of part 1 is: {openSides}");
		}

		private static void Part2(List<Point3D> points)
		{
			HashSet<Point3D> droplet = new HashSet<Point3D>(points);

			int maxX = points.Max(p => p.X) + 1;
			int maxY = points.Max(p => p.Y) + 1;
			int maxZ = points.Max(p => p.Z) + 1;

			HashSet<Point3D> visited = new HashSet<Point3D>();
			int outsideCount = 0;

			//Do a floodfill from outside the droplet, each time we hit a part of the droplet, we've found an outside face. 
			Queue<Point3D> explore = new Queue<Point3D>();
			explore.Enqueue(new Point3D(-1, -1, -1));
			while(explore.Count > 0)
			{
				Point3D p = explore.Dequeue();
				if (visited.Contains(p) == false)
				{
					visited.Add(p);

					foreach (Point3D nb in p.Neighbours)
					{
						if (nb.X < -1 || nb.X > maxX || nb.Y < -1 || nb.Y > maxY || nb.Z < -1 || nb.Z > maxZ || visited.Contains(nb))
							continue;

						if (droplet.Contains(nb))
							outsideCount++;
						else
							explore.Enqueue(nb);
					}
				}
			}

			Console.WriteLine($"The result of part 2 is: {outsideCount}");
		}

	}
}
