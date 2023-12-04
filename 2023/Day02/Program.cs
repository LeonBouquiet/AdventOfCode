using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Shared;

namespace Day02
{
	public enum BlockColor
	{
		Red = 0,
		Green,
		Blue
	}

	public struct BlockCount
	{
		public BlockColor BlockColor;
		public int Count;
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
			Dictionary<BlockColor, int> maxAvailable = new Dictionary<BlockColor, int>() {
				{ BlockColor.Red, 12},
				{ BlockColor.Green, 13},
				{ BlockColor.Blue, 14 }
			};

			var result = InputReader.Read<Program>()
				.Select(line => ParseGame(line))
				.Where(line => line.Item2.All(bc => bc.Count <= maxAvailable[bc.BlockColor]))
				.Select(line => line.Item1)
				.Sum();

			Console.WriteLine($"The result of part 1 is: {result}");
		}

		private static void Part2()
		{
			var result = InputReader.Read<Program>()
				.Select(line => ParseGame(line))
				.Select(line => new
				{
					MinRed = line.Item2.Where(bc => bc.BlockColor == BlockColor.Red).Max(bc => bc.Count),
					MinGreen = line.Item2.Where(bc => bc.BlockColor == BlockColor.Green).Max(bc => bc.Count),
					MinBlue = line.Item2.Where(bc => bc.BlockColor == BlockColor.Blue).Max(bc => bc.Count),
				})
				.Select(a => 
					(long)(a.MinRed * a.MinGreen * a.MinBlue))
				.Sum();

			Console.WriteLine($"The result of part 2 is: {result}");
		}

		private static Tuple<int, List<BlockCount>> ParseGame(string line)
		{
			string[] parts = line.Split(':');
			int id = Int32.Parse(parts[0].Substring(4));
			var blockCounts = parts[1]
				.Split(',', ';')
				.Select(s => ParseBlockCount(s.Trim()))
				.ToList();

			return new Tuple<int, List<BlockCount>>(id, blockCounts);
		}

		private static BlockCount ParseBlockCount(string text)
		{
			string[] parts = text.Split(' ');
			return new BlockCount()
			{
				BlockColor = Enum.Parse<BlockColor>(parts[1], ignoreCase: true),
				Count = Int32.Parse(parts[0])
			};
		}
	}
}
