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
			Part1();
			Part2();
		}

		private static void Part1()
		{
			_grid = new Grid<int>(InputReader.Read<Program>()
				.Select(line => line
					.Select(c => Int32.Parse("" + c)))
				.ToList());

			int result = 0;
			foreach(Location loc in _grid.Locations)
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

				if (highestFromLeft < _grid[loc] || highestFromRight < _grid[loc] || highestFromTop < _grid[loc] || highestFromBottom < _grid[loc])
					result++;
			}

			Console.WriteLine($"The result of part 1 is: {result}");
		}

		private static void Part2()
		{
			var result = InputReader.Read<Program>();

			Console.WriteLine($"The result of part 2 is: {result}");
		}

	}
}
