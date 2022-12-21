using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Shared;

namespace Day12
{
	public class Program
	{
		public static void Main(string[] args)
		{
			Grid<int> heightmap = new Grid<int>(InputReader.Read<Program>()
				.Select(line => line
					.Select(c => (c - 'a')))
				.ToList());
			heightmap.FormatCell = (i => "" + (char)('a' + i));

			Location start = heightmap.Locations.Where(loc => heightmap[loc] == ('S' - 'a')).First();
			heightmap[start] = 0;
			Location end = heightmap.Locations.Where(loc => heightmap[loc] == ('E' - 'a')).First();
			heightmap[end] = 'z' - 'a';

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

	}
}
