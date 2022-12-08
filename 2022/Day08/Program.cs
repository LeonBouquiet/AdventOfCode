using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Transactions;
using Shared;

namespace Day08
{
	public class Program
	{
		private static Grid<int> _grid = null!;

		public static void Main(string[] args)
		{
			_grid = new Grid<int>(InputReader.Read<Program>()
				.Select(line => line
					.Select(c => (int)(c - '0')))
				.ToList());

			Part1();
			Part2();
		}

		private static void Part1()
		{
			var result = _grid.Locations
				.Where(loc => IsVisibleFromOutside(loc))
				.Count();

			Console.WriteLine($"The result of part 1 is: {result}");
		}

		private static bool IsVisibleFromOutside(Location target)
		{
			foreach(Direction dir in DirectionHelper.AllDirections)
			{
				int highest = -1;
				for(Location loc = target.Displace(dir); _grid.IsInBounds(loc); loc = loc.Displace(dir))
					highest = Math.Max(highest, _grid[loc]);

				if (highest < _grid[target])
					return true;
			}

			return false;
		}

		private static void Part2()
		{
			var result = _grid.Locations
				.Select(loc => CalculateScenicScore(loc))
				.Max();

			Console.WriteLine($"The result of part 2 is: {result}");
		}

		private static int CalculateScenicScore(Location target)
		{
			int result = 1;
			foreach (Direction dir in DirectionHelper.AllDirections)
			{
				int scenicScoreInThisDirection = 0;
				for (Location loc = target.Displace(dir); _grid.IsInBounds(loc); loc = loc.Displace(dir))
				{
					scenicScoreInThisDirection++;
					if (_grid[loc] >= _grid[target])
						break;
				}

				result *= scenicScoreInThisDirection;
			}

			return result;
		}
	}
}
