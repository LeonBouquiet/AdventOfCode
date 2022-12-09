using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Shared;

namespace Day09
{
	public class Program
	{
		private static readonly Dictionary<char, Direction> CharToDirectionMap = new Dictionary<char, Direction>()
		{
			{ 'U', Direction.Up },
			{ 'D', Direction.Down },
			{ 'L', Direction.Left },
			{ 'R', Direction.Right }
		};


		private static List<Direction> _instructions = null!;
		private static Dictionary<Location, Location> _distinctTailLocations = new Dictionary<Location, Location>();

		public static void Main(string[] args)
		{
			_instructions = InputReader.Read<Program>()
				.Select(line => line.Split(' '))
				.Select(parts => new { Direction = CharToDirectionMap[parts[0][0]], Count = Int32.Parse(parts[1]) })
				.SelectMany(a => Enumerable.Range(0, a.Count).Select(i => a.Direction))
				.ToList();

			Part1();
			Part2();
		}

		private static void Part1()
		{
			Location head = new Location(0, 0);
			Location tail = new Location(0, 0);

			_distinctTailLocations.Clear();
			_distinctTailLocations[tail] = tail;

			foreach (Direction move in _instructions)
				Execute(move, ref head, ref tail);

			int result = _distinctTailLocations.Count;
			Console.WriteLine($"The result of part 1 is: {result}");
		}

		private static void Execute(Direction move, ref Location head, ref Location tail)
		{
			head = head.Displace(move);

			if(AreAdjacent(head, tail) == false)
			{
				int displacementX = Normalize(head.X - tail.X);
				int displacementY = Normalize(head.Y - tail.Y);
				tail = tail.Displace(displacementX, displacementY);
				_distinctTailLocations[tail] = tail;
			}
		}

		private static bool AreAdjacent(Location l1, Location l2)
		{
			return (Math.Abs(l1.X - l2.X) <= 1) && (Math.Abs(l1.Y - l2.Y) <= 1);
		}

		private static int Normalize(int value)
		{
			if (value >= 0)
				return Math.Min(value, 1);
			else
				return Math.Max(value, -1);
		}

		private static void Part2()
		{
			var result = InputReader.Read<Program>();

			Console.WriteLine($"The result of part 2 is: {result}");
		}

	}
}
