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
				Execute1(move, ref head, ref tail);

			int result = _distinctTailLocations.Count;
			Console.WriteLine($"The result of part 1 is: {result}");
		}

		private static void Execute1(Direction move, ref Location head, ref Location tail)
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

		private static void Part2()
		{
			List<Location> rope = Enumerable.Range(0, 10)
				.Select(i => new Location(0, 0))
				.ToList();

			_distinctTailLocations.Clear();
			_distinctTailLocations[rope.Last()] = rope.Last();

			foreach (Direction move in _instructions)
				Execute2(move, rope);

			int result = _distinctTailLocations.Count;
			Console.WriteLine($"The result of part 2 is: {result}");
		}

		private static void Execute2(Direction move, List<Location> rope)
		{
			rope[0] = rope[0].Displace(move);

			for(int tailIndex = 1; tailIndex < rope.Count; tailIndex++)
			{
				if (AreAdjacent(rope[tailIndex - 1], rope[tailIndex]))
					break;

				int displacementX = Normalize(rope[tailIndex - 1].X - rope[tailIndex].X);
				int displacementY = Normalize(rope[tailIndex - 1].Y - rope[tailIndex].Y);
				rope[tailIndex] = rope[tailIndex].Displace(displacementX, displacementY);

				if(tailIndex == rope.Count - 1)
					_distinctTailLocations[rope[tailIndex]] = rope[tailIndex];
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

	}
}
