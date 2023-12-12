using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Shared;

namespace Day11
{
	public class Image: Grid<int>
	{
		public Image(List<IEnumerable<int>> cellsPerRow)
			: base(cellsPerRow)
		{
		}

		public void InsertRow(int y, int value)
		{
			int[,] clone = new int[_cells.GetLength(0) + 1, _cells.GetLength(1)];
			for (int ly = 0; ly < y; ly++)
				for (int x = 0; x < Width; x++)
					clone[ly, x] = _cells[ly, x];

			for (int x = 0; x < Width; x++)
				clone[y, x] = value;

			for (int ly = y; ly < Height; ly++)
				for (int x = 0; x < Width; x++)
					clone[ly + 1, x] = _cells[ly, x];

			_cells = clone;
			Height += 1;
		}

		public void InsertColumn(int x, int value)
		{
			int[,] clone = new int[_cells.GetLength(0), _cells.GetLength(1) + 1];
			for (int lx = 0; lx < x; lx++)
				for (int y = 0; y < Height; y++)
					clone[y, lx] = _cells[y, lx];

			for (int y = 0; y < Height; y++)
				clone[y, x] = value;

			for (int lx = x; lx < Width; lx++)
				for (int y = 0; y < Height; y++)
					clone[y, lx + 1] = _cells[y, lx];

			_cells = clone;
			Width += 1;
		}
	}

	public class Program
	{
		public static void Main(string[] args)
		{
			Part1();
			Part2();
		}

		private static void Part1()
		{
			Image image = new Image(
				InputReader.Read<Program>()
				.Select(line => line
					.AsEnumerable()
					.Select(c => (c == '#') ? 1 : 0))
				.ToList());

			//Find all empty rows
			for(int y = 0; y < image.Height; y++)
			{
				if (image.GetLocationsInRow(y).All(loc => image[loc] == 0))
					image.InsertRow(y++, 0);
			}

			//Find all empty columns
			for (int x = 0; x < image.Width; x++)
			{
				if (image.GetLocationsInColumn(x).All(loc => image[loc] == 0))
					image.InsertColumn(x++, 0);
			}

			Console.WriteLine(image);

			var result = 0;
			Console.WriteLine($"The result of part 1 is: {result}");
		}

		private static void Part2()
		{
			var result = InputReader.Read<Program>();

			Console.WriteLine($"The result of part 2 is: {result}");
		}

	}
}
