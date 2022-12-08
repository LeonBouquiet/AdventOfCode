using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

		private static bool IsVisibleFromOutside(Location loc)
		{
			int highestFromLeft = -1;
			for (int x = 0; x < loc.X; x++)
				highestFromLeft = Math.Max(highestFromLeft, _grid[x, loc.Y]);
			int highestFromRight = -1;
			for (int x = _grid.Width - 1; x > loc.X; x--)
				highestFromRight = Math.Max(highestFromRight, _grid[x, loc.Y]);
			int highestFromTop = -1;
			for (int y = 0; y < loc.Y; y++)
				highestFromTop = Math.Max(highestFromTop, _grid[loc.X, y]);
			int highestFromBottom = -1;
			for (int y = _grid.Height - 1; y > loc.Y; y--)
				highestFromBottom = Math.Max(highestFromBottom, _grid[loc.X, y]);

			return (highestFromLeft < _grid[loc] || highestFromRight < _grid[loc] || highestFromTop < _grid[loc] || highestFromBottom < _grid[loc]);
		}

		private static void Part2()
		{
			var result = _grid.Locations
				.Select(loc => CalculateScenicScore(loc))
				.Max();

			Console.WriteLine($"The result of part 2 is: {result}");
		}

		private static int CalculateScenicScore(Location loc)
		{
			int scenicScoreLeft = 0;
			for (int x = loc.X - 1; x >= 0; x--)
			{
				scenicScoreLeft++;
				if (_grid[x, loc.Y] >= _grid[loc])
					break;
			}

			int scenicScoreRight = 0;
			for (int x = loc.X + 1; x < _grid.Width; x++)
			{
				scenicScoreRight++;
				if (_grid[x, loc.Y] >= _grid[loc])
					break;
			}

			int scenicScoreTop = 0;
			for (int y = loc.Y - 1; y >= 0; y--)
			{
				scenicScoreTop++;
				if (_grid[loc.X, y] >= _grid[loc])
					break;
			}

			int scenicScoreBottom = 0;
			for (int y = loc.Y + 1; y < _grid.Height; y++)
			{
				scenicScoreBottom++;
				if (_grid[loc.X, y] >= _grid[loc])
					break;
			}

			return scenicScoreLeft * scenicScoreRight * scenicScoreTop * scenicScoreBottom;
		}
	}
}
