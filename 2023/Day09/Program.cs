using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Shared;

namespace Day09
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
			List<List<int>> historyLines = InputReader.Read<Program>()
				.Select(line => line
					.Split(' ', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
					.Select(s => Int32.Parse(s))
					.ToList())
				.ToList();

			int result = 0;
			foreach(List<int> historyLine in historyLines)
			{
				int extrapolated = ExtrapolateLast(historyLine);
				result += extrapolated;
			}

			Console.WriteLine($"The result of part 1 is: {result}");
		}

		private static int ExtrapolateLast(List<int> input)
		{
			List<int> child = new List<int>(input.Count - 1);
			for (int index = 0; index < input.Count - 1; index++)
				child.Add(input[index + 1] - input[index]);

			if (child.All(i => i == 0) == false)
			{
				//Recursively explore further
				ExtrapolateLast(child);
			}
			else
			{
				//Found the all-zero's line, append a zero placeholder.
				child.Add(0);
				return child[child.Count - 1];
			}

			//After the recursive call, the child list now has an extra extrapolated value.
			input.Add(input[input.Count - 1] + child[child.Count - 1]);
			return input[input.Count - 1];
		}

		private static void Part2()
		{
			List<List<int>> historyLines = InputReader.Read<Program>()
				.Select(line => line
					.Split(' ', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
					.Select(s => Int32.Parse(s))
					.ToList())
				.ToList();

			int result = 0;
			foreach (List<int> historyLine in historyLines)
			{
				int extrapolated = ExtrapolateFirst(historyLine);
				result += extrapolated;
			}

			Console.WriteLine($"The result of part 2 is: {result}");

		}

		private static int ExtrapolateFirst(List<int> input)
		{
			List<int> child = new List<int>(input.Count - 1);
			for (int index = 0; index < input.Count - 1; index++)
				child.Add(input[index + 1] - input[index]);

			if (child.All(i => i == 0) == false)
			{
				//Recursively explore further
				ExtrapolateFirst(child);
			}
			else
			{
				//Found the all-zero's line, prepend a zero placeholder.
				child.Insert(0, 0);
				return child[0];
			}

			//After the recursive call, the child list now has an extra extrapolated value at the start
			input.Insert(0, input[0] - child[0]);
			return input[0];
		}
	}
}
