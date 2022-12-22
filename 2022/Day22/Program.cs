using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using Shared;

namespace Day22
{


	public class Program
	{
		private static Grid<char> Map = null!;

		private static List<(int, char)> Instructions = null!;

		private static Location Current = null!;

		private static Direction Direction = Direction.Right;


		public static void Main(string[] args)
		{
			List<string> lines = InputReader.Read<Program>()
				.ToList();

			Map = ParseMap(lines.Take(lines.Count - 2));
			Instructions = ParseInstructions(lines.Last());
			Current = Map
				.GetLocationsInRow(y: 0)
				.First(loc => Map[loc] == '.');

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

		private static List<(int, char)> ParseInstructions(string line)
		{
			List<(int, char)> result = line.PartitionIntoRangesBy(c => (c == 'L' || c == 'R'), includeDelimiters: true)
				.Select(chars => (Int32.Parse(new string(chars.Take(chars.Count - 1).ToArray())), chars.Last()))
				.ToList();

			return result;
		}

		private static void Part1()
		{
			foreach (var ins in Instructions)
				PerformInstruction(ins.Item1, ins.Item2);

			//Rows and columns are base-1!
			int result = (Current.Y + 1) * 1000 + (Current.X + 1) * 4 + Array.IndexOf(CircularDirections, Direction);

			Console.WriteLine($"The result of part 1 is: {result}");
		}

		private static void Part2()
		{
			var result = InputReader.Read<Program>();

			Console.WriteLine($"The result of part 2 is: {result}");
		}

		private static void PerformInstruction(int stepCount, char rotate)
		{
			for(int index = 0; index < stepCount; index++)
			{
				Location dest = Current.Displace(Direction);
				if(Map.IsInBounds(dest) == false || Map[dest] == ' ')
				{
					//Teleport to the other side
					Direction opposite = DirectionHelper.Opposite(Direction);
					Location lastValidLocation = Current;
					for (Location loc = Current; Map.IsInBounds(loc) && Map[loc] != ' '; loc = loc.Displace(opposite))
						lastValidLocation = loc;

					dest = lastValidLocation;
				}

				if (Map[dest] != '#')
					Current = dest;
				else
					break;  //Wall, stop walking.
			}

			Direction = Rotate(Direction, rotate);
		}

		private static Direction[] CircularDirections = new Direction[] { Direction.Right, Direction.Down, Direction.Left, Direction.Up };

		private static Direction Rotate(Direction current, char rotation)
		{
			int index = Array.IndexOf(CircularDirections, current);
			int offset = (rotation == 'L') ? -1 : 1;
			return CircularDirections[(index + offset + 4) % 4];
		}
	}
}
