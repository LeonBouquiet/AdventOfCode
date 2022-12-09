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

		private static Location _head = null!;
		private static Location _tail = null!;
		private static Dictionary<Location, Location> _distinctTailLocations = new Dictionary<Location, Location>();

		public static void Main(string[] args)
		{
			Part1();
			Part2();
		}

		private static void Part1()
		{
			_head = new Location(0, 0);
			_tail = new Location(0, 0);
			_distinctTailLocations[_tail] = _tail;

			var instructions = InputReader.Read<Program>()
				.Select(line => line.Split(' '))
				.Select(parts => new { Direction = CharToDirectionMap[parts[0][0]], Count = Int32.Parse(parts[1]) })
				.SelectMany(a => Enumerable.Range(0, a.Count).Select(i => a.Direction));

			foreach (Direction move in instructions)
				Execute(move);

			int result = _distinctTailLocations.Count;
			Console.WriteLine($"The result of part 1 is: {result}");
		}

		private static void Execute(Direction move)
		{
			_head = _head.Displace(move);

			if(AreAdjacent(_head, _tail) == false)
			{
				int displacementX = Normalize(_head.X - _tail.X);
				int displacementY = Normalize(_head.Y - _tail.Y);
				_tail = _tail.Displace(displacementX, displacementY);
				_distinctTailLocations[_tail] = _tail;
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
