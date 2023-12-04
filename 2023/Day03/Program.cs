using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Shared;

namespace Day03
{
	public class Program
	{
		public static void Main(string[] args)
		{
			Part1();
			Part2();
		}

		private static void Part1()
		{
			Grid<char> grid = new Grid<char>(
				InputReader.Read<Program>()
				.Select(line => line.AsEnumerable())
				.ToList());

			string collectedNumber = "";
			bool hasAdjacentSymbol = false;

			List<int> partNumbers = new List<int>();
			for(int row = 0; row < grid.Height; row++)
			{
				foreach(Location loc in grid.GetLocationsInRow(row))
				{
					if (Char.IsDigit(grid[loc]))
					{
						collectedNumber += grid[loc];
						hasAdjacentSymbol |= loc.AllNeighbours
							.Where(l => grid.IsInBounds(l))
							.Any(l => grid[l] != '.' && !Char.IsDigit(grid[l]));
					}
					else
					{
						if (collectedNumber != "")
						{
							//Collected a number, terminated by a non-digit
							if (hasAdjacentSymbol)
								partNumbers.Add(Int32.Parse(collectedNumber));

							collectedNumber = "";
							hasAdjacentSymbol = false;
						}
					}
				}

				if (collectedNumber != "")
				{
					//Collected a number, terminated by the end of the line.
					if (hasAdjacentSymbol)
						partNumbers.Add(Int32.Parse(collectedNumber));

					collectedNumber = "";
					hasAdjacentSymbol = false;
				}
			}

			var result = partNumbers.Sum();
			Console.WriteLine($"The result of part 1 is: {result}");
		}

		private static void Part2()
		{
			long result = 0;

			Grid<char> grid = new Grid<char>(
				InputReader.Read<Program>()
				.Select(line => line.AsEnumerable())
				.ToList());

			List<Location> gearSymbolLocations = grid.Locations
				.Where(l => grid[l] == '*')
				.ToList();

			foreach(Location gearSymbolLocation in gearSymbolLocations)
			{
				List<int> collectedNumbers = new List<int>();

				HashSet<Location> exploredNeighbours = new HashSet<Location>();
				foreach (Location nb in gearSymbolLocation.AllNeighbours)
				{
					if (exploredNeighbours.Contains(nb))
						continue;

					if (Char.IsDigit(grid[nb]))
					{
						//Walk to the leftmost and rightmost digits
						Location beforeBegin = nb.Displace(Direction.Left);
						while (grid.IsInBounds(beforeBegin) && Char.IsDigit(grid[beforeBegin]))
							beforeBegin = beforeBegin.Displace(Direction.Left);

						Location pastEnd = nb.Displace(Direction.Right);
						while (grid.IsInBounds(pastEnd) && Char.IsDigit(grid[pastEnd]))
							pastEnd = pastEnd.Displace(Direction.Right);

						//Found the complete number span, collect the digits and mark them as already visited
						string collectedNumber = "";
						for(Location loc = beforeBegin.Displace(Direction.Right); loc.X < pastEnd.X; loc = loc.Displace(Direction.Right))
						{
							collectedNumber += grid[loc];
							exploredNeighbours.Add(loc);
						}

						collectedNumbers.Add(Int32.Parse(collectedNumber));
					}
				}

				if (collectedNumbers.Count == 2)
					result += (long)(collectedNumbers[0] * collectedNumbers[1]);
			}

			Console.WriteLine($"The result of part 2 is: {result}");
		}

	}
}
