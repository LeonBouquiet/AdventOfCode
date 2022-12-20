using System;
using System.Collections.Generic;
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
			Part1();
			Part2();
		}

		private static void Part1()
		{
			var points = InputReader.Read<Program>()
				.Select(line => line.Split(',')
					.Select(s => Int32.Parse(s))
					.ToArray())
				.Select(arr => new Point3D(arr[0], arr[1], arr[2]))
				.ToList();

			HashSet<Point3D> allPoints = new HashSet<Point3D>(points);
			
			int openSides = 0;
			foreach(Point3D point in allPoints)
				openSides += (6 - point.Neighbours.Count(nb => allPoints.Contains(nb)));

			Console.WriteLine($"The result of part 1 is: {openSides}");
		}

		private static void Part2()
		{
			var result = InputReader.Read<Program>();

			Console.WriteLine($"The result of part 2 is: {result}");
		}

	}
}
