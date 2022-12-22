using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Shared;

namespace Day22
{


	public class Program
	{
		private static Grid<char> Map = null!;

		private static List<(int, char)> PathDescription = null!;

		private static Direction[] CircularDirections = new Direction[] { Direction.Right, Direction.Down, Direction.Left, Direction.Up };

		public static void Main(string[] args)
		{
			List<string> lines = InputReader.Read<Program>()
				.ToList();

			Map = ParseMap(lines.Take(lines.Count - 2));
			PathDescription = ParsePathDescription(lines.Last());

			Part1();
			Part2();
		}

		private static Grid<char> ParseMap(IEnumerable<string> mapLines)
		{
			//Make all lines equally wide.
			int mapWidth = mapLines.Max(line => line.Length);
			mapLines = mapLines
				.Select(line => line + new string(' ', mapWidth - line.Length))
				.ToList();

			Grid<char> result = new Grid<char>(mapLines
				.Select(line => line.AsEnumerable())
				.ToList());

			return result;
		}

		private static List<(int, char)> ParsePathDescription(string line)
		{
			List<(int, char)> result = line.PartitionIntoRangesBy(c => (c == 'L' || c == 'R'), includeDelimiters: true)
				.Select(chars => (Int32.Parse(new string(chars.Take(chars.Count - 1).ToArray())), chars.Last()))
				.ToList();

			return result;
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
