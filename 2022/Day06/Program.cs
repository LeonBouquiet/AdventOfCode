using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Shared;

namespace Day06
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
			var result = InputReader.Read<Program>()
				.First()
				.LaggingWindow(4, (index, window) => (window.Distinct().Count() == 4 ? (int?)index : null))
				.First(i => i != null) + 1;

			Console.WriteLine($"The result of part 1 is: {result}");
		}

		private static void Part2()
		{
			var result = InputReader.Read<Program>()
				.First()
				.LaggingWindow(14, (index, window) => (window.Distinct().Count() == 14 ? (int?)index : null))
				.First(i => i != null) + 1;

			Console.WriteLine($"The result of part 2 is: {result}");
		}
	}
}
